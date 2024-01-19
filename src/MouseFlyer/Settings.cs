using MouseFlyer;
using UnityEngine;

public class Settings
{
    public enum FlyingMode
    {
        Normal,
        Alternative
    }

    // Global settings
    public FlyingMode CurrentFlyingMode { get; set; }
    public bool IsMouseSteeringEnabled { get; set; }
    public bool ShowDebugValues { get; set; }
    public bool IsCursorLocked { get; set; }
    public bool IsWindowOpen { get; set; }
    public KeyCode ToggleFlyingModeKey { get; set; }
    public KeyCode ToggleMouseSteeringKey { get; set; }
    public KeyCode ToggleMenuKey { get; set; }
    public KeyCode PreviousProfileKey { get; set; }
    public KeyCode NextProfileKey { get; set; }

    // Profile specific settings
    public CustomConfig.Profile CurrentProfile { get; set; }
    public CustomConfig config { get; set; }

    public Settings(CustomConfig config)
    {
        this.config = config;
        // Initialize global settings...
        ToggleFlyingModeKey = config.toggleFlyingModeKey.Value;
        ToggleMouseSteeringKey = config.toggleMouseSteeringKey.Value;
        ToggleMenuKey = config.toggleMenuKey.Value;
        PreviousProfileKey = config.previousProfileKey.Value;
        NextProfileKey = config.nextProfileKey.Value;

        // Initialize profile-specific settings...
        CurrentProfile = config.CurrentProfile;
    }
}