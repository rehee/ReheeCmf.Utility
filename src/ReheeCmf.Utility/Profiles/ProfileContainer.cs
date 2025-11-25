using ReheeCmf.Helpers;
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

    public virtual Profile? GetProfile(string key)
    {
      if (string.IsNullOrEmpty(key))
      {
        return null;
      }

      if (Profiles.TryGetValue(key, out var profile))
      {
        return profile;
      }
      return null;
    }

    public virtual Profile? GetProfile(Enum key, string? keyOverride = null)
    {
      int intValue = Convert.ToInt32(key);
      if (Profiles.TryGetValue(intValue == 0 ? keyOverride ?? "" : key.ToString(), out var profile))
      {
        return profile;
      }
      return null;
    }

    public void AddProfile(Profile profile)
    {
      if (profile == null)
      {
        return;
      }

      var key = profile.EffectiveKey;
      if (string.IsNullOrEmpty(key))
      {
        return;
      }

      if (Profiles == null)
      {
        return;
      }

      Profiles.TryAdd(key, profile);
    }

    public bool RemoveProfile(string key, out Profile? value)
    {
      if (string.IsNullOrEmpty(key))
      {
        value = null;
        return false;
      }

      if (Profiles == null)
      {
        value = null;
        return false;
      }

      return Profiles.TryRemove(key, out value);
    }

    public IEnumerable<Profile> GetAllProfiles()
    {
      return Profiles.Values;
    }
  }
}
