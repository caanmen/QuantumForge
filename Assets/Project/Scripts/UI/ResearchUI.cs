using UnityEngine;


public class ResearchUI : MonoBehaviour

{

    [Header("Referencias")]
    public Transform listContainer;   // ResearchList
    public GameObject itemPrefab;     // Prefab ResearchItem

    private void Start()
    {
        

        RefreshList();
    }


    public void RefreshList()
    {
        if (ResearchManager.I == null)
        {
            Debug.LogWarning("[ResearchUI] ResearchManager.I es null al refrescar la lista.");
            return;
        }

        if (listContainer == null || itemPrefab == null)
        {
            Debug.LogWarning("[ResearchUI] Faltan referencias (listContainer o itemPrefab).");
            return;
        }

        // 1) Borrar lo que hubiera antes
        foreach (Transform child in listContainer)
        {
            Destroy(child.gameObject);
        }

        // 2) Crear un item por cada ResearchDef
        foreach (var def in ResearchManager.I.defs)
        {
            if (def == null) continue;

            GameObject go = Instantiate(itemPrefab, listContainer);
            var ui = go.GetComponent<ResearchItemUI>();
            if (ui != null)
            {
                ui.Setup(def);
            }
            else
            {
                Debug.LogWarning("[ResearchUI] El prefab no tiene ResearchItemUI.");
            }
        }
    }
   

}

