namespace MouseFlyer
{

    // This class encapsulates all the settings for the mod
    public class Settings
    {
        public ProfileManager ProfileManager { get; }
        public GlobalSettings Global { get; }
        public RuntimeState Runtime { get; }

        public Settings(ProfileManager profileManager, GlobalSettings globalSettings, RuntimeState runtimeState)
        {
            ProfileManager = profileManager;
            Global = globalSettings;
            Runtime = runtimeState;
        }

        public Profile ActiveProfile => ProfileManager.ActiveProfile;

    }
}