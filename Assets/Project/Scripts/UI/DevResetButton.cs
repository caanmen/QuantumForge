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
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        var save = FindFirstObjectByType<SaveService>();
        if (save != null)
        {
            try { save.ResetSave(); } catch { }
        }

        var scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }
    }