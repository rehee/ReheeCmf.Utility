using System;

namespace ReheeCmf.Commons
{
	/// <summary>
	/// Delegate for initializing profiles during DIPool initialization.
	/// Used to execute custom actions on each discovered Profile type.
	/// </summary>
	/// <param name="profileType">The type of the profile being initialized.</param>
	public delegate void PoolInitialize(Type profileType);
}
