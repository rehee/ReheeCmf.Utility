using System.Collections.Generic;

namespace ReheeCmf.Helpers
{
  public static class DictionaryHelper
  {
    /// <summary>
    /// Adds a new key-value pair or updates an existing one.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to modify.</param>
    /// <param name="key">The key to add or update.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the operation succeeded, false if dictionary or key is null.</returns>
    public static bool TryAddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
      if (dictionary == null)
      {
        return false;
      }
      if (key == null)
      {
        return false;
      }
      if (dictionary.ContainsKey(key))
      {
        dictionary[key] = value;
        return true;
      }
      else
      {
        dictionary.Add(key, value);
        return true;
      }
    }
    /// <summary>
    /// Attempts to add a key-value pair only if the key doesn't already exist.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to modify.</param>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>True if the key-value pair was added, false if dictionary is null, key is null, or key already exists.</returns>
    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
      if (dictionary == null)
      {
        return false;
      }
      if (key == null)
      {
        return false;
      }
      if (dictionary.ContainsKey(key))
      {
        return false;
      }
      else
      {
        dictionary.Add(key, value);
        return true;
      }
    }
    /// <summary>
    /// Attempts to remove a key-value pair and returns the removed value.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to modify.</param>
    /// <param name="key">The key to remove.</param>
    /// <param name="value">Output parameter receiving the removed value, or default if removal failed.</param>
    /// <returns>True if the key was found and removed, false if dictionary is null, key is null, or key doesn't exist.</returns>
    public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue? value)
    {
      if (dictionary == null)
      {
        value = default;
        return false;
      }
      if (key == null)
      {
        value = default;
        return false;
      }
      if (dictionary.ContainsKey(key))
      {
        dictionary.Remove(key, out value);
        return true;
      }
      else
      {
        value = default;
        return false;
      }
    }
  }
}
