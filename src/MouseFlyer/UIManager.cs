using MouseFlyer;
using SpaceWarp.API.Assets;
using UnityEngine;
public class UIManager
{
    private Settings settings;
    private MouseFlyerPlugin plugin;
    private CustomConfig config;

    // Fonts
    private GUIStyle boldLabelStyle;
    private GUIStyle smallLabelStyle;
    private GUIStyle detailsLabelStyle;

    public UIManager(Settings settings, MouseFlyerPlugin plugin, CustomConfig config)
    {
        this.settings = settings;
        this.plugin = plugin;
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
                Texture2D cursorTexture = AssetManager.GetAsset<Texture2D>($"{MouseFlyerPlugin.ModGuid}/images/icon.png");
                Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                break;

            case Settings.FlyingMode.Normal:
                // Lock the cursor and reset to the default texture
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;

            case null:
                // Reset to default cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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
        
        settings.IsMouseSteeringEnabled = GUILayout.Toggle(settings.IsMouseSteeringEnabled, "Enable Mouse Steering (" + settings.ToggleMouseSteeringKey.ToString() + ")", GUILayout.ExpandWidth(false));
        settings.IsYAxisInverted = GUILayout.Toggle(settings.IsYAxisInverted, "Invert Y Axis", GUILayout.ExpandWidth(false));
        // Roll sensitivity slider
        GUILayout.Label("Roll Sensitivity: " + settings.RollSensitivity.ToString("0.00"), boldLabelStyle);
        settings.RollSensitivity = GUILayout.HorizontalSlider(settings.RollSensitivity, 0.01f, 4.0f);

        // Pitch sensitivity slider
        GUILayout.Label("Pitch Sensitivity: " + settings.PitchSensitivity.ToString("0.00"), boldLabelStyle);
        settings.PitchSensitivity = GUILayout.HorizontalSlider(settings.PitchSensitivity, 0.01f, 4.0f);

        // Yaw correction slider
        GUILayout.Label("Yaw Correction: " + settings.YawCorrection.ToString("0.00"), boldLabelStyle);
        settings.YawCorrection = GUILayout.HorizontalSlider(settings.YawCorrection, 0.0f, 1.0f);
        
        // Deadzone slider
        if (settings.CurrentFlyingMode == Settings.FlyingMode.Alternative){
            GUILayout.Label("Deadzone: " + settings.Deadzone.ToString("0.00"), boldLabelStyle);
            settings.Deadzone = GUILayout.HorizontalSlider(settings.Deadzone, 0.0f, 0.5f);
        }

        // Smoothing slider
        GUILayout.Label("Smoothing: " + settings.SmoothingFactor.ToString("0.00"), boldLabelStyle);
        settings.SmoothingFactor = GUILayout.HorizontalSlider(settings.SmoothingFactor, 0.0f, 1.0f);

        // Horizontal line
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

        if (GUILayout.Button("Save Settings"))
        {
            config.SmoothingFactor = settings.SmoothingFactor;
            config.IsYAxisInverted = settings.IsYAxisInverted;
            config.Deadzone = settings.Deadzone;
            config.RollSensitivity = settings.RollSensitivity;
            config.PitchSensitivity = settings.PitchSensitivity;
            config.YawCorrection = settings.YawCorrection;
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