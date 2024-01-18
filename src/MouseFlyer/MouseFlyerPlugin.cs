using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using KSP.UI.Binding;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.Game;
using SpaceWarp.API.Game.Extensions;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using UnityEngine;
using Castle.Components.DictionaryAdapter.Xml;
using KSP.Game.Load;
using VehiclePhysics;
using KSP.Sim.State;
using BepInEx.Configuration;
using KSP.Rendering.impl;
using KSP.Sim.impl;
using UnityEngine.UI.Extensions;
using KSP.Game;

namespace MouseFlyer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class MouseFlyerPlugin : BaseSpaceWarpPlugin
{
    // Useful in case some other mod wants to use this mod a dependency
    [PublicAPI] public const string ModGuid = MyPluginInfo.PLUGIN_GUID;
    [PublicAPI] public const string ModName = MyPluginInfo.PLUGIN_NAME;
    [PublicAPI] public const string ModVer = MyPluginInfo.PLUGIN_VERSION;

    // Singleton instance of the plugin class
    [PublicAPI] public static MouseFlyerPlugin Instance { get; set; }

    // Settings / Config / UI
    public Settings settings;
    private CustomConfig config;
    public UIManager uiManager;
    public FlightController flightController;

    // UI
    private Rect _windowRect;

    // AppBar button IDs
    private const string ToolbarFlightButtonID = "BTN-MouseFlyerFlight";
    private const string ToolbarOabButtonID = "BTN-MouseFlyerOAB";
    private const string ToolbarKscButtonID = "BTN-MouseFlyerKSC";

  
    // Vessel
    public static KSP.Sim.impl.VesselVehicle Vessel { get; private set; }  

    void Awake() {
        try {
            // Load config
            string configPath = Path.Combine(BepInEx.Paths.ConfigPath, $"{ModName}.cfg");
            config = new CustomConfig(new ConfigFile(configPath, true));

            // Load settings
            settings = new Settings
            {
                CurrentFlyingMode = config.FlyingMode,
                IsMouseSteeringEnabled = false,
                IsAutoCamEnabled = config.IsAutoCamEnabled,
                IsYAxisInverted = config.IsYAxisInverted,
                RollSensitivity = config.RollSensitivity,
                PitchSensitivity = config.PitchSensitivity,
                YawCorrection = config.YawCorrection,
                Deadzone = config.Deadzone,
                SmoothingFactor = config.SmoothingFactor,
                ShowDebugValues = false,
                IsCursorLocked = false,
                IsWindowOpen = false,
                ToggleFlyingModeKey = config.ToggleFlyingModeKey,
                ToggleMouseSteeringKey = config.ToggleMouseSteeringKey,
                ToggleMenuKey = config.ToggleMenuKey
            };

            // Load UI
            uiManager = new UIManager(settings, config);

            // Load the flight controller
            flightController = new FlightController(settings, Vessel, uiManager);

            Logger.LogInfo("MouseFlyer loaded successfully.");
        }

        catch (Exception e)
        {
            Logger.LogError($"Error loading MouseFlyer: {e}");
        }
                
    }

    void Update()
    {
        flightController.HandleKeyPresses();

    }

    void FixedUpdate()
    {
        flightController.UpdateFlightControls();
    }


    /// <summary>
    /// Runs when the mod is first initialized.
    /// </summary>
    public override void OnInitialized()
    {
        base.OnInitialized();

        Instance = this;

        // Register Flight AppBar button
        Appbar.RegisterAppButton(
            ModName,
            ToolbarFlightButtonID,
            AssetManager.GetAsset<Texture2D>($"{ModGuid}/images/icon.png"),
            isOpen =>
            {
                settings.IsWindowOpen = isOpen;
                GameObject.Find(ToolbarFlightButtonID)?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(isOpen);
            }
        );

        // Register all Harmony patches in the project
        Harmony.CreateAndPatchAll(typeof(MouseFlyerPlugin).Assembly);
    }

    /// <summary>
    /// Draws a simple UI window when <code>this._isWindowOpen</code> is set to <code>true</code>.
    /// </summary>
    private void OnGUI()
    {
        // Set the UI
        GUI.skin = Skins.ConsoleSkin;

        if (settings.IsWindowOpen)
        {
            _windowRect = GUILayout.Window(
                GUIUtility.GetControlID(FocusType.Passive),
                _windowRect,
                uiManager.FillWindow,
                "MouseFlyer",
                GUILayout.Height(380),
                GUILayout.Width(350)
            );
        }
    }
    
}

