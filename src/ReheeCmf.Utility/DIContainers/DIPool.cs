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
              _containers[containerType] = instance;
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
            // Execute actions on the profile type
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

            // Try to find a matching container for this profile
            var matchingContainer = FindMatchingContainer(profileType);
            if (matchingContainer != null)
            {
              try
              {
                var profileInstance = Activator.CreateInstance(profileType) as Profile;
                if (profileInstance != null)
                {
                  matchingContainer.AddProfile(profileInstance);
                }
              }
              catch (Exception)
              {
                // Skip profiles that can't be instantiated or added
                continue;
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
    /// Finds a ProfileContainer that can hold the given Profile type.
    /// Checks if the container's generic type parameter matches the profile type.
    /// </summary>
    private static ProfileContainer? FindMatchingContainer(Type profileType)
    {
      var genericProfileContainerDef = typeof(ProfileContainer<,>);
      
      foreach (var kvp in _containers)
      {
        var containerType = kvp.Key;
        
        // Check if the container is a generic ProfileContainer<TKey, TProfile>
        var baseType = containerType.BaseType;
        while (baseType != null)
        {
          if (baseType.IsGenericType)
          {
            var genericDef = baseType.GetGenericTypeDefinition();
            if (genericDef == genericProfileContainerDef)
            {
              var genericArgs = baseType.GetGenericArguments();
              if (genericArgs.Length >= 2)
              {
                var profileTypeArg = genericArgs[1];
                // Check if profileType is assignable to the container's profile type parameter
                if (profileTypeArg.IsAssignableFrom(profileType))
                {
                  return kvp.Value;
                }
              }
            }
          }
          baseType = baseType.BaseType;
        }
      }

      return null;
    }

    /// <summary>
    /// Gets a Profile by string key from all containers.
    /// </summary>
    /// <param name="key">The string key to search for.</param>
    /// <returns>The Profile if found, otherwise null.</returns>
    public static Profile? GetProfile(string key)
    {
      if (!_initialized || string.IsNullOrEmpty(key))
      {
        return null;
      }

      foreach (var container in _containers.Values)
      {
        var profile = container.GetProfile(key);
        if (profile != null)
        {
          return profile;
        }
      }

      return null;
    }

    /// <summary>
    /// Gets a Profile by enum key from all containers.
    /// </summary>
    /// <param name="key">The enum key to search for.</param>
    /// <param name="keyOverride">Optional string key override when enum value is 0.</param>
    /// <returns>The Profile if found, otherwise null.</returns>
    public static Profile? GetProfile(Enum key, string? keyOverride = null)
    {
      if (!_initialized || key == null)
      {
        return null;
      }

      foreach (var container in _containers.Values)
      {
        var profile = container.GetProfile(key, keyOverride);
        if (profile != null)
        {
          return profile;
        }
      }

      return null;
    }

    /// <summary>
    /// Gets a Profile by generic type from all containers.
    /// </summary>
    /// <typeparam name="T">The Profile type to retrieve.</typeparam>
    /// <returns>The Profile if found, otherwise null.</returns>
    public static T? GetProfile<T>() where T : Profile
    {
      if (!_initialized)
      {
        return null;
      }

      foreach (var container in _containers.Values)
      {
        var profiles = container.GetAllProfiles();
        foreach (var profile in profiles)
        {
          if (profile is T typedProfile)
          {
            return typedProfile;
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Gets all profiles of a specific type from all containers.
    /// </summary>
    /// <typeparam name="T">The Profile type to retrieve.</typeparam>
    /// <returns>Enumerable of all matching profiles.</returns>
    public static IEnumerable<T> GetAllProfiles<T>() where T : Profile
    {
      if (!_initialized)
      {
        return Enumerable.Empty<T>();
      }

      var result = new List<T>();
      foreach (var container in _containers.Values)
      {
        var profiles = container.GetAllProfiles();
        foreach (var profile in profiles)
        {
          if (profile is T typedProfile)
          {
            result.Add(typedProfile);
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Gets a ProfileContainer by its type.
    /// </summary>
    /// <typeparam name="TContainer">The container type to retrieve.</typeparam>
    /// <returns>The container if found, otherwise null.</returns>
    public static TContainer? GetContainer<TContainer>() where TContainer : ProfileContainer
    {
      if (!_initialized)
      {
        return null;
      }

      var containerType = typeof(TContainer);
      if (_containers.TryGetValue(containerType, out var container))
      {
        return container as TContainer;
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
