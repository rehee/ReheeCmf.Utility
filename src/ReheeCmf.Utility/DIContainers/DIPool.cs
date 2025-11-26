using ReheeCmf.Commons;
using ReheeCmf.Helpers;
using ReheeCmf.ProfileContainers;
using ReheeCmf.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReheeCmf.DIContainers
{
	/// <summary>
	/// A simple "Poor Man's DI" dependency injection container pool.
	/// This static class manages ProfileContainer and Profile instances.
	/// </summary>
	public static class DIPool
	{
		private static readonly object _lock = new object();
		private static bool _initialized = false;
		private static readonly Dictionary<Type, ProfileContainer> _containers = new Dictionary<Type, ProfileContainer>();

		/// <summary>
		/// Initializes the DI container pool. This method can only be called once.
		/// Scans all assemblies for ProfileContainer and Profile implementations.
		/// </summary>
		/// <param name="actions">Optional actions to execute for each discovered Profile type.</param>
		public static void Initialize(params PoolInitialize[] actions)
		{
			if (_initialized)
			{
				return;
			}
			lock (_lock)
			{
				if (_initialized)
				{
					return;
				}

				// Step 1: Scan all assemblies and find ProfileContainer implementations
				var assemblies = AppDomain.CurrentDomain.GetAssemblies();
				var containerTypes = new List<Type>();
				foreach (var assembly in assemblies)
				{
					try
					{
						var types = assembly.GetTypes();
						foreach (var type in types)
						{
							// Check if type inherits from ProfileContainer and is not abstract
							if (type.InheritsFrom<ProfileContainer>() && !type.IsAbstract && type.IsClass)
							{
								containerTypes.Add(type);
							}
						}
					}
					catch (ReflectionTypeLoadException)
					{
						// Skip assemblies that can't be loaded
						continue;
					}
					catch (Exception)
					{
						// Skip assemblies with other issues
						continue;
					}
				}

				// Step 2: Create instances of ProfileContainer types
				foreach (var containerType in containerTypes)
				{
					try
					{
						var instance = Activator.CreateInstance(containerType) as ProfileContainer;
						if (instance == null)
						{
							continue;
						}
						_containers.TryAdd(instance.KeyType, instance);
					}
					catch (Exception)
					{
						// Skip containers that can't be instantiated
						continue;
					}
				}

				// Step 3: Scan all assemblies again and find Profile implementations
				var profileTypes = new List<Type>();

				foreach (var assembly in assemblies)
				{
					try
					{
						var types = assembly.GetTypes();
						foreach (var type in types)
						{
							// Check if type inherits from Profile and is not abstract
							if (type.InheritsFrom<Profile>() && !type.IsAbstract && type.IsClass)
							{
								profileTypes.Add(type);
							}
							// Execute actions on the profile type (in second loop)
							if (actions != null)
							{
								foreach (var action in actions)
								{
									try
									{
										action?.Invoke(type);
									}
									catch (Exception)
									{
										// Continue even if an action fails
										continue;
									}
								}
							}
						}
					}
					catch (ReflectionTypeLoadException)
					{
						// Skip assemblies that can't be loaded
						continue;
					}
					catch (Exception)
					{
						// Skip assemblies with other issues
						continue;
					}
				}

				// Step 4: Create Profile instances and add them to corresponding containers
				foreach (var profileType in profileTypes)
				{
					try
					{
						var profileInstance = Activator.CreateInstance(profileType) as Profile;
						if (profileInstance == null)
						{
							continue;
						}
						// Get the KeyType from the profile type to check if a container exists
						if (!_containers.TryGetValue(profileInstance.KeyType, out var container) || container == null)
						{
							continue;
						}
						container.AddProfile(profileInstance);
					}
					catch (Exception)
					{
						// Continue with next profile type
						continue;
					}
				}

				_initialized = true;
			}
		}

		/// <summary>
		/// Gets the KeyType from a ProfileContainer type by examining its generic arguments.
		/// </summary>


		/// <summary>
		/// Gets the KeyType from a Profile type by examining its inheritance chain.
		/// </summary>
		private static Type? GetProfileKeyType(Type profileType, Type? originalType = null)
		{
			var genericProfileDef = typeof(Profile<>);
			if (_profileKeyTypeCache.TryGetValue(profileType, out var cachedType))
			{
				return cachedType;
			}
			if (originalType == null)
			{
				originalType = profileType;
			}
			if (profileType.IsGenericType)
			{
				var genericDef = profileType.GetGenericTypeDefinition();
				if (genericDef == genericProfileDef)
				{
					var genericArgs = profileType.GetGenericArguments();
					if (genericArgs.Length >= 1)
					{
						_profileKeyTypeCache.TryAdd(profileType, genericArgs[0]);
						return genericArgs[0]; // TKey is the generic argument
					}
				}
			}
			var baseType = profileType.BaseType;
			if (baseType != null)
			{
				return GetProfileKeyType(baseType, originalType);
			}
			return null;
		}
		private static Dictionary<Type, Type> _profileKeyTypeCache = new Dictionary<Type, Type>();

		/// <summary>
		/// Gets all Profiles by string key from all containers.
		/// </summary>
		/// <param name="key">The string key to search for.</param>
		/// <returns>Enumerable of all matching profiles.</returns>
		public static IEnumerable<Profile> GetProfile(string key)
		{
			if (!_initialized || string.IsNullOrEmpty(key))
			{
				return Enumerable.Empty<Profile>();
			}

			return _containers.Values
			  .Select(b => b.GetProfile(key))
			  .Where(b => b != null)
			  .Cast<Profile>();
		}

		/// <summary>
		/// Gets all Profiles by enum key from all containers.
		/// </summary>
		/// <param name="key">The enum key to search for.</param>
		/// <param name="keyOverride">Optional string key override when enum value is 0.</param>
		/// <returns>Enumerable of all matching profiles.</returns>
		public static IEnumerable<Profile> GetProfile(Enum key, string? keyOverride = null)
		{
			if (!_initialized || key == null)
			{
				return Enumerable.Empty<Profile>();
			}

			return _containers.Values
			  .Select(b => b.GetProfile(key, keyOverride))
			  .Where(b => b != null)
			  .Cast<Profile>();
		}

		/// <summary>
		/// Gets all profiles of a specific type from the container associated with T's KeyType.
		/// </summary>
		/// <typeparam name="T">The Profile type to retrieve.</typeparam>
		/// <returns>Enumerable of all matching profiles.</returns>
		public static IEnumerable<T> GetAllProfiles<T>() where T : Profile
		{
			if (!_initialized)
			{
				return Enumerable.Empty<T>();
			}

			// Get the KeyType from T using helper method that traverses inheritance chain
			var enumKeyType = GetProfileKeyType(typeof(T));
			if (enumKeyType != null && _containers.TryGetValue(enumKeyType, out var container))
			{
				return container.GetAllProfiles().OfType<T>();
			}

			return Enumerable.Empty<T>();
		}

		/// <summary>
		/// Gets all profiles from the container associated with the specified enum type.
		/// </summary>
		/// <typeparam name="TKey">The enum type that serves as the key type.</typeparam>
		/// <returns>Enumerable of all profiles in the container.</returns>
		public static IEnumerable<Profile> GetAllProfilesByKeyType<TKey>() where TKey : Enum
		{
			if (!_initialized)
			{
				return Enumerable.Empty<Profile>();
			}

			var keyType = typeof(TKey);
			if (_containers.TryGetValue(keyType, out var container))
			{
				return container.GetAllProfiles();
			}

			return Enumerable.Empty<Profile>();
		}

		public static T? GetAllProfilesByKeyType<T>(this Enum key, string? keyOverride = null) where T : Profile
		{
			if (!_initialized)
			{
				return default;
			}
			var keyType = GetProfileKeyType(typeof(T));
			if (keyType == null)
			{
				return default;
			}
			if (_containers.TryGetValue(keyType, out var container))
			{
				return container.GetProfile(key, keyOverride) as T;
			}
			return null;
		}

		/// <summary>
		/// Resets the DIPool (for testing purposes).
		/// </summary>
		public static void Reset()
		{
			lock (_lock)
			{
				_initialized = false;
				_containers.Clear();
			}
		}

		/// <summary>
		/// Gets whether the pool has been initialized.
		/// </summary>
		public static bool IsInitialized => _initialized;
	}
}
