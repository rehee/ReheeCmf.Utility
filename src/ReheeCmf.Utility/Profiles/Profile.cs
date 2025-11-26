using ReheeCmf.Commons;
using ReheeCmf.Entities;
using System;

namespace ReheeCmf.Profiles
{
	public abstract class Profile : IWithName, IWIthKeyType
	{
		public string? Name { get; set; }
		public string? Description { get; set; }

		public abstract Type KeyType { get; }

		public abstract int KeyValue { get; }

		public abstract string? StringKeyValue { get; }

		public string? StringKeyValueOverride { get; set; }

		public string? EffectiveKey => KeyValue != 0 ? StringKeyValue : StringKeyValueOverride;
	}
}
