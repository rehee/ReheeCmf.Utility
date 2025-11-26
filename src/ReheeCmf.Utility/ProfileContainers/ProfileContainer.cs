using ReheeCmf.Commons;
using ReheeCmf.Helpers;
using ReheeCmf.Profiles;
using System;
using System.Collections.Generic;

namespace ReheeCmf.ProfileContainers
{
	public abstract class ProfileContainer : IWIthKeyType
	{
		public virtual Type KeyType => this.GetType().BaseType;
		protected Dictionary<string, Profile> Profiles { get; set; }

		protected ProfileContainer()
		{
			Profiles = new Dictionary<string, Profile>();
		}

		public virtual Profile? GetProfile(Enum key, string? keyOverride = null)
		{
			int intValue = Convert.ToInt32(key);
			return GetProfile(intValue == 0 ? keyOverride ?? "" : key.ToString());
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
