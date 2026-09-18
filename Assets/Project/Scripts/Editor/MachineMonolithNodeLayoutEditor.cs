#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public sealed class MachineMonolithNodeLayoutEditor : EditorWindow
{
    private const string DraftRelativePath =
        "Logs/VisualQA/MonolithNodeLayoutManual/draft.json";
    private const float SolidSurfaceAlphaThreshold = .98f;

    private static readonly string[] FaceNames =
        { "CARA 1", "CARA 2", "CARA 3" };
    private static readonly string[] StageNames =
        { "0 % · 3 nodos", "20 % · 5 nodos", "40 % · 7 nodos", "60 % · 8 nodos" };
    private static readonly string[] TexturePaths =
    {
        "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_1_exposed_surface_progression_v35.png",
        "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_2_exposed_surface_progression_v43.png",
        "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_3_lower_right_progression_aligned_v57.png"
    };

    private static readonly Rect[] StageUvRects =
    {
        new Rect(0f, .5f, .5f, .5f),
        new Rect(.5f, .5f, .5f, .5f),
        new Rect(0f, 0f, .5f, .5f),
        new Rect(.5f, 0f, .5f, .5f)
    };

    [Serializable]
    private sealed class LayoutDraft
    {
        public string format = "quantum-forge-monolith-node-layout-v1";
        public string savedAtUtc;
        public FaceDraft[] faces;
    }

    [Serializable]
    private sealed class FaceDraft
    {
        public Vector2[] positions;
    }

    private readonly Stack<string> undoSnapshots = new Stack<string>();
    private Texture2D[] faceTextures;
    private Texture2D[] readableFaceTextures;
    private Vector2[][] positions;
    private Vector2[][] productionPositions;
    private int selectedFace;
    private int selectedStage;
    private int selectedNode = -1;
    private bool dragging;
    private bool dirty;
    private string status = "Borrador separado de producción.";

    [MenuItem("Tools/Quantum Forge/Machine/Editor manual de nodos")]
    public static void OpenWindow()
    {
        MachineMonolithNodeLayoutEditor window = GetWindow<MachineMonolithNodeLayoutEditor>();
        window.titleContent = new GUIContent("Nodos del Monolito");
        window.minSize = new Vector2(660f, 780f);
        window.Show();
        window.Focus();
    }

    [MenuItem("Tools/Quantum Forge/Machine/Validar editor manual de nodos")]
    public static void ValidateEditorTool()
    {
        var failures = new List<string>();
        string root = Directory.GetParent(Application.dataPath).FullName;
        LayoutDraft draft = null;
        string draftPath = Path.Combine(root,
            DraftRelativePath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(draftPath))
        {
            try
            {
                draft = JsonUtility.FromJson<LayoutDraft>(File.ReadAllText(draftPath));
                if (!DraftIsComplete(draft))
                {
                    failures.Add("El borrador manual no contiene tres caras completas.");
                    draft = null;
                }
            }
            catch (Exception exception)
            {
                failures.Add("No se pudo leer el borrador manual: " + exception.Message);
            }
        }
        else
        {
            failures.Add("No existe el borrador manual: " + DraftRelativePath);
        }

        for (int face = 0; face < TexturePaths.Length; face++)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePaths[face]);
            if (texture == null)
                failures.Add($"Falta textura canónica de la cara {face + 1}: {TexturePaths[face]}");

            if (MachineMonolith2DVisualUI.GetFaceDisplayNodeCount(face) != 8)
                failures.Add($"La cara {face + 1} no conserva ocho nodos de presentación.");

            int[] expected = { 3, 5, 7, 8 };
            for (int stage = 0; stage < expected.Length; stage++)
            {
                if (MachineMonolith2DVisualUI.GetFaceVisibleNodeCount(face, stage) != expected[stage])
                    failures.Add($"Cara {face + 1}, etapa {stage}: conteo distinto de {expected[stage]}.");
            }

            if (draft == null)
                continue;
            for (int node = 0; node < draft.faces[face].positions.Length; node++)
            {
                Vector2 approved = draft.faces[face].positions[node];
                Vector2 production = MachineMonolith2DVisualUI.GetNodeDisplayPosition(
                    face, node, new Vector2(.5f, .5f));
                if (Vector2.Distance(approved, production) > .000002f)
                    failures.Add($"Cara {face + 1}, nodo {node + 1}: producción " +
                        $"({production.x:F6},{production.y:F6}) no coincide con el borrador " +
                        $"({approved.x:F6},{approved.y:F6}).");
            }
            string absoluteTexturePath = Path.Combine(root,
                TexturePaths[face].Replace('/', Path.DirectorySeparatorChar));
            Texture2D readable = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!File.Exists(absoluteTexturePath) ||
                !readable.LoadImage(File.ReadAllBytes(absoluteTexturePath)))
            {
                failures.Add($"No se pudo leer el alfa de la cara {face + 1}.");
                DestroyImmediate(readable);
                continue;
            }
            try
            {
                for (int stage = 0; stage < expected.Length; stage++)
                {
                    for (int node = 0; node < expected[stage]; node++)
                    {
                        Vector2 center = draft.faces[face].positions[node];
                        if (!IsNodeFootprintSafe(face, stage, center, readable))
                            failures.Add($"Cara {face + 1}, {stage * 20} %, nodo {node + 1}: " +
                                "el tamaño real no cabe por completo en piedra sana.");
                    }
                }
            }
            finally
            {
                DestroyImmediate(readable);
            }
        }

        if (failures.Count > 0)
            throw new InvalidOperationException("[MONOLITH NODE EDITOR FAIL]\n" +
                string.Join("\n", failures));

        Debug.Log("[MONOLITH NODE EDITOR PASS] 3 caras canónicas | 4 estados | " +
            "3/5/7/8 nodos | producción = borrador | huellas completas sobre piedra sana");
    }

    private void OnEnable()
    {
        LoadTextures();
        LoadProductionPositions();
        LoadDraftOrProduction();
    }

    private void OnDisable()
    {
        if (readableFaceTextures == null)
            return;
        foreach (Texture2D texture in readableFaceTextures)
            if (texture != null)
                DestroyImmediate(texture);
        readableFaceTextures = null;
    }

    private void OnGUI()
    {
        EnsureData();

        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField("EDITOR MANUAL DE NODOS DEL MONOLITO", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Arrastra los nodos sobre piedra sana. Cada nodo conserva la misma posición en " +
            "los estados posteriores; al reparar sólo aparecen más nodos (3/5/7/8). " +
            "El borrador no cambia todavía el juego.", MessageType.Info);

        int previousFace = selectedFace;
        selectedFace = GUILayout.Toolbar(selectedFace, FaceNames, GUILayout.Height(28f));
        if (previousFace != selectedFace)
        {
            selectedNode = -1;
            dragging = false;
        }

        selectedStage = GUILayout.Toolbar(selectedStage, StageNames, GUILayout.Height(28f));
        EditorGUILayout.Space(6f);

        float reservedHeight = 220f;
        float canvasSize = Mathf.Max(360f,
            Mathf.Min(position.width - 24f, position.height - reservedHeight));
        Rect canvasRect = GUILayoutUtility.GetRect(canvasSize, canvasSize,
            GUILayout.ExpandWidth(false));
        canvasRect.x = (position.width - canvasRect.width) * .5f;

        DrawCanvas(canvasRect);
        HandleDragging(canvasRect);

        EditorGUILayout.Space(6f);
        DrawSelectedNodeInfo();
        DrawActions();
        EditorGUILayout.LabelField(status, EditorStyles.miniLabel);
    }

    private void DrawCanvas(Rect canvasRect)
    {
        EditorGUI.DrawRect(new Rect(canvasRect.x - 2f, canvasRect.y - 2f,
            canvasRect.width + 4f, canvasRect.height + 4f), new Color(.08f, .70f, .92f, 1f));
        EditorGUI.DrawRect(canvasRect, new Color(.01f, .02f, .025f, 1f));

        Texture2D texture = faceTextures != null && selectedFace < faceTextures.Length
            ? faceTextures[selectedFace]
            : null;
        if (texture != null)
            GUI.DrawTextureWithTexCoords(canvasRect, texture, StageUvRects[selectedStage], true);
        else
            GUI.Label(canvasRect, "No se encontró la hoja de esta cara.", EditorStyles.centeredGreyMiniLabel);

        int visibleCount = MachineMonolith2DVisualUI.GetFaceVisibleNodeCount(
            selectedFace, selectedStage);
        int previousCount = selectedStage == 0
            ? 0
            : MachineMonolith2DVisualUI.GetFaceVisibleNodeCount(selectedFace, selectedStage - 1);
        int unsafeCount = 0;

        Handles.BeginGUI();
        for (int index = 0; index < visibleCount; index++)
        {
            Vector2 center = UvToGui(canvasRect, positions[selectedFace][index]);
            bool safe = IsNodeFootprintSafe(selectedFace, selectedStage,
                positions[selectedFace][index]);
            if (!safe)
                unsafeCount++;
            bool selected = index == selectedNode;
            bool newlyRevealed = index >= previousCount;
            Color fill = selected
                ? new Color(.15f, .93f, 1f, 1f)
                : newlyRevealed
                    ? new Color(1f, .58f, .12f, .96f)
                    : new Color(.68f, .34f, 1f, .93f);

            Rect footprint = GetFootprintGuiRect(canvasRect, center);
            EditorGUI.DrawRect(footprint, safe
                ? new Color(.12f, .90f, .98f, .08f)
                : new Color(1f, .12f, .10f, .24f));
            DrawOutline(footprint, safe
                ? new Color(.25f, .92f, 1f, .95f)
                : new Color(1f, .16f, .12f, 1f), safe ? 1f : 3f);
            Handles.color = new Color(0f, 0f, 0f, .92f);
            Handles.DrawSolidDisc(center, Vector3.forward, selected ? 17f : 15f);
            Handles.color = fill;
            Handles.DrawSolidDisc(center, Vector3.forward, selected ? 13f : 11f);

            Rect labelRect = new Rect(center.x - 14f, center.y - 9f, 28f, 18f);
            GUIStyle numberStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.black },
                fontSize = 11
            };
            GUI.Label(labelRect, (index + 1).ToString(CultureInfo.InvariantCulture), numberStyle);
        }
        Handles.EndGUI();

        if (unsafeCount > 0)
        {
            Rect warning = new Rect(canvasRect.x + 10f, canvasRect.yMax - 38f,
                canvasRect.width - 20f, 28f);
            EditorGUI.DrawRect(warning, new Color(.28f, .01f, .01f, .94f));
            GUI.Label(warning,
                $"  {unsafeCount} nodo(s) en rojo: el tamaño real toca vacío, riel, base o borde.",
                EditorStyles.whiteBoldLabel);
        }
    }

    private void HandleDragging(Rect canvasRect)
    {
        Event current = Event.current;
        int visibleCount = MachineMonolith2DVisualUI.GetFaceVisibleNodeCount(
            selectedFace, selectedStage);

        if (current.type == EventType.MouseDown && current.button == 0 &&
            canvasRect.Contains(current.mousePosition))
        {
            int nearest = FindNearestVisibleNode(canvasRect, current.mousePosition, visibleCount);
            if (nearest >= 0)
            {
                PushUndoSnapshot();
                selectedNode = nearest;
                dragging = true;
                current.Use();
                Repaint();
            }
        }
        else if (current.type == EventType.MouseDrag && dragging && selectedNode >= 0)
        {
            positions[selectedFace][selectedNode] = GuiToUv(canvasRect, current.mousePosition);
            dirty = true;
            status = $"Cara {selectedFace + 1}, nodo {selectedNode + 1}: posición modificada.";
            current.Use();
            Repaint();
        }
        else if (current.type == EventType.MouseUp && dragging)
        {
            dragging = false;
            current.Use();
            Repaint();
        }
    }

    private int FindNearestVisibleNode(Rect canvasRect, Vector2 mousePosition, int visibleCount)
    {
        int nearest = -1;
        float nearestDistance = 24f;
        for (int index = 0; index < visibleCount; index++)
        {
            float distance = Vector2.Distance(mousePosition,
                UvToGui(canvasRect, positions[selectedFace][index]));
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = index;
            }
        }
        return nearest;
    }

    private void DrawSelectedNodeInfo()
    {
        if (selectedNode < 0)
        {
            EditorGUILayout.LabelField(
                "Haz clic sobre un nodo y arrástralo. El rectángulo es el tamaño real; rojo = no cabe por completo en piedra sana.",
                EditorStyles.wordWrappedMiniLabel);
            return;
        }

        Vector2 value = positions[selectedFace][selectedNode];
        EditorGUILayout.LabelField(
            $"Seleccionado: cara {selectedFace + 1}, nodo {selectedNode + 1}  ·  " +
            $"X {value.x:F4}  Y {value.y:F4}", EditorStyles.boldLabel);
    }

    private void DrawActions()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(undoSnapshots.Count == 0);
        if (GUILayout.Button("DESHACER", GUILayout.Height(30f)))
            UndoLastChange();
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("RESTAURAR ESTA CARA", GUILayout.Height(30f)))
            RestoreCurrentFace();

        if (GUILayout.Button("GUARDAR BORRADOR", GUILayout.Height(30f)))
            SaveDraft();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("RECARGAR BORRADOR", GUILayout.Height(26f)))
            LoadDraftOrProduction();
        if (GUILayout.Button("COPIAR COORDENADAS", GUILayout.Height(26f)))
            CopyCoordinates();
        EditorGUILayout.EndHorizontal();

        if (dirty)
            EditorGUILayout.HelpBox("Hay cambios manuales sin guardar en el borrador.", MessageType.Warning);
    }

    private void LoadTextures()
    {
        faceTextures = new Texture2D[TexturePaths.Length];
        if (readableFaceTextures != null)
            foreach (Texture2D oldTexture in readableFaceTextures)
                if (oldTexture != null)
                    DestroyImmediate(oldTexture);
        readableFaceTextures = new Texture2D[TexturePaths.Length];
        string root = Directory.GetParent(Application.dataPath).FullName;
        for (int i = 0; i < TexturePaths.Length; i++)
        {
            faceTextures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePaths[i]);
            string absolutePath = Path.Combine(root,
                TexturePaths[i].Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(absolutePath))
                continue;
            Texture2D readable = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (readable.LoadImage(File.ReadAllBytes(absolutePath)))
            {
                readable.name = $"MonolithFace{i + 1}ReadableValidation";
                readable.hideFlags = HideFlags.HideAndDontSave;
                readableFaceTextures[i] = readable;
            }
            else
            {
                DestroyImmediate(readable);
            }
        }
    }

    private static Rect GetFootprintGuiRect(Rect canvasRect, Vector2 center)
    {
        float width = canvasRect.width * MachineMonolith2DVisualUI.NodeSymbolUvWidth;
        float height = canvasRect.height * MachineMonolith2DVisualUI.NodeSymbolUvHeight;
        return new Rect(center.x - width * .5f, center.y - height * .5f,
            width, height);
    }

    private static void DrawOutline(Rect rect, Color color, float thickness)
    {
        EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMax - thickness,
            rect.width, thickness), color);
        EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.yMin,
            thickness, rect.height), color);
    }

    private bool IsNodeFootprintSafe(int face, int stage, Vector2 center)
    {
        if (readableFaceTextures == null || face < 0 ||
            face >= readableFaceTextures.Length || readableFaceTextures[face] == null)
            return false;

        return IsNodeFootprintSafe(face, stage, center, readableFaceTextures[face]);
    }

    private static bool IsNodeFootprintSafe(int face, int stage, Vector2 center,
        Texture2D surface)
    {
        Vector2 halfExtent = new Vector2(
            MachineMonolith2DVisualUI.NodeSymbolUvWidth * .5f,
            MachineMonolith2DVisualUI.NodeSymbolUvHeight * .5f);
        if (!MachineMonolith2DVisualUI.IsNodeFootprintInsideDarkSurface(
            face, center, halfExtent))
            return false;

        Rect stageUv = StageUvRects[Mathf.Clamp(stage, 0, StageUvRects.Length - 1)];
        for (int row = 0; row <= 6; row++)
        {
            for (int column = 0; column <= 6; column++)
            {
                float localU = Mathf.Lerp(center.x - halfExtent.x,
                    center.x + halfExtent.x, column / 6f);
                float localV = Mathf.Lerp(center.y - halfExtent.y,
                    center.y + halfExtent.y, row / 6f);
                if (localU < 0f || localU > 1f || localV < 0f || localV > 1f)
                    return false;
                Color pixel = surface.GetPixelBilinear(
                    stageUv.x + localU * stageUv.width,
                    stageUv.y + localV * stageUv.height);
                if (pixel.a < SolidSurfaceAlphaThreshold)
                    return false;
            }
        }
        return true;
    }

    private void LoadProductionPositions()
    {
        productionPositions = new Vector2[3][];
        for (int face = 0; face < productionPositions.Length; face++)
        {
            productionPositions[face] = new Vector2[8];
            for (int node = 0; node < productionPositions[face].Length; node++)
                productionPositions[face][node] = MachineMonolith2DVisualUI.GetNodeDisplayPosition(
                    face, node, new Vector2(.5f, .5f));
        }
    }

    private void LoadDraftOrProduction()
    {
        EnsureProductionPositions();
        string path = AbsoluteDraftPath;
        if (File.Exists(path))
        {
            try
            {
                LayoutDraft draft = JsonUtility.FromJson<LayoutDraft>(File.ReadAllText(path));
                if (DraftIsComplete(draft))
                {
                    positions = CloneFaces(draft.faces);
                    dirty = false;
                    undoSnapshots.Clear();
                    status = "Borrador manual cargado.";
                    Repaint();
                    return;
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"No se pudo cargar el borrador manual: {exception.Message}");
            }
        }

        positions = CloneFaces(productionPositions);
        dirty = false;
        undoSnapshots.Clear();
        status = "Se cargaron las posiciones actuales de producción.";
        Repaint();
    }

    private void SaveDraft()
    {
        string path = AbsoluteDraftPath;
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        LayoutDraft draft = CreateDraft();
        File.WriteAllText(path, JsonUtility.ToJson(draft, true));
        dirty = false;
        status = "Borrador guardado en " + DraftRelativePath;
        Debug.Log("[MONOLITH NODE EDITOR] Borrador guardado: " + path);
        Repaint();
    }

    private void CopyCoordinates()
    {
        var builder = new StringBuilder();
        builder.AppendLine("Distribución manual del Monolito");
        for (int face = 0; face < positions.Length; face++)
        {
            builder.AppendLine($"CARA {face + 1}");
            for (int node = 0; node < positions[face].Length; node++)
            {
                Vector2 value = positions[face][node];
                builder.AppendLine(string.Format(CultureInfo.InvariantCulture,
                    "{0}: ({1:F4}, {2:F4})", node + 1, value.x, value.y));
            }
        }
        EditorGUIUtility.systemCopyBuffer = builder.ToString();
        status = "Coordenadas de las tres caras copiadas al portapapeles.";
    }

    private void RestoreCurrentFace()
    {
        PushUndoSnapshot();
        positions[selectedFace] = (Vector2[])productionPositions[selectedFace].Clone();
        selectedNode = -1;
        dirty = true;
        status = $"Cara {selectedFace + 1} restaurada a las posiciones actuales del juego.";
        Repaint();
    }

    private void UndoLastChange()
    {
        if (undoSnapshots.Count == 0)
            return;
        LayoutDraft draft = JsonUtility.FromJson<LayoutDraft>(undoSnapshots.Pop());
        if (DraftIsComplete(draft))
        {
            positions = CloneFaces(draft.faces);
            dirty = true;
            status = "Último movimiento deshecho.";
            Repaint();
        }
    }

    private void PushUndoSnapshot()
    {
        undoSnapshots.Push(JsonUtility.ToJson(CreateDraft()));
        while (undoSnapshots.Count > 40)
        {
            string[] snapshots = undoSnapshots.ToArray();
            undoSnapshots.Clear();
            for (int i = Mathf.Min(39, snapshots.Length - 1); i >= 0; i--)
                undoSnapshots.Push(snapshots[i]);
        }
    }

    private LayoutDraft CreateDraft()
    {
        var draft = new LayoutDraft
        {
            savedAtUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
            faces = new FaceDraft[positions.Length]
        };
        for (int face = 0; face < positions.Length; face++)
            draft.faces[face] = new FaceDraft
            {
                positions = (Vector2[])positions[face].Clone()
            };
        return draft;
    }

    private static Vector2[][] CloneFaces(Vector2[][] source)
    {
        var clone = new Vector2[source.Length][];
        for (int face = 0; face < source.Length; face++)
            clone[face] = (Vector2[])source[face].Clone();
        return clone;
    }

    private static Vector2[][] CloneFaces(FaceDraft[] source)
    {
        var clone = new Vector2[source.Length][];
        for (int face = 0; face < source.Length; face++)
            clone[face] = (Vector2[])source[face].positions.Clone();
        return clone;
    }

    private static bool DraftIsComplete(LayoutDraft draft)
    {
        if (draft == null || draft.faces == null || draft.faces.Length != 3)
            return false;
        for (int face = 0; face < draft.faces.Length; face++)
        {
            if (draft.faces[face] == null || draft.faces[face].positions == null ||
                draft.faces[face].positions.Length != 8)
                return false;
        }
        return true;
    }

    private void EnsureData()
    {
        if (faceTextures == null || faceTextures.Length != 3)
            LoadTextures();
        EnsureProductionPositions();
        if (positions == null || positions.Length != 3)
            LoadDraftOrProduction();
    }

    private void EnsureProductionPositions()
    {
        if (productionPositions == null || productionPositions.Length != 3)
            LoadProductionPositions();
    }

    private static Vector2 UvToGui(Rect rect, Vector2 uv)
    {
        return new Vector2(rect.x + uv.x * rect.width,
            rect.y + (1f - uv.y) * rect.height);
    }

    private static Vector2 GuiToUv(Rect rect, Vector2 point)
    {
        return new Vector2(
            Mathf.Clamp01((point.x - rect.x) / rect.width),
            Mathf.Clamp01(1f - (point.y - rect.y) / rect.height));
    }

    private static string AbsoluteDraftPath
    {
        get
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            return Path.Combine(root, DraftRelativePath.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
#endif
