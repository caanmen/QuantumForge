using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Genera la lista de edificios en la UI usando:
/// - BuildingDatabase (definiciones)
/// - BuildingRow prefab (fila)
/// - GameState (para la LE)
/// </summary>
public class BuildingListUI : MonoBehaviour
{
    [Header("Prefab y contenedor")]
    [Tooltip("Prefab de la fila de edificio (BuildingRow).")]
    public GameObject buildingRowPrefab;

    [Tooltip("Padre donde se instanciarán las filas. Normalmente este mismo objeto.")]
    public Transform rowsParent;

    [Header("Presentacion progresiva")]
    [Tooltip("Oculta por completo los artefactos que aun no cumplen su gate real.")]
    public bool hideLockedRows;

    [SerializeField] private float visibilityRefreshInterval = 0.25f;

    // Lista de estados en runtime de los edificios
    private List<BuildingState> buildingStates = new List<BuildingState>();
    private readonly List<BuildingRowUI> spawnedRows = new List<BuildingRowUI>();
    private float visibilityTimer;
    private bool initialized;

    private void Start()
    {
        EnsureInitialized();
    }

    public bool EnsureInitialized()
    {
        if (initialized)
            return true;

        // Obtener referencias globales
        var db = BuildingDatabase.I;
        var gs = GameState.I;

        if (db == null)
        {
            Debug.LogError("[BuildingListUI] No hay BuildingDatabase en la escena.");
            return false;
        }

        if (gs == null)
        {
            Debug.LogError("[BuildingListUI] No hay GameState en la escena.");
            return false;
        }

        if (buildingRowPrefab == null)
        {
            Debug.LogError("[BuildingListUI] Falta asignar buildingRowPrefab en el Inspector.");
            return false;
        }

        if (rowsParent == null)
        {
            // Si no se asigna, usamos el propio transform
            rowsParent = this.transform;
        }

        // Crear un BuildingState por cada BuildingDef cargado
        foreach (var def in db.buildings)
        {
            if (def == null)
                continue;

            var state = new BuildingState();
            state.InitFromDef(def);
            buildingStates.Add(state);

            // 🔹 Registrar este edificio en el GameState
            gs.RegisterBuildingState(state);

            // Instanciar la fila
            var rowGO = Instantiate(buildingRowPrefab, rowsParent);
            var rowUI = rowGO.GetComponent<BuildingRowUI>();

            if (rowUI == null)
            {
                Debug.LogError("[BuildingListUI] El prefab no tiene componente BuildingRowUI.");
                continue;
            }

            // Inicializar la UI con este estado y el GameState
            rowUI.Init(state, gs);
            spawnedRows.Add(rowUI);
            ApplyRowVisibility(rowUI);
        }

        initialized = true;
        Debug.Log($"[BuildingListUI] Generadas {buildingStates.Count} filas de edificios.");
        return true;
    }

    private void Update()
    {
        if (!hideLockedRows)
            return;

        visibilityTimer += Time.unscaledDeltaTime;
        if (visibilityTimer < visibilityRefreshInterval)
            return;
        visibilityTimer = 0f;

        foreach (BuildingRowUI row in spawnedRows)
            ApplyRowVisibility(row);
    }

    private void ApplyRowVisibility(BuildingRowUI row)
    {
        if (row == null)
            return;
        bool visible = !hideLockedRows || row.IsKnown;
        if (row.gameObject.activeSelf != visible)
            row.gameObject.SetActive(visible);
    }
}
