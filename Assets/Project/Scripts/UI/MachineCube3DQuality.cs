using System;
using UnityEngine;

public enum MachineCube3DQualityLevel
{
    Low = 0,
    Balanced = 1,
    High = 2
}

public readonly struct MachineCube3DQualityProfile
{
    public readonly int renderTextureSize;
    public readonly int antiAliasing;
    public readonly bool shadows;
    public readonly float idleRefreshRate;

    public MachineCube3DQualityProfile(int renderTextureSize, int antiAliasing,
        bool shadows, float idleRefreshRate)
    {
        this.renderTextureSize = renderTextureSize;
        this.antiAliasing = antiAliasing;
        this.shadows = shadows;
        this.idleRefreshRate = idleRefreshRate;
    }
}

public static class MachineCube3DQuality
{
    private const string PreferenceKey = "qf.machine_cube_3d_quality";

    public static event Action Changed;

    public static MachineCube3DQualityLevel Current
    {
        get
        {
            int fallback = Application.isMobilePlatform
                ? (int)MachineCube3DQualityLevel.Balanced
                : (int)MachineCube3DQualityLevel.High;
            return (MachineCube3DQualityLevel)Mathf.Clamp(
                PlayerPrefs.GetInt(PreferenceKey, fallback), 0, 2);
        }
    }

    public static MachineCube3DQualityProfile Profile => GetProfile(Current);

    public static void Set(MachineCube3DQualityLevel level)
    {
        level = (MachineCube3DQualityLevel)Mathf.Clamp((int)level, 0, 2);
        if (Current == level && PlayerPrefs.HasKey(PreferenceKey))
            return;
        PlayerPrefs.SetInt(PreferenceKey, (int)level);
        PlayerPrefs.Save();
        Changed?.Invoke();
    }

    public static MachineCube3DQualityProfile GetProfile(
        MachineCube3DQualityLevel level)
    {
        return level switch
        {
            MachineCube3DQualityLevel.Low =>
                new MachineCube3DQualityProfile(512, 1, false, 2f),
            MachineCube3DQualityLevel.Balanced =>
                new MachineCube3DQualityProfile(768, 1, false, 4f),
            _ => new MachineCube3DQualityProfile(1024, 2, true, 8f)
        };
    }
}
