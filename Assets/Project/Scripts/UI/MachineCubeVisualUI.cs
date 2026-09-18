using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class MachineCubeVisualUI : MonoBehaviour
{
    private static readonly Color[] FaceAccents =
    {
        new Color(0.08f, 0.72f, 1f, 1f),
        new Color(0.63f, 0.36f, 1f, 1f),
        new Color(1f, 0.62f, 0.12f, 1f),
        new Color(0.05f, 0.9f, 0.78f, 1f)
    };
    private static readonly Rect FullArtworkUv = new Rect(0f, 0f, 1f, 1f);
    private static readonly Rect FrontSurfaceUv = new Rect(0.04f, 0.045f, 0.81f, 0.945f);
    private static readonly Color CardText = new Color(0.61f, 0.72f, 0.77f, 1f);
    private static readonly Color RequirementText = new Color(0.80f, 0.63f, 0.36f, 1f);
    private static readonly Color BlockerGlow = new Color(1f, 0.58f, 0.10f, 1f);
    private static readonly Color AnalysisColor = new Color(1f, 0.62f, 0.12f, 1f);
    private static readonly Color RepairedColor = new Color(0.16f, 0.92f, 0.62f, 1f);

    [Header("Integración")]
    [SerializeField] private MachinePanelUI machinePanel;
    [SerializeField] private GameObject visualContentRoot;
    [SerializeField] private RectTransform faceViewport;
    [SerializeField] private CanvasGroup interactionGroup;
    [SerializeField] private MachineCubeFaceViewUI[] faces;

    [Header("Cubo 3D real")]
    [SerializeField] private MachineCube3DPrototypeController true3DController;
    [SerializeField] private bool useTrue3DForBuiltFaces = true;

    [Header("Navegación")]
    [SerializeField] private Button previousFaceButton;
    [SerializeField] private Button nextFaceButton;

    [Header("Rig de rotaciÃ³n fÃ­sica")]
    [SerializeField] private GameObject rotationRigRoot;
    [SerializeField] private MachineCubePerspectiveFaceGraphic rotationFromFace;
    [SerializeField] private MachineCubePerspectiveFaceGraphic rotationToFace;
    [SerializeField] private RectTransform rotationEdgeShadow;
    [SerializeField] private Image rotationEdgeHighlight;
    [SerializeField, Range(0.3f, 0.9f)] private float rotationDuration = 0.52f;
    [SerializeField, Range(2f, 8f)] private float rotationCameraDistance = 3.4f;
    [SerializeField, Range(0.82f, 1f)] private float rotationMidFramingScale = 0.92f;

    [Header("Cabecera")]
    [SerializeField] private TextMeshProUGUI faceIndexText;
    [SerializeField] private TextMeshProUGUI faceTitleText;
    [SerializeField] private TextMeshProUGUI leResourceText;
    [SerializeField] private TextMeshProUGUI tracesResourceText;
    [SerializeField] private TextMeshProUGUI globalProgressText;
    [SerializeField] private TextMeshProUGUI convergenceText;
    [SerializeField] private Image globalProgressFill;
    [SerializeField] private Image[] faceDots;
    [SerializeField] private RectTransform selectionGuide;
    [SerializeField] private RectTransform selectedCardRect;

    [Header("Tarjeta de nodo")]
    [SerializeField] private TextMeshProUGUI selectedNameText;
    [SerializeField] private TextMeshProUGUI selectedIconText;
    [SerializeField] private Image selectedIconImage;
    [SerializeField] private TextMeshProUGUI selectedStateText;
    [SerializeField] private TextMeshProUGUI selectedDescriptionText;
    [SerializeField] private TextMeshProUGUI selectedEffectText;
    [SerializeField] private TextMeshProUGUI selectedCostText;
    [SerializeField] private TextMeshProUGUI selectedRequirementsText;
    [SerializeField] private TextMeshProUGUI selectedFaceProgressText;
    [SerializeField] private Sprite iconLe;
    [SerializeField] private Sprite iconTraces;
    [SerializeField] private Sprite iconTriangle;
    [SerializeField] private Sprite iconArtifact;
    [SerializeField] private Sprite iconFusion;
    [SerializeField] private Sprite iconDiagnostic;
    [SerializeField] private Sprite iconStructure;
    [SerializeField] private Sprite iconConvergence;
    [SerializeField] private Sprite iconAnchor;
    [SerializeField] private Sprite iconSynthesis;

    private int _currentFaceIndex;
    private bool _rotating;
    private bool _selectionGuideLayoutDirty;
    private Coroutine _rotationRoutine;
    private float _refreshRemaining;

    private static readonly string[] NodeContentNames =
    {
        "MachineSectorHeader",
        "FaceViewport",
        "SelectionGuide",
        "PreviousFaceButton",
        "NextFaceButton",
        "SelectedNodeCard",
        "RotationArc",
        "SwipeHint",
        "FaceDot_1",
        "FaceDot_2",
        "FaceDot_3",
        "FaceDot_4"
    };

    public bool IsRotating => _rotating;

    private bool IsTrue3DFace(int faceIndex) =>
        useTrue3DForBuiltFaces && true3DController != null &&
        true3DController.SupportsFace(faceIndex);

    private void Awake()
    {
        previousFaceButton?.onClick.AddListener(() => RotateBy(-1));
        nextFaceButton?.onClick.AddListener(() => RotateBy(1));

        ConfigureSelectedCardLayout();

        if (faces != null)
        {
            foreach (MachineCubeFaceViewUI face in faces)
                face?.Initialize(this);
        }
    }

    private void OnEnable()
    {
        ResetInterruptedRotation();
        SyncFaceImmediate();
        RefreshNow();
    }

    private void Start()
    {
        SyncTrue3DPresentation();
    }

    private void OnDisable()
    {
        ResetInterruptedRotation();
        true3DController?.ShowPrototype(false);
    }

    private void Update()
    {
        _refreshRemaining -= Time.unscaledDeltaTime;
        if (_refreshRemaining > 0f)
            return;

        _refreshRemaining = MachineManager.I != null && MachineManager.I.IsAnalyzingNode
            ? 0.08f
            : 0.25f;
        RefreshNow();
    }

    private void LateUpdate()
    {
        if (!_selectionGuideLayoutDirty)
            return;
        _selectionGuideLayoutDirty = false;
        Canvas.ForceUpdateCanvases();
        string selectedId = machinePanel != null ? machinePanel.SelectedNodeId : "";
        RefreshSelectionGuide(selectedId);
    }

    public void RefreshNow()
    {
        // Los paneles operativos viven sobre la carcasa de la Máquina. La cabecera,
        // los recursos y NODOS / MEZCLAS deben permanecer visibles en ambos.
        bool fusionOverlayOpen = machinePanel != null && machinePanel.FusionPanelVisible;
        bool seedsOverlayOpen = machinePanel != null && machinePanel.SeedsPanelVisible;
        bool sharedOverlayOpen = fusionOverlayOpen || seedsOverlayOpen;
        bool showVisualContent = machinePanel == null ||
            !machinePanel.HasAuxiliaryViewOpen || sharedOverlayOpen;
        if (visualContentRoot != null && machinePanel != null)
            visualContentRoot.SetActive(showVisualContent);

        if (showVisualContent)
            SetNodeContentVisible(!sharedOverlayOpen);

        if (!showVisualContent || sharedOverlayOpen)
        {
            true3DController?.ShowPrototype(false);
            if (sharedOverlayOpen)
                RefreshHeader();
            return;
        }

        if (MachineManager.I == null || faces == null || faces.Length != 4)
            return;

        if (!_rotating && machinePanel != null)
        {
            int requested = Mathf.Clamp((int)machinePanel.CurrentZone - 1, 0, 3);
            if (requested != _currentFaceIndex)
            {
                _currentFaceIndex = requested;
                ApplyFaceVisibility();
            }
        }

        SyncTrue3DPresentation();

        string selectedId = machinePanel != null ? machinePanel.SelectedNodeId : "";
        for (int i = 0; i < faces.Length; i++)
        {
            MachineCubeFaceViewUI face = faces[i];
            if (face != null && (i == _currentFaceIndex || face.gameObject.activeSelf))
                face.RefreshFace(selectedId, FaceAccents[i]);
        }

        RefreshHeader();
        RefreshSelectedCard();
        RefreshSelectionGuide(selectedId);
    }

    private void SetNodeContentVisible(bool visible)
    {
        if (visualContentRoot == null)
            return;

        Transform root = visualContentRoot.transform;
        foreach (string childName in NodeContentNames)
        {
            Transform child = root.Find(childName);
            if (child != null && child.gameObject.activeSelf != visible)
                child.gameObject.SetActive(visible);
        }
    }

    public void SelectNode(string nodeId)
    {
        if (_rotating)
            return;
        machinePanel?.SelectNodeFromCube(nodeId);
    }

    public void RotateBy(int direction)
    {
        if (_rotating || direction == 0 || MachineManager.I == null ||
            faces == null || faces.Length != 4)
            return;

        int target = GetAdjacentFaceIndex(_currentFaceIndex, direction);
        if (target < 0)
            return;
        MachineZoneType targetZone = (MachineZoneType)(target + 1);
        if (!MachineManager.I.CanAccessZone(targetZone))
            return;

        if (IsTrue3DFace(_currentFaceIndex) && IsTrue3DFace(target))
        {
            _rotationRoutine = StartCoroutine(
                RotateTrue3DRoutine(target, direction > 0 ? 1 : -1));
            return;
        }

        if (IsTrue3DFace(_currentFaceIndex) || IsTrue3DFace(target))
        {
            _rotationRoutine = StartCoroutine(SwitchHybridFaceRoutine(target));
            return;
        }

        if (_rotationRoutine != null)
            StopCoroutine(_rotationRoutine);
        _rotationRoutine = StartCoroutine(RotateRoutine(target, direction > 0 ? 1 : -1));
    }

    public static int GetAdjacentFaceIndex(int currentFaceIndex, int direction)
    {
        if (currentFaceIndex < 0 || currentFaceIndex > 3 || direction == 0)
            return -1;
        int target = currentFaceIndex + (direction > 0 ? 1 : -1);
        return target >= 0 && target <= 3 ? target : -1;
    }

    private IEnumerator RotateTrue3DRoutine(int targetIndex, int direction)
    {
        _rotating = true;
        if (selectionGuide != null)
            selectionGuide.gameObject.SetActive(false);
        if (interactionGroup != null)
        {
            interactionGroup.interactable = false;
            interactionGroup.blocksRaycasts = false;
        }

        true3DController.ShowPrototype(true);
        if (!true3DController.RotateBy(direction))
        {
            RestoreInteractionAfterRotation();
            yield break;
        }

        while (true3DController.IsRotating)
            yield return null;

        _currentFaceIndex = targetIndex;
        RestoreInteractionAfterRotation();
        machinePanel?.SelectZoneFromCube((MachineZoneType)(targetIndex + 1));
        ApplyFaceVisibility();
        RefreshNow();
    }

    private IEnumerator SwitchHybridFaceRoutine(int targetIndex)
    {
        _rotating = true;
        if (selectionGuide != null)
            selectionGuide.gameObject.SetActive(false);
        if (interactionGroup != null)
        {
            interactionGroup.interactable = false;
            interactionGroup.blocksRaycasts = false;
        }

        // Faces 1-2 already use real 3D geometry. Faces 3-4 still use their
        // functional 2D views, so crossing the boundary swaps presentations
        // instead of simulating a missing 3D side with a moving flat image.
        true3DController?.ShowPrototype(false);
        yield return null;

        _currentFaceIndex = targetIndex;
        machinePanel?.SelectZoneFromCube((MachineZoneType)(targetIndex + 1));
        RestoreInteractionAfterRotation();
        ApplyFaceVisibility();
        RefreshNow();
    }

    private void RestoreInteractionAfterRotation()
    {
        _rotating = false;
        _rotationRoutine = null;
        if (interactionGroup != null)
        {
            interactionGroup.interactable = true;
            interactionGroup.blocksRaycasts = true;
        }
    }

    private IEnumerator RotateRoutine(int targetIndex, int direction)
    {
        _rotating = true;
        if (selectionGuide != null)
            selectionGuide.gameObject.SetActive(false);
        if (interactionGroup != null)
        {
            interactionGroup.interactable = false;
            interactionGroup.blocksRaycasts = false;
        }

        MachineCubeFaceViewUI current = faces[_currentFaceIndex];
        MachineCubeFaceViewUI target = faces[targetIndex];
        target.gameObject.SetActive(true);
        target.RefreshFace(machinePanel != null ? machinePanel.SelectedNodeId : "",
            FaceAccents[targetIndex]);
        Canvas.ForceUpdateCanvases();
        if (!BeginPhysicalRotation(current, target, direction))
        {
            Debug.LogError("[Machine Cube] El rig de rotaciÃ³n fÃ­sica no estÃ¡ configurado.");
            current.ShowImmediate(true);
            target.ShowImmediate(false);
            _rotating = false;
            _rotationRoutine = null;
            if (interactionGroup != null)
            {
                interactionGroup.interactable = true;
                interactionGroup.blocksRaycasts = true;
            }
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / rotationDuration);
            float eased = t * t * (3f - 2f * t);
            ApplyPhysicalRotationPose(eased, direction);
            yield return null;
        }

        ApplyPhysicalRotationPose(1f, direction);
        if (rotationRigRoot != null)
            rotationRigRoot.SetActive(false);

        current.gameObject.SetActive(false);
        target.ShowImmediate(true);

        _currentFaceIndex = targetIndex;
        _rotating = false;
        _rotationRoutine = null;
        if (interactionGroup != null)
        {
            interactionGroup.interactable = true;
            interactionGroup.blocksRaycasts = true;
        }

        machinePanel?.SelectZoneFromCube((MachineZoneType)(targetIndex + 1));
    }

    private bool BeginPhysicalRotation(MachineCubeFaceViewUI current,
        MachineCubeFaceViewUI target, int direction)
    {
        if (rotationRigRoot == null || rotationFromFace == null ||
            rotationToFace == null || current == null || target == null ||
            current.BaseArtworkTexture == null || target.BaseArtworkTexture == null)
            return false;

        rotationFromFace.SetFace(current.BaseArtworkTexture,
            MachineCubePerspectiveFaceGraphic.CubePlane.Front);
        rotationToFace.SetFace(target.BaseArtworkTexture, direction > 0
            ? MachineCubePerspectiveFaceGraphic.CubePlane.Right
            : MachineCubePerspectiveFaceGraphic.CubePlane.Left);
        rotationRigRoot.transform.SetAsLastSibling();
        rotationRigRoot.SetActive(true);
        current.gameObject.SetActive(false);
        target.gameObject.SetActive(false);
        ApplyPhysicalRotationPose(0f, direction);
        return true;
    }

    private void ApplyPhysicalRotationPose(float normalizedTime, int direction)
    {
        float t = Mathf.Clamp01(normalizedTime);
        float angle = -Mathf.Sign(direction) * 90f * t;
        float middle = Mathf.Sin(t * Mathf.PI);
        float framing = Mathf.Lerp(1f, rotationMidFramingScale, middle);
        Rect uv = LerpRect(FullArtworkUv, FrontSurfaceUv, middle);
        rotationFromFace?.SetUvRect(uv);
        rotationToFace?.SetUvRect(uv);
        rotationFromFace?.SetPose(angle, rotationCameraDistance, framing);
        rotationToFace?.SetPose(angle, rotationCameraDistance, framing);
        ApplyRotationEdgePose(angle, direction, framing, middle);
    }

    private void ApplyRotationEdgePose(float angleDegrees, int direction,
        float framing, float middle)
    {
        if (rotationEdgeShadow == null || faceViewport == null)
            return;

        float radians = angleDegrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        float sharedX = direction > 0 ? 0.5f : -0.5f;
        const float sharedZ = 0.5f;
        float rotatedX = sharedX * cos + sharedZ * sin;
        float rotatedZ = -sharedX * sin + sharedZ * cos;
        float perspective = (rotationCameraDistance - 0.5f) /
            Mathf.Max(0.2f, rotationCameraDistance - rotatedZ);
        float width = faceViewport.rect.width;
        float height = faceViewport.rect.height;

        rotationEdgeShadow.anchorMin = rotationEdgeShadow.anchorMax =
            new Vector2(0.5f, 0.5f);
        rotationEdgeShadow.anchoredPosition = new Vector2(
            rotatedX * perspective * width * framing, 0f);
        rotationEdgeShadow.sizeDelta = new Vector2(
            Mathf.Lerp(8f, 18f, middle),
            Mathf.Min(height, height * perspective * framing));

        Image shadowImage = rotationEdgeShadow.GetComponent<Image>();
        if (shadowImage != null)
            shadowImage.color = new Color(0.005f, 0.008f, 0.01f,
                Mathf.Lerp(0.22f, 0.78f, middle));
        if (rotationEdgeHighlight != null)
            rotationEdgeHighlight.color = new Color(0.42f, 0.46f, 0.47f,
                Mathf.Lerp(0.12f, 0.68f, middle));
    }

    private static Rect LerpRect(Rect from, Rect to, float t)
    {
        return new Rect(
            Mathf.Lerp(from.x, to.x, t),
            Mathf.Lerp(from.y, to.y, t),
            Mathf.Lerp(from.width, to.width, t),
            Mathf.Lerp(from.height, to.height, t));
    }

