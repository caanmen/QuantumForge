#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public static class MachineCube3DPbrPrototypeSetup
{
    private const string VendorRoot =
        "Assets/SciFi_Materials_vol2/Textures";
    private const string PrototypeRoot =
        "Assets/Project/UI/Vertical/Machine/Prototype3D";
    private const string MaterialFolder = PrototypeRoot + "/Materials/PBR";

    public static bool ApplyCubeArtDirection(GameObject prototypeRoot)
    {
        if (!AssetDatabase.IsValidFolder(VendorRoot))
        {
            Debug.LogWarning("[Machine Cube PBR] Vendor textures are not installed; " +
                "the modular cube keeps its fallback materials.");
            return false;
        }

        EnsureFolder(MaterialFolder);
        PbrTextureSet panel35 = LoadTextureSet(35);
        PbrTextureSet panel32 = LoadTextureSet(32);
        PbrTextureSet panel33 = LoadTextureSet(33);
        PbrTextureSet panel48 = LoadTextureSet(48);

        Material surface35 = CreatePbrMaterial("M3D_PBR_Cube_Surface35",
            panel35, new Color(0.27f, 0.25f, 0.23f, 1f), 1.0f,
            0.21f, 0.96f, 0.78f);
        Material armor32 = CreatePbrMaterial("M3D_PBR_Cube_Armor32",
            panel32, new Color(0.35f, 0.32f, 0.28f, 1f), 0.55f,
            0.25f, 1.0f, 0.86f);
        Material frame32 = CreatePbrMaterial("M3D_PBR_Cube_ExposedFrame32",
            panel32, new Color(0.48f, 0.43f, 0.36f, 1f), 0.62f,
            0.30f, 1.05f, 0.82f);
        Material module33 = CreatePbrMaterial("M3D_PBR_Cube_Module33",
            panel33, new Color(0.30f, 0.28f, 0.24f, 1f), 0.58f,
            0.20f, 0.94f, 0.90f);
        Material conduit33 = CreatePbrMaterial("M3D_PBR_Cube_Conduit33",
            panel33, new Color(0.18f, 0.19f, 0.18f, 1f), 0.72f,
            0.16f, 0.82f, 0.94f);
        Material socket33 = CreatePbrMaterial("M3D_PBR_Cube_Socket33",
            panel33, new Color(0.048f, 0.050f, 0.050f, 1f), 0.68f,
            0.08f, 0.82f, 1.0f);
        Material cavity48 = CreatePbrMaterial("M3D_PBR_Cube_CarbonCavity48",
            panel48, new Color(0.018f, 0.010f, 0.006f, 1f), 0.88f,
            0.035f, 1.15f, 1.0f);
        Material torn48 = CreatePbrMaterial("M3D_PBR_Cube_TornArmor48",
            panel48, new Color(0.40f, 0.32f, 0.24f, 1f), 0.72f,
            0.17f, 1.10f, 0.92f);

        Material[] originalFaces =
        {
            LoadPrototypeMaterial("M3D_Face1Surface"),
            LoadPrototypeMaterial("M3D_Face2Surface"),
            LoadPrototypeMaterial("M3D_Face3Surface"),
            LoadPrototypeMaterial("M3D_Face4Surface")
        };
        Material originalBody = LoadPrototypeMaterial("M3D_Body");
        Material originalFrame = LoadPrototypeMaterial("M3D_Frame");
        Material originalModule = LoadPrototypeMaterial("M3D_Module");
        Material originalTrim = LoadPrototypeMaterial("M3D_Trim");
        Material originalChannel = LoadPrototypeMaterial("M3D_Channel");
        Material originalDamage = LoadPrototypeMaterial("M3D_DamageMark");

        int surfaceCount = 0;
        int armorCount = 0;
        int frameCount = 0;
        int moduleCount = 0;
        int conduitCount = 0;
        int socketCount = 0;
        int damageCount = 0;
        for (int faceIndex = 0; faceIndex < originalFaces.Length; faceIndex++)
        {
            Transform face = prototypeRoot != null
                ? prototypeRoot.transform.Find(
                    $"PhysicalCubeRoot/PhysicalFace_{faceIndex + 1}")
                : null;
            if (face == null)
                throw new InvalidOperationException(
                    $"[Machine Cube PBR] PhysicalFace_{faceIndex + 1} was not generated.");
            Material originalFace = originalFaces[faceIndex];
            foreach (Renderer renderer in face.GetComponentsInChildren<Renderer>(true))
            {
                bool insideDamage = IsInsideDamageModule(renderer.transform, face);
                Material[] assigned = renderer.sharedMaterials;
                bool materialsChanged = false;
                for (int materialIndex = 0; materialIndex < assigned.Length;
                    materialIndex++)
                {
                    Material current = assigned[materialIndex];
                    if (current == null)
                        continue;

                    Material replacement = null;
                    if (insideDamage && IsStructuralMaterial(current,
                            originalFace, originalDamage))
                    {
                        replacement = cavity48;
                        damageCount++;
                    }
                    else if (insideDamage && IsStructuralMaterial(current,
                                 originalBody, originalFrame, originalModule))
                    {
                        replacement = torn48;
                        damageCount++;
                    }
                    else if (insideDamage && IsStructuralMaterial(current,
                                 originalTrim, originalChannel))
                    {
                        replacement = frame32;
                        damageCount++;
                    }
                    else if (current == originalFace)
                    {
                        replacement = surface35;
                        surfaceCount++;
                    }
                    else if (current == originalBody)
                    {
                        replacement = armor32;
                        armorCount++;
                    }
                    else if (current == originalFrame || current == originalTrim)
                    {
                        replacement = frame32;
                        frameCount++;
                    }
                    else if (current == originalModule)
                    {
                        replacement = module33;
                        moduleCount++;
                    }
                    else if (current == originalDamage &&
                             (renderer.name == "PhysicalMetalBackplate" ||
                              renderer.name == "RearArmor"))
                    {
                        replacement = cavity48;
                        damageCount++;
                    }
                    else if (current == originalDamage)
                    {
                        replacement = socket33;
                        socketCount++;
                    }
                    else if (current == originalChannel)
                    {
                        replacement = conduit33;
                        conduitCount++;
                    }

                    if (replacement != null)
                    {
                        assigned[materialIndex] = replacement;
                        materialsChanged = true;
                    }
                }
                if (materialsChanged)
                    renderer.sharedMaterials = assigned;
            }
        }

        if (surfaceCount < 4 || armorCount == 0 || frameCount == 0 ||
            moduleCount == 0 || conduitCount == 0 || socketCount == 0 ||
            damageCount == 0)
        {
            throw new InvalidOperationException(
                $"[Machine Cube PBR] Incomplete cube assignment: surface={surfaceCount}, " +
                $"armor={armorCount}, frames={frameCount}, modules={moduleCount}, " +
                $"conduits={conduitCount}, sockets={socketCount}, damage={damageCount}.");
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[Machine Cube PBR] FULL CUBE ART DIRECTION | Panel35 surface={surfaceCount} | " +
            $"Panel32 armor={armorCount} | Panel33 modules={moduleCount} " +
            $"frames={frameCount} conduits={conduitCount} sockets={socketCount} | " +
            $"Panel48 layered damage={damageCount} | " +
            "URP Lit | Android ASTC 6x6 1024 | no parallax");
        return true;
    }

    public static bool ApplyFace1Preview(GameObject prototypeRoot)
    {
        return ApplyCubeArtDirection(prototypeRoot);
    }

    private static PbrTextureSet LoadTextureSet(int panelNumber)
    {
        string id = panelNumber.ToString("00");
        string folder = $"{VendorRoot}/SciFiPanels{id}";
        Texture2D albedo = LoadAndConfigureTexture(
            $"{folder}/SciFiPanels{id}_a.png", TextureRole.Albedo);
        Texture2D normal = LoadAndConfigureTexture(
            $"{folder}/SciFiPanels{id}_n.png", TextureRole.Normal);
        Texture2D metallic = LoadAndConfigureTexture(
            $"{folder}/SciFiPanels{id}_m.png", TextureRole.MetallicSmoothness);
        Texture2D occlusion = LoadAndConfigureTexture(
            $"{folder}/SciFiPanels{id}_o.png", TextureRole.Occlusion);
        return new PbrTextureSet(albedo, normal, metallic, occlusion);
    }

    private static Texture2D LoadAndConfigureTexture(string path, TextureRole role)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            throw new InvalidOperationException(
                "[Machine Cube PBR] Missing texture importer: " + path);

        bool normalMap = role == TextureRole.Normal;
        bool dataMap = role != TextureRole.Albedo;
        bool changed = false;
        if (importer.wrapMode != TextureWrapMode.Repeat)
        {
            importer.wrapMode = TextureWrapMode.Repeat;
            changed = true;
        }
        if (importer.filterMode != FilterMode.Trilinear)
        {
            importer.filterMode = FilterMode.Trilinear;
            changed = true;
        }
        if (!importer.mipmapEnabled)
        {
            importer.mipmapEnabled = true;
            changed = true;
        }
        if (importer.anisoLevel != 4)
        {
            importer.anisoLevel = 4;
            changed = true;
        }
        if (importer.maxTextureSize != 2048)
        {
            importer.maxTextureSize = 2048;
            changed = true;
        }
        if (importer.textureCompression != TextureImporterCompression.CompressedHQ)
        {
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            changed = true;
        }
        TextureImporterType expectedType = normalMap
            ? TextureImporterType.NormalMap
            : TextureImporterType.Default;
        if (importer.textureType != expectedType)
        {
            importer.textureType = expectedType;
            changed = true;
        }
        bool expectedSrgb = !dataMap;
        if (importer.sRGBTexture != expectedSrgb)
        {
            importer.sRGBTexture = expectedSrgb;
            changed = true;
        }
        if (role == TextureRole.MetallicSmoothness &&
            importer.alphaSource != TextureImporterAlphaSource.FromInput)
        {
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            changed = true;
        }

        TextureImporterPlatformSettings android =
            importer.GetPlatformTextureSettings("Android");
        if (!android.overridden || android.maxTextureSize != 1024 ||
            android.format != TextureImporterFormat.ASTC_6x6 ||
            android.compressionQuality != 75)
        {
            android.name = "Android";
            android.overridden = true;
            android.maxTextureSize = 1024;
            android.format = TextureImporterFormat.ASTC_6x6;
            android.compressionQuality = 75;
            importer.SetPlatformTextureSettings(android);
            changed = true;
        }

        if (changed)
            importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path) ??
            throw new InvalidOperationException(
                "[Machine Cube PBR] Texture failed to load: " + path);
    }

    private static Material CreatePbrMaterial(string name, PbrTextureSet textures,
        Color tint, float tiling, float smoothness, float normalStrength,
        float occlusionStrength)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            throw new InvalidOperationException(
                "[Machine Cube PBR] Universal Render Pipeline/Lit is unavailable.");

        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(material, path);
        }

        material.shader = shader;
        material.enableInstancing = true;
        material.SetColor("_BaseColor", tint);
        material.SetTexture("_BaseMap", textures.albedo);
        material.SetTextureScale("_BaseMap", Vector2.one * tiling);
        if (material.HasProperty("_MainTex"))
        {
            material.SetTexture("_MainTex", textures.albedo);
            material.SetTextureScale("_MainTex", Vector2.one * tiling);
        }

        material.SetFloat("_WorkflowMode", 1f);
        material.SetFloat("_Metallic", 1f);
        material.SetTexture("_MetallicGlossMap", textures.metallicSmoothness);
        material.SetFloat("_Smoothness", smoothness);
        material.SetFloat("_SmoothnessTextureChannel", 0f);
        material.EnableKeyword("_METALLICSPECGLOSSMAP");
        material.DisableKeyword("_SPECGLOSSMAP");

        material.SetTexture("_BumpMap", textures.normal);
        material.SetFloat("_BumpScale", normalStrength);
        material.EnableKeyword("_NORMALMAP");
        material.SetTexture("_OcclusionMap", textures.occlusion);
        material.SetFloat("_OcclusionStrength", occlusionStrength);
        material.EnableKeyword("_OCCLUSIONMAP");

        material.DisableKeyword("_PARALLAXMAP");
        if (material.HasProperty("_ParallaxMap"))
            material.SetTexture("_ParallaxMap", null);
        material.DisableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", Color.black);
        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static Material LoadPrototypeMaterial(string name)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(
            PrototypeRoot + "/" + name + ".mat");
        if (material == null)
            throw new InvalidOperationException(
                "[Machine Cube PBR] Missing prototype material: " + name);
        return material;
    }

    private static bool IsStructuralMaterial(Material material,
        params Material[] candidates)
    {
        for (int i = 0; i < candidates.Length; i++)
            if (material == candidates[i])
                return true;
        return false;
    }

    private static bool IsInsideDamageModule(Transform current, Transform face)
    {
        for (Transform item = current; item != null; item = item.parent)
        {
            MachineCube3DModule module = item.GetComponent<MachineCube3DModule>();
            if (module != null && module.Role == MachineCube3DModuleRole.Damage)
                return true;
            if (item == face)
                break;
        }
        return false;
    }

    private static void EnsureFolder(string path)
    {
        string[] parts = path.Replace('\\', '/').Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    private enum TextureRole
    {
        Albedo,
        Normal,
        MetallicSmoothness,
        Occlusion
    }

    private sealed class PbrTextureSet
    {
        public readonly Texture2D albedo;
        public readonly Texture2D normal;
        public readonly Texture2D metallicSmoothness;
        public readonly Texture2D occlusion;

        public PbrTextureSet(Texture2D albedo, Texture2D normal,
            Texture2D metallicSmoothness, Texture2D occlusion)
        {
            this.albedo = albedo;
            this.normal = normal;
            this.metallicSmoothness = metallicSmoothness;
            this.occlusion = occlusion;
        }
    }
}
#endif
