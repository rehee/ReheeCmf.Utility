using System;
using System.Collections.Generic;
using System.Linq;

namespace ReheeCmf.Profiles
{
  public abstract class ProfileContainer<T> : ProfileContainer where T : Profile
  {
    public new T? GetProfile(string key)
    {
      var profile = base.GetProfile(key);
      return profile as T;
    }

    public void AddProfile(string key, T profile)
    {
      base.AddProfile(key, profile);
    }

    public new IEnumerable<T> GetAllProfiles()
    {
      return base.GetAllProfiles().OfType<T>();
    }
  }
}
