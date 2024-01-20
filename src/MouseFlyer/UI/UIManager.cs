using MouseFlyer;
using SpaceWarp.API.Assets;
using UnityEngine;
public class UIManager
{
    private Settings settings;

    // Fonts
    private GUIStyle boldLabelStyle;
    private GUIStyle smallLabelStyle;
    private GUIStyle detailsLabelStyle;

    public UIManager(Settings settings)
    {
        this.settings = settings;
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
    public void SetCursorState(GlobalSettings.FlyingMode? mode)
    {
        switch (mode)
        {
            case GlobalSettings.FlyingMode.Alternative:
                // Unlock the cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // Change the cursor to a custom texture
                Texture2D cursorTexture = AssetManager.GetAsset<Texture2D>($"{MouseFlyerPlugin.ModGuid}/images/crosshair.png");
                Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

                settings.Runtime.IsCursorLocked = false;
                break;

            case GlobalSettings.FlyingMode.Normal:
                // Lock and hide the cursor while moving it off screen
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Cursor.SetCursor(null, new Vector2(Screen.width, Screen.height), CursorMode.Auto);
                settings.Runtime.IsCursorLocked = true;
                break;

            case null:
                // Reset to default cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

                settings.Runtime.IsCursorLocked = false;
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
        GUILayout.Label("Toggle Menu: (" + settings.Global.ToggleMenuKey.Value.ToString() + ")", smallLabelStyle);
        GUILayout.FlexibleSpace();
        // Close button on the top right
        if (GUILayout.Button("X", boldLabelStyle, GUILayout.ExpandWidth(false)))
        {
            settings.Runtime.IsWindowOpen = false;

        }
        // End the horizontal group
        GUILayout.EndHorizontal();

        // Horizontal line
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

        // Flying mode switch
        GUILayout.Label("Flying Mode (" + settings.Global.ToggleFlyingModeKey.Value.ToString() + "):", boldLabelStyle);
        string[] flyingModes = { "Normal", "Alternative" };
        settings.Global.CurrentFlyingMode.Value = (GlobalSettings.FlyingMode)GUILayout.SelectionGrid((int)settings.Global.CurrentFlyingMode.Value, flyingModes, 2);

        if (settings.Global.CurrentFlyingMode.Value == GlobalSettings.FlyingMode.Normal)
        {
            GUILayout.Label("Mouse cursor is locked and roll/pitch only occurs while the mouse is moving.", detailsLabelStyle);
        }
        else
        {
            GUILayout.Label("Mouse cursor is unlocked and roll/pitch rate is based on distance from the center of the screen. Deadzone can be configured.", detailsLabelStyle);
        }
        
        // Mouse steering toggle
        settings.Runtime.IsMouseSteeringEnabled = GUILayout.Toggle(settings.Runtime.IsMouseSteeringEnabled, "Enable Mouse Steering (" + settings.Global.ToggleMouseSteeringKey.Value.ToString() + ")", GUILayout.ExpandWidth(false));
        
        // Invert Y axis toggle
        settings.ActiveProfile.IsYAxisInvertedCopy = GUILayout.Toggle(settings.ActiveProfile.IsYAxisInvertedCopy, "Invert Y Axis", GUILayout.ExpandWidth(false));
        
        // Auto cam toggle
        settings.ActiveProfile.IsAutoCamEnabledCopy = GUILayout.Toggle(settings.ActiveProfile.IsAutoCamEnabledCopy, "Enable Auto Camera", GUILayout.ExpandWidth(false));
        
        // Roll sensitivity input
        GUILayout.BeginHorizontal();
        GUILayout.Label("Roll Sensitivity:");
        float rollSensitivity;
        if (float.TryParse(GUILayout.TextField(settings.ProfileManager.ActiveProfile.RollSensitivityCopy.ToString("0.00"), GUILayout.ExpandWidth(false)), out rollSensitivity))
        {
            settings.ActiveProfile.RollSensitivityCopy = rollSensitivity;
        }
        GUILayout.EndHorizontal();
        settings.ActiveProfile.RollSensitivityCopy = GUILayout.HorizontalSlider(settings.ActiveProfile.RollSensitivityCopy, 0.0f, 5f);

        // Pitch sensitivity input
        GUILayout.BeginHorizontal();
        GUILayout.Label("Pitch Sensitivity:");
        float pitchSensitivity;
        if (float.TryParse(GUILayout.TextField(settings.ActiveProfile.PitchSensitivityCopy.ToString("0.00"), GUILayout.ExpandWidth(false)), out pitchSensitivity))
        {
            settings.ActiveProfile.PitchSensitivityCopy = pitchSensitivity;
        }
        GUILayout.EndHorizontal();
        settings.ActiveProfile.PitchSensitivityCopy = GUILayout.HorizontalSlider(settings.ActiveProfile.PitchSensitivityCopy, 0.0f, 5f);

        // Yaw correction input
        GUILayout.BeginHorizontal();
        GUILayout.Label("Yaw Correction:");
        float yawCorrection;
        if (float.TryParse(GUILayout.TextField(settings.ActiveProfile.YawCorrectionCopy.ToString("0.00"), GUILayout.ExpandWidth(false)), out yawCorrection))
        {
            settings.ActiveProfile.YawCorrectionCopy = yawCorrection;
        }
        GUILayout.EndHorizontal();
        settings.ActiveProfile.YawCorrectionCopy = GUILayout.HorizontalSlider(settings.ActiveProfile.YawCorrectionCopy, 0.0f, 5f);

        // Deadzone input
        if (settings.Global.CurrentFlyingMode.Value == GlobalSettings.FlyingMode.Alternative)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Deadzone:");
            float deadzone;
            if (float.TryParse(GUILayout.TextField(settings.ActiveProfile.DeadzoneCopy.ToString("0.00"), GUILayout.ExpandWidth(false)), out deadzone))
            {
                settings.ActiveProfile.DeadzoneCopy = deadzone;
            }
            GUILayout.EndHorizontal();
            settings.ActiveProfile.DeadzoneCopy = GUILayout.HorizontalSlider(settings.ActiveProfile.DeadzoneCopy, 0.0f, 1.0f);
        }

        // Smoothing input
        GUILayout.BeginHorizontal();
        GUILayout.Label("Smoothing:");
        float smoothingFactor;
        if (float.TryParse(GUILayout.TextField(settings.ActiveProfile.SmoothingFactorCopy.ToString("0.00"), GUILayout.ExpandWidth(false)), out smoothingFactor))
        {
            settings.ActiveProfile.SmoothingFactorCopy = smoothingFactor;
        }
        GUILayout.EndHorizontal();
        settings.ActiveProfile.SmoothingFactorCopy = GUILayout.HorizontalSlider(settings.ActiveProfile.SmoothingFactorCopy, 0.0f, 1.0f);

        // Horizontal line
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

        // Profile selection

        GUILayout.Label("Profile:");
        int selectedProfileIndex = settings.ProfileManager.Profiles.ToList().IndexOf(settings.ProfileManager.ActiveProfile);
        string[] profileNames = settings.ProfileManager.Profiles.Select(p => p.Name).ToArray();
        int newSelectedProfileIndex = GUILayout.SelectionGrid(selectedProfileIndex, profileNames, profileNames.Length);
        if (newSelectedProfileIndex != selectedProfileIndex)
        {
            settings.ProfileManager.SetActiveProfile(settings.ProfileManager.Profiles[newSelectedProfileIndex]);
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Previous (" + settings.Global.PreviousProfileKey.Value.ToString() + ")", detailsLabelStyle, GUILayout.ExpandWidth(false));
        GUILayout.FlexibleSpace();
        GUILayout.Label("Next (" + settings.Global.NextProfileKey.Value.ToString() + ")", detailsLabelStyle, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        // Horizontal line
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

        if (GUILayout.Button("Save Profile"))
        {
            settings.ProfileManager.ActiveProfile.ApplyChanges();
            settings.ProfileManager.ActiveProfile.SaveProfile();
        }

        if (GUILayout.Button("Revert Profile"))
        {
            settings.ProfileManager.ActiveProfile.RevertChanges();
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