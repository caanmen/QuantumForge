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

    // Lista de estados en runtime de los edificios
    private List<BuildingState> buildingStates = new List<BuildingState>();

    private void Start()
    {
        // Obtener referencias globales
        var db = BuildingDatabase.I;
        var gs = GameState.I;

        if (db == null)
        {
            Debug.LogError("[BuildingListUI] No hay BuildingDatabase en la escena.");
            return;
        }

        if (gs == null)
        {
            Debug.LogError("[BuildingListUI] No hay GameState en la escena.");
            return;
        }

        if (buildingRowPrefab == null)
        {
            Debug.LogError("[BuildingListUI] Falta asignar buildingRowPrefab en el Inspector.");
            return;
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
        }

        Debug.Log($"[BuildingListUI] Generadas {buildingStates.Count} filas de edificios.");
    }
}
