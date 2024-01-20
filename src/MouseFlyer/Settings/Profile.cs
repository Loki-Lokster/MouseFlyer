using BepInEx.Configuration;

// This class defines the structure of a profile and handles the loading and saving of the profile to config
public class Profile 
{
    private ConfigFile configFile;

    // Profile variables
    public string Name { get; set; }
    public ConfigEntry<float> SmoothingFactor { get; private set; }
    public ConfigEntry<bool> IsYAxisInverted { get; private set; }
    public ConfigEntry<bool> IsAutoCamEnabled { get; private set; }
    public ConfigEntry<float> Deadzone { get; private set; }
    public ConfigEntry<float> RollSensitivity { get; private set; }
    public ConfigEntry<float> PitchSensitivity { get; private set; }
    public ConfigEntry<float> YawCorrection { get; private set; }

    // Copy of the profile variables
    public float SmoothingFactorCopy { get; set; }
    public bool IsYAxisInvertedCopy { get; set; }
    public bool IsAutoCamEnabledCopy { get; set; }
    public float DeadzoneCopy { get; set; }
    public float RollSensitivityCopy { get; set; }
    public float PitchSensitivityCopy { get; set; }
    public float YawCorrectionCopy { get; set; }

    public Profile(ConfigFile configFile, string profileName)
    {
        this.configFile = configFile;
        Name = profileName;
        LoadProfile();
    }


    private void LoadProfile()
    {
        SmoothingFactor = configFile.Bind("MouseFlyer", $"{Name}-SmoothingFactor", 0.05f, "Smoothing factor for mouse input");
        IsYAxisInverted = configFile.Bind("MouseFlyer", $"{Name}-IsYAxisInverted", true, "Invert the Y-axis for mouse input");
        IsAutoCamEnabled = configFile.Bind("MouseFlyer", $"{Name}-IsAutoCamEnabled", true, "Choose whether the camera automatically changes to Chase");
        Deadzone = configFile.Bind("MouseFlyer", $"{Name}-Deadzone", 0.01f, "Deadzone for mouse input");
        RollSensitivity = configFile.Bind("MouseFlyer", $"{Name}-RollSensitivity", 0.2f, "Roll sensitivity for mouse input");
        PitchSensitivity = configFile.Bind("MouseFlyer", $"{Name}-PitchSensitivity", 0.9f, "Pitch sensitivity for mouse input");
        YawCorrection = configFile.Bind("MouseFlyer", $"{Name}-YawCorrection", 0.3f, "Yaw correction for mouse input");

        // Initialize the copy properties with the values from the config file
        SmoothingFactorCopy = SmoothingFactor.Value;
        IsYAxisInvertedCopy = IsYAxisInverted.Value;
        IsAutoCamEnabledCopy = IsAutoCamEnabled.Value;
        DeadzoneCopy = Deadzone.Value;
        RollSensitivityCopy = RollSensitivity.Value;
        PitchSensitivityCopy = PitchSensitivity.Value;
        YawCorrectionCopy = YawCorrection.Value;
    }

    // Save changes to the profile
    public void ApplyChanges()
    {
        SmoothingFactor.Value = SmoothingFactorCopy;
        IsYAxisInverted.Value = IsYAxisInvertedCopy;
        IsAutoCamEnabled.Value = IsAutoCamEnabledCopy;
        Deadzone.Value = DeadzoneCopy;
        RollSensitivity.Value = RollSensitivityCopy;
        PitchSensitivity.Value = PitchSensitivityCopy;
        YawCorrection.Value = YawCorrectionCopy;
    }

    public void SaveProfile()
    {
        configFile.Save();
    }

    public void RevertChanges()
    {
        SmoothingFactorCopy = SmoothingFactor.Value;
        IsYAxisInvertedCopy = IsYAxisInverted.Value;
        IsAutoCamEnabledCopy = IsAutoCamEnabled.Value;
        DeadzoneCopy = Deadzone.Value;
        RollSensitivityCopy = RollSensitivity.Value;
        PitchSensitivityCopy = PitchSensitivity.Value;
        YawCorrectionCopy = YawCorrection.Value;
    }
}