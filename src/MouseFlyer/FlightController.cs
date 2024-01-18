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
    private MouseFlyerPlugin plugin;

    // Variables
    public static Vector2 lastInput { get; private set; } = Vector2.zero;

    // Constructor
    public FlightController(Settings settings, KSP.Sim.impl.VesselVehicle vessel)
    {
        this.settings = settings;
        this.vessel = vessel;
    }
    
    public void HandleKeyPresses()
    {
        // If the Escape key is pressed or IsMouseSteering is disabled, unlock the cursor
        if (Input.GetKeyDown(KeyCode.Escape) || !settings.IsMouseSteeringEnabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            settings.IsCursorLocked = false;
            settings.IsMouseSteeringEnabled = false;
        }

        // If the enable steering mode key is pressed
        if (Input.GetKeyDown(settings.ToggleMouseSteeringKey))
        {
            settings.IsMouseSteeringEnabled = !settings.IsMouseSteeringEnabled; // Toggle mouse steering

            if (settings.IsMouseSteeringEnabled)
            {
                // Set the camera to chase mode, havent figured out how to do this yet
                GameManager.Instance.Game.CameraManager.FlightCamera.SelectCameraMode(KSP.Sim.CameraMode.Chase);

            }

            else {
                // Set to Auto camera
                GameManager.Instance.Game.CameraManager.FlightCamera.SelectCameraMode(KSP.Sim.CameraMode.Auto);
            }
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
    }

    Vector2 GetNormalizedMouseInput()
    {
        Vector2 input;

        switch (settings.CurrentFlyingMode)
        {
            case Settings.FlyingMode.Alternative:
                // Unlock the cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // Change the cursor to a crosshair
                Texture2D cursorTexture = AssetManager.GetAsset<Texture2D>($"{MouseFlyerPlugin.ModGuid}/images/icon.png");
                Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                
                settings.IsCursorLocked = false;

                Vector3 mousePosition = Input.mousePosition;
                float roll = ((mousePosition.x / Screen.width - 0.5f) * 2f) * settings.RollSensitivity;
                float pitch = ((mousePosition.y / Screen.height - 0.5f) * 2f * (settings.IsYAxisInverted ? -1 : 1)) * settings.PitchSensitivity;

                // Deadzone
                if (Mathf.Abs(roll) < settings.Deadzone) roll = 0;
                if (Mathf.Abs(pitch) < settings.Deadzone) pitch = 0;

                input = new Vector2(roll, pitch);
                break;

            case Settings.FlyingMode.Normal:
                // Only lock the cursor if it's not already locked
                if (!settings.IsCursorLocked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    settings.IsCursorLocked = true;
                }

                // Get the mouse movement
                Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                // Adjust pitch and roll based on mouse movement and sensitivity
                float altRoll = mouseMovement.x * settings.RollSensitivity;
                float altPitch = mouseMovement.y * settings.PitchSensitivity * (settings.IsYAxisInverted ? -1 : 1);
                
                input = new Vector2(altRoll, altPitch);
                break;

            default:
                // Unlock the cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                settings.IsCursorLocked = false;

                input = Vector2.zero;
                break;
        }

        // Smoothing
        input = Vector2.Lerp(lastInput, input, settings.SmoothingFactor);

        lastInput = input;

        return input;
    }

    void ApplyMouseInputToVessel(Vector2 mouseInput)
    {
        vessel.SetRoll(mouseInput.x);
        vessel.SetPitch(mouseInput.y);

        // Calculate the yaw as a percentage of the roll
        float yaw = mouseInput.x * settings.YawCorrection;

        // Apply the yaw to the vessel
        vessel.SetYaw(yaw);
    }


    public void UpdateFlightControls()
    {
        if (settings.IsMouseSteeringEnabled)
        {
            try
            {
                if (vessel == null)
                {
                    vessel = Vehicle.ActiveVesselVehicle;
                }

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