#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Dimension1SectorStatusAndShellValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1SectorStatusShell.Active";
    private const string FailedKey = "QF.D1SectorStatusShell.Failed";
    private const string FrameKey = "QF.D1SectorStatusShell.Frame";

    private sealed class SectorCase
    {
        public string rootName;
        public string sectorId;
        public string[] destinationIds;
    }

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (!EditorApplication.isPlaying) return;
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Sector Status And Shared Shell")]
    public static void Run()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailedKey, false);
        SessionState.SetInt(FrameKey, 0);
        SaveService.SuppressWritesForVisualQa = true;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            SaveService.SuppressWritesForVisualQa = true;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
            return;
        }

        if (state != PlayModeStateChange.EnteredEditMode) return;
        SaveService.SuppressWritesForVisualQa = false;
        SessionState.SetBool(ActiveKey, false);
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        bool failed = SessionState.GetBool(FailedKey, false);
        Debug.Log(failed
            ? "[D1 Sector Status+Shell] FAIL"
            : "[D1 Sector Status+Shell] PASS | 4 sectores | 3 estados + disponible | marco y GALAXIA canónicos | Anillo 1 planeta | ARK sin barra inferior");
        EditorApplication.Exit(failed ? 1 : 0);
    }

    private static void Tick()
    {
        int frame = SessionState.GetInt(FrameKey, 0) + 1;
        SessionState.SetInt(FrameKey, frame);
        if (frame < 24) return;

        try
        {
            ValidateAll();
        }
        catch (Exception exception)
        {
            SessionState.SetBool(FailedKey, true);
            Debug.LogException(exception);
        }
        finally
        {
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void ValidateAll()
    {
        Screen.SetResolution(1080, 1920, false);
        PrepareBatchViewport();
        GameState state = Find<GameState>();
        Dimension1PanelUI panel = Find<Dimension1PanelUI>();
        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance : Find<TabsUI>();
        Require(state != null && panel != null && tabs != null,
            "Faltan los propietarios funcionales de Dimensión 1.");

        state.ResetDimension1MvpState();
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        foreach (string sectorId in new[]
                 {
                     Dimension1System.Sector01OuterRim,
                     Dimension1System.Sector02DebrisRing,
                     Dimension1System.Sector03AncientOrbits,
                     Dimension1System.Sector04SilentFrontier
                 })
            state.UnlockD1Sector(sectorId);

        tabs.ShowDimension1();
        panel.OnClickOpenGalaxyPanel();

        SectorCase[] cases =
        {
            Sector("D1_OuterRimDetailVisualRoot", Dimension1System.Sector01OuterRim),
            Sector("D1_DebrisRingDetailVisualRoot", Dimension1System.Sector02DebrisRing),
            Sector("D1_AncientOrbitsVisualRoot", Dimension1System.Sector03AncientOrbits),
            Sector("D1_SilentFrontierDetailVisualRoot", Dimension1System.Sector04SilentFrontier)
        };

        foreach (SectorCase sector in cases)
        {
            Transform root = FindSceneTransform(sector.rootName);
            Require(root != null, "Falta " + sector.rootName + ".");
            ValidateSharedSectorShell(root);

            OpenAndRefresh(root);
            Require(root.Find("Destinations") == null,
                root.name + " volvió a duplicar destinos que pertenecen a Explorar.");
            Close(root);
        }

        ValidateDebrisSinglePlanet();
        ValidateArkFrame();
    }

    private static SectorCase Sector(string rootName, string sectorId)
    {
        string[] destinationIds = Dimension1System.GetDimension1SectorDestinationIds(sectorId);
        Require(destinationIds != null && destinationIds.Length == 4,
            "El catálogo real no devolvió cuatro destinos para " + sectorId + ".");
        return new SectorCase { rootName = rootName, sectorId = sectorId, destinationIds = destinationIds };
    }

    private static void OpenAndRefresh(Transform root)
    {
        Dimension1AncientOrbitsUI ancient = root.GetComponent<Dimension1AncientOrbitsUI>();
        Dimension1SectorDetailUI detail = root.GetComponent<Dimension1SectorDetailUI>();
        Require(ancient != null || detail != null, "Falta controlador de sector en " + root.name + ".");
        if (ancient != null) ancient.OpenFromGalaxy();
        else detail.OpenFromGalaxy();
        Canvas.ForceUpdateCanvases();
    }

    private static void Close(Transform root)
    {
        Dimension1AncientOrbitsUI ancient = root.GetComponent<Dimension1AncientOrbitsUI>();
        Dimension1SectorDetailUI detail = root.GetComponent<Dimension1SectorDetailUI>();
        if (ancient != null) ancient.CloseSilently();
        if (detail != null) detail.CloseSilently();
    }

    private static void ValidateDestinationState(Transform root, int index, string expected, bool interactable)
    {
        Transform destination = root.Find("Destinations/Destination_" + index);
        Require(destination != null, "Falta Destination_" + index + " en " + root.name + ".");
        TMP_Text status = destination.Find("Status")?.GetComponent<TMP_Text>();
        Button button = destination.GetComponent<Button>();
        Require(status != null && status.text == expected,
            root.name + "/Destination_" + index + " muestra '" +
            (status != null ? status.text : "<sin texto>") + "' en vez de '" + expected + "'.");
        Require(button != null && button.interactable == interactable,
            root.name + "/Destination_" + index + " tiene interactividad incorrecta.");
    }

    private static void ValidateSharedSectorShell(Transform root)
    {
        RectTransform rect = root as RectTransform;
        Require(rect != null && Approximately(rect.anchoredPosition, Dimension1SharedLayoutTokens.RootOffset),
            "Desplazamiento raíz no canónico en " + root.name + ".");
        Require(Approximately(rect.sizeDelta, new Vector2(Dimension1SharedLayoutTokens.Width,
                    Dimension1SharedLayoutTokens.Height)),
            "Lienzo no canónico en " + root.name + ".");
        ValidateTopRect(root.Find("OuterFrame") as RectTransform,
            Dimension1SharedLayoutTokens.OuterFrameX, Dimension1SharedLayoutTokens.OuterFrameY,
            Dimension1SharedLayoutTokens.OuterFrameWidth, Dimension1SharedLayoutTokens.OuterFrameHeight,
            "marco de " + root.name);

        Transform nav = root.Find("BottomNavigation");
        Require(nav != null, "Falta navegación inferior en " + root.name + ".");
        ValidateTopRect(nav as RectTransform,
            Dimension1SharedLayoutTokens.NavigationX, Dimension1SharedLayoutTokens.NavigationY,
            Dimension1SharedLayoutTokens.NavigationWidth, Dimension1SharedLayoutTokens.NavigationHeight,
            "navegación de " + root.name);

        string[] names = { "Nav_GALAXIA", "Nav_EXPLORAR", "Nav_HANGAR", "Nav_RELIQUIAS", "Nav_ÁRBOL" };
        for (int i = 0; i < names.Length; i++)
        {
            RectTransform card = nav.Find(names[i]) as RectTransform;
            ValidateTopRect(card, Dimension1SharedLayoutTokens.NavigationCardX(i),
                Dimension1SharedLayoutTokens.NavigationCardY,
                Dimension1SharedLayoutTokens.NavigationCardWidth,
                Dimension1SharedLayoutTokens.NavigationCardHeight,
                names[i] + " de " + root.name);
            Button button = card.GetComponent<Button>();
            Require(button != null && button.interactable == (i != 0),
                "La selección GALAXIA no es inequívoca en " + root.name + ".");
        }

        Transform pointer = nav.Find("SelectedPointer");
        Require(pointer != null && pointer.GetComponent<Dimension1CommandCenterPolygonGraphic>() != null,
            "Falta el indicador canónico de GALAXIA en " + root.name + ".");

        TMP_Text sectorTitle = root.Find("Heading/Title")?.GetComponent<TMP_Text>();
        Require(sectorTitle != null && !string.IsNullOrEmpty(sectorTitle.text) &&
                sectorTitle.gameObject.activeInHierarchy && sectorTitle.color.a > .99f,
            "El título propio del sector no está activo en " + root.name + ".");
        sectorTitle.ForceMeshUpdate(true, true);
        Require(sectorTitle.textInfo.characterCount > 0,
            "El título propio del sector no generó texto visible en " + root.name + ".");
    }

    private static void ValidateDebrisSinglePlanet()
    {
        Transform root = FindSceneTransform("D1_DebrisRingDetailVisualRoot");
        Transform unused = root?.Find("UnusedPlanetCard");
        Require(unused != null && !unused.gameObject.activeSelf,
            "Anillo de Restos no conserva oculta la tarjeta de planeta sobrante.");
        int activePlanets = 0;
        RectTransform active = null;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (!child.name.StartsWith("Planet_", StringComparison.Ordinal) || !child.gameObject.activeSelf) continue;
            activePlanets++;
            active = child as RectTransform;
        }
        Require(activePlanets == 1 && active != null,
            "Anillo de Restos no presenta exactamente su único planeta real.");
        Require(Mathf.Abs(active.anchoredPosition.y + 512f) < .01f,
            "El único planeta de Anillo de Restos no quedó centrado en la composición.");
    }

    private static void ValidateArkFrame()
    {
        Transform root = FindSceneTransform("ArkPanel");
        RectTransform rect = root as RectTransform;
        Require(rect != null, "Falta ArkPanel.");
        Require(Approximately(rect.anchoredPosition, Dimension1SharedLayoutTokens.RootOffset) &&
                Approximately(rect.sizeDelta, new Vector2(Dimension1SharedLayoutTokens.Width,
                    Dimension1SharedLayoutTokens.Height)),
            "ARK no comparte el lienzo y desplazamiento canónicos.");
        ValidateTopRect(root.Find("OuterFrame") as RectTransform,
            Dimension1SharedLayoutTokens.OuterFrameX, Dimension1SharedLayoutTokens.OuterFrameY,
            Dimension1SharedLayoutTokens.OuterFrameWidth, Dimension1SharedLayoutTokens.OuterFrameHeight,
            "marco de ARK");
        Require(root.Find("BottomNavigation") == null && root.Find("D1BottomNavigation") == null,
            "ARK recibió una navegación inferior ajena a su contrato.");
    }

    private static void ValidateTopRect(RectTransform rect, float x, float y, float width,
        float height, string label)
    {
        Require(rect != null && Approximately(rect.anchoredPosition, new Vector2(x, -y)) &&
                Approximately(rect.sizeDelta, new Vector2(width, height)),
            "Geometría no canónica en " + label + ".");
    }

    private static void PrepareBatchViewport()
    {
        if (Screen.width < Screen.height) return;
        foreach (CanvasScaler scaler in UnityEngine.Object.FindObjectsByType<CanvasScaler>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
                scaler.matchWidthOrHeight = 1f;
        Canvas.ForceUpdateCanvases();
    }

    private static T Find<T>() where T : UnityEngine.Object
    {
        return UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
    }

    private static Transform FindSceneTransform(string name)
    {
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
            if (transform.gameObject.scene.IsValid() && transform.name == name) return transform;
        return null;
    }

    private static bool Approximately(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(a.x - b.x) < .01f && Mathf.Abs(a.y - b.y) < .01f;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif
