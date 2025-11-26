using UnityEngine;

public class AchievementListUI : MonoBehaviour
{
    [Header("Refs")]
    [Tooltip("Content del ScrollView (donde se instancian las filas).")]
    public RectTransform contentRoot;

    [Tooltip("Fila plantilla que se clonará por cada logro.")]
    public AchievementRowUI rowTemplate;

    private void Start()
    {
        BuildList();
    }

    /// <summary>
    /// Genera la lista de logros según los datos del AchievementManager.
    /// </summary>
    public void BuildList()
    {
        var mgr = AchievementManager.I;
        if (mgr == null)
        {
            Debug.LogWarning("[AchievementListUI] No hay AchievementManager en escena.");
            return;
        }

        if (contentRoot == null || rowTemplate == null)
        {
            Debug.LogWarning("[AchievementListUI] Falta asignar contentRoot o rowTemplate en el Inspector.");
            return;
        }

        // Limpiar filas antiguas (dejamos solo la plantilla)
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
            var child = contentRoot.GetChild(i);
            if (child == rowTemplate.transform) continue;
            Destroy(child.gameObject);
        }

        // Asegurarnos de que la plantilla está oculta
        rowTemplate.gameObject.SetActive(false);

        // Crear una fila por cada logro definido
        foreach (var def in mgr.defs)
        {
            if (def == null || string.IsNullOrEmpty(def.id)) continue;

            var rowInstance = Instantiate(rowTemplate, contentRoot);
            bool unlocked = mgr.IsUnlocked(def.id);

            rowInstance.SetData(def, unlocked);
            rowInstance.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Llamar a esto si quieres refrescar la UI cuando se desbloquee un logro
    /// mientras la pantalla está abierta.
    /// </summary>
    public void Refresh()
    {
        BuildList();
    }
}
