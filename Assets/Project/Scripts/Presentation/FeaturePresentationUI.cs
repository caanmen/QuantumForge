using UnityEngine;
using UnityEngine.UI;


public static class FeaturePresentationUI
{
    public static void Apply(
        FeaturePresentationState state,
        GameObject featureRoot,
        Button actionButton = null,
        GameObject teaserRoot = null,
        GameObject newBadge = null)
    {
        FeaturePresentationVisualState visualState = state == null
            ? FeaturePresentationVisualState.Hidden
            : state.visualState;
        bool visible = visualState != FeaturePresentationVisualState.Hidden;
        bool teaser = visualState == FeaturePresentationVisualState.Teaser;

        if (featureRoot != null) featureRoot.SetActive(visible && !teaser);
        if (teaserRoot != null) teaserRoot.SetActive(visible && teaser);
        if (actionButton != null) actionButton.interactable = visible && !teaser;
        if (newBadge != null) newBadge.SetActive(state != null && state.isNew);
    }
}


public sealed class PresentationObjective
{
    public string nowTitle = "";
    public string nowBody = "";
    public string progress = "";
    public string primaryAction = "";
    public string nextTitle = "";
    public string nextBody = "";
}

