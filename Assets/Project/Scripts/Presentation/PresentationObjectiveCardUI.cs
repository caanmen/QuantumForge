using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PresentationObjectiveCardUI : MonoBehaviour
{
    public TMP_Text nowTitleText;
    public TMP_Text nowBodyText;
    public TMP_Text progressText;
    public Button primaryActionButton;
    public TMP_Text primaryActionText;
    public GameObject nextRoot;
    public TMP_Text nextTitleText;
    public TMP_Text nextBodyText;

    public void Render(PresentationObjective objective, UnityEngine.Events.UnityAction action)
    {
        if (objective == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        SetText(nowTitleText, objective.nowTitle);
        SetText(nowBodyText, objective.nowBody);
        SetText(progressText, objective.progress);
        SetText(primaryActionText, objective.primaryAction);
        bool hasNext = !string.IsNullOrEmpty(objective.nextTitle) ||
            !string.IsNullOrEmpty(objective.nextBody);
        if (nextRoot != null) nextRoot.SetActive(hasNext);
        SetText(nextTitleText, objective.nextTitle);
        SetText(nextBodyText, objective.nextBody);

        if (primaryActionButton != null)
        {
            primaryActionButton.onClick.RemoveAllListeners();
            primaryActionButton.interactable = action != null;
            if (action != null) primaryActionButton.onClick.AddListener(action);
        }
    }

    private static void SetText(TMP_Text text, string value)
    {
        if (text != null) text.text = value ?? "";
    }
}
