using System.Collections.Generic;

namespace ReheeCmf.Helpers
{
	public static class DictionaryHelper
	{
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
