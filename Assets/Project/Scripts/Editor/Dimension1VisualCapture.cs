#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Dimension1VisualCapture
{
    private const string ActiveKey = "QF.Dimension1VisualCapture.Active";
    private const string IndexKey = "QF.Dimension1VisualCapture.Index";
    private const string FramesKey = "QF.Dimension1VisualCapture.Frames";
    private static readonly Vector2Int[] Sizes =
    {
        new(1344, 600), new(1920, 1080), new(1080, 1920)
    };
    private static string OutputDirectory => Path.GetFullPath("Logs/VisualQA");

    [InitializeOnLoadMethod]
    private static void ResumeAfterDomainReload()
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
        Directory.CreateDirectory(OutputDirectory);
        EditorSceneManager.OpenScene("Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetInt(IndexKey, 0);
        SessionState.SetInt(FramesKey, 0);
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    public static void RunEditorOnly()
    {
        Directory.CreateDirectory(OutputDirectory);
        EditorSceneManager.OpenScene("Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        Dimension1DarkThemeRuntime theme = Object.FindFirstObjectByType<Dimension1DarkThemeRuntime>(FindObjectsInactive.Include);
        if (theme != null) theme.ApplyTheme();
        SetNamedActive("Dimension1Panel", false);
        SetNamedActive("Dimension2Panel", false);
        SetNamedActive("Dimension3Panel", false);
        SetNamedActive("Panel_Generacion", true);
        SetNamedActive("Panel_HUD", true);
        SetNamedActive("HUD", true);
        SetNamedActive("BottomDrawer", true);
        foreach (Vector2Int size in Sizes)
        {
            string path = Path.Combine(OutputDirectory, $"dimension1_{size.x}x{size.y}.png");
            RenderToPng(size.x, size.y, path);
        }
        Debug.Log("[Dimension1VisualCapture] EDITOR PASS · 1344x600 · 1920x1080 · 1080x1920 · sin Play Mode ni guardado.");
    }

    private static void SetNamedActive(string objectName, bool active)
    {
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (!transform.gameObject.scene.IsValid() || transform.name != objectName) continue;
            if (active)
                for (Transform current = transform; current != null; current = current.parent) current.gameObject.SetActive(true);
            else transform.gameObject.SetActive(false);
            return;
        }
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
            EditorApplication.update += Tick;
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            SessionState.SetBool(ActiveKey, false);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            Debug.Log("[Dimension1VisualCapture] PASS · 1344x600 · 1920x1080 · 1080x1920");
            EditorApplication.Exit(0);
        }
    }

    private static void Tick()
    {
        int frames = SessionState.GetInt(FramesKey, 0) + 1;
        SessionState.SetInt(FramesKey, frames);
        if (frames < 30) return;
        int sizeIndex = SessionState.GetInt(IndexKey, 0);
        if (sizeIndex >= Sizes.Length)
        {
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
            return;
        }
        Vector2Int size = Sizes[sizeIndex];
        SessionState.SetInt(IndexKey, sizeIndex + 1);
        string path = Path.Combine(OutputDirectory, $"dimension1_{size.x}x{size.y}.png");
        RenderToPng(size.x, size.y, path);
        SessionState.SetInt(FramesKey, 0);
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new System.InvalidOperationException("No hay cámara para la captura visual.");
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var modes = new RenderMode[canvases.Length];
        var cameras = new Camera[canvases.Length];
        for (int i = 0; i < canvases.Length; i++)
        {
            modes[i] = canvases[i].renderMode;
            cameras[i] = canvases[i].worldCamera;
            if (modes[i] == RenderMode.ScreenSpaceOverlay)
            {
                canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                canvases[i].worldCamera = camera;
                canvases[i].planeDistance = 1f;
            }
        }
        RenderTexture target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            camera.targetTexture = target;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            Object.DestroyImmediate(image);
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            target.Release();
            Object.DestroyImmediate(target);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].renderMode = modes[i];
                canvases[i].worldCamera = cameras[i];
            }
        }
    }
}
#endif
