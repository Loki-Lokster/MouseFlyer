using BepInEx.Configuration;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace MouseFlyer;

public class CustomConfig
{
    private ConfigFile _configFile;

    private ConfigEntry<float> smoothingFactor;
    private ConfigEntry<bool> isYAxisInverted;
    private ConfigEntry<float> deadzone;
    private ConfigEntry<float> rollSensitivity;
    private ConfigEntry<float> pitchSensitivity;
    private ConfigEntry<float> yawCorrection;
    private ConfigEntry<KeyCode> toggleFlyingModeKey;
    private ConfigEntry<KeyCode> toggleMouseSteeringKey;
    private ConfigEntry<KeyCode> toggleMenuKey;
    private ConfigEntry<Settings.FlyingMode> flyingMode;

    public CustomConfig(ConfigFile configFile)
    {
        Init(configFile);
    }


    [PublicAPI]
    public float SmoothingFactor
    {
        get => smoothingFactor.Value;
        set => smoothingFactor.Value = value;
    }

    [PublicAPI]
    public  bool IsYAxisInverted
    {
        get => isYAxisInverted.Value;
        set => isYAxisInverted.Value = value;
    }

    [PublicAPI]
    public  float Deadzone
    {
        get => deadzone.Value;
        set => deadzone.Value = value;
    }

    [PublicAPI]
    public  float RollSensitivity
    {
        get => rollSensitivity.Value;
        set => rollSensitivity.Value = value;
    }

    [PublicAPI]
    public  float PitchSensitivity
    {
        get => pitchSensitivity.Value;
        set => pitchSensitivity.Value = value;
    }

    [PublicAPI]
    public  float YawCorrection
    {
        get => yawCorrection.Value;
        set => yawCorrection.Value = value;
    }

    [PublicAPI]
    public  KeyCode ToggleFlyingModeKey
    {
        get => toggleFlyingModeKey.Value;
        set => toggleFlyingModeKey.Value = value;
    }

    [PublicAPI]
    public  KeyCode ToggleMouseSteeringKey
    {
        get => toggleMouseSteeringKey.Value;
        set => toggleMouseSteeringKey.Value = value;
    }

    [PublicAPI]
    public  KeyCode ToggleMenuKey
    {
        get => toggleMenuKey.Value;
        set => toggleMenuKey.Value = value;
    }

    [PublicAPI]
    public  Settings.FlyingMode FlyingMode
    {
        get => flyingMode.Value;
        set => flyingMode.Value = value;
    }


    internal void Init(ConfigFile configFile)
    {
        _configFile = configFile;
        smoothingFactor = _configFile.Bind("MouseFlyer", "SmoothingFactor", 0.05f, "Smoothing factor for mouse input");
        isYAxisInverted = _configFile.Bind("MouseFlyer", "IsYAxisInverted", true, "Invert the Y-axis for mouse input");
        deadzone = _configFile.Bind("MouseFlyer", "Deadzone", 0.01f, "Deadzone for mouse input");
        rollSensitivity = _configFile.Bind("MouseFlyer", "RollSensitivity", 0.2f, "Roll sensitivity for mouse input");
        pitchSensitivity = _configFile.Bind("MouseFlyer", "PitchSensitivity", 0.9f, "Pitch sensitivity for mouse input");
        yawCorrection = _configFile.Bind("MouseFlyer", "YawCorrection", 0.3f, "Yaw correction for mouse input");
        toggleFlyingModeKey = _configFile.Bind("MouseFlyer", "ToggleFlyingModeKey", KeyCode.I, "Key to toggle flying mode");
        toggleMouseSteeringKey = _configFile.Bind("MouseFlyer", "ToggleMouseSteeringKey", KeyCode.O, "Key to toggle mouse steering");
        toggleMenuKey = _configFile.Bind("MouseFlyer", "ToggleMenuKey", KeyCode.P, "Key to toggle menu");
        flyingMode = _configFile.Bind("MouseFlyer", "FlyingMode", Settings.FlyingMode.Normal, "Normal or Alternative flying mode");

        Save();
    }

    public void Save()
    {
        _configFile.Save();
    }

}