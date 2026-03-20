using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DevResetButton : MonoBehaviour
{
    public Button button;

    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(DoReset);
    }

    public void DoReset()
    {
        // 1) Borra PlayerPrefs (si usas PlayerPrefs para settings/flags)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // 2) Si tienes un SaveService singleton, intenta llamarlo
        // (si no existe, no pasa nada)
        var save = FindFirstObjectByType<SaveService>();
        if (save != null)
        {
            // Ajusta el nombre si tu método se llama distinto
            // Ejemplos típicos: DeleteSave(), Wipe(), ResetSave(), ClearSave()
            // Aquí dejo 2 intentos comunes:
            try { save.SendMessage("DeleteSave", SendMessageOptions.DontRequireReceiver); } catch { }
            try { save.SendMessage("Wipe", SendMessageOptions.DontRequireReceiver); } catch { }
        }

        // 3) Reinicia escena para que todo vuelva a iniciar limpio
        var scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }
}