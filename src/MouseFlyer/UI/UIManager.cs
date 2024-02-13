using BepInEx.Configuration;
using MouseFlyer;
using SpaceWarp.API.Assets;
using UnityEngine;
using UnityEngine.UI;
public class UIManager
{
    private Settings settings;

    // Fonts
    private GUIStyle boldLabelStyle;
    private GUIStyle smallLabelStyle;
    private GUIStyle detailsLabelStyle;
    private GUIStyle setKeyButtonStyle;

    // Textures
    private Texture2D _lineTexture;

    // Shortcut key bools
    private bool isSettingToggleMenuKey = false;
    private bool isSettingToggleHUDKey = false;
    private bool isSettingFlyingModeKey = false;
    private bool isSettingToggleSteeringKey = false;
    private bool isSettingPreviousProfileKey = false;
    private bool isSettingNextProfileKey = false;

    public UIManager(Settings settings)
    {
        this.settings = settings;
        Init_Fonts();
        Init_Textures();
    }

    /// <summary>
    /// Initializes the fonts used by the UI manager.
    /// </summary>
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

    // Initialize the textures
    public void Init_Textures()
    {
        _lineTexture = new Texture2D(1, 1);
        _lineTexture.SetPixel(0, 0, Color.white);
        _lineTexture.Apply();
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

                Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
                Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);

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
    /// Draws a button for setting a key in the UI.
    /// </summary>
    /// <param name="settings">The settings object.</param>
    /// <param name="keyName">The name of the key property.</param>
    /// <param name="isSettingKey">A reference to a boolean indicating whether the user is currently setting a key.</param>
    void DrawSetKeyButton(Settings settings, string keyName, ref bool isSettingKey)
    {

        setKeyButtonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 13,
            hover = { textColor = Color.yellow }

        };

        KeyCode currentKey = ((ConfigEntry<KeyCode>)settings.Global.GetType().GetProperty(keyName).GetValue(settings.Global, null)).Value;

        if (isSettingKey)
        {
            setKeyButtonStyle.normal.background = MakeTex(2, 2, Color.green);
            setKeyButtonStyle.normal.textColor = Color.red;

            GUILayout.Button("Press any key", setKeyButtonStyle, GUILayout.Width(140));
            SetNewShortcutKey(settings, keyName, ref isSettingKey);
        }
        else
        {
            if (GUILayout.Button(currentKey.ToString(), setKeyButtonStyle, GUILayout.Width(140)))
            {
                isSettingKey = true;
            }
        }

        setKeyButtonStyle.normal.background = null;
        setKeyButtonStyle.normal.textColor = Color.white;
    }

    /// <summary>
    /// Sets a new shortcut key for a specific setting in the provided settings object.
    /// </summary>
    /// <param name="settings">The settings object to modify.</param>
    /// <param name="keyName">The name of the setting key to modify.</param>
    /// <param name="isSettingKey">A reference to a boolean indicating whether the key is being set.</param>
    private void SetNewShortcutKey(Settings settings, string keyName, ref bool isSettingKey)
    {
        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Event.current.isKey && Event.current.keyCode == keyCode)
            {
                switch (keyName)
                {
                    case "ToggleMenuKey":
                        settings.Global.ToggleMenuKey.Value = keyCode;
                        break;

                    case "ToggleHUDKey":
                        settings.Global.ToggleHUDKey.Value = keyCode;
                        break;
                    
                    case "ToggleFlyingModeKey":
                        settings.Global.ToggleFlyingModeKey.Value = keyCode;
                        break;

                    case "ToggleMouseSteeringKey":
                        settings.Global.ToggleMouseSteeringKey.Value = keyCode;
                        break;

                    case "PreviousProfileKey":
                        settings.Global.PreviousProfileKey.Value = keyCode;
                        break;

                    case "NextProfileKey":
                        settings.Global.NextProfileKey.Value = keyCode;
                        break;
                }
                isSettingKey = false;
                break;
            }
        }
    }

    /// <summary>
    /// Creates a new Texture2D with the specified width, height, and color.
    /// </summary>
    /// <param name="width">The width of the texture.</param>
    /// <param name="height">The height of the texture.</param>
    /// <param name="col">The color to fill the texture with.</param>
    /// <returns>The newly created Texture2D.</returns>
    Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }


    /// <summary>
    /// Draws a line between two points with a specified color and width.
    /// </summary>
    /// <param name="pointA">The starting point of the line.</param>
    /// <param name="pointB">The ending point of the line.</param>
    /// <param name="color">The color of the line.</param>
    /// <param name="width">The width of the line.</param>
    private void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
    {
        // Save the current GUI color
        Color savedColor = GUI.color;

        // Set the GUI color to the color of the line
        GUI.color = color;

        // Determine the angle of the line
        float angle = Vector3.Angle(pointB - pointA, Vector2.right);

        // Vector3.Angle always returns a positive result.
        // If pointB is above pointA, then angle needs to be negated.
        if (pointA.y > pointB.y) { angle = -angle; }

        // Use ScaleAroundPivot to adjust the size of the line
        // We can use a 1x1 pixel texture (created above) and stretch it to fill the line area.
        GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), pointA);

        // Use RotateAroundPivot to rotate the GUI around the start of the line
        GUIUtility.RotateAroundPivot(angle, pointA);

        // Draw the line
        // GUI.DrawTexture will stretch the 1x1 pixel texture to fill the designated area
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), _lineTexture);

        // We're done. Restore the GUI matrix and color to their previous settings.
        GUI.matrix = Matrix4x4.identity;
        GUI.color = savedColor;
    }
    

    /// <summary>
    /// Draws a circle on the screen.
    /// </summary>
    /// <param name="center">The center position of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color of the circle.</param>
    /// <param name="opacity">The opacity of the circle.</param>
    public void DrawCircle(Vector2 center, float radius, Color color, float opacity)
    {
        Texture2D _circleTexture = AssetManager.GetAsset<Texture2D>($"{MouseFlyerPlugin.ModGuid}/images/circle.png");
        // Save the current GUI color
        Color savedColor = GUI.color;

        // Set the GUI color to the color of the circle
        GUI.color = new Color(color.r, color.g, color.b, opacity);

        // Draw the circle
        GUI.DrawTexture(new Rect(center.x - radius, center.y - radius, radius * 2, radius * 2), _circleTexture);

        // Restore the GUI color
        GUI.color = savedColor;
    }

    /// <summary>
    /// Draws a line from the specified center point to the current mouse position.
    /// </summary>
    /// <param name="center">The center point of the line.</param>
    /// <param name="color">The color of the line.</param>
    /// <param name="width">The width of the line.</param>
    void DrawLineToMouse(Vector2 mousePosition, Color color, float width, float opacity)
    {
        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);

        // Invert the y-coordinate of the mouse position if IsYAxisInvertedCopy is false
        if (!settings.ActiveProfile.IsYAxisInvertedCopy && settings.Global.CurrentFlyingMode.Value == GlobalSettings.FlyingMode.Normal)
        {
            mousePosition.y = Screen.height - mousePosition.y;
        }

        Vector2 direction = (mousePosition - center).normalized;

        // Calculate the distance from the center to the mouse position
        float distance = Vector2.Distance(center, mousePosition);

        // Calculate the maximum distance (the radius of the circle)
        float maxDistance = (settings.Global.OuterCircleRadiusRatio.Value * Screen.height) - 5;

        // If the mouse position is outside the circle, limit the distance to the radius of the circle
        if (distance > maxDistance)
        {
            distance = maxDistance;
            mousePosition = center + direction * distance; // Adjust the mouse position to be on the circle
        }

        // If the mouse position is within the deadzone, set the color to red
        float deadzone = Screen.height * settings.Global.OuterCircleRadiusRatio.Value * settings.ActiveProfile.DeadzoneCopy;
        if (distance < deadzone && settings.Global.CurrentFlyingMode.Value == GlobalSettings.FlyingMode.Alternative)
        {
            color = Color.red;
        }

        // Set the number of segments and the base length of each segment
        int numSegments = 15;
        float baseSegmentLength = 10;
        float segmentLengthIncrease = 15;
        float gapLength = 15; // Adjust this value to increase or decrease the gap size

        // Set the opacity of the color
        color.a = opacity;

        float totalLength = 0;
        for (int i = 0; i < numSegments; i++)
        {
            // Modify the segment length based on its distance from the center
            float segmentLength = baseSegmentLength + i * segmentLengthIncrease;

            if (totalLength + segmentLength > distance)
            {
                segmentLength = distance - totalLength;
            }

            Vector2 offset = new Vector2(-direction.y, direction.x) * width / 2;
            Vector2 startPoint = center + direction * totalLength - offset;
            Vector2 endPoint = startPoint + direction * segmentLength;

            // Calculate the opacity of the current segment
            float currentOpacity = opacity * (numSegments - i) / numSegments;
            color.a = currentOpacity;

            DrawLine(startPoint, endPoint, color, width);

            totalLength += segmentLength;

            if (totalLength >= distance)
            {
                break;
            }

            totalLength += gapLength;
            color.a -= opacity / numSegments;

            if (totalLength >= distance)
            {
                break;
            }
        }
    }

    public void DrawHUDText()
    {
        string flyingMode = "FLYING MODE: ";
        string profile = "PROFILE: ";
        string flyingModeValue = settings.Global.CurrentFlyingMode.Value.ToString().ToUpper();
        string profileValue = settings.ProfileManager.ActiveProfile.Name;
        float labelWidth = 210;
        float labelHeight = 50;
        float padding = 10; // Padding inside the box
        float bottomMargin = 100;
        float circleRadius = Screen.height * settings.Global.OuterCircleRadiusRatio.Value;
        float xPos = (Screen.width - labelWidth) / 2 + padding; // Center horizontally and add padding
        float yPos = (Screen.height / 2) - circleRadius - bottomMargin + padding; // Position at the top of the circle and add padding

        // Create a GUIStyle for the labels
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.normal.textColor = new Color(settings.Global.HUDColor.Value.r, settings.Global.HUDColor.Value.g, settings.Global.HUDColor.Value.b, settings.Global.HUDOpacity.Value); // Set the text color to HUDColor with opacity
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.fontSize = 13;

        // Create a GUIStyle for the values
        GUIStyle valueStyle = new GUIStyle(GUI.skin.label);
        valueStyle.normal.textColor = new Color(255, 255, 255, settings.Global.HUDOpacity.Value); // Set the text color to white with opacity
        valueStyle.fontSize = 13;

        // Draw a box with HUD background colour
        GUI.backgroundColor = new Color(settings.Global.HUDColor.Value.r, settings.Global.HUDColor.Value.g, settings.Global.HUDColor.Value.b, settings.Global.HUDOpacity.Value);
        GUI.Box(new Rect(xPos - padding, yPos - padding, labelWidth + 2 * padding, labelHeight + 2 * padding), "");

        // Draw the labels and values
        GUI.Label(new Rect(xPos, yPos, labelWidth, labelHeight), flyingMode, labelStyle);
        GUI.Label(new Rect(xPos + GUI.skin.label.CalcSize(new GUIContent(flyingMode)).x, yPos, labelWidth, labelHeight), flyingModeValue, valueStyle);
        GUI.Label(new Rect(xPos, yPos + GUI.skin.label.CalcSize(new GUIContent(flyingMode)).y, labelWidth, labelHeight), profile, labelStyle);
        GUI.Label(new Rect(xPos + GUI.skin.label.CalcSize(new GUIContent(profile)).x, yPos + GUI.skin.label.CalcSize(new GUIContent(flyingMode)).y, labelWidth, labelHeight), profileValue, valueStyle);
    }

    public void DrawHUD(Vector2 mousePosition)
    {
        if (!settings.Runtime.IsHUDVisible)
        {
            return;
        }
        
        if (settings.Runtime.ShowHUDOuterCircle)
        {
            // Draw the outer perimeter circle of the HUD (30% of the screen width and 50% opacity)
            DrawCircle(new Vector2(Screen.width / 2, Screen.height / 2), Screen.height * settings.Global.OuterCircleRadiusRatio.Value, settings.Global.HUDColor.Value, settings.Global.HUDOpacity.Value);
        }

        // Draw the line from the center to the mouse position
        DrawLineToMouse(mousePosition, settings.Global.HUDColor.Value, 4, settings.Global.HUDOpacity.Value);

        if (settings.Global.CurrentFlyingMode.Value == GlobalSettings.FlyingMode.Alternative && settings.Runtime.ShowHUDInnerCircle)
        {
            // Draw the inner deadzone radius circle
            DrawCircle(new Vector2(Screen.width / 2, Screen.height / 2), Screen.height * settings.Global.OuterCircleRadiusRatio.Value * settings.ActiveProfile.DeadzoneCopy, settings.Global.HUDColor.Value, settings.Global.HUDOpacity.Value);
        }
        
        if (settings.Runtime.ShowHUDTextPanel)
        {
            // Draw the HUD text
            DrawHUDText();
        }
    }

    /// <summary>
    /// Defines the content of the UI window drawn in the <code>OnGui</code> method.
    /// </summary>
    /// <param name="windowID"></param>
    public void FillWindow(int windowID)
    {
        DrawTopBar();
        // Horizontal line
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

        switch (selectedTab)
        {
            case 0:
                DrawFlightSettings();
                break;
            case 1:
                DrawHUDSettings();
                break;
            case 2:
                DrawKeyBindSettings();
                break;
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

    private int selectedTab = 0;
    private string[] tabs = { "Flight Settings", "HUD Settings", "Key Bindings" };

    private void DrawTopBar()
    {
        GUILayout.BeginHorizontal();

        // Tabs
        selectedTab = GUILayout.Toolbar(selectedTab, tabs);

        GUILayout.FlexibleSpace();

        // Close button on the top right
        if (GUILayout.Button(" X ", GUILayout.ExpandWidth(false)))
        {
            settings.Runtime.IsWindowOpen = false;
        }

        GUILayout.EndHorizontal();
    }

    private void DrawKeyBindSettings()
    {
        // Horizontal line
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

        GUILayout.Space(5);

        // Mouse steering toggle
        GUILayout.BeginHorizontal();
        GUILayout.Label("Toggle Mouse Steering:", smallLabelStyle);
        GUILayout.FlexibleSpace();
        DrawSetKeyButton(settings, "ToggleMouseSteeringKey", ref isSettingToggleSteeringKey);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Flying mode switch
        GUILayout.BeginHorizontal();
        GUILayout.Label("Toggle Flying Mode:", smallLabelStyle);
        GUILayout.FlexibleSpace();
        DrawSetKeyButton(settings, "ToggleFlyingModeKey", ref isSettingFlyingModeKey);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Toggle Menu key
        GUILayout.BeginHorizontal();
        GUILayout.Label("Toggle Menu:", smallLabelStyle);
        GUILayout.FlexibleSpace();
        DrawSetKeyButton(settings, "ToggleMenuKey", ref isSettingToggleMenuKey);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Toggle HUD key
        GUILayout.BeginHorizontal();
        GUILayout.Label("Toggle HUD:", smallLabelStyle);
        GUILayout.FlexibleSpace();
        DrawSetKeyButton(settings, "ToggleHUDKey", ref isSettingToggleHUDKey);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Next profile key
        GUILayout.BeginHorizontal();
        GUILayout.Label("Next Profile:", smallLabelStyle);
        GUILayout.FlexibleSpace();
        DrawSetKeyButton(settings, "NextProfileKey", ref isSettingNextProfileKey);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Previous profile key
        GUILayout.BeginHorizontal();
        GUILayout.Label("Prev. Profile:", smallLabelStyle);
        GUILayout.FlexibleSpace();
        DrawSetKeyButton(settings, "PreviousProfileKey", ref isSettingPreviousProfileKey);
        GUILayout.EndHorizontal();
    }

    private void DrawHUDSettings()
    {
        // Toggle HUD visibility
        settings.Runtime.IsHUDVisible = GUILayout.Toggle(settings.Runtime.IsHUDVisible, "Toggle HUD", GUILayout.ExpandWidth(false));

        // Toggle HUD Text Panel
        settings.Runtime.ShowHUDTextPanel = GUILayout.Toggle(settings.Runtime.ShowHUDTextPanel, "Show HUD Text Panel", GUILayout.ExpandWidth(false));
        
        // Toggle HUD Outer Circle
        settings.Runtime.ShowHUDOuterCircle = GUILayout.Toggle(settings.Runtime.ShowHUDOuterCircle, "Show HUD Outer Perimeter", GUILayout.ExpandWidth(false));

        // Toggle HUD Inner Circle
        settings.Runtime.ShowHUDInnerCircle = GUILayout.Toggle(settings.Runtime.ShowHUDInnerCircle, "Show HUD Deadzone Radius", GUILayout.ExpandWidth(false));

        // HUD Options
        GUILayout.Label("HUD Size: ", smallLabelStyle);
        settings.Global.OuterCircleRadiusRatio.Value = GUILayout.HorizontalSlider(settings.Global.OuterCircleRadiusRatio.Value, 0.1f, 0.4f);

        GUILayout.Label("HUD Opacity:", smallLabelStyle);
        settings.Global.HUDOpacity.Value = GUILayout.HorizontalSlider(settings.Global.HUDOpacity.Value, 0.0f, 1.0f);
        // HUD Color picker
        GUILayout.Label("HUD Color:", smallLabelStyle);

        // RGB sliders
        GUILayout.Label("Red:", detailsLabelStyle);
        float red = GUILayout.HorizontalSlider(settings.Global.HUDColor.Value.r, 0.0f, 1.0f);
        GUILayout.Label("Green:", detailsLabelStyle);
        float green = GUILayout.HorizontalSlider(settings.Global.HUDColor.Value.g, 0.0f, 1.0f);
        GUILayout.Label("Blue:", detailsLabelStyle);
        float blue = GUILayout.HorizontalSlider(settings.Global.HUDColor.Value.b, 0.0f, 1.0f);

        // Update the HUD color
        settings.Global.HUDColor.Value = new Color(red, green, blue);
    }

    private void DrawFlightSettings()
    {
        // Mouse steering toggle
        settings.Runtime.IsMouseSteeringEnabled = GUILayout.Toggle(settings.Runtime.IsMouseSteeringEnabled, "Enable Mouse Steering", GUILayout.ExpandWidth(false));
        
        // Invert Y axis toggle
        settings.ActiveProfile.IsYAxisInvertedCopy = GUILayout.Toggle(settings.ActiveProfile.IsYAxisInvertedCopy, "Invert Y Axis", GUILayout.ExpandWidth(false));
        
        // Auto cam toggle
        settings.ActiveProfile.IsAutoCamEnabledCopy = GUILayout.Toggle(settings.ActiveProfile.IsAutoCamEnabledCopy, "Enable Auto Camera", GUILayout.ExpandWidth(false));
        
        // Flying mode selection
        GUILayout.Label("Flying Mode:");
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
        GUILayout.Label("Yaw amount applied to counteract roll (0-5)", detailsLabelStyle, GUILayout.ExpandWidth(false));
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
            settings.ActiveProfile.DeadzoneCopy = GUILayout.HorizontalSlider(settings.ActiveProfile.DeadzoneCopy, 0.0f, 0.5f);
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
        GUILayout.Label("Lower is smoother (0-1)", detailsLabelStyle, GUILayout.ExpandWidth(false));
        settings.ActiveProfile.SmoothingFactorCopy = GUILayout.HorizontalSlider(settings.ActiveProfile.SmoothingFactorCopy, 0.0f, 1.0f);


        // Profile selection

        GUILayout.Label("Profile:");
        int selectedProfileIndex = settings.ProfileManager.Profiles.ToList().IndexOf(settings.ProfileManager.ActiveProfile);
        string[] profileNames = settings.ProfileManager.Profiles.Select(p => p.Name).ToArray();
        int newSelectedProfileIndex = GUILayout.SelectionGrid(selectedProfileIndex, profileNames, profileNames.Length);
        if (newSelectedProfileIndex != selectedProfileIndex)
        {
            settings.ProfileManager.SetActiveProfile(settings.ProfileManager.Profiles[newSelectedProfileIndex]);
        }
        GUILayout.FlexibleSpace();

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
    }
}