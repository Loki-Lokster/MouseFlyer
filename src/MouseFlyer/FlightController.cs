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
        if (Input.GetKeyDown(KeyCode.Escape) || !settings.IsMouseSteeringEnabled)
        {
            settings.IsMouseSteeringEnabled = false;
            uiManager.SetCursorState(null);
        }

        // If the enable steering mode key is pressed
        if (Input.GetKeyDown(settings.ToggleMouseSteeringKey) || settings.IsMouseSteeringEnabled != previousMouseSteeringState)
        {
            settings.IsMouseSteeringEnabled = !settings.IsMouseSteeringEnabled; // Toggle mouse steering

            if (settings.IsMouseSteeringEnabled && settings.CurrentProfile.IsAutoCamEnabled.Value)
            {
                // Set the camera to chase mode
                GameManager.Instance.Game.CameraManager.FlightCamera.SelectCameraMode(KSP.Sim.CameraMode.Chase);

            }

            else if (!settings.IsMouseSteeringEnabled && settings.CurrentProfile.IsAutoCamEnabled.Value){
                // Set to Auto camera
                GameManager.Instance.Game.CameraManager.FlightCamera.SelectCameraMode(KSP.Sim.CameraMode.Auto);
            }

            previousMouseSteeringState = settings.IsMouseSteeringEnabled;
        }

        // If the menu key is pressed, toggle the window
        if (Input.GetKeyDown(settings.ToggleMenuKey))
        {
            settings.IsWindowOpen = !settings.IsWindowOpen;
        }

        // If the flying mode key is pressed, toggle the flying mode
        if (Input.GetKeyDown(settings.ToggleFlyingModeKey))
        {
            if (settings.CurrentFlyingMode == Settings.FlyingMode.Normal)
            {
                settings.CurrentFlyingMode = Settings.FlyingMode.Alternative;
            }
            else
            {
                settings.CurrentFlyingMode = Settings.FlyingMode.Normal;
            }
        }

        if (Input.GetKeyDown(settings.PreviousProfileKey))
        {
            int currentIndex = settings.config.Profiles.IndexOf(settings.CurrentProfile);
            int newIndex = (currentIndex - 1 + settings.config.Profiles.Count) % settings.config.Profiles.Count;
            settings.CurrentProfile = settings.config.Profiles[newIndex];
        }

        if (Input.GetKeyDown(settings.NextProfileKey))
        {
            int currentIndex = settings.config.Profiles.IndexOf(settings.CurrentProfile);
            int newIndex = (currentIndex + 1) % settings.config.Profiles.Count;
            settings.CurrentProfile = settings.config.Profiles[newIndex];
        }
    }

    Vector2 GetNormalizedMouseInput()
    {
        Vector2 input;

        switch (settings.CurrentFlyingMode)
        {
            case Settings.FlyingMode.Alternative:
                // Set the cursor state
                uiManager.SetCursorState(settings.CurrentFlyingMode);
                
                // Measure x,y distance from center of screen to mouse position and multiply 
                Vector3 mousePosition = Input.mousePosition;
                float roll = ((mousePosition.x / Screen.width - 0.5f) * 2f) * settings.CurrentProfile.RollSensitivity.Value;
                float pitch = ((mousePosition.y / Screen.height - 0.5f) * 2f * (settings.CurrentProfile.IsYAxisInverted.Value ? -1 : 1)) * settings.CurrentProfile.PitchSensitivity.Value;

                // Deadzone
                if (Mathf.Abs(roll) < settings.CurrentProfile.Deadzone.Value) roll = 0;
                if (Mathf.Abs(pitch) < settings.CurrentProfile.Deadzone.Value) pitch = 0;

                input = new Vector2(roll, pitch);
                break;

            case Settings.FlyingMode.Normal:
                // Only lock the cursor if it's not already locked
                if (!settings.IsCursorLocked)
                {
                    uiManager.SetCursorState(settings.CurrentFlyingMode);
                }

                // Get the mouse movement
                Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                // Adjust pitch and roll based on mouse movement and sensitivity
                float altRoll = mouseMovement.x * settings.CurrentProfile.RollSensitivity.Value;
                float altPitch = mouseMovement.y * settings.CurrentProfile.PitchSensitivity.Value * (settings.CurrentProfile.IsYAxisInverted.Value ? -1 : 1);
                
                input = new Vector2(altRoll, altPitch);
                break;

            default:
                uiManager.SetCursorState(null);

                input = Vector2.zero;
                break;
        }

        // Smoothing
        input = Vector2.Lerp(lastInput, input, settings.CurrentProfile.SmoothingFactor.Value);

        lastInput = input;

        return input;
    }

    void ApplyMouseInputToVessel(Vector2 mouseInput)
    {
        // Apply the roll and pitch to the vessel
        vessel.SetRoll(mouseInput.x);
        vessel.SetPitch(mouseInput.y);

        // Calculate the yaw as a percentage of the roll
        float yaw = mouseInput.x * settings.CurrentProfile.YawCorrection.Value;

        // Apply the yaw to the vessel
        vessel.SetYaw(yaw);
    }


    public void UpdateFlightControls()
    {
        if (settings.IsMouseSteeringEnabled)
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