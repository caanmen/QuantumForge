#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Fixtures técnicos con datos canónicos; no simulan tiempos ni progresión natural.
// No cargan/guardan partidas ni sustituyen singletons. El runner principal registra PASS/FAIL.
public static class ReleaseD1BudgetValidation
{
    public static void Run(Action<string, Action> run)
    {
        if (run == null) throw new ArgumentNullException(nameof(run));
        run("D1_BUDGET_INITIAL_SOURCE_X2", ValidateInitialSource);
        run("D1_BUDGET_CAP_AND_SPENDING", ValidateCap);
        run("D1_BUDGET_OLD_BASELINE_SINGLE_CREDIT", ValidateOldBaseline);
        run("D1_BUDGET_OPTIONALS_BEFORE_READING_I_II_III", ValidateOptionalSpending);
        run("D1_BUDGET_COSTS_FULL_27_MINIMUM_15", ValidateCosts);
    }

    private static void ValidateInitialSource()
    {
        using (var fixture = new Fixture())
        {
            GameState state = fixture.state;
            state.dimension01Unlocked = false;
            Require(Dimension1System.CalculateD1TreePointsFromProgress(state) == 0,
                "D1 bloqueada debe aportar cero.");
            AssertNoCredit(state);
            state.dimension01Unlocked = true;
            // Un planeta y una sonda iniciales aportaban 2; ahora deben aportar 4.
            Require(Dimension1System.CalculateD1TreePointsFromProgress(state) == 4,
                "Las fuentes iniciales no aportan exactamente 4.");
            Require(Dimension1System.SyncD1TreePointsFromProgress(state, out int credit) && credit == 4,
                "El primer crédito debe ser 4.");
            Require(state.d1TreePoints == 4 && state.d1TreePointsProgressBaseline == 4,
                "Saldo/baseline inicial incorrecto.");
            fixture.Buy(Dimension1System.D1TreeExplorationDestinationReading, 1);
            Require(state.d1TreePoints == 3, "La compra inicial debe descontar un punto.");
            AssertNoCredit(state);
        }
    }

    private static void ValidateCap()
    {
        using (var fixture = new Fixture())
        {
            fixture.DevelopAllSources();
            GameState state = fixture.state;
            Require(Dimension1System.Dimension1Prestige1PreviewPointCap == 27,
                "El cap acumulado aprobado debe ser 27.");
            Require(Dimension1System.CalculateD1TreePointsFromProgress(state) == 27,
                "Las fuentes desarrolladas deben llegar exactamente a 27, no 54.");
            Require(Dimension1System.SyncD1TreePointsFromProgress(state, out int credit) && credit == 27,
                "El crédito desde cero debe detenerse en 27.");
            fixture.Buy(Dimension1System.D1TreeExplorationDestinationReading, 1);
            Require(state.d1TreePoints == 26 && state.d1TreePointsProgressBaseline == 27,
                "Gastar al cap debe conservar el baseline acumulado.");
            AssertNoCredit(state);
            // Aumentar otra fuente después del cap tampoco repone lo gastado.
            foreach (D1RelicState relic in state.dimension1Relics)
            {
                relic.unlocked = true;
                relic.level = Dimension1System.Dimension1RelicMaxLevel;
            }
            AssertNoCredit(state);
            Require(state.d1TreePoints == 26, "El progreso posterior al cap repuso gasto.");
        }
    }

    private static void ValidateOldBaseline()
    {
        using (var fixture = new Fixture())
        {
            fixture.DevelopAllSources();
            GameState state = fixture.state;
            // Estado ya restaurado por SaveService: antiguo cap 12, saldo 5 tras gasto.
            // Prueba el contrato de Sync, no sustituye la prueba de migración real en disco.
            state.d1TreePointsProgressBaseline = 12;
            state.d1TreePoints = 5;
            Require(Dimension1System.CalculateClaimablePrestige1Points(state) == 15,
                "Desde baseline antiguo 12 deben quedar 15 por acreditar.");
            Require(Dimension1System.SyncD1TreePointsFromProgress(state, out int credit) && credit == 15,
                "El ajuste positivo antiguo debe acreditarse una vez.");
            Require(state.d1TreePoints == 20 && state.d1TreePointsProgressBaseline == 27,
                "Debe sumarse 15 al saldo 5, sin multiplicarlo ni restaurar gasto previo.");
            AssertNoCredit(state);
            fixture.Buy(Dimension1System.D1TreeFleetHangarPreparation, 1);
            Require(state.d1TreePoints == 19, "No debe repetirse el ajuste después de gastar.");
            state.dimension01Unlocked = false;
            AssertNoCredit(state);
            state.dimension01Unlocked = true;
            AssertNoCredit(state);
        }
    }

    private static void ValidateOptionalSpending()
    {
        using (var fixture = new Fixture())
        {
            GameState state = fixture.state;
            Dimension1System.SyncD1TreePointsFromProgress(state, out _);
            fixture.Buy(Dimension1System.D1TreeExplorationDestinationReading, 1);
            fixture.Buy(Dimension1System.D1TreeFleetHangarPreparation, 1);
            fixture.Reject(Dimension1System.D1TreeRelicReading);

            // Sector 2: desarrollar fuentes reales antes de gastar en opcionales.
            fixture.OpenSector(Dimension1System.Sector02DebrisRing);
            fixture.DevelopPlanets(3);
            fixture.UnlockShip(Dimension1System.ShipExtractorDrone);
            state.dimension1ScannerLevel = 4;
            Dimension1System.SyncD1TreePointsFromProgress(state, out _);
            fixture.Buy(Dimension1System.D1TreeRecoveryCopyRegistry, 3);
            fixture.Buy(Dimension1System.D1TreeExplorationScanMemory, 3);
            fixture.Buy(Dimension1System.D1TreeExplorationHiddenFindTracking, 1);
            fixture.Reject(Dimension1System.D1TreeRelicReading); // Escáner < 5.
            state.dimension1ScannerLevel = 5;
            fixture.Buy(Dimension1System.D1TreeRelicReading, 1);
            fixture.Reject(Dimension1System.D1TreeRelicReading); // Sector 3 pendiente.

            fixture.OpenSector(Dimension1System.Sector03AncientOrbits);
            fixture.DevelopPlanets(5);
            fixture.UnlockShip(Dimension1System.ShipAnalyticProbe);
            state.dimension1ScannerLevel = 9;
            Dimension1System.SyncD1TreePointsFromProgress(state, out _);
            fixture.Buy(Dimension1System.D1TreeExplorationHiddenFindTracking, 3);
            fixture.Buy(Dimension1System.D1TreeFleetSupportFormation, 1);
            fixture.Reject(Dimension1System.D1TreeRelicReading); // Escáner < 10.
            state.dimension1ScannerLevel = 10;
            fixture.Buy(Dimension1System.D1TreeRelicReading, 2);
            fixture.Reject(Dimension1System.D1TreeRelicReading); // Sector 4 pendiente.

            fixture.OpenSector(Dimension1System.Sector04SilentFrontier);
            fixture.DevelopPlanets(7);
            fixture.UnlockShip(Dimension1System.ShipCargoShip);
            state.dimension1ScannerLevel = 14;
            Dimension1System.SyncD1TreePointsFromProgress(state, out _);
            fixture.Buy(Dimension1System.D1TreeExplorationAdvancedCartography, 1);
            fixture.Buy(Dimension1System.D1TreeConvergenceUnstableZoneStabilization, 1);
            fixture.Reject(Dimension1System.D1TreeRelicReading);
            state.dimension1ScannerLevel = 15;
            fixture.Reject(Dimension1System.D1TreeRelicReading); // Falta Coordinación.
            fixture.Buy(Dimension1System.D1TreeFleetCoordination, 1);
            fixture.Buy(Dimension1System.D1TreeRelicReading, 3);

            Require(fixture.spent == 27 && state.d1TreePoints == 0 &&
                state.d1TreePointsProgressBaseline == 27,
                "El árbol completo debe costar 27 y ser comprable tras gastar primero en opcionales.");
            foreach (string id in Dimension1System.Dimension1TreeNodeIds)
                Require(state.GetD1TreeNodeTier(id) == Dimension1System.GetDimension1TreeNodeMaxTier(id),
                    "Nodo sin completar: " + id);
            AssertNoCredit(state);
        }
    }

    private static void ValidateCosts()
    {
        Require(Dimension1System.GetDimension1TreeTotalFullPurchaseCost() == 27,
            "El coste completo cambió respecto de 27.");
        using (var fixture = new Fixture())
        {
            var requirements = Dimension1System.GetD1ArkRequirements(fixture.state);
            int nodes = requirements.Single(r => r.requirementId == "tree_nodes_8").requiredValue;
            int reading = requirements.Single(r => r.requirementId == "relic_reading_3").requiredValue;
            Require(nodes == 8 && reading == 3 &&
                requirements.Any(r => r.requirementId == "fleet_coordination"),
                "Los requisitos del árbol para Ark cambiaron.");
            string[] ids = Dimension1System.Dimension1TreeNodeIds;
            int minimum = int.MaxValue;
            string[] cheapest = null;
            for (int mask = 0; mask < (1 << ids.Length); mask++)
            {
                string[] selected = ids.Where((id, index) => (mask & (1 << index)) != 0).ToArray();
                if (selected.Length < nodes || !selected.Contains(Dimension1System.D1TreeRelicReading) ||
                    !selected.Contains(Dimension1System.D1TreeFleetCoordination)) continue;
                int cost = selected.Sum(id => Enumerable.Range(1,
                    id == Dimension1System.D1TreeRelicReading ? reading : 1)
                    .Sum(tier => Dimension1System.GetDimension1TreeNodeCost(id, tier)));
                if (cost >= minimum) continue;
                minimum = cost;
                cheapest = selected;
            }
            Require(minimum == 15 && cheapest != null, "El presupuesto mínimo de Ark debe seguir siendo 15.");
            // Confirmar el mínimo con compras y requisitos reales, no sólo con una suma.
            fixture.DevelopAllSources();
            Dimension1System.SyncD1TreePointsFromProgress(fixture.state, out _);
            foreach (string id in cheapest.Where(id => id != Dimension1System.D1TreeRelicReading))
                fixture.Buy(id, 1);
            fixture.Buy(Dimension1System.D1TreeRelicReading, reading);
            Require(fixture.spent == 15 && fixture.state.d1TreePoints == 12,
                "Las compras del mínimo deben gastar exactamente 15.");
            foreach (var requirement in Dimension1System.GetD1ArkRequirements(fixture.state)
                .Where(r => r.requirementId == "tree_nodes_8" || r.requirementId == "relic_reading_3" ||
                    r.requirementId == "fleet_coordination"))
                Require(requirement.met, "Requisito del árbol incumplido: " + requirement.requirementId);
        }
    }

