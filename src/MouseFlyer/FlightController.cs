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
    private Vector2 virtualMousePosition = new Vector2(Screen.width / 2, Screen.height /2);
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
        // If the Escape key is pressed, unlock the cursor and disable steering
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settings.Runtime.IsMouseSteeringEnabled = false;
            uiManager.SetCursorState(null);
        }

        if (Input.GetKeyDown(settings.Global.ToggleHUDKey.Value)) {
            settings.Runtime.IsHUDVisible = !settings.Runtime.IsHUDVisible; // Toggle HUD
        }

        // If IsMouseSteering is disabled, unlock the cursor
        if (!settings.Runtime.IsMouseSteeringEnabled)
        {
            uiManager.SetCursorState(null);
        }

        // If the enable steering mode key is pressed
        if (Input.GetKeyDown(settings.Global.ToggleMouseSteeringKey.Value)) {
            settings.Runtime.IsMouseSteeringEnabled = !settings.Runtime.IsMouseSteeringEnabled; // Toggle mouse steering
            SwitchCameraMode(); // Switch camera mode

        }

        // If the state of IsMouseSteeringEnabled has changed
        if (settings.Runtime.IsMouseSteeringEnabled != previousMouseSteeringState)
        {
            SwitchCameraMode();
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

    public void SwitchCameraMode()
    {
        var currentCameraMode = GameManager.Instance.Game.CameraManager.FlightCamera.Mode;

        if (settings.Runtime.IsMouseSteeringEnabled && settings.ActiveProfile.IsAutoCamEnabledCopy && currentCameraMode != KSP.Sim.CameraMode.Chase)
        {
            // Set the camera to chase mode
            GameManager.Instance.Game.CameraManager.FlightCamera.SelectCameraMode(KSP.Sim.CameraMode.Chase);
        }
        else if (!settings.Runtime.IsMouseSteeringEnabled && settings.ActiveProfile.IsAutoCamEnabledCopy && currentCameraMode != KSP.Sim.CameraMode.Auto)
        {
            // Set to Auto camera
            GameManager.Instance.Game.CameraManager.FlightCamera.SelectCameraMode(KSP.Sim.CameraMode.Auto);
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
                Vector2 mousePosition = Input.mousePosition;
                Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
                Vector2 direction = mousePosition - center;

                // Constrain the direction to the radius of the outer circle
                float maxDistance = Screen.height * settings.Global.OuterCircleRadiusRatio.Value;
                if (direction.magnitude > maxDistance)
                {
                    direction = direction.normalized * maxDistance;
                }

                // Calculate roll and pitch based on the direction
                float roll = direction.x;
                float pitch = direction.y * (settings.ActiveProfile.IsYAxisInvertedCopy ? -1 : 1);

                // Deadzone
                float deadzone = maxDistance * settings.ActiveProfile.DeadzoneCopy;
                if (Mathf.Abs(roll) < deadzone) roll = 0;
                if (Mathf.Abs(pitch) < deadzone) pitch = 0;

                // Normalize roll and pitch
                roll = (roll / maxDistance) * settings.ActiveProfile.RollSensitivityCopy;
                pitch = (pitch / maxDistance) * settings.ActiveProfile.PitchSensitivityCopy;

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
                float altPitch = mouseMovement.y * settings.ActiveProfile.PitchSensitivityCopy * (settings.ActiveProfile.IsYAxisInvertedCopy ? -1 : 1);

                // Update the virtual mouse position based on the mouse movement
                virtualMousePosition += mouseMovement;

                // Clamp the virtual mouse position to the screen bounds
                virtualMousePosition = new Vector2(Mathf.Clamp(virtualMousePosition.x, 0, Screen.width), Mathf.Clamp(virtualMousePosition.y, 0, Screen.height));

                // If the mouse is not moving, gradually reset the virtual mouse position back to the center
                if (mouseMovement == Vector2.zero)
                {
                    float returnSpeed = settings.ActiveProfile.SmoothingFactorCopy;
                    virtualMousePosition = Vector2.Lerp(virtualMousePosition, new Vector2(Screen.width / 2, Screen.height / 2), returnSpeed);
                }

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

    public Vector2 GetCurrentMousePosition()
    {
        switch (settings.Global.CurrentFlyingMode.Value)
        {
            case GlobalSettings.FlyingMode.Alternative:
                return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

            case GlobalSettings.FlyingMode.Normal:
                return virtualMousePosition;

            default:
                return Vector2.zero;
        }
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