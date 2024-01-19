using BepInEx.Configuration;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace MouseFlyer;

public class CustomConfig
{
    private ConfigFile _configFile;

    private const int MaxProfiles = 5; // Set the maximum number of profiles

    // Global settings
    public ConfigEntry<KeyCode> toggleFlyingModeKey;
    public ConfigEntry<KeyCode> toggleMouseSteeringKey;
    public ConfigEntry<KeyCode> toggleMenuKey;
    public ConfigEntry<KeyCode> nextProfileKey;
    public ConfigEntry<KeyCode> previousProfileKey;
    public ConfigEntry<Settings.FlyingMode> flyingMode;

    // Profile specific settings
    public List<Profile> Profiles { get; set; }
    public Profile CurrentProfile { get; set; }
    public class Profile 
    {
        public string Name { get; set; }
        public ConfigEntry<float> SmoothingFactor { get; private set; }
        public ConfigEntry<bool> IsYAxisInverted { get; private set; }
        public ConfigEntry<bool> IsAutoCamEnabled { get; private set; }
        public ConfigEntry<float> Deadzone { get; private set; }
        public ConfigEntry<float> RollSensitivity { get; private set; }
        public ConfigEntry<float> PitchSensitivity { get; private set; }
        public ConfigEntry<float> YawCorrection { get; private set; }

        public Profile(ConfigFile configFile, string profileName)
        {
            Name = profileName;
            SmoothingFactor = configFile.Bind("MouseFlyer", $"{profileName}-SmoothingFactor", 0.05f, "Smoothing factor for mouse input");
            IsYAxisInverted = configFile.Bind("MouseFlyer", $"{profileName}-IsYAxisInverted", true, "Invert the Y-axis for mouse input");
            IsAutoCamEnabled = configFile.Bind("MouseFlyer", $"{profileName}-IsAutoCamEnabled", true, "Choose whether the camera automatically changes to Chase");
            Deadzone = configFile.Bind("MouseFlyer", $"{profileName}-Deadzone", 0.01f, "Deadzone for mouse input");
            RollSensitivity = configFile.Bind("MouseFlyer", $"{profileName}-RollSensitivity", 0.2f, "Roll sensitivity for mouse input");
            PitchSensitivity = configFile.Bind("MouseFlyer", $"{profileName}-PitchSensitivity", 0.9f, "Pitch sensitivity for mouse input");
            YawCorrection = configFile.Bind("MouseFlyer", $"{profileName}-YawCorrection", 0.3f, "Yaw correction for mouse input");
        }
    }



    public CustomConfig(ConfigFile configFile)
    {
        _configFile = configFile;

        // Global settings...
        toggleFlyingModeKey = _configFile.Bind("MouseFlyer", "ToggleFlyingModeKey", KeyCode.I, "Key to toggle flying mode");
        toggleMouseSteeringKey = _configFile.Bind("MouseFlyer", "ToggleMouseSteeringKey", KeyCode.O, "Key to toggle mouse steering");
        toggleMenuKey = _configFile.Bind("MouseFlyer", "ToggleMenuKey", KeyCode.P, "Key to toggle menu");
        previousProfileKey = _configFile.Bind("MouseFlyer", "PreviousProfileKey", KeyCode.LeftBracket, "Key to switch to previous profile");
        nextProfileKey = _configFile.Bind("MouseFlyer", "NextProfileKey", KeyCode.RightBracket, "Key to switch to next profile");
        flyingMode = _configFile.Bind("MouseFlyer", "FlyingMode", Settings.FlyingMode.Normal, "Normal or Alternative flying mode");

        // Initialize Profiles list and CurrentProfile...
        Profiles = new List<Profile>();
        for (int i = 1; i <= MaxProfiles; i++)
        {
            Profiles.Add(new Profile(_configFile, $"{i}"));
        }
        CurrentProfile = Profiles[0];

        Save();
    }


    [PublicAPI]
    public float SmoothingFactor
    {
        get => CurrentProfile.SmoothingFactor.Value;
        set => CurrentProfile.SmoothingFactor.Value = value;
    }

    [PublicAPI]
    public  bool IsYAxisInverted
    {
        get => CurrentProfile.IsYAxisInverted.Value;
        set => CurrentProfile.IsYAxisInverted.Value = value;
    }

    [PublicAPI]
    public  float Deadzone
    {
        get => CurrentProfile.Deadzone.Value;
        set => CurrentProfile.Deadzone.Value = value;
    }

    [PublicAPI]
    public  float RollSensitivity
    {
        get => CurrentProfile.RollSensitivity.Value;
        set => CurrentProfile.RollSensitivity.Value = value;
    }

    [PublicAPI]
    public  float PitchSensitivity
    {
        get => CurrentProfile.PitchSensitivity.Value;
        set => CurrentProfile.PitchSensitivity.Value = value;
    }

    [PublicAPI]
    public  float YawCorrection
    {
        get => CurrentProfile.YawCorrection.Value;
        set => CurrentProfile.YawCorrection.Value = value;
    }

    [PublicAPI]
    public bool IsAutoCamEnabled
    {
        get => CurrentProfile.IsAutoCamEnabled.Value;
        set => CurrentProfile.IsAutoCamEnabled.Value = value;
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

    public void Save()
    {
        _configFile.Save();
    }

}