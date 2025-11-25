using System;
using System.Collections.Generic;

namespace ReheeCmf.Profiles
{
  public abstract class ProfileContainer
  {
    protected Dictionary<string, Profile> Profiles { get; set; }

    protected ProfileContainer()
    {
      Profiles = new Dictionary<string, Profile>();
    }

    public Profile? GetProfile(string key)
    {
      if (Profiles.TryGetValue(key, out var profile))
      {
        return profile;
      }
      return null;
    }

    public void AddProfile(Profile profile)
    {
      if (profile == null)
      {
        throw new ArgumentNullException(nameof(profile));
      }

      var key = profile.GetEffectiveKey();
      if (string.IsNullOrEmpty(key))
      {
        throw new ArgumentException("Profile's effective key cannot be null or empty", nameof(profile));
      }

      Profiles[key] = profile;
    }

    public bool RemoveProfile(string key)
    {
      if (string.IsNullOrEmpty(key))
      {
        return false;
      }

      return Profiles.Remove(key);
    }

    public IEnumerable<Profile> GetAllProfiles()
    {
      return Profiles.Values;
    }
  }
}
