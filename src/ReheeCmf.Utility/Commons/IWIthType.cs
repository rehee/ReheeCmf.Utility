using System;

namespace ReheeCmf.Commons
{
	/// <summary>
	/// Interface for types that have a key type property.
	/// Used to identify the type of key used by profiles and other keyed entities.
	/// </summary>
	public interface IWIthKeyType
	{
		/// <summary>
		/// Gets the type of the key used by this entity.
		/// </summary>
		Type KeyType { get; }
	}
}
