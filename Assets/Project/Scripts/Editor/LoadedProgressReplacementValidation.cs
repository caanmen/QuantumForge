#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class LoadedProgressReplacementValidation
{
    [MenuItem("Tools/Quantum Forge/QA/Validate Loaded Progress Replacement")]
    public static void Validate()
    {
        GameObject achievementsObject = new GameObject(
            "QA Loaded Achievements") { hideFlags = HideFlags.HideAndDontSave };
        achievementsObject.SetActive(false);
        AchievementManager achievements =
            achievementsObject.AddComponent<AchievementManager>();

        GameObject researchObject = new GameObject(
            "QA Loaded Research") { hideFlags = HideFlags.HideAndDontSave };
        researchObject.SetActive(false);
        ResearchManager research = researchObject.AddComponent<ResearchManager>();

        try
        {
            achievements.states["a"] = new AchievementState
                { id = "a", unlocked = true };
            achievements.states["b"] = new AchievementState
                { id = "b", unlocked = true };
            achievements.ApplyLoadedAchievements(new List<string> { "a" });
            Require(achievements.states["a"].unlocked &&
                    !achievements.states["b"].unlocked,
                "Los logros cargados se sumaron al estado anterior.");
            achievements.ApplyLoadedAchievements(new List<string>());
            Require(!achievements.states["a"].unlocked &&
                    !achievements.states["b"].unlocked,
                "RESET no limpio los logros en memoria.");

            research.states["a"] = new ResearchState
                { id = "a", purchased = true };
            research.states["b"] = new ResearchState
                { id = "b", purchased = true };
            research.ApplyLoadedResearch(new List<string> { "a" });
            Require(research.states["a"].purchased &&
                    !research.states["b"].purchased,
                "Las investigaciones no reemplazaron el estado anterior.");
            research.ApplyLoadedResearch(new List<string>());
            Require(!research.states["a"].purchased &&
                    !research.states["b"].purchased,
                "RESET no limpio las investigaciones en memoria.");

            Debug.Log("[Loaded Progress Replacement] PASS | logros | " +
                "investigaciones | checkpoint | RESET | sin progreso residual");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(researchObject);
            UnityEngine.Object.DestroyImmediate(achievementsObject);
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
}
#endif
