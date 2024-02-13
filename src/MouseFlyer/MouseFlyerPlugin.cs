using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using KSP.UI.Binding;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using UnityEngine;
using BepInEx.Configuration;
using KSP.Game;
using UnityEngine.UI;
using System.Numerics;


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

    // Property to get the current game state
    public GameState CurrentGameState => (GameState)(GameManager.Instance?.Game?.GlobalGameState?.GetGameState().GameState);

    // Settings / Config / UI
    private ConfigFile configFile;
    public UIManager uiManager;
    public FlightController flightController;
    public ProfileManager profileManager;
    public Settings settings;

    // UI
    private Rect _windowRect;

    // AppBar button IDs
    private const string ToolbarFlightButtonID = "BTN-MouseFlyerFlight";
    private const string ToolbarOabButtonID = "BTN-MouseFlyerOAB";
    private const string ToolbarKscButtonID = "BTN-MouseFlyerKSC";

  
    // Vessel
    public static KSP.Sim.impl.VesselVehicle Vessel { get; private set; }  

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    void Awake() {
        try {
            
            // Load config
            string configPath = Path.Combine(BepInEx.Paths.ConfigPath, $"{ModName}.cfg");
            configFile = new ConfigFile(configPath, true);

            // Create ProfileManager
            profileManager = new ProfileManager(configFile, maxProfiles: 5); // Maximum 5 profiles

            // Set the active profile to the first profile
            profileManager.SetActiveProfile(profileManager.Profiles[0]);

            // Create GlobalSettings
            GlobalSettings globalSettings = new GlobalSettings(configFile);

            // Create RuntimeState
            RuntimeState runtimeState = new RuntimeState();

            // Create Settings
            settings = new Settings(profileManager, globalSettings, runtimeState);

            // Load UI
            uiManager = new UIManager(settings);

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
        // If the GameState is in flight mode
        if(CurrentGameState == GameState.FlightView)
        {
            flightController.HandleKeyPresses();
        }
    }

    void FixedUpdate()
    {
        // If the GameState is in flight mode
        if(CurrentGameState == GameState.FlightView)
        {
            flightController.UpdateFlightControls();
        }
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
                settings.Runtime.IsWindowOpen = isOpen;
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

        if (CurrentGameState == GameState.FlightView || CurrentGameState == GameState.Map3DView)
        {
            if (settings.Runtime.IsWindowOpen)
            {
            _windowRect = GUILayout.Window(
                GUIUtility.GetControlID(FocusType.Passive),
                _windowRect,
                uiManager.FillWindow,
                "MouseFlyer",
                GUILayout.Height(380),
                GUILayout.Width(380)
            );
            }

            if (settings.Runtime.IsMouseSteeringEnabled)
            {
                // Draw the HUD
                UnityEngine.Vector2 mousePosition = flightController.GetCurrentMousePosition();
                uiManager.DrawHUD(mousePosition);
            }
        }
    }
}
