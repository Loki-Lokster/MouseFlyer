// This class encapsulates all the runtime properties/states for the mod
public class RuntimeState
{
    public bool IsMouseSteeringEnabled { get; set; }
    public bool ShowDebugValues { get; set; }
    public bool IsCursorLocked { get; set; }
    public bool IsWindowOpen { get; set; }
    public bool IsHUDVisible { get; set; } = true;
    public bool ShowHUDTextPanel { get; set; } = true;
    public bool ShowHUDOuterCircle { get; set; } = true;
    public bool ShowHUDInnerCircle { get; set; } = true;
}