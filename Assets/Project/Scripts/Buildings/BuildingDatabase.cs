using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base de datos de definiciones de edificios.
/// Carga el JSON de Resources/Data/buildings y lo deja accesible en memoria.
/// </summary>
public class BuildingDatabase : MonoBehaviour
{
    // Singleton para acceder desde cualquier parte: BuildingDatabase.I
    public static BuildingDatabase I { get; private set; }

    // Lista de definiciones de edificios cargadas desde el JSON
    public List<BuildingDef> buildings = new List<BuildingDef>();

    // Clase contenedora para que JsonUtility pueda leer { "buildings": [...] }
    [System.Serializable]
    private class BuildingDefCollection
    {
        public BuildingDef[] buildings;
    }

    private void Awake()
    {
        // Patrón singleton básico
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        LoadFromResources();
    }

    /// <summary>
    /// Carga el archivo Resources/Data/buildings (TextAsset) y lo parsea.
    /// </summary>
    private void LoadFromResources()
    {
        // Importante: el path NO lleva extensión, ni "Assets/" al inicio.
        TextAsset jsonAsset = Resources.Load<TextAsset>("Data/buildings");

        if (jsonAsset == null)
        {
            Debug.LogError("[BuildingDatabase] No encontré Resources/Data/buildings (TextAsset).");
            return;
        }

        // Parsear el JSON usando la clase contenedora
        BuildingDefCollection collection = JsonUtility.FromJson<BuildingDefCollection>(jsonAsset.text);

        if (collection == null || collection.buildings == null)
        {
            Debug.LogError("[BuildingDatabase] No pude parsear el JSON de buildings.");
            return;
        }

        buildings = new List<BuildingDef>(collection.buildings);
        Debug.Log($"[BuildingDatabase] Cargados {buildings.Count} edificio(s) desde JSON.");
    }

    /// <summary>
    /// Buscar una definición por id. Ej: "vacuum_observer".
    /// </summary>
    public BuildingDef GetById(string id)
    {
        return buildings.Find(b => b.id == id);
    }
}
