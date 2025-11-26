using ReheeCmf.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReheeCmf.ProfileContainers
{
	public abstract class ProfileContainer<TKey, TProfile> : ProfileContainer
	  where TKey : Enum
	  where TProfile : Profile<TKey>
	{
		public override Type KeyType => typeof(TKey);
		public new TProfile? GetProfile(string key)
		{
			var profile = base.GetProfile(key);
			return profile as TProfile;
		}

		public TProfile? GetProfile(TKey enumKey, string keyOverride)
		{
			var profile = base.GetProfile(enumKey, keyOverride);
			return profile as TProfile;
		}

		public void AddProfile(TProfile profile)
		{
			base.AddProfile(profile);
		}

		public new IEnumerable<TProfile> GetAllProfiles()
		{
			return base.GetAllProfiles().OfType<TProfile>();
		}
	}
}
