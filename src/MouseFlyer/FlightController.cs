using KSP.Game;
using MouseFlyer;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Game;
using SpaceWarp.API.Game.Extensions;
using UnityEngine;
public class FlightController
{
    private Settings settings;
    private KSP.Sim.impl.VesselVehicle vessel;
    private UIManager uiManager;

    // Variables
    public static Vector2 lastInput { get; private set; } = Vector2.zero;
    private bool previousMouseSteeringState;

    // Constructor
    public FlightController(Settings settings, KSP.Sim.impl.VesselVehicle vessel, UIManager uiManager)
    {
        this.settings = settings;
        this.vessel = vessel;
        this.uiManager = uiManager;
        
    }
    
    public void HandleKeyPresses()
    {
        // If the Escape key is pressed or IsMouseSteering is disabled, unlock the cursor and disable steering
        if (Input.GetKeyDown(KeyCode.Escape) || !settings.Runtime.IsMouseSteeringEnabled)
        {
            settings.Runtime.IsMouseSteeringEnabled = false;
            uiManager.SetCursorState(null);
        }

        // If the enable steering mode key is pressed
        if (Input.GetKeyDown(settings.Global.ToggleMouseSteeringKey.Value) || settings.Runtime.IsMouseSteeringEnabled != previousMouseSteeringState)
        {
            settings.Runtime.IsMouseSteeringEnabled = !settings.Runtime.IsMouseSteeringEnabled; // Toggle mouse steering

            if (settings.Runtime.IsMouseSteeringEnabled && settings.ActiveProfile.IsAutoCamEnabledCopy)
            {
                // Set the camera to chase mode
                GameManager.Instance.Game.CameraManager.FlightCamera.SelectCameraMode(KSP.Sim.CameraMode.Chase);

            }

            else if (!settings.Runtime.IsMouseSteeringEnabled && settings.ActiveProfile.IsAutoCamEnabledCopy){
                // Set to Auto camera
                GameManager.Instance.Game.CameraManager.FlightCamera.SelectCameraMode(KSP.Sim.CameraMode.Auto);
            }

            previousMouseSteeringState = settings.Runtime.IsMouseSteeringEnabled;
        }

        // If the menu key is pressed, toggle the window
        if (Input.GetKeyDown(settings.Global.ToggleMenuKey.Value))
        {
            settings.Runtime.IsWindowOpen = !settings.Runtime.IsWindowOpen;
        }

        // If the flying mode key is pressed, toggle the flying mode
        if (Input.GetKeyDown(settings.Global.ToggleFlyingModeKey.Value))
        {
            if (settings.Global.CurrentFlyingMode.Value == GlobalSettings.FlyingMode.Normal)
            {
                settings.Global.CurrentFlyingMode.Value = GlobalSettings.FlyingMode.Alternative;
            }
            else
            {
                settings.Global.CurrentFlyingMode.Value = GlobalSettings.FlyingMode.Normal;
            }
        }

        if (Input.GetKeyDown(settings.Global.PreviousProfileKey.Value))
        {
            int currentIndex = settings.ProfileManager.Profiles.ToList().IndexOf(settings.ProfileManager.ActiveProfile);
            int newIndex = (currentIndex - 1 + settings.ProfileManager.Profiles.Count) % settings.ProfileManager.Profiles.Count;
            settings.ProfileManager.SetActiveProfile(settings.ProfileManager.Profiles[newIndex]);
        }

        if (Input.GetKeyDown(settings.Global.NextProfileKey.Value))
        {
            int currentIndex = settings.ProfileManager.Profiles.ToList().IndexOf(settings.ProfileManager.ActiveProfile);
            int newIndex = (currentIndex + 1) % settings.ProfileManager.Profiles.Count;
            settings.ProfileManager.SetActiveProfile(settings.ProfileManager.Profiles[newIndex]);
        }
    }

    Vector2 GetNormalizedMouseInput()
    {
        Vector2 input;

        switch (settings.Global.CurrentFlyingMode.Value)
        {
            case GlobalSettings.FlyingMode.Alternative:
                // Set the cursor state
                uiManager.SetCursorState(settings.Global.CurrentFlyingMode.Value);
                
                // Measure x,y distance from center of screen to mouse position and multiply 
                Vector3 mousePosition = Input.mousePosition;
                float roll = ((mousePosition.x / Screen.width - 0.5f) * 2f) * settings.ActiveProfile.RollSensitivityCopy;
                float pitch = ((mousePosition.y / Screen.height - 0.5f) * 2f * (settings.ActiveProfile.IsYAxisInvertedCopy ? -1 : 1)) * settings.ActiveProfile.PitchSensitivityCopy;

                // Deadzone
                if (Mathf.Abs(roll) < settings.ActiveProfile.DeadzoneCopy) roll = 0;
                if (Mathf.Abs(pitch) < settings.ActiveProfile.DeadzoneCopy) pitch = 0;

                input = new Vector2(roll, pitch);
                break;

            case GlobalSettings.FlyingMode.Normal:
                // Only lock the cursor if it's not already locked
                if (!settings.Runtime.IsCursorLocked)
                {
                    uiManager.SetCursorState(settings.Global.CurrentFlyingMode.Value);
                }

                // Get the mouse movement
                Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                // Adjust pitch and roll based on mouse movement and sensitivity
                float altRoll = mouseMovement.x * settings.ActiveProfile.RollSensitivityCopy;
                float altPitch = mouseMovement.y * settings.ActiveProfile.PitchSensitivityCopy* (settings.ActiveProfile.IsYAxisInvertedCopy ? -1 : 1);
                
                input = new Vector2(altRoll, altPitch);
                break;

            default:
                uiManager.SetCursorState(null);

                input = Vector2.zero;
                break;
        }

        // Smoothing
        input = Vector2.Lerp(lastInput, input, settings.ActiveProfile.SmoothingFactorCopy);

        lastInput = input;

        return input;
    }

    void ApplyMouseInputToVessel(Vector2 mouseInput)
    {
        // Apply the roll and pitch to the vessel
        vessel.SetRoll(mouseInput.x);
        vessel.SetPitch(mouseInput.y);

        // Calculate the yaw as a percentage of the roll
        float yaw = mouseInput.x * settings.ActiveProfile.YawCorrectionCopy;

        // Apply the yaw to the vessel
        vessel.SetYaw(yaw);
    }


    public void UpdateFlightControls()
    {
        if (settings.Runtime.IsMouseSteeringEnabled)
        {
            try
            {
                vessel = Vehicle.ActiveVesselVehicle;

                if (vessel != null)
                {
                    Vector2 mouseInput = GetNormalizedMouseInput();
                    ApplyMouseInputToVessel(mouseInput);
                }
            }
            catch
            {
                // Ignore exceptions
            }
        }
    }
}