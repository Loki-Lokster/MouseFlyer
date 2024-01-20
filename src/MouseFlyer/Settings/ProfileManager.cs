using BepInEx.Configuration;

// This class handles the creation, loading, saving and switching of profiles
public class ProfileManager
{
    private ConfigFile configFile;
    private List<Profile> profiles;
    public IReadOnlyList<Profile> Profiles => profiles.AsReadOnly();
    public Profile ActiveProfile { get; private set; }
    public int MaxProfiles { get; private set; }

    public ProfileManager(ConfigFile configFile, int maxProfiles)
    {
        this.configFile = configFile;
        this.MaxProfiles = maxProfiles;
        profiles = new List<Profile>();
        LoadProfiles();
    }

    private void LoadProfiles()
    {
        // Load profiles from configFile
        for (int i = 1; i <= MaxProfiles; i++)
        {
            profiles.Add(new Profile(configFile, i.ToString()));
        }
    }

    public void CreateProfile(string profileName)
    {
        // Create a new Profile instance with the next available number and add it to the profiles list
        if (profiles.Count < MaxProfiles)
        {
            int newProfileNumber = profiles.Count + 1;
            profiles.Add(new Profile(configFile, newProfileNumber.ToString()));
        }
    }

    public void SaveProfiles()
    {
        // Save all profiles to the configFile
        foreach (Profile profile in profiles)
        {
            profile.ApplyChanges();
            profile.SaveProfile();
        }
    }

    public void SetActiveProfile(Profile profile)
    {
        // Set the active profile
        ActiveProfile = profile;
    }
}