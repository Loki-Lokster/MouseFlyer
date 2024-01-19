using MouseFlyer;
using SpaceWarp.API.Assets;
using UnityEngine;
public class UIManager
{
    private Settings settings;
    private CustomConfig config;

    // Fonts
    private GUIStyle boldLabelStyle;
    private GUIStyle smallLabelStyle;
    private GUIStyle detailsLabelStyle;

    public UIManager(Settings settings, CustomConfig config)
    {
        this.settings = settings;
        this.config = config;
        Init_Fonts();
    }

    // Initialise the font styles, better way to do this but can't be bothered ¯\_(ツ)_/¯
    private void Init_Fonts()
    {
        // Set the font style to bold and white
        boldLabelStyle = new GUIStyle();
        boldLabelStyle.fontSize = 14;
        boldLabelStyle.fontStyle = FontStyle.Bold;
        boldLabelStyle.normal.textColor = Color.white;
        boldLabelStyle.wordWrap = true;

        smallLabelStyle = new GUIStyle();
        smallLabelStyle.fontSize = 13;
        smallLabelStyle.normal.textColor = Color.white;
        smallLabelStyle.wordWrap = true;

        detailsLabelStyle = new GUIStyle();
        detailsLabelStyle.fontSize = 12;
        detailsLabelStyle.alignment = TextAnchor.MiddleCenter;
        detailsLabelStyle.normal.textColor = Color.gray;
        detailsLabelStyle.wordWrap = true;
    }

    /// <summary>
    /// Set the cursor state based on the current flying mode.
    /// </summary>
    /// <param name="mode">The current flying mode.</param>
    public void SetCursorState(Settings.FlyingMode? mode)
    {
        switch (mode)
        {
            case Settings.FlyingMode.Alternative:
                // Unlock the cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // Change the cursor to a custom texture
                Texture2D cursorTexture = AssetManager.GetAsset<Texture2D>($"{MouseFlyerPlugin.ModGuid}/images/crosshair.png");
                Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

                settings.IsCursorLocked = false;
                break;

            case Settings.FlyingMode.Normal:
                // Lock and hide the cursor while moving it off screen
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Cursor.SetCursor(null, new Vector2(Screen.width, Screen.height), CursorMode.Auto);
                settings.IsCursorLocked = true;
                break;

            case null:
                // Reset to default cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

                settings.IsCursorLocked = false;
                break;
        }
    }

    /// <summary>
    /// Defines the content of the UI window drawn in the <code>OnGui</code> method.
    /// </summary>
    /// <param name="windowID"></param>
    public void FillWindow(int windowID)
    {
        // Start a horizontal group
        GUILayout.BeginHorizontal();
        // Add flexible space
        GUILayout.FlexibleSpace();
        GUILayout.Label("Toggle Menu: (" + settings.ToggleMenuKey.ToString() + ")", smallLabelStyle);
        GUILayout.FlexibleSpace();
        // Close button on the top right
        if (GUILayout.Button("X", boldLabelStyle, GUILayout.ExpandWidth(false)))
        {
            settings.IsWindowOpen = false;

        }
        // End the horizontal group
        GUILayout.EndHorizontal();

        // Horizontal line
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

        // Flying mode switch
        GUILayout.Label("Flying Mode (" + settings.ToggleFlyingModeKey.ToString() + "):", boldLabelStyle);
        string[] flyingModes = { "Normal", "Alternative" };
        settings.CurrentFlyingMode = (Settings.FlyingMode)GUILayout.SelectionGrid((int)settings.CurrentFlyingMode, flyingModes, 2);

        if (settings.CurrentFlyingMode == Settings.FlyingMode.Normal)
        {
            GUILayout.Label("Mouse cursor is locked and roll/pitch only occurs while the mouse is moving.", detailsLabelStyle);
        }
        else
        {
            GUILayout.Label("Mouse cursor is unlocked and roll/pitch rate is based on distance from the center of the screen. Deadzone can be configured.", detailsLabelStyle);
        }
        
        // Mouse steering toggle
        settings.IsMouseSteeringEnabled = GUILayout.Toggle(settings.IsMouseSteeringEnabled, "Enable Mouse Steering (" + settings.ToggleMouseSteeringKey.ToString() + ")", GUILayout.ExpandWidth(false));
        
        // Invert Y axis toggle
        settings.CurrentProfile.IsYAxisInverted.Value = GUILayout.Toggle(settings.CurrentProfile.IsYAxisInverted.Value, "Invert Y Axis", GUILayout.ExpandWidth(false));
        
        // Auto cam toggle
        settings.CurrentProfile.IsAutoCamEnabled.Value = GUILayout.Toggle(settings.CurrentProfile.IsAutoCamEnabled.Value, "Enable Auto Camera", GUILayout.ExpandWidth(false));
        
        // Roll sensitivity input
        GUILayout.BeginHorizontal();
        GUILayout.Label("Roll Sensitivity:");
        float rollSensitivity;
        if (float.TryParse(GUILayout.TextField(settings.CurrentProfile.RollSensitivity.Value.ToString("0.00"), GUILayout.ExpandWidth(false)), out rollSensitivity))
        {
            settings.CurrentProfile.RollSensitivity.Value = rollSensitivity;
        }
        GUILayout.EndHorizontal();
        settings.CurrentProfile.RollSensitivity.Value = GUILayout.HorizontalSlider(settings.CurrentProfile.RollSensitivity.Value, 0.0f, 5f);

        // Pitch sensitivity input
        GUILayout.BeginHorizontal();
        GUILayout.Label("Pitch Sensitivity:");
        float pitchSensitivity;
        if (float.TryParse(GUILayout.TextField(settings.CurrentProfile.PitchSensitivity.Value.ToString("0.00"), GUILayout.ExpandWidth(false)), out pitchSensitivity))
        {
            settings.CurrentProfile.PitchSensitivity.Value = pitchSensitivity;
        }
        GUILayout.EndHorizontal();
        settings.CurrentProfile.PitchSensitivity.Value = GUILayout.HorizontalSlider(settings.CurrentProfile.PitchSensitivity.Value, 0.0f, 5f);

        // Yaw correction input
        GUILayout.BeginHorizontal();
        GUILayout.Label("Yaw Correction:");
        float yawCorrection;
        if (float.TryParse(GUILayout.TextField(settings.CurrentProfile.YawCorrection.Value.ToString("0.00"), GUILayout.ExpandWidth(false)), out yawCorrection))
        {
            settings.CurrentProfile.YawCorrection.Value = yawCorrection;
        }
        GUILayout.EndHorizontal();
        settings.CurrentProfile.YawCorrection.Value = GUILayout.HorizontalSlider(settings.CurrentProfile.YawCorrection.Value, 0.0f, 5f);

        // Deadzone input
        if (settings.CurrentFlyingMode == Settings.FlyingMode.Alternative)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Deadzone:");
            float deadzone;
            if (float.TryParse(GUILayout.TextField(settings.CurrentProfile.Deadzone.Value.ToString("0.00"), GUILayout.ExpandWidth(false)), out deadzone))
            {
                settings.CurrentProfile.Deadzone.Value = deadzone;
            }
            GUILayout.EndHorizontal();
            settings.CurrentProfile.Deadzone.Value = GUILayout.HorizontalSlider(settings.CurrentProfile.Deadzone.Value, 0.0f, 1.0f);
        }

        // Smoothing input
        GUILayout.BeginHorizontal();
        GUILayout.Label("Smoothing:");
        float smoothingFactor;
        if (float.TryParse(GUILayout.TextField(settings.CurrentProfile.SmoothingFactor.Value.ToString("0.00"), GUILayout.ExpandWidth(false)), out smoothingFactor))
        {
            settings.CurrentProfile.SmoothingFactor.Value = smoothingFactor;
        }
        GUILayout.EndHorizontal();
        settings.CurrentProfile.SmoothingFactor.Value = GUILayout.HorizontalSlider(settings.CurrentProfile.SmoothingFactor.Value, 0.0f, 1.0f);

        // Horizontal line
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

        // Profile selection

        GUILayout.Label("Profile:");
        int selectedProfileIndex = config.Profiles.IndexOf(settings.CurrentProfile);
        string[] profileNames = config.Profiles.Select(p => p.Name).ToArray();
        int newSelectedProfileIndex = GUILayout.SelectionGrid(selectedProfileIndex, profileNames, profileNames.Length);
        if (newSelectedProfileIndex != selectedProfileIndex)
        {
            settings.CurrentProfile = config.Profiles[newSelectedProfileIndex];
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Previous (" + settings.PreviousProfileKey.ToString() + ")", detailsLabelStyle, GUILayout.ExpandWidth(false));
        GUILayout.FlexibleSpace();
        GUILayout.Label("Next (" + settings.NextProfileKey.ToString() + ")", detailsLabelStyle, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        // Horizontal line
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

        if (GUILayout.Button("Save Settings"))
        {

            config.FlyingMode = settings.CurrentFlyingMode;
            config.Save();
        }

        // Debug values
        // if (GUILayout.Button(settings.ShowDebugValues ? "Hide Debug Values" : "Show Debug Values"))
        //     {
        //         settings.ShowDebugValues = !settings.ShowDebugValues;
        //     }

        // if (settings.ShowDebugValues)
        // {
        //     var Vessel = MouseFlyerPlugin.Vessel;
        //     GUILayout.Label("Debug Values", boldLabelStyle);
        //     Vector3 mousePosition = Input.mousePosition;
        //     GUILayout.Label("Mouse Position: " + mousePosition);
        //     GUILayout.Label("Mouse Steering Enabled: " + (settings.IsMouseSteeringEnabled ? "Yes" : "No"));
        //     GUILayout.Label($"Pitch: {Vessel.pitch.ToString("0.00")}");
        //     GUILayout.Label($"Roll: {Vessel.roll.ToString("0.00")}");
        //     GUILayout.Label($"Yaw: {Vessel.yaw.ToString("0.00")}");
        //     GUILayout.Label($"Throttle: {Vessel.mainThrottle.ToString("0.00")}");
        // }

        GUI.DragWindow(new Rect(0, 0, 10000, 500));
    }
}