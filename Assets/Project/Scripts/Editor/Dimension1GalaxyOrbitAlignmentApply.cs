#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Dimension1GalaxyOrbitAlignmentApply
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static readonly string[] NodeNames =
    {
        "Sector01", "Planet02Node", "Sector02", "Sector03",
        "Planet05Node", "Sector04", "Planet07Node"
    };
    private static readonly Vector2[] OrbitSizes =
    {
        new Vector2(430f, 250f), new Vector2(590f, 330f),
        new Vector2(760f, 420f), new Vector2(920f, 510f),
        new Vector2(1100f, 610f), new Vector2(1280f, 720f),
        new Vector2(1480f, 830f)
    };
    private static readonly Vector2[] NodeHints =
    {
        new Vector2(735f, 585f), new Vector2(360f, 790f),
        new Vector2(850f, 705f), new Vector2(225f, 475f),
        new Vector2(790f, 325f), new Vector2(850f, 1005f),
        new Vector2(150f, 1015f)
    };

    [MenuItem("Quantum Forge/Dimension 1/Align Galaxy Bodies To Orbits")]
    public static void Apply()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Transform root = FindSceneTransform("D1_GalaxyVisualRoot");
        if (root == null) throw new InvalidOperationException("No existe D1_GalaxyVisualRoot.");

        Vector2 center = new Vector2(540f, 650f);
        for (int i = 0; i < NodeNames.Length; i++)
        {
            RectTransform node = FindChild(root, NodeNames[i]) as RectTransform;
            if (node == null) throw new InvalidOperationException("Falta el nodo " + NodeNames[i] + ".");
            Vector2 top = AlignNodeCenterToOrbit(center, OrbitSizes[i], -8f, NodeHints[i]);
            node.anchoredPosition = new Vector2(top.x, -top.y);
            EditorUtility.SetDirty(node);
            ValidateOnOrbit(node, center, OrbitSizes[i], -8f);
        }

        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("No se pudo guardar Main.unity.");
        AssetDatabase.SaveAssets();
        Debug.Log("[D1 Galaxy Orbit Alignment] TARGETED_APPLY_PASS | 7 centros visuales sobre 7 órbitas | sin reconstruir la pantalla");
    }

    private static Vector2 AlignNodeCenterToOrbit(
        Vector2 centerTop, Vector2 ellipseSize, float tiltDegrees, Vector2 nodeCenterHintTop)
    {
        const float bodyCenterOffset = 36f;
        Vector2 bodyHintTop = nodeCenterHintTop - new Vector2(0f, bodyCenterOffset);
        Vector2 relative = new Vector2(bodyHintTop.x - centerTop.x, centerTop.y - bodyHintTop.y);
        float radians = tiltDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        Vector2 unrotated = new Vector2(
            relative.x * cos + relative.y * sin,
            -relative.x * sin + relative.y * cos);
        Vector2 semi = ellipseSize * 0.5f;
        float angle = Mathf.Atan2(unrotated.y / semi.y, unrotated.x / semi.x);
        Vector2 point = new Vector2(semi.x * Mathf.Cos(angle), semi.y * Mathf.Sin(angle));
        Vector2 rotated = new Vector2(
            point.x * cos - point.y * sin,
            point.x * sin + point.y * cos);
        Vector2 bodyCenterTop = new Vector2(centerTop.x + rotated.x, centerTop.y - rotated.y);
        return bodyCenterTop + new Vector2(0f, bodyCenterOffset);
    }

    private static void ValidateOnOrbit(
        RectTransform node, Vector2 centerTop, Vector2 ellipseSize, float tiltDegrees)
    {
        const float bodyCenterOffset = 36f;
        Vector2 nodeTop = new Vector2(node.anchoredPosition.x, -node.anchoredPosition.y);
        Vector2 bodyTop = nodeTop - new Vector2(0f, bodyCenterOffset);
        Vector2 relative = new Vector2(bodyTop.x - centerTop.x, centerTop.y - bodyTop.y);
        float radians = tiltDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        Vector2 unrotated = new Vector2(
            relative.x * cos + relative.y * sin,
            -relative.x * sin + relative.y * cos);
        Vector2 semi = ellipseSize * 0.5f;
        float equation = Mathf.Pow(unrotated.x / semi.x, 2f) +
            Mathf.Pow(unrotated.y / semi.y, 2f);
        if (Mathf.Abs(equation - 1f) > 0.0005f)
            throw new InvalidOperationException(node.name + " no quedó sobre su elipse: " + equation);
    }

    private static Transform FindSceneTransform(string name)
    {
        foreach (Transform item in Resources.FindObjectsOfTypeAll<Transform>())
            if (item.gameObject.scene.IsValid() && item.name == name) return item;
        return null;
    }

    private static Transform FindChild(Transform root, string name)
    {
        foreach (Transform item in root.GetComponentsInChildren<Transform>(true))
            if (item.name == name) return item;
        return null;
    }
}
#endif
