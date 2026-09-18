#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Dimension1CommandCenterDrawerRuntimeValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1CommandDrawerValidation.Active";
    private const string FailedKey = "QF.D1CommandDrawerValidation.Failed";
    private const string FrameKey = "QF.D1CommandDrawerValidation.Frame";

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (EditorApplication.isPlaying)
        {
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

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
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            SaveService.SuppressWritesForVisualQa = false;
            EditorApplication.update -= Tick;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            bool failed = SessionState.GetBool(FailedKey, false);
            SessionState.SetBool(ActiveKey, false);
            Debug.Log(failed
                ? "[D1 Command Drawer Runtime] FAIL"
                : "[D1 Command Drawer Runtime] PASS | cerrado -> abierto -> cerrado | sin superposición");
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            TickCore();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            SessionState.SetBool(FailedKey, true);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void TickCore()
    {
        int frame = SessionState.GetInt(FrameKey, 0) + 1;
        SessionState.SetInt(FrameKey, frame);

        if (frame == 20)
        {
            GameState state = GameState.I != null
                ? GameState.I
                : UnityEngine.Object.FindFirstObjectByType<GameState>();
            if (state == null) throw new InvalidOperationException("GameState no inició.");
            state.dimension01Unlocked = true;
            state.EnsureDimension1State();

            TabsUI tabs = TabsUI.Instance != null
                ? TabsUI.Instance
                : UnityEngine.Object.FindFirstObjectByType<TabsUI>();
            if (tabs == null) throw new InvalidOperationException("TabsUI no inició.");
            tabs.ShowDimension1();
        }

        if (frame == 45)
        {
            ValidateState(false);
            Find("DimensionDrawerToggle").GetComponent<Button>().onClick.Invoke();
        }
        else if (frame == 52)
        {
            ValidateState(true);
            Find("DimensionDrawerToggle").GetComponent<Button>().onClick.Invoke();
        }
        else if (frame == 59)
        {
            ValidateState(false);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void ValidateState(bool expanded)
    {
        Transform commandRoot = Find("D1CommandCenterProductionRoot").transform;
        GameObject primary = Find("PrimaryNavigationSlot");
        GameObject secondary = Find("SecondaryNavigationSlot");
        if (primary.activeSelf)
            throw new InvalidOperationException("La navegación principal invadió el Centro de Mando.");
        if (secondary.activeSelf != expanded)
            throw new InvalidOperationException("El carrusel global no coincide con el estado de la bandeja.");
        if (expanded && secondary.GetComponentsInChildren<Button>(false).Length == 0)
            throw new InvalidOperationException("La bandeja está abierta pero no muestra controles globales.");

        foreach (string name in new[]
        {
            "Nav_GALAXIA", "Nav_EXPLORAR", "Nav_HANGAR", "Nav_RELIQUIAS", "Nav_ÁRBOL"
        })
        {
            GameObject commandCard = FindCommandChild(commandRoot, name).gameObject;
            if (commandCard.activeSelf == expanded)
                throw new InvalidOperationException(name + " se mezcló con el carrusel global.");
        }

        GameObject drawerToggle = FindCommandChild(commandRoot, "DimensionDrawerToggle").gameObject;
        TMP_Text tmpLabel = drawerToggle.GetComponentInChildren<TMP_Text>(true);
        Text label = drawerToggle.GetComponentInChildren<Text>(true);
        string value = tmpLabel != null ? tmpLabel.text : label != null ? label.text : "";
        string expected = expanded ? "CENTRO DE MANDO ▲" : "DIMENSIONES ▼";
        if (value != expected)
            throw new InvalidOperationException("La pestaña no muestra el estado correcto.");

        VerticalNavigationUI navigation = UnityEngine.Object.FindFirstObjectByType<VerticalNavigationUI>(
            FindObjectsInactive.Include);
        if (navigation == null || navigation.tabs == null ||
            navigation.tabs.btnDimension2 != navigation.dimension2Button)
        {
            throw new InvalidOperationException("El botón visible de Dimensión 2 no está conectado.");
        }
    }

    private static Transform FindCommandChild(Transform root, string name)
    {
        foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
            if (current.name == name) return current;
        throw new InvalidOperationException("No se encontró " + root.name + "/" + name + ".");
    }

    private static GameObject Find(string name)
    {
        foreach (Transform current in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (current.gameObject.scene.IsValid() && current.name == name)
                return current.gameObject;
        }
        throw new InvalidOperationException("No se encontró " + name + ".");
    }
}
#endif
