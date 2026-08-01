#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SpanishReleaseReadinessValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/QA/Configure Spanish Default And Validate")]
    public static void ConfigureSpanishDefaultAndValidate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        LocalizationManager manager =
            UnityEngine.Object.FindFirstObjectByType<LocalizationManager>(
                FindObjectsInactive.Include);
        if (manager == null)
            throw new InvalidOperationException("Falta LocalizationManager en Main.unity.");

        SerializedObject serializedManager = new SerializedObject(manager);
        SerializedProperty defaultLanguage =
            serializedManager.FindProperty("defaultLanguage");
        if (defaultLanguage == null)
            throw new InvalidOperationException("Falta defaultLanguage serializado.");
        defaultLanguage.enumValueIndex = (int)LocalizationManager.Language.ES;
        serializedManager.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(manager);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("No se pudo guardar Main.unity.");

        Validate();
    }

    public static void Validate()
    {
        var failures = new List<string>();
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        LocalizationManager[] managers =
            UnityEngine.Object.FindObjectsByType<LocalizationManager>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);
        Check(managers.Length == 1,
            "Debe existir exactamente un LocalizationManager.", failures);
        if (managers.Length == 1)
        {
            SerializedObject serializedManager = new SerializedObject(managers[0]);
            SerializedProperty defaultLanguage =
                serializedManager.FindProperty("defaultLanguage");
            Check(defaultLanguage != null &&
                defaultLanguage.enumValueIndex ==
                    (int)LocalizationManager.Language.ES,
                "El idioma inicial no es español.", failures);
        }

        ValidateLanguageButton(scene, failures);
        ValidateCatalogs(failures);
        ValidateD3RoundTrip(failures);
        Finish(failures);
    }

    private static void ValidateLanguageButton(
        Scene scene, List<string> failures)
    {
        GameObject languageObject = FindInScene(scene, "BtnLanguage");
        Check(languageObject != null, "Falta BtnLanguage.", failures);
        if (languageObject == null)
            return;

        LanguageToggleButton toggle =
            languageObject.GetComponent<LanguageToggleButton>();
        Button button = languageObject.GetComponent<Button>();
        TMP_Text label = languageObject.GetComponentInChildren<TMP_Text>(true);
        Check(toggle != null, "BtnLanguage no tiene LanguageToggleButton.", failures);
        Check(button != null, "BtnLanguage no tiene Button.", failures);
        Check(label != null && label.text == "EN/ES",
            "El botón de idioma no muestra EN/ES.", failures);

        bool wired = false;
        if (button != null)
        {
            for (int index = 0; index < button.onClick.GetPersistentEventCount(); index++)
            {
                if (button.onClick.GetPersistentTarget(index) == toggle &&
                    button.onClick.GetPersistentMethodName(index) == "ToggleLanguage")
                {
                    wired = true;
                    break;
                }
            }
        }
        Check(wired, "BtnLanguage no invoca ToggleLanguage.", failures);
    }

    private static void ValidateCatalogs(List<string> failures)
    {
        TextAsset esAsset = Resources.Load<TextAsset>("Localization/lang_es");
        TextAsset enAsset = Resources.Load<TextAsset>("Localization/lang_en");
        Check(esAsset != null && enAsset != null,
            "Falta un catálogo ES/EN.", failures);
        if (esAsset == null || enAsset == null)
            return;

        Dictionary<string, object> es =
            MiniJSON.Deserialize(esAsset.text) as Dictionary<string, object>;
        Dictionary<string, object> en =
            MiniJSON.Deserialize(enAsset.text) as Dictionary<string, object>;
        Check(es != null && en != null,
            "Un catálogo de idioma no es JSON válido.", failures);
        if (es == null || en == null)
            return;

        foreach (string key in es.Keys)
            Check(en.ContainsKey(key), "Falta en inglés: " + key, failures);
        foreach (string key in en.Keys)
            Check(es.ContainsKey(key), "Falta en español: " + key, failures);

        string[] criticalKeys =
        {
            "ui.menu.generation", "ui.menu.lab", "ui.menu.achievements",
            "ui.panel.research_title", "ui.buy", "ui.locked",
            "achv.ach_le_1M.name", "room2.intro_locked"
        };
        for (int index = 0; index < criticalKeys.Length; index++)
        {
            string key = criticalKeys[index];
            Check(es.TryGetValue(key, out object value) &&
                !string.IsNullOrWhiteSpace(value as string) &&
                (string)value != key,
                "Texto español ausente: " + key, failures);
        }

        string spanishDump = string.Join("\n", es.Values);
        string[] visibleAnglicisms =
        {
            "Upgrade", "Boost", "keycard", "Tier", "Auto: ON",
            "Auto: OFF", "Ticks más rápidos"
        };
        for (int index = 0; index < visibleAnglicisms.Length; index++)
            Check(!spanishDump.Contains(visibleAnglicisms[index]),
                "Anglicismo visible en ES: " + visibleAnglicisms[index], failures);

        ValidateActiveAchievements(es, en, failures);
    }

    private static void ValidateActiveAchievements(
        Dictionary<string, object> es, Dictionary<string, object> en,
        List<string> failures)
    {
        TextAsset data = Resources.Load<TextAsset>("Data/achievements");
        Dictionary<string, object> root = data == null ? null :
            MiniJSON.Deserialize(data.text) as Dictionary<string, object>;
        List<object> achievements = root != null &&
            root.TryGetValue("achievements", out object raw) ?
            raw as List<object> : null;
        Check(achievements != null,
            "No se pudo leer achievements.json.", failures);
        if (achievements == null)
            return;

        for (int index = 0; index < achievements.Count; index++)
        {
            Dictionary<string, object> achievement =
                achievements[index] as Dictionary<string, object>;
            if (achievement == null ||
                !achievement.TryGetValue("id", out object rawId))
                continue;
            string id = rawId as string;
            Check(es.ContainsKey("achv." + id + ".name") &&
                es.ContainsKey("achv." + id + ".desc") &&
                en.ContainsKey("achv." + id + ".name") &&
                en.ContainsKey("achv." + id + ".desc"),
                "Logro activo sin traducción completa: " + id, failures);
        }
    }

    private static void ValidateD3RoundTrip(List<string> failures)
    {
        const string spanish = "FÁBRICA — BANCO DE PROCESOS";
        string english = D3RuntimeLocalizer.TranslateText(spanish, true);
        string restored = D3RuntimeLocalizer.TranslateText(english, false);
        Check(english == "FACTORY — PROCESS BANK" && restored == spanish,
            "La traducción D3 no completa ES→EN→ES.", failures);
        Check(PresentationTextCatalog.Get(
            "d3.onboarding.notice.factory_ready", false).Contains("Banco"),
            "La presentación D3 no devuelve español.", failures);
    }

    private static GameObject FindInScene(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
            {
                if (current.name == name)
                    return current.gameObject;
            }
        }
        return null;
    }

    private static void Check(
        bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[Spanish Release Readiness] PASS | inicio ES | " +
                "EN/ES conectado | catálogos alineados | logros activos | " +
                "D3 ES→EN→ES");
            return;
        }

        foreach (string failure in failures)
            Debug.LogError("[Spanish Release Readiness] " + failure);
        throw new InvalidOperationException(
            "Spanish Release Readiness falló con " +
            failures.Count + " error(es).");
    }
}
#endif
