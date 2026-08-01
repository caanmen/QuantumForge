#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class BuildingSceneReloadValidation
{
    [MenuItem("Tools/Quantum Forge/QA/Validate Building Scene Reload")]
    public static void Validate()
    {
        GameObject target = new GameObject("QA Building Scene Reload")
            { hideFlags = HideFlags.HideAndDontSave };
        target.SetActive(false);
        GameState state = target.AddComponent<GameState>();

        GameState previousState = GameState.I;
        List<string> previousLoadedResearchIds =
            SaveService.LastLoadedResearchIds;
        List<string> previousLoadedAchievementIds =
            SaveService.LastLoadedAchievementIds;
        List<SavedBuildingLevel> previousLoadedLevels =
            SaveService.LastLoadedBuildingLevels;

        try
        {
            SetGameStateSingleton(state);
            SaveService.LastLoadedResearchIds = new List<string>();
            SaveService.LastLoadedAchievementIds = new List<string>();
            SaveService.LastLoadedBuildingLevels =
                new List<SavedBuildingLevel>();

            TextAsset buildingData =
                Resources.Load<TextAsset>("Data/buildings");
            Require(buildingData != null,
                "No se encontro Resources/Data/buildings.");
            BuildingCollection collection =
                JsonUtility.FromJson<BuildingCollection>(buildingData.text);
            Require(collection != null && collection.buildings != null,
                "No se pudieron leer las definiciones reales de edificios.");

            BuildingDef higgs = collection.buildings.Find(
                building => building.id == "vacuum_observer");
            BuildingDef tetraquark = collection.buildings.Find(
                building => building.id == "casimir_panel");
            Require(higgs != null && tetraquark != null,
                "Falta el Higgs o el Nucleo Tetraquark en buildings.json.");
            Require(tetraquark.baseCost == 700.0 &&
                    tetraquark.unlockRequireId == higgs.id &&
                    tetraquark.unlockRequireLevel == 1,
                "La configuracion real del Nucleo Tetraquark no coincide con " +
                "precio 700 LE y requisito Higgs nivel 1.");

            BuildingState oldSceneState = new BuildingState();
            oldSceneState.InitFromDef(higgs);
            oldSceneState.level = 20;
            oldSceneState.currentCost = 163.665;
            oldSceneState.tickTimer = 0.12f;
            state.RegisterBuildingState(oldSceneState);

            BuildingState reloadedSceneState = new BuildingState();
            reloadedSceneState.InitFromDef(higgs);
            state.RegisterBuildingState(reloadedSceneState);

            Require(ReferenceEquals(
                    state.GetBuildingState(higgs.id), reloadedSceneState),
                "La recarga no reemplazo la referencia vieja del edificio.");
            Require(state.GetBuildingLevel(higgs.id) == 20,
                "La recarga perdio el nivel del edificio.");
            Require(Math.Abs(reloadedSceneState.currentCost - 163.665) < 0.000001,
                "La recarga perdio el coste actual del edificio.");
            Require(Math.Abs(reloadedSceneState.tickTimer - 0.12f) < 0.000001f,
                "La recarga perdio el temporizador del edificio.");

            state.DebugResetRunState();
            state.LE = 1013.0;
            Require(state.GetBuildingLevel(higgs.id) == 0,
                "RESET no devolvio el Higgs al nivel 0.");
            Require(!BuildingUnlock.IsUnlocked(tetraquark),
                "El Nucleo se desbloqueo antes de recomprar el Higgs.");

            reloadedSceneState.OnPurchased();
            Require(state.GetBuildingLevel(higgs.id) == 1,
                "El registro no refleja la compra realizada tras RESET.");
            Require(BuildingUnlock.IsUnlocked(tetraquark),
                "El Nucleo sigue bloqueado tras comprar Higgs nivel 1.");

            List<SavedBuildingLevel> saved = state.GetBuildingLevelsForSave();
            Require(saved.Count == 1 && saved[0].id == higgs.id &&
                    saved[0].level == 1,
                "El registro conserva edificios duplicados tras recargar.");

            for (int second = 0; second < 300; second++)
                state.Tick(1.0);
            Require(BuildingUnlock.IsUnlocked(tetraquark),
                "El Nucleo volvio a bloquearse tras avanzar 5 minutos.");

            BuildingState tetraquarkState = new BuildingState();
            tetraquarkState.InitFromDef(tetraquark);
            state.RegisterBuildingState(tetraquarkState);
            Require(state.LE >= state.GetEffectiveBuildingCost(tetraquarkState),
                "Los 1013 LE no alcanzan para comprar el Nucleo de 700 LE.");
            state.LE -= state.GetEffectiveBuildingCost(tetraquarkState);
            tetraquarkState.OnPurchased();
            Require(state.GetBuildingLevel(tetraquark.id) == 1,
                "El Nucleo desbloqueado no pudo comprarse.");

            Debug.Log("[Building Scene Reload Validation] PASS | " +
                "referencia renovada | progreso conservado | " +
                "sin duplicados | RESET | Higgs 1 | Nucleo desbloqueado | " +
                "+5 MIN | compra por 700 LE");
        }
        finally
        {
            SetGameStateSingleton(previousState);
            SaveService.LastLoadedResearchIds = previousLoadedResearchIds;
            SaveService.LastLoadedAchievementIds =
                previousLoadedAchievementIds;
            SaveService.LastLoadedBuildingLevels = previousLoadedLevels;
            UnityEngine.Object.DestroyImmediate(target);
        }
    }

    public static void ValidateBatch()
    {
        try
        {
            Validate();
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private static void SetGameStateSingleton(GameState state)
    {
        PropertyInfo property = typeof(GameState).GetProperty(
            "I", BindingFlags.Public | BindingFlags.Static);
        property?.SetValue(null, state, null);
    }
}
#endif