    private static void AssertNoCredit(GameState state)
    {
        int balance = state.d1TreePoints, baseline = state.d1TreePointsProgressBaseline;
        Require(!Dimension1System.SyncD1TreePointsFromProgress(state, out int credit) && credit == 0,
            "El progreso ya acreditado o inferior produjo crédito.");
        Require(state.d1TreePoints == balance && state.d1TreePointsProgressBaseline == baseline,
            "Un sync sin crédito modificó saldo/baseline.");
    }

    private sealed class Fixture : IDisposable
    {
        public readonly GameState state;
        public int spent;

        public Fixture()
        {
            Require(!EditorApplication.isPlayingOrWillChangePlaymode, "Ejecutar fuera de Play Mode.");
            var obj = new GameObject("Release D1 budget fixture") { hideFlags = HideFlags.HideAndDontSave };
            obj.SetActive(false);
            try
            {
                state = obj.AddComponent<GameState>();
                state.EnsureDimension1State(); // Normaliza versiones antes de preparar datos actuales.
                state.dimension1ScannerLevel = Dimension1System.SimpleScannerMinLevel;
                state.dimension01Unlocked = true;
                DevelopPlanets(1);
                UnlockShip(Dimension1System.ShipLightProbe);
                OpenSector(Dimension1System.Sector01OuterRim);
            }
            catch
            {
                UnityEngine.Object.DestroyImmediate(obj);
                throw;
            }
        }

        public void DevelopPlanets(int count)
        {
            foreach (string id in Dimension1System.StarterPlanets.Take(count))
                state.dimension1Planets.Single(p => p.planetId == id).unlocked = true;
        }

        public void UnlockShip(string id) => state.dimension1Ships.Single(s => s.shipId == id).unlocked = true;
        public void OpenSector(string id) => state.dimension1Sectors.Single(s => s.sectorId == id).unlocked = true;

        public void DevelopAllSources()
        {
            DevelopPlanets(Dimension1System.StarterPlanets.Length);
            foreach (string id in Dimension1System.Dimension1ActiveShipIds) UnlockShip(id);
            foreach (string id in Dimension1System.Dimension1SectorIds) OpenSector(id);
            state.dimension1ScannerLevel = Dimension1System.SimpleScannerMaxLevel;
        }

        public void Buy(string id, int targetTier)
        {
            while (state.GetD1TreeNodeTier(id) < targetTier)
            {
                int tier = state.GetD1TreeNodeTier(id);
                int cost = Dimension1System.GetDimension1TreeNodeCost(id, tier + 1);
                int balance = state.d1TreePoints, baseline = state.d1TreePointsProgressBaseline;
                Require(Dimension1System.TryBuyDimension1TreeNode(state, id),
                    "Compra bloqueada tras gasto previo: " + id + " tier " + (tier + 1));
                spent += cost;
                Require(state.GetD1TreeNodeTier(id) == tier + 1, "La compra no avanzó exactamente un tier.");
                Require(state.d1TreePoints == balance - cost + state.d1TreePointsProgressBaseline - baseline &&
                    state.d1TreePoints >= 0 && state.d1TreePointsProgressBaseline <= 27,
                    "Compra/crédito violó conservación de puntos o cap: " + id);
                AssertNoCredit(state);
            }
        }

        public void Reject(string id)
        {
            int balance = state.d1TreePoints, baseline = state.d1TreePointsProgressBaseline;
            int tier = state.GetD1TreeNodeTier(id);
            Require(balance >= Dimension1System.GetDimension1TreeNodeCost(id, tier + 1),
                "El rechazo debe probar prerequisitos, no saldo insuficiente.");
            Require(!Dimension1System.CanBuyDimension1TreeNode(state, id) &&
                !Dimension1System.TryBuyDimension1TreeNode(state, id), "Se omitió un prerequisito: " + id);
            Require(state.d1TreePoints == balance && state.d1TreePointsProgressBaseline == baseline &&
                state.GetD1TreeNodeTier(id) == tier, "La compra rechazada modificó el estado.");
        }

        public void Dispose() => UnityEngine.Object.DestroyImmediate(state.gameObject);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif
