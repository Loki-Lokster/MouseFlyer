using BepInEx.Configuration;
using UnityEngine;

// This class encapsulates global settings for the mod (profile agnostic)
public class GlobalSettings
{
    public enum FlyingMode
    {
        Normal,
        Alternative
    }
    private ConfigFile _configFile;

    // Config settings
    public ConfigEntry<KeyCode> ToggleFlyingModeKey { get; private set; }
    public ConfigEntry<KeyCode> ToggleMouseSteeringKey { get; private set; }
    public ConfigEntry<KeyCode> ToggleMenuKey { get; private set; }
    public ConfigEntry<KeyCode> ToggleHUDKey { get; private set; }
    public ConfigEntry<float> HUDOpacity { get; private set; }
    public ConfigEntry<float> OuterCircleRadiusRatio { get; private set; }
    public ConfigEntry<Color> HUDColor { get; private set; }
    public ConfigEntry<KeyCode> PreviousProfileKey { get; private set; }
    public ConfigEntry<KeyCode> NextProfileKey { get; private set; }
    public ConfigEntry<GlobalSettings.FlyingMode> CurrentFlyingMode { get; private set; }

    public GlobalSettings(ConfigFile configFile)
    {
        _configFile = configFile;
        LoadSettings();
    }

    private void LoadSettings()
    {
        ToggleFlyingModeKey = _configFile.Bind("MouseFlyer", "ToggleFlyingModeKey", KeyCode.I, "Key to toggle flying mode");
        ToggleMouseSteeringKey = _configFile.Bind("MouseFlyer", "ToggleMouseSteeringKey", KeyCode.O, "Key to toggle mouse steering");
        ToggleMenuKey = _configFile.Bind("MouseFlyer", "ToggleMenuKey", KeyCode.P, "Key to toggle menu");
        ToggleHUDKey = _configFile.Bind("MouseFlyer", "ToggleHUDKey", KeyCode.Backslash, "Key to toggle HUD");
        HUDOpacity = _configFile.Bind("MouseFlyer", "HUDOpacity", 0.5f, "HUD opacity");
        OuterCircleRadiusRatio = _configFile.Bind("MouseFlyer", "OuterCircleRadiusRatio", 0.35f, "Outer circle radius ratio (HUD Size)");
        HUDColor = _configFile.Bind("MouseFlyer", "HUDColor", Color.white, "HUD color");
        PreviousProfileKey = _configFile.Bind("MouseFlyer", "PreviousProfileKey", KeyCode.LeftBracket, "Key to switch to previous profile");
        NextProfileKey = _configFile.Bind("MouseFlyer", "NextProfileKey", KeyCode.RightBracket, "Key to switch to next profile");
        CurrentFlyingMode = _configFile.Bind("MouseFlyer", "FlyingMode", GlobalSettings.FlyingMode.Normal, "Normal or Alternative flying mode");
    }

    public void SaveSettings()
    {
        _configFile.Save();
    }
}