using UnityEngine;

[CreateAssetMenu(
    fileName = "PrestigeDimensionTransitionConfig",
    menuName = "Quantum Forge/Prestige/Dimensional Transition Config")]
public sealed class PrestigeDimensionTransitionConfig : ScriptableObject
{
    public Texture2D cardsTexture;
    public Texture2D portraitReference;
    public Texture2D laboratoryBackground;
    public Texture2D monolithTexture;
    public Material monolithCutoutMaterial;
    public Texture2D entryLightTexture;
    public Material entryLightAdditiveMaterial;
    public Texture2D portalRingTexture;
}
