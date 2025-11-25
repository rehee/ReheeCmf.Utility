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

    public void AddProfile(string key, Profile profile)
    {
      Profiles[key] = profile;
    }

    public bool RemoveProfile(string key)
    {
      return Profiles.Remove(key);
    }

    public IEnumerable<Profile> GetAllProfiles()
    {
      return Profiles.Values;
    }
  }
}
