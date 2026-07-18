using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension1CoordinatedUISetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Quantum Forge/Dimension 1/Configure Coordinated Mission UI")]
    public static void ConfigureCoordinatedMissionUI()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);

        if (panel == null)
            throw new InvalidOperationException("Dimension1PanelUI no existe en Main.unity.");

        Transform mainContent = FindChild(panel.transform, "Dimension1MainContent");
        TMP_Dropdown primaryDropdown = FindChildComponent<TMP_Dropdown>(
            mainContent,
            "Dropdown_D1_Ships"
        );
        Button exploreButton = FindChildComponent<Button>(
            mainContent,
            "Btn_ExploreD1"
        );
        TextMeshProUGUI statusText = FindChildComponent<TextMeshProUGUI>(
            mainContent,
            "Txt_Dimension1Status"
        );

        if (mainContent == null || primaryDropdown == null ||
            exploreButton == null || statusText == null)
        {
            throw new InvalidOperationException(
                "Faltan controles base de Dimensión 1 para crear la interfaz coordinada."
            );
        }

        TMP_Dropdown supportDropdown = GetOrCreateSupportDropdown(
            mainContent,
            primaryDropdown
        );
        Button coordinatedModeButton = GetOrCreateModeButton(
            mainContent,
            exploreButton
        );
        TextMeshProUGUI synergyText = GetOrCreateSynergyText(
            mainContent,
            statusText
        );

        SerializedObject serializedPanel = new SerializedObject(panel);
        AssignReference(serializedPanel, "supportShipDropdown", supportDropdown);
        AssignReference(
            serializedPanel,
            "coordinatedModeButton",
            coordinatedModeButton
        );
        AssignReference(serializedPanel, "coordinatedSynergyText", synergyText);
        serializedPanel.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(panel);
        EditorUtility.SetDirty(supportDropdown);
        EditorUtility.SetDirty(coordinatedModeButton);
        EditorUtility.SetDirty(synergyText);
        EditorSceneManager.MarkSceneDirty(scene);

        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");

        ValidateSavedConnections(panel);
        Debug.Log("[D1 Coordinated UI] CONFIGURATION_PASS");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Coordinated Mission UI")]
    public static void ValidateCoordinatedMissionUI()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);

        if (panel == null)
            throw new InvalidOperationException("Dimension1PanelUI no existe en Main.unity.");

        ValidateSavedConnections(panel);

        Transform mainContent = FindChild(panel.transform, "Dimension1MainContent");
        ValidateChildComponent<TMP_Dropdown>(
            mainContent,
            "Dropdown_D1_SupportShip"
        );
        ValidateChildComponent<Button>(
            mainContent,
            "Btn_D1_CoordinatedMode"
        );
        ValidateChildComponent<TextMeshProUGUI>(
            mainContent,
            "Txt_D1_CoordinatedSynergy"
        );

        string[] ships = Dimension1System.Dimension1ActiveShipIds;
        System.Collections.Generic.HashSet<string> synergies =
            new System.Collections.Generic.HashSet<string>();

        for (int first = 0; first < ships.Length; first++)
        {
            for (int second = first + 1; second < ships.Length; second++)
            {
                string direct = Dimension1System.GetD1SynergyId(
                    ships[first],
                    ships[second]
                );
                string reverse = Dimension1System.GetD1SynergyId(
                    ships[second],
                    ships[first]
                );

                if (string.IsNullOrEmpty(direct) || direct != reverse)
                    throw new InvalidOperationException("Pareja de sinergia inválida.");

                synergies.Add(direct);
            }
        }

        if (ships.Length != 4 || synergies.Count != 6 ||
            Dimension1System.Dimension1SynergyIds.Length != 6)
        {
            throw new InvalidOperationException(
                "El catálogo coordinado no contiene 4 naves y 6 sinergias únicas."
            );
        }

        if (Dimension1System.CoordinatedDurationMultiplier != 2.5 ||
            Dimension1System.CoordinatedMetalRewardMultiplier != 4.0 ||
            Dimension1System.CoordinatedAdaptiveMatrixMultiplier != 2.5 ||
            !Mathf.Approximately(
                Dimension1System.CoordinatedSpecificMatrixChanceBonus,
                0.15f
            ) ||
            !Mathf.Approximately(
                Dimension1System.CoordinatedRelicChanceBonus,
                0.08f
            ))
        {
            throw new InvalidOperationException(
                "Los multiplicadores coordinados oficiales no coinciden."
            );
        }

        Debug.Log("[D1 Coordinated UI] VALIDATION_PASS | 4 ships | 6 synergies");
    }

    private static TMP_Dropdown GetOrCreateSupportDropdown(
        Transform parent,
        TMP_Dropdown template
    )
    {
        Transform existing = FindChild(parent, "Dropdown_D1_SupportShip");
        TMP_Dropdown dropdown;

        if (existing != null)
        {
            dropdown = existing.GetComponent<TMP_Dropdown>();
        }
        else
        {
            dropdown = UnityEngine.Object.Instantiate(template, parent, false);
            dropdown.gameObject.name = "Dropdown_D1_SupportShip";
        }

        if (dropdown == null)
            throw new InvalidOperationException("No se pudo crear el selector de apoyo.");

        dropdown.onValueChanged = new TMP_Dropdown.DropdownEvent();
        dropdown.ClearOptions();
        dropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Elegir nave de apoyo"
        });

        RectTransform rect = dropdown.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(500f, 150f);
        rect.sizeDelta = new Vector2(220f, 32f);
        dropdown.transform.SetSiblingIndex(template.transform.GetSiblingIndex() + 1);
        dropdown.gameObject.SetActive(false);
        return dropdown;
    }

    private static Button GetOrCreateModeButton(
        Transform parent,
        Button template
    )
    {
        Transform existing = FindChild(parent, "Btn_D1_CoordinatedMode");
        Button button;

        if (existing != null)
        {
            button = existing.GetComponent<Button>();
        }
        else
        {
            button = UnityEngine.Object.Instantiate(template, parent, false);
            button.gameObject.name = "Btn_D1_CoordinatedMode";
        }

        if (button == null)
            throw new InvalidOperationException("No se pudo crear el botón coordinado.");

        button.onClick = new Button.ButtonClickedEvent();
        TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>(true);

        if (label != null)
            label.text = "Coordinada: NO";

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(500f, 105f);
        rect.sizeDelta = new Vector2(220f, 34f);
        button.gameObject.SetActive(false);
        return button;
    }

    private static TextMeshProUGUI GetOrCreateSynergyText(
        Transform parent,
        TextMeshProUGUI template
    )
    {
        Transform existing = FindChild(parent, "Txt_D1_CoordinatedSynergy");
        TextMeshProUGUI text;

        if (existing != null)
        {
            text = existing.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            text = UnityEngine.Object.Instantiate(template, parent, false);
            text.gameObject.name = "Txt_D1_CoordinatedSynergy";
        }

        if (text == null)
            throw new InvalidOperationException("No se pudo crear el texto de sinergia.");

        LocalizedTMP localized = text.GetComponent<LocalizedTMP>();

        if (localized != null)
            UnityEngine.Object.DestroyImmediate(localized);

        text.text = "Sinergia: Sin selección";
        text.fontSize = 18f;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;

        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(500f, 35f);
        rect.sizeDelta = new Vector2(380f, 90f);
        text.gameObject.SetActive(false);
        return text;
    }

    private static void AssignReference(
        SerializedObject target,
        string propertyName,
        UnityEngine.Object value
    )
    {
        SerializedProperty property = target.FindProperty(propertyName);

        if (property == null)
        {
            throw new InvalidOperationException(
                "No existe el campo serializado: " + propertyName
            );
        }

        property.objectReferenceValue = value;
    }

    private static void ValidateSavedConnections(Dimension1PanelUI panel)
    {
        SerializedObject serializedPanel = new SerializedObject(panel);
        string[] fields =
        {
            "supportShipDropdown",
            "coordinatedModeButton",
            "coordinatedSynergyText"
        };

        foreach (string field in fields)
        {
            SerializedProperty property = serializedPanel.FindProperty(field);

            if (property == null || property.objectReferenceValue == null)
                throw new InvalidOperationException("Conexión faltante: " + field);
        }
    }

    private static T FindSceneComponent<T>(Scene scene) where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            T component = root.GetComponentInChildren<T>(true);

            if (component != null)
                return component;
        }

        return null;
    }

    private static T FindChildComponent<T>(Transform parent, string objectName)
        where T : Component
    {
        Transform child = FindChild(parent, objectName);
        return child != null ? child.GetComponent<T>() : null;
    }

    private static void ValidateChildComponent<T>(
        Transform parent,
        string objectName
    ) where T : Component
    {
        if (FindChildComponent<T>(parent, objectName) == null)
        {
            throw new InvalidOperationException(
                "Control coordinado faltante o inválido: " + objectName
            );
        }
    }

    private static Transform FindChild(Transform parent, string objectName)
    {
        if (parent == null)
            return null;

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == objectName)
                return child;
        }

        return null;
    }
}
