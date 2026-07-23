#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class Dimension3Block6CatalogValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 6 Catalog")]
    public static void ValidateBlock6Catalog()
    {
        var failures = new List<string>();
        var ids = new HashSet<string>();
        for (int i = 0; i < D3AutomationCatalog.Actions.Length; i++)
        {
            D3AutomationActionDefinition action =
                D3AutomationCatalog.Actions[i];
            Check(action != null && !string.IsNullOrWhiteSpace(action.actionId),
                "Acción sin ID estable.", failures);
            if (action == null) continue;
            Check(ids.Add(action.actionId),
                "ID de acción duplicado: " + action.actionId, failures);
            Check(!string.IsNullOrWhiteSpace(action.ownerSystemId),
                "Acción sin propietario: " + action.actionId, failures);
            Check((action.status != D3AutomationCatalogStatus.Authorized &&
                   action.status != D3AutomationCatalogStatus.Internal) ||
                  action.requiredFacilityLevel >= 1,
                "Acción ejecutable sin nivel: " + action.actionId, failures);
            if (action.status == D3AutomationCatalogStatus.Prohibited)
            {
                Check(!action.offlineAllowedWithCore5,
                    "Acción prohibida marcada offline: " + action.actionId,
                    failures);
            }
        }

        Check(D3AutomationCatalog.Destinations.Length == 10,
            "El catálogo no contiene los diez destinos normales actuales.",
            failures);
        var destinations = new HashSet<string>();
        for (int i = 0; i < D3AutomationCatalog.Destinations.Length; i++)
        {
            D3AutomationDestinationDefinition destination =
                D3AutomationCatalog.Destinations[i];
            Check(destination != null && destinations.Add(destination.destinationId),
                "Destino nulo o duplicado.", failures);
            if (destination != null)
                Check(D3AutomationCatalog.IsRepeatableSafeDestination(
                        destination.destinationId),
                    "Destino normal actual no quedó clasificado con seguridad: " +
                    destination.destinationId, failures);
        }

        Check(!D3AutomationCatalog.IsRepeatableSafeDestination(
                Dimension1System.DestinationAbandonedProbe),
            "Un destino antiguo quedó autorizado.", failures);
        Check(D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionPortArk).status ==
              D3AutomationCatalogStatus.Prohibited,
            "Ark no quedó prohibida.", failures);
        Check(D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionPortCoordinated).status ==
              D3AutomationCatalogStatus.Prohibited,
            "Misiones coordinadas no quedaron prohibidas.", failures);
        Check(D3AutomationCatalog.GetAction(
                D3AutomationCatalog.ActionDiagnosticAnalyze).status ==
              D3AutomationCatalogStatus.Authorized,
            "Autoanálisis seguro no quedó autorizado.", failures);
        string[] authorizedPortActions =
        {
            D3AutomationCatalog.ActionPortScan,
            D3AutomationCatalog.ActionPortRepeatLast,
            D3AutomationCatalog.ActionPortPriorityRoutes,
            D3AutomationCatalog.ActionPortSafeRoute,
            D3AutomationCatalog.ActionPortExtractor
        };
        for (int i = 0; i < authorizedPortActions.Length; i++)
            Check(D3AutomationCatalog.GetAction(authorizedPortActions[i]).status ==
                  D3AutomationCatalogStatus.Authorized,
                "Acción segura del Puerto no quedó autorizada: " +
                authorizedPortActions[i], failures);
        ValidateManualHistoryAndStableDestinationApi(failures);

        if (failures.Count == 0)
            Debug.Log("[D3 Block 6 Catalog] PASS | IDs | 10 destinos | " +
                "APIs auditadas | decisiones únicas excluidas");
        else Debug.LogError("[D3 Block 6 Catalog] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    private static void ValidateManualHistoryAndStableDestinationApi(
        List<string> failures)
    {
        var go = new GameObject("D3 B6 D1 API")
            { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        GameState state = go.AddComponent<GameState>();
        try
        {
            state.dimension01Unlocked = true;
            state.EnsureDimension1State();
            state.dimension1SelectedSectorId = Dimension1System.Sector01OuterRim;
            state.UnlockD1Sector(Dimension1System.Sector01OuterRim);
            D1ShipState probe = null;
            for (int i = 0; i < state.dimension1Ships.Count; i++)
                if (state.dimension1Ships[i].shipId ==
                    Dimension1System.ShipLightProbe)
                    probe = state.dimension1Ships[i];
            Check(probe != null, "No existe Sonda Ligera en el estado D1.", failures);
            if (probe != null) probe.unlocked = true;
            state.dimension1ScannedDestinations.Add(
                new D1ScannedDestinationState
                {
                    destinationId = Dimension1System.DestinationMineralBelt,
                    sectorId = Dimension1System.Sector01OuterRim,
                    available = true
                });
            Check(Dimension1System.TryStartExplorationByDestinationId(
                    state, Dimension1System.ShipLightProbe,
                    Dimension1System.DestinationMineralBelt, true),
                "La API D1 por destinationId no inicia una ruta válida.", failures);
            Check(probe != null && probe.explorationStartedByAutomation,
                "La ruta automática no queda identificada.", failures);

            Check(state.RegisterManualD1SimpleDestination(
                    Dimension1System.DestinationMineralBelt),
                "No registra destino manual válido.", failures);
            Check(!state.RegisterManualD1SimpleDestination(
                    Dimension1System.DestinationAbandonedProbe),
                "Registró destino antiguo como manual válido.", failures);
            Check(state.RegisterManualD1ExtractorUpgrade(
                    Dimension1System.Planet01),
                "No registra mejora manual de extractor válida.", failures);
            Check(!state.RegisterManualD1ExtractorUpgrade("planet_unknown"),
                "Registró planeta desconocido.", failures);

            string json = JsonUtility.ToJson(state);
            Check(json.Contains(Dimension1System.DestinationMineralBelt) &&
                  json.Contains(Dimension1System.Planet01),
                "El historial manual no se serializa.", failures);
        }
        finally { Object.DestroyImmediate(go); }
    }

    private static void Check(
        bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif
