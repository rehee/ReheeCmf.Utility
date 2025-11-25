using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReheeCmf.Helpers;
using ReheeCmf.Profiles;
using ReheeCmf.ProfileContainers;

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
    public static void Initialize(params Action<Type>[] actions)
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
              if (type.InheritsFrom(typeof(ProfileContainer)) && !type.IsAbstract && type.IsClass)
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
            if (instance != null)
            {
              // Get the KeyType from the container
              Type? keyType = GetContainerKeyType(containerType);
              if (keyType != null)
              {
                _containers.TryAdd(keyType, instance);
              }
            }
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
              if (type.InheritsFrom(typeof(Profile)) && !type.IsAbstract && type.IsClass)
              {
                profileTypes.Add(type);
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
            // Instantiate the profile
            var profileInstance = Activator.CreateInstance(profileType) as Profile;
            if (profileInstance != null)
            {
              // Get the container by profile's KeyType
              var keyType = profileInstance.KeyType;
              if (_containers.TryGetValue(keyType, out var container))
              {
                // Add profile to the corresponding container
                container.AddProfile(profileInstance);
              }
            }

            // Execute actions on the profile type (in second loop)
            if (actions != null)
            {
              foreach (var action in actions)
              {
                try
                {
                  action?.Invoke(profileType);
                }
                catch (Exception)
                {
                  // Continue even if an action fails
                  continue;
                }
              }
            }
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
    private static Type? GetContainerKeyType(Type containerType)
    {
      var genericProfileContainerDef = typeof(ProfileContainer<,>);
      
      var baseType = containerType.BaseType;
      while (baseType != null)
      {
        if (baseType.IsGenericType)
        {
          var genericDef = baseType.GetGenericTypeDefinition();
          if (genericDef == genericProfileContainerDef)
          {
            var genericArgs = baseType.GetGenericArguments();
            if (genericArgs.Length >= 1)
            {
              return genericArgs[0]; // TKey is the first generic argument
            }
          }
        }
        baseType = baseType.BaseType;
      }

      return null;
    }

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

      // Get the KeyType from T
      var keyType = typeof(T).BaseType;
      if (keyType != null && keyType.IsGenericType && keyType.GetGenericTypeDefinition() == typeof(Profile<>))
      {
        var genericArgs = keyType.GetGenericArguments();
        if (genericArgs.Length > 0)
        {
          var enumKeyType = genericArgs[0];
          if (_containers.TryGetValue(enumKeyType, out var container))
          {
            return container.GetAllProfiles().OfType<T>();
          }
        }
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