#if UNITY_EDITOR
    public bool ShowRotationPrototypePose(int targetIndex, int direction,
        float normalizedTime)
    {
        if (faces == null || faces.Length != 4 || targetIndex < 0 ||
            targetIndex >= faces.Length || direction == 0)
            return false;

        if (_rotationRoutine != null)
            StopCoroutine(_rotationRoutine);
        _rotationRoutine = null;
        _rotating = true;
        if (selectionGuide != null)
            selectionGuide.gameObject.SetActive(false);
        if (interactionGroup != null)
        {
            interactionGroup.interactable = false;
            interactionGroup.blocksRaycasts = false;
        }

        MachineCubeFaceViewUI current = faces[_currentFaceIndex];
        MachineCubeFaceViewUI target = faces[targetIndex];
        target.gameObject.SetActive(true);
        target.RefreshFace(machinePanel != null ? machinePanel.SelectedNodeId : "",
            FaceAccents[targetIndex]);
        Canvas.ForceUpdateCanvases();
        if (!BeginPhysicalRotation(current, target, direction))
            return false;
        ApplyPhysicalRotationPose(normalizedTime, direction);
        return true;
    }

    public void CompleteRotationPrototypePose(int targetIndex)
    {
        if (faces == null || faces.Length != 4 || targetIndex < 0 ||
            targetIndex >= faces.Length)
            return;

        if (_rotationRoutine != null)
            StopCoroutine(_rotationRoutine);
        _rotationRoutine = null;
        if (rotationRigRoot != null)
            rotationRigRoot.SetActive(false);
        _currentFaceIndex = targetIndex;
        _rotating = false;
        if (interactionGroup != null)
        {
            interactionGroup.interactable = true;
            interactionGroup.blocksRaycasts = true;
        }
        ApplyFaceVisibility();
        machinePanel?.SelectZoneFromCube((MachineZoneType)(targetIndex + 1));
        RefreshNow();
    }
