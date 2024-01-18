using UnityEngine;

public class Settings
{
    public enum FlyingMode
    {
        Normal,
        Alternative
    }

    public FlyingMode CurrentFlyingMode { get; set; }
    public bool IsMouseSteeringEnabled { get; set; }
    public bool IsYAxisInverted { get; set; }
    public float RollSensitivity { get; set; }
    public float PitchSensitivity { get; set; }
    public float YawCorrection { get; set; }
    public float Deadzone { get; set; }
    public float SmoothingFactor { get; set; }
    public bool ShowDebugValues { get; set; }
    public bool IsCursorLocked { get; set; }
    public bool IsWindowOpen { get; set; }
    public KeyCode ToggleFlyingModeKey { get; set; }
    public KeyCode ToggleMouseSteeringKey { get; set; }
    public KeyCode ToggleMenuKey { get; set; }

}