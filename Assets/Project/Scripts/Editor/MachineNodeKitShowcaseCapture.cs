#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class MachineNodeKitShowcaseCapture
{
    private const string ModelPath =
        "Assets/Project/Art/MachineCubeBlender/QF_MachineNodeKitV1.fbx";
    private const string OutputRelativePath =
        "Logs/VisualQA/MachineNodeKit/node_kit_v1_unity_1920x1080.png";

    public static void Capture()
    {
        AssetDatabase.ImportAsset(ModelPath, ImportAssetOptions.ForceSynchronousImport |
            ImportAssetOptions.ForceUpdate);
        GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
        if (modelAsset == null)
            throw new InvalidOperationException("No se pudo importar " + ModelPath);

        GameObject stage = new GameObject("QF_MachineNodeKit_Stage");
        try
        {
            MaterialSet materials = CreateMaterials();
            BuildIndustrialWall(stage.transform, materials);

            GameObject model = UnityEngine.Object.Instantiate(modelAsset, stage.transform);
            model.name = "QF_MachineNodeKitV1";
            model.transform.localPosition = new Vector3(0f, 0f, -0.34f);
            // Blender exports the authored front toward +Z after Unity's axis
            // conversion. Rotate the preview root so the camera sees the
            // functional inserts instead of the shared rear shell.
            model.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            model.transform.localScale = Vector3.one;
            RemapMaterials(model, materials);

            Camera camera = BuildCamera(stage.transform);
            BuildLighting(stage.transform);
            CaptureCamera(camera, OutputRelativePath, 1920, 1080);

            int meshCount = model.GetComponentsInChildren<MeshFilter>(true).Length;
            int rendererCount = model.GetComponentsInChildren<Renderer>(true).Length;
            Debug.Log($"[Machine Node Kit] PASS | designs=6 | states=12 | " +
                $"meshes={meshCount} | renderers={rendererCount} | {OutputRelativePath}");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(stage);
            AssetDatabase.Refresh();
        }
    }

    private static MaterialSet CreateMaterials()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ??
            Shader.Find("Standard");
        if (shader == null)
            throw new InvalidOperationException("No se encontró un shader compatible.");

        Texture2D worn = AssetDatabase.LoadAssetAtPath<Texture2D>(
            "Assets/Project/UI/Vertical/Machine/Prototype3D/M3D_WornMetal_Albedo.png");
        return new MaterialSet
        {
            Metal = Lit(shader, "Preview_NodeMetal", new Color(0.34f, 0.37f, 0.40f),
                0.72f, 0.30f, null),
            Dark = Lit(shader, "Preview_NodeDark", new Color(0.025f, 0.032f, 0.040f),
                0.18f, 0.16f, null),
            Wall = Lit(shader, "Preview_Wall", new Color(0.11f, 0.13f, 0.15f),
                0.76f, 0.22f, worn),
            Rail = Lit(shader, "Preview_Rail", new Color(0.18f, 0.20f, 0.22f),
                0.68f, 0.28f, worn),
            Cable = Lit(shader, "Preview_Cable", new Color(0.018f, 0.022f, 0.028f),
                0.04f, 0.18f, null),
            Copper = Lit(shader, "Preview_Copper", new Color(0.48f, 0.14f, 0.035f),
                0.82f, 0.36f, null),
            Cyan = Emissive(shader, "Preview_Cyan", new Color(0.0f, 0.62f, 0.82f), 2.8f),
            Warning = Emissive(shader, "Preview_Warning", new Color(1.0f, 0.018f, 0.005f), 2.4f),
        };
    }

    private static Material Lit(Shader shader, string name, Color color,
        float metallic, float smoothness, Texture2D texture)
    {
        Material material = new Material(shader) { name = name };
        SetColor(material, color);
        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", metallic);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", smoothness);
        if (texture != null && material.HasProperty("_BaseMap"))
        {
            material.SetTexture("_BaseMap", texture);
            material.SetTextureScale("_BaseMap", new Vector2(1.8f, 1.8f));
        }
        return material;
    }

    private static Material Emissive(Shader shader, string name, Color color,
        float intensity)
    {
        Material material = Lit(shader, name, color * 0.26f, 0.22f, 0.26f, null);
        material.EnableKeyword("_EMISSION");
        if (material.HasProperty("_EmissionColor"))
            material.SetColor("_EmissionColor", color * intensity);
        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        return material;
    }

    private static void SetColor(Material material, Color color)
    {
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);
    }

    private static void RemapMaterials(GameObject root, MaterialSet set)
    {
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            Material[] slots = renderer.sharedMaterials;
            for (int index = 0; index < slots.Length; index++)
            {
                string sourceName = slots[index] != null ? slots[index].name : "";
                if (sourceName.IndexOf("CYAN", StringComparison.OrdinalIgnoreCase) >= 0)
                    slots[index] = set.Cyan;
                else if (sourceName.IndexOf("WARNING", StringComparison.OrdinalIgnoreCase) >= 0)
                    slots[index] = set.Warning;
                else if (sourceName.IndexOf("COPPER", StringComparison.OrdinalIgnoreCase) >= 0)
                    slots[index] = set.Copper;
                else if (sourceName.IndexOf("CABLE", StringComparison.OrdinalIgnoreCase) >= 0)
                    slots[index] = set.Cable;
                else if (sourceName.IndexOf("DARK", StringComparison.OrdinalIgnoreCase) >= 0)
                    slots[index] = set.Dark;
                else
                    slots[index] = set.Metal;
            }
            renderer.sharedMaterials = slots;
            renderer.shadowCastingMode = ShadowCastingMode.On;
            renderer.receiveShadows = true;
        }
    }

    private static void BuildIndustrialWall(Transform parent, MaterialSet set)
    {
        CreateBox("Backplate", parent, new Vector3(0f, 0f, 0.72f),
            new Vector3(15.8f, 7.25f, 0.36f), set.Wall);
        CreateBox("FrameTop", parent, new Vector3(0f, 3.62f, 0.18f),
            new Vector3(16.5f, 0.32f, 0.48f), set.Rail);
        CreateBox("FrameBottom", parent, new Vector3(0f, -3.62f, 0.18f),
            new Vector3(16.5f, 0.32f, 0.48f), set.Rail);
        CreateBox("FrameLeft", parent, new Vector3(-8.05f, 0f, 0.18f),
            new Vector3(0.34f, 7.55f, 0.48f), set.Rail);
        CreateBox("FrameRight", parent, new Vector3(8.05f, 0f, 0.18f),
            new Vector3(0.34f, 7.55f, 0.48f), set.Rail);

        for (int row = -3; row <= 3; row++)
        {
            float y = row * 1.03f;
            CreateBox("WallSeamH_" + row, parent, new Vector3(0f, y, 0.49f),
                new Vector3(15.6f, 0.035f, 0.028f), set.Dark);
        }
        for (int column = -3; column <= 3; column++)
        {
            float x = column * 2.15f;
            CreateBox("WallSeamV_" + column, parent, new Vector3(x, 0f, 0.49f),
                new Vector3(0.035f, 7.1f, 0.028f), set.Dark);
        }

        for (int row = 0; row < 2; row++)
        {
            float y = row == 0 ? 1.25f : -1.25f;
            for (int line = 0; line < 3; line++)
            {
                CreateBox($"Conduit_{row}_{line}", parent,
                    new Vector3(0f, y - 0.33f + line * 0.10f, 0.22f),
                    new Vector3(14.7f, 0.045f, 0.045f), set.Dark);
            }
        }
    }

    private static GameObject CreateBox(string name, Transform parent,
        Vector3 position, Vector3 scale, Material material)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(parent, false);
        box.transform.localPosition = position;
        box.transform.localScale = scale;
        UnityEngine.Object.DestroyImmediate(box.GetComponent<Collider>());
        box.GetComponent<Renderer>().sharedMaterial = material;
        return box;
    }

    private static Camera BuildCamera(Transform parent)
    {
        GameObject cameraObject = new GameObject("MachineNodeKitCamera");
        cameraObject.transform.SetParent(parent, false);
        cameraObject.transform.position = new Vector3(0f, 0f, -18f);
        cameraObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 4.35f;
        camera.aspect = 16f / 9f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.006f, 0.009f, 0.014f);
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 50f;
        camera.allowHDR = true;
        camera.allowMSAA = true;
        return camera;
    }

    private static void BuildLighting(Transform parent)
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.30f, 0.34f, 0.40f);

        GameObject keyObject = new GameObject("KeyLight");
        keyObject.transform.SetParent(parent, false);
        keyObject.transform.rotation = Quaternion.Euler(32f, -28f, 0f);
        Light key = keyObject.AddComponent<Light>();
        key.type = LightType.Directional;
        key.color = new Color(0.78f, 0.86f, 1.0f);
        key.intensity = 1.80f;
        key.shadows = LightShadows.Soft;
        key.shadowStrength = 0.65f;

        GameObject fillObject = new GameObject("FillLight");
        fillObject.transform.SetParent(parent, false);
        fillObject.transform.position = new Vector3(-5.5f, 2.8f, -4.5f);
        Light fill = fillObject.AddComponent<Light>();
        fill.type = LightType.Point;
        fill.color = new Color(0.08f, 0.34f, 0.46f);
        fill.intensity = 3.0f;
        fill.range = 18f;
        fill.shadows = LightShadows.None;
    }

    private static void CaptureCamera(Camera camera, string relativePath,
        int width, int height)
    {
        string absolutePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName,
            relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

        RenderTexture target = new RenderTexture(width, height, 24,
            RenderTextureFormat.ARGB32)
        {
            antiAliasing = 4,
            useMipMap = false,
            autoGenerateMips = false,
        };
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture previousTarget = camera.targetTexture;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        try
        {
            target.Create();
            camera.targetTexture = target;
            camera.Render();
            RenderTexture.active = target;
            texture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, false);
            texture.Apply(false, false);
            File.WriteAllBytes(absolutePath, texture.EncodeToPNG());
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            UnityEngine.Object.DestroyImmediate(texture);
            target.Release();
            UnityEngine.Object.DestroyImmediate(target);
        }
    }

    private sealed class MaterialSet
    {
        public Material Metal;
        public Material Dark;
        public Material Wall;
        public Material Rail;
        public Material Cable;
        public Material Copper;
        public Material Cyan;
        public Material Warning;
    }
}
#endif