#endif

    private void ResetInterruptedRotation()
    {
        if (_rotationRoutine != null)
            StopCoroutine(_rotationRoutine);
        _rotationRoutine = null;
        _rotating = false;
        if (true3DController != null && true3DController.IsRotating)
            true3DController.SetFaceImmediate(_currentFaceIndex);
        if (rotationRigRoot != null)
            rotationRigRoot.SetActive(false);
        if (interactionGroup != null)
        {
            interactionGroup.interactable = true;
            interactionGroup.blocksRaycasts = true;
        }

        if (faces == null)
            return;
        foreach (MachineCubeFaceViewUI face in faces)
        {
            if (face == null)
                continue;
            RectTransform rect = face.FaceRect;
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
            }
            if (face.Group != null)
                face.Group.alpha = 1f;
        }
    }

    private void SyncFaceImmediate()
    {
        _currentFaceIndex = machinePanel != null
            ? Mathf.Clamp((int)machinePanel.CurrentZone - 1, 0, 3)
            : MachineManager.I != null
                ? Mathf.Clamp(MachineManager.I.SelectedMachineFaceIndex, 0, 3)
                : 1;
        ApplyFaceVisibility();
    }

    private void ApplyFaceVisibility()
    {
        if (faces == null)
            return;
        if (rotationRigRoot != null)
            rotationRigRoot.SetActive(false);
        bool showTrue3D = IsTrue3DFace(_currentFaceIndex);
        for (int i = 0; i < faces.Length; i++)
            faces[i]?.ShowImmediate(!showTrue3D && i == _currentFaceIndex);
        if (true3DController != null)
        {
            if (showTrue3D && !_rotating)
                true3DController.SetFaceImmediate(_currentFaceIndex);
            true3DController.ShowPrototype(showTrue3D);
        }
        _selectionGuideLayoutDirty = true;
        // AspectRatioFitter updates the board geometry during the canvas layout pass.
        // Resolve that pass before RefreshSelectionGuide reads the active slot position.
        Canvas.ForceUpdateCanvases();
    }

    private void SyncTrue3DPresentation()
    {
        if (true3DController == null)
            return;
        bool show = visualContentRoot != null && visualContentRoot.activeInHierarchy &&
            IsTrue3DFace(_currentFaceIndex);
        if (show && !_rotating &&
            true3DController.CurrentFaceIndex != _currentFaceIndex)
            true3DController.SetFaceImmediate(_currentFaceIndex);
        true3DController.ShowPrototype(show);
    }

    private void RefreshHeader()
    {
        Color accent = FaceAccents[_currentFaceIndex];
        if (faceIndexText != null)
        {
            faceIndexText.text = $"CARA {_currentFaceIndex + 1} / 4";
            faceIndexText.color = accent;
        }
        if (faceTitleText != null)
            faceTitleText.text = GetZoneTitle((MachineZoneType)(_currentFaceIndex + 1));
        RefreshNavigationButtons();
        if (GameState.I != null)
        {
            if (leResourceText != null)
                leResourceText.text = "LE  " + FormatNumber(GameState.I.LE);
            if (tracesResourceText != null)
                tracesResourceText.text = "TRAZAS  " + FormatNumber(GameState.I.Traces);
        }

        double total = MachineManager.I.GetTotalMachineRepairProgress01();
        if (globalProgressText != null)
            globalProgressText.text = $"REPARACIÓN TOTAL  {total * 100.0:0}%  /  80%";
        if (globalProgressFill != null)
        {
            globalProgressFill.fillAmount = Mathf.Clamp01((float)(total / 0.8));
            globalProgressFill.color = total >= 0.8
                ? new Color(0.25f, 1f, 0.72f, 1f)
                : accent;
        }

        if (faceDots != null)
        {
            for (int i = 0; i < faceDots.Length; i++)
            {
                Image dot = faceDots[i];
                if (dot == null)
                    continue;
                bool active = i == _currentFaceIndex;
                dot.color = active
                    ? new Color(0.86f, 0.95f, 1f, 1f)
                    : new Color(0.34f, 0.38f, 0.40f, 0.82f);
                dot.rectTransform.localScale = active
                    ? Vector3.one * 1.22f
                    : Vector3.one;
            }
        }

        if (selectedFaceProgressText != null)
        {
            MachineZoneType zone = (MachineZoneType)(_currentFaceIndex + 1);
            double faceProgress = MachineManager.I.GetZoneRepairProgress01(zone);
            selectedFaceProgressText.text = $"PROGRESO DEL SECTOR  {faceProgress * 100.0:0}%";
            selectedFaceProgressText.color = Color.Lerp(accent, Color.white, 0.35f);
        }

        bool channel = MachineManager.I.IsNodeRepaired("z3_convergence_channel");
        if (convergenceText != null)
        {
            convergenceText.text = channel ? "CANAL: ESTABLE" : "CANAL: BLOQUEADO";
            convergenceText.color = channel
                ? new Color(0.25f, 1f, 0.72f, 1f)
                : new Color(0.65f, 0.68f, 0.72f, 1f);
        }
    }

    private void RefreshNavigationButtons()
    {
        if (MachineManager.I == null)
            return;
        int previousIndex = GetAdjacentFaceIndex(_currentFaceIndex, -1);
        int nextIndex = GetAdjacentFaceIndex(_currentFaceIndex, 1);
        if (previousFaceButton != null)
            previousFaceButton.interactable = !_rotating && previousIndex >= 0 &&
                MachineManager.I.CanAccessZone((MachineZoneType)(previousIndex + 1));
        if (nextFaceButton != null)
            nextFaceButton.interactable = !_rotating && nextIndex >= 0 &&
                MachineManager.I.CanAccessZone((MachineZoneType)(nextIndex + 1));
    }

    private void RefreshSelectedCard()
    {
        MachineNodeDef node = machinePanel != null
            ? MachineManager.I.GetDef(machinePanel.SelectedNodeId)
            : null;
        if (node == null)
        {
            SetText(selectedIconText, "--");
            if (selectedIconImage != null)
                selectedIconImage.enabled = false;
            SetText(selectedNameText, "SELECCIONA UN NODO");
            SetText(selectedStateText, "ESTADO: DESCONOCIDO");
            SetText(selectedDescriptionText, "Explora los circuitos de la cara actual.");
            SetText(selectedEffectText, "EFECTO  —");
            SetText(selectedCostText, "COSTE  —");
            SetText(selectedRequirementsText, "REQUISITOS  —");
            return;
        }

        bool repaired = MachineManager.I.IsNodeRepaired(node.id);
        bool analyzed = MachineManager.I.IsNodeAnalyzed(node.id);
        bool analyzing = MachineManager.I.IsAnalyzingNode &&
            (MachineManager.I.AnalysisNodeId == node.id ||
             (!string.IsNullOrWhiteSpace(node.tierGroup) &&
              MachineManager.I.AnalysisNodeId == "tierGroup:" + node.tierGroup));
        bool canRepair = MachineManager.I.CanRepairNode(node.id, out string reason);

        SetText(selectedIconText, GetEffectGlyph(node));
        if (selectedIconImage != null)
        {
            selectedIconImage.sprite = ResolveEffectIcon(node);
            selectedIconImage.enabled = selectedIconImage.sprite != null;
            selectedIconImage.color = Color.white;
        }
        SetText(selectedNameText, node.name.ToUpperInvariant());
        SetText(selectedStateText,
            "NODO SELECCIONADO  •  " +
            GetStateLabel(node, repaired, analyzed, analyzing));
        SetText(selectedDescriptionText, node.description);
        SetText(selectedEffectText, "EFECTO  " + FormatEffect(node));

        bool blockedByRequirement = !repaired && !canRepair && IsRequirementBlock(reason);
        bool blockedByResources = !repaired && !canRepair && IsResourceBlock(reason);
        SetText(selectedCostText, blockedByResources
            ? reason.ToUpperInvariant()
            : "COSTE  " + FormatCost(MachineManager.I.GetEffectiveNodeCost(node)));
        SetText(selectedRequirementsText, blockedByRequirement
            ? FormatRequirementBlock(reason)
            : "REQUISITOS  " + FormatRequirements(node));

        ApplyBlockerEmphasis(selectedRequirementsText, blockedByRequirement,
            RequirementText);
        ApplyBlockerEmphasis(selectedCostText, blockedByResources, CardText);

        if (selectedStateText != null)
        {
            selectedStateText.color = repaired
                ? RepairedColor
                : analyzing || (node.damaged && !analyzed)
                    ? AnalysisColor
                    : FaceAccents[Mathf.Clamp(_currentFaceIndex, 0, FaceAccents.Length - 1)];
        }
    }

    private void RefreshSelectionGuide(string selectedId)
    {
        if (IsTrue3DFace(_currentFaceIndex))
        {
            if (selectionGuide != null)
                selectionGuide.gameObject.SetActive(false);
            return;
        }
        if (selectionGuide == null || selectedCardRect == null ||
            faces == null || _currentFaceIndex < 0 || _currentFaceIndex >= faces.Length)
            return;

        RectTransform slot = faces[_currentFaceIndex]?.FindSlotForNode(selectedId);
        if (slot == null || _rotating || machinePanel == null || machinePanel.HasAuxiliaryViewOpen)
        {
            selectionGuide.gameObject.SetActive(false);
            return;
        }

        RectTransform parent = selectionGuide.parent as RectTransform;
        if (parent == null)
            return;
        parent.ForceUpdateRectTransforms();
        slot.ForceUpdateRectTransforms();
        selectedCardRect.ForceUpdateRectTransforms();
        Bounds slotBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(parent, slot);
        Bounds cardBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
            parent, selectedCardRect);
        Vector2 start = slotBounds.center;
        Vector2 end = new Vector2(start.x, cardBounds.max.y);
        Vector2 delta = end - start;
        selectionGuide.gameObject.SetActive(delta.magnitude > 6f);
        selectionGuide.anchorMin = selectionGuide.anchorMax = new Vector2(0.5f, 0.5f);
        selectionGuide.anchoredPosition = (start + end) * 0.5f;
        selectionGuide.sizeDelta = new Vector2(delta.magnitude, 3f);
        selectionGuide.localRotation = Quaternion.Euler(0f, 0f,
            Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
    }

    private static string GetStateLabel(MachineNodeDef node, bool repaired,
        bool analyzed, bool analyzing)
    {
        if (repaired) return "REPARADO";
        if (analyzing) return "ANALIZANDO";
        if (node.damaged && !analyzed) return "DAÑADO";
        if (analyzed) return "ANALIZADO";
        if (node.hidden) return "REVELADO";
        return "PENDIENTE";
    }

    private static bool IsRequirementBlock(string reason) =>
        !string.IsNullOrWhiteSpace(reason) &&
        reason.StartsWith("Falta reparar nodo requerido", System.StringComparison.Ordinal);

    private static bool IsResourceBlock(string reason) =>
        !string.IsNullOrWhiteSpace(reason) &&
        !IsRequirementBlock(reason) &&
        (reason.StartsWith("Falta ", System.StringComparison.Ordinal) ||
         reason.StartsWith("Faltan ", System.StringComparison.Ordinal));

    private static string FormatRequirementBlock(string reason)
    {
        const string prefix = "Falta reparar nodo requerido: ";
        if (reason.StartsWith(prefix, System.StringComparison.Ordinal))
            return "FALTA REPARAR  " + reason.Substring(prefix.Length).ToUpperInvariant();
        return reason.ToUpperInvariant();
    }

    private static void ConfigureSelectedCardText(
        TextMeshProUGUI text, float minimumSize, bool allowWrapping)
    {
        if (text == null)
            return;
        text.enableAutoSizing = true;
        text.fontSizeMin = Mathf.Min(text.fontSizeMin, minimumSize);
        text.textWrappingMode = allowWrapping
            ? TextWrappingModes.Normal
            : TextWrappingModes.NoWrap;
    }

    private void ConfigureSelectedCardLayout()
    {
        // Bandas separadas y contenidas: la cara de Anclajes usa descripciones y
        // costes más largos que el resto, por lo que no deben invadir la línea
        // de efecto ni los requisitos en pantallas verticales.
        ConfigureSelectedCardText(selectedNameText, 14f, false);
        ConfigureSelectedCardText(selectedStateText, 14f, false);
        ConfigureSelectedCardText(selectedDescriptionText, 13f, true);
        ConfigureSelectedCardText(selectedEffectText, 12f, true);
        ConfigureSelectedCardText(selectedCostText, 12f, false);
        ConfigureSelectedCardText(selectedRequirementsText, 12f, true);

        // Keep every row inside the flat center of the approved metal frame.
        // In particular, the former 0.03-0.16 cost band sat on the lower bevel.
        SetCardTextBand(selectedNameText, 0.76f, 0.90f, 1);
        SetCardTextBand(selectedStateText, 0.66f, 0.77f, 1);
        SetCardTextBand(selectedDescriptionText, 0.51f, 0.66f, 2);
        SetCardTextBand(selectedEffectText, 0.40f, 0.50f, 2);
        SetCardTextBand(selectedRequirementsText, 0.30f, 0.40f, 2);
        SetCardTextBand(selectedCostText, 0.20f, 0.30f, 1);
    }

    private static void SetCardTextBand(
        TextMeshProUGUI text, float anchorMinY, float anchorMaxY, int maxLines)
    {
        if (text == null)
            return;

        RectTransform rect = text.rectTransform;
        Vector2 anchorMin = rect.anchorMin;
        Vector2 anchorMax = rect.anchorMax;
        anchorMin.y = anchorMinY;
        anchorMax.y = anchorMaxY;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        text.maxVisibleLines = maxLines;
        text.overflowMode = TextOverflowModes.Truncate;
    }

    private static void ApplyBlockerEmphasis(TextMeshProUGUI text, bool emphasize,
        Color normalColor)
    {
        if (text == null)
            return;

        if (!emphasize)
        {
            text.color = normalColor;
            text.fontStyle = FontStyles.Normal;
            return;
        }

        float pulse = 0.68f + 0.32f *
            (0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 5.5f));
        text.color = Color.Lerp(normalColor, BlockerGlow, pulse);
        text.fontStyle = FontStyles.Bold;
    }

    private static string FormatRequirements(MachineNodeDef node)
    {
        if (node.requiredNodeIds == null || node.requiredNodeIds.Count == 0)
            return "NINGUNO";

        StringBuilder builder = new StringBuilder();
        foreach (string id in node.requiredNodeIds)
        {
            if (builder.Length > 0)
                builder.Append("  ·  ");
            MachineNodeDef required = MachineManager.I.GetDef(id);
            builder.Append(MachineManager.I.IsNodeRepaired(id) ? "[OK] " : "[ ] ");
            builder.Append(required != null ? required.name : id);
        }
        return builder.ToString();
    }

    private static string FormatCost(MachineNodeCostDef cost)
    {
        if (cost == null)
            return "—";
        StringBuilder builder = new StringBuilder();
        AppendCost(builder, cost.le, "LE", "LE");
        AppendCost(builder, cost.traces, "TRAZA", "TRAZAS");
        AppendCost(builder, cost.hallazgo, "ANOMALÍA", "ANOMALÍAS");
        AppendCost(builder, cost.muestra, "CONDENSADO", "CONDENSADOS");
        AppendCost(builder, cost.lecturaIncompleta,
            "VESTIGIO", "VESTIGIOS");
        AppendCost(builder, cost.compuestoUtil,
            "COMPUESTO", "COMPUESTOS");
        AppendCost(builder, cost.pureInstant, "ANCLAJE PURO", "ANCLAJES PUROS");
        AppendCost(builder, cost.stableInstant,
            "ANCLAJE ESTABLE", "ANCLAJES ESTABLES");
        AppendCost(builder, cost.forcedInstant,
            "ANCLAJE FORZADO", "ANCLAJES FORZADOS");
        return builder.Length == 0 ? "SIN COSTE" : builder.ToString();
    }

    private static void AppendCost(
        StringBuilder builder, double value, string singular, string plural)
    {
        if (value <= 0.0)
            return;
        if (builder.Length > 0)
            builder.Append("  ·  ");
        builder.Append(FormatNumber(value)).Append(' ')
            .Append(System.Math.Abs(value - 1.0) < 0.000001 ? singular : plural);
    }

    private static string FormatNumber(double value)
    {
        if (value >= 1_000_000_000.0) return (value / 1_000_000_000.0).ToString("0.##") + "B";
        if (value >= 1_000_000.0) return (value / 1_000_000.0).ToString("0.##") + "M";
        if (value >= 1_000.0) return (value / 1_000.0).ToString("0.##") + "K";
        return value.ToString("0.##");
    }

    private static string FormatEffect(MachineNodeDef node)
    {
        string value = node.effectValue > 0.0 ? "  +" + node.effectValue.ToString("0.##") : "";
        string percentValue = node.effectValue > 0.0
            ? "  +" + (node.effectValue * 100.0).ToString("0.##") + "%"
            : "";
        string reducedPercentValue = node.effectValue > 0.0
            ? "  -" + (node.effectValue * 100.0).ToString("0.##") + "%"
            : "";
        return node.effectType switch
        {
            MachineNodeEffectType.GlobalLEBonus => "PRODUCCIÓN GLOBAL DE LE" + percentValue,
            MachineNodeEffectType.TracesBonus => "GENERACIÓN DE TRAZAS" + percentValue,
            MachineNodeEffectType.TriangleBonus => "SINCRONIZACIÓN TRIANGULAR" + percentValue,
            MachineNodeEffectType.ArtifactBonus => "CALIBRACIÓN DE ARTEFACTOS" + percentValue,
            MachineNodeEffectType.Room1GlobalBonus => "SINCRONIZACIÓN DEL CUARTO 1" + percentValue,
            MachineNodeEffectType.TriangleEnergyBaseBonus =>
                "ENERGÍA BASE DEL TRIÁNGULO  +" + node.effectValue.ToString("0.##") + "/S",
            MachineNodeEffectType.UnlockFusionSlot => "RANURAS DE FUSIÓN  " + node.effectValue.ToString("0"),
            MachineNodeEffectType.FusionFailureReduction => "RIESGO DE FALLO" + reducedPercentValue,
            MachineNodeEffectType.FusionUsefulResultBonus => "PROBABILIDAD DE RESULTADO ÚTIL" + percentValue,
            MachineNodeEffectType.FusionTimeReduction => "DURACIÓN DE FUSIÓN" + reducedPercentValue,
            MachineNodeEffectType.RevealFusionProbabilities => "LECTURA DE COMPOSICIÓN",
            MachineNodeEffectType.CatalystTuning => "AFINACIÓN DE CATALIZADORES",
            MachineNodeEffectType.StableReactionChamber => "CÁMARA DE REACCIÓN ESTABLE",
            MachineNodeEffectType.GuidedSynthesis => "SÍNTESIS GUIADA",
            MachineNodeEffectType.SynthesisCore => "NÚCLEO DE SÍNTESIS",
            MachineNodeEffectType.UnlockDiagnostics => "DIAGNÓSTICO INTERNO",
            MachineNodeEffectType.RevealHiddenSubnodes => "REVELAR NODOS SECRETOS",
            MachineNodeEffectType.CompensationCircuit => "CIRCUITO DE COMPENSACIÓN",
            MachineNodeEffectType.UnlockMachineMemory => "MEMORIA DE LA MÁQUINA",
            MachineNodeEffectType.ZoneProgressSyncBonus => "SINCRONIZACIÓN POR SECTOR",
            MachineNodeEffectType.StructuralSupportBonus => "REFUERZO ESTRUCTURAL",
            MachineNodeEffectType.EnablePrestige1 => "UMBRAL ESTRUCTURAL",
            MachineNodeEffectType.DiagnosticReasonMarker => "DIAGNÓSTICO DE FALLAS",
            MachineNodeEffectType.InternalSupportBonus => "SOPORTE INTERNO" + percentValue,
            MachineNodeEffectType.UnlockInstantChamber => "CÁMARA DE ANCLAJES",
            MachineNodeEffectType.SeedReadingBonus => "LECTURA DE MADURACIÓN",
            MachineNodeEffectType.ArchiveSlotBonus => "ESPACIOS DE ARCHIVO" + value,
            MachineNodeEffectType.InstantInitialStabilityBonus => "ESTABILIDAD INICIAL" + percentValue,
            MachineNodeEffectType.SynchronizeStabilityBonus => "ESTABILIDAD POR SINCRONIZACIÓN" + value,
            MachineNodeEffectType.TensionContainmentBonus => "TENSIÓN POR ESTABILIZACIÓN  -" + node.effectValue.ToString("0.##"),
            MachineNodeEffectType.SafeRewindBonus => "PÉRDIDA AL COMPENSAR  -" + node.effectValue.ToString("0.##"),
            MachineNodeEffectType.SeedSlotBonus => "ESPACIOS DE SEMILLA" + value,
            MachineNodeEffectType.PureMaterialThresholdReduction => "UMBRAL DE ANCLAJE PURO" + reducedPercentValue,
            _ => node.effectType.ToString().ToUpperInvariant().Replace('_', ' ') + value
        };
    }

    private static string GetZoneTitle(MachineZoneType zone)
    {
        return zone switch
        {
            MachineZoneType.Room1Link => "ENLACE CON EL CUARTO 1",
            MachineZoneType.FusionSector => "SECTOR DE FUSIONES",
            MachineZoneType.InternalSupport => "SOPORTE INTERNO",
            MachineZoneType.InstantChamber => "CÁMARA DE ANCLAJES",
            _ => "SECTOR DESCONOCIDO"
        };
    }

    public static string GetEffectGlyph(MachineNodeDef node)
    {
        if (node == null)
            return "--";
        if (node.effectType == MachineNodeEffectType.TriangleEnergyBaseBonus)
            return "EN";
        string effect = node.effectType.ToString();
        if (effect.Contains("Triangle", System.StringComparison.OrdinalIgnoreCase)) return "TI";
        if (effect.Contains("LE", System.StringComparison.OrdinalIgnoreCase)) return "LE";
        if (effect.Contains("Trace", System.StringComparison.OrdinalIgnoreCase)) return "TR";
        if (effect.Contains("Artifact", System.StringComparison.OrdinalIgnoreCase)) return "AR";
        if (effect.Contains("Fusion", System.StringComparison.OrdinalIgnoreCase) ||
            effect.Contains("Synthesis", System.StringComparison.OrdinalIgnoreCase) ||
            effect.Contains("Catal", System.StringComparison.OrdinalIgnoreCase)) return "FU";
        if (effect.Contains("Diagnostic", System.StringComparison.OrdinalIgnoreCase) ||
            effect.Contains("Memory", System.StringComparison.OrdinalIgnoreCase)) return "DX";
        if (effect.Contains("Structural", System.StringComparison.OrdinalIgnoreCase) ||
            effect.Contains("Support", System.StringComparison.OrdinalIgnoreCase) ||
            effect.Contains("Compensation", System.StringComparison.OrdinalIgnoreCase)) return "ST";
        if (effect.Contains("Prestige", System.StringComparison.OrdinalIgnoreCase) ||
            effect.Contains("Convergence", System.StringComparison.OrdinalIgnoreCase)) return "CV";
        if (effect.Contains("Instant", System.StringComparison.OrdinalIgnoreCase) ||
            effect.Contains("Seed", System.StringComparison.OrdinalIgnoreCase) ||
            effect.Contains("Anchor", System.StringComparison.OrdinalIgnoreCase) ||
            effect.Contains("Archive", System.StringComparison.OrdinalIgnoreCase)) return "AN";
        return "SY";
    }

    private Sprite ResolveEffectIcon(MachineNodeDef node)
    {
        return GetEffectGlyph(node) switch
        {
            "LE" => iconLe,
            "TR" => iconTraces,
            "TI" => iconTriangle,
            "EN" => iconTriangle,
            "AR" => iconArtifact,
            "FU" => iconFusion,
            "DX" => iconDiagnostic,
            "ST" => iconStructure,
            "CV" => iconConvergence,
            "AN" => iconAnchor,
            _ => iconSynthesis
        };
    }

    private static void SetText(TextMeshProUGUI text, string value)
    {
        if (text != null)
            text.text = value ?? "";
    }
}
