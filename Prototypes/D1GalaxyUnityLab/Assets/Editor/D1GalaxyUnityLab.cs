#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class D1GalaxyUnityLab
{
    private const int Width = 720;
    private const int Height = 1280;
    private static readonly string OutputDirectory = Path.GetFullPath("Captures");
    private static Material galaxyMaterial;
    private static Camera labCamera;

    public static void BuildAndCapture()
    {
        Directory.CreateDirectory(OutputDirectory);
        SceneView.lastActiveSceneView?.Close();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject cameraObject = new GameObject("LabCamera");
        labCamera = cameraObject.AddComponent<Camera>();
        labCamera.orthographic = true;
        labCamera.orthographicSize = Height * 0.5f;
        labCamera.transform.position = new Vector3(0f, 0f, -10f);
        labCamera.clearFlags = CameraClearFlags.SolidColor;
        labCamera.backgroundColor = new Color32(2, 8, 14, 255);

        Texture2D starfield = LoadTexture("Assets/Art/d1_starfield.png");
        Texture2D galaxy = LoadTexture("Assets/Art/d1_galaxy_option1_animated_v7.png");
        Texture2D frame = LoadTexture("Assets/Art/d1_premium_frame_v4.png");

        CreateSprite("Starfield", starfield, new Vector2(0f, 170f), new Vector2(720f, 940f),
            new Color(0.55f, 0.80f, 0.90f, 0.30f), -5);

        Shader shader = Shader.Find("QuantumForgeLab/GalaxyFlow");
        if (shader == null) throw new InvalidOperationException("GalaxyFlow shader no compiló.");
        galaxyMaterial = new Material(shader);
        galaxyMaterial.SetTexture("_MainTex", galaxy);
        galaxyMaterial.SetVector("_Center", new Vector4(0.47f, 0.53f, 0f, 0f));
        galaxyMaterial.SetFloat("_AxisRatio", 0.68f);
        galaxyMaterial.SetFloat("_FlowSpeed", 0.23f);
        galaxyMaterial.SetFloat("_ArmFlow", 0.72f);
        galaxyMaterial.SetFloat("_DustFlow", 0.36f);
        SpriteRenderer galaxyRenderer = CreateSprite("FlowingGalaxy", galaxy,
            new Vector2(0f, 195f), new Vector2(650f, 650f), Color.white, -2);
        galaxyRenderer.sharedMaterial = galaxyMaterial;

        CreateTitle("PROTOTIPO · GALAXIA CON FLUJO", new Vector2(0f, 585f), 25, new Color32(225, 243, 250, 255));
        CreateTitle("SIN PARTÍCULAS ORBITANDO", new Vector2(0f, 551f), 14, new Color32(85, 207, 255, 255));

        CreatePanel(frame);

        for (int i = 0; i < 12; i++)
        {
            float time = i * 0.5f;
            galaxyMaterial.SetFloat("_LabTime", time);
            Render(Path.Combine(OutputDirectory, $"D1GalaxyLab_{i:00}_{time:0.0}s.png"));
        }

        galaxyMaterial.SetFloat("_LabTime", 2.5f);
        Render(Path.Combine(OutputDirectory, "D1GalaxyLab_final_720x1280.png"));
        EditorSceneManager.SaveScene(scene, "Assets/D1GalaxyUnityLab.unity");
        Debug.Log("[D1 Galaxy Unity Lab] PASS | proyecto aislado | 12 frames | sin partículas");
    }

    private static void CreatePanel(Texture2D frame)
    {
        CreateSprite("PanelFill", null, new Vector2(0f, -378f), new Vector2(670f, 340f),
            new Color32(4, 15, 24, 252), 2);
        CreateSprite("PanelBorder", frame, new Vector2(0f, -378f), new Vector2(670f, 340f),
            new Color32(104, 136, 148, 255), 3, true);
        CreateTitle("SELECCIONA UN SECTOR", new Vector2(0f, -245f), 27, new Color32(242, 179, 68, 255));
        CreateTitle("TOCA UN SECTOR PARA VER SUS DATOS", new Vector2(0f, -365f), 16, new Color32(148, 173, 188, 255));
        CreateTitle("CENTRO DEL PANEL  X = 0", new Vector2(0f, -503f), 11, new Color32(70, 112, 127, 255));
    }

    private static SpriteRenderer CreateSprite(string name, Texture2D texture, Vector2 position,
        Vector2 size, Color color, int order, bool sliced = false)
    {
        GameObject item = new GameObject(name);
        item.transform.position = new Vector3(position.x, position.y, 0f);
        SpriteRenderer renderer = item.AddComponent<SpriteRenderer>();
        if (texture == null)
        {
            texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
        }
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f), 1f, 0, sliced ? SpriteMeshType.FullRect : SpriteMeshType.Tight,
            sliced ? new Vector4(28f, 28f, 28f, 28f) : Vector4.zero);
        renderer.drawMode = sliced ? SpriteDrawMode.Sliced : SpriteDrawMode.Simple;
        if (sliced)
            renderer.size = size;
        else
            item.transform.localScale = new Vector3(size.x / texture.width, size.y / texture.height, 1f);
        renderer.color = color;
        renderer.sortingOrder = order;
        return renderer;
    }

    private static void CreateTitle(string value, Vector2 position, int fontSize, Color color)
    {
        GameObject item = new GameObject("Text_" + value);
        item.transform.position = new Vector3(position.x, position.y, 0f);
        TextMesh text = item.AddComponent<TextMesh>();
        text.text = value;
        text.fontSize = fontSize;
        text.characterSize = 9f;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = color;
        MeshRenderer renderer = item.GetComponent<MeshRenderer>();
        renderer.sortingOrder = 10;
    }

    private static Texture2D LoadTexture(string path)
    {
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (texture == null) throw new FileNotFoundException(path);
        return texture;
    }

    private static void Render(string path)
    {
        RenderTexture target = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previous = RenderTexture.active;
        labCamera.targetTexture = target;
        labCamera.Render();
        RenderTexture.active = target;
        Texture2D image = new Texture2D(Width, Height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
        image.Apply();
        File.WriteAllBytes(path, image.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(image);
        labCamera.targetTexture = null;
        RenderTexture.active = previous;
        target.Release();
        UnityEngine.Object.DestroyImmediate(target);
    }
}
#endif
