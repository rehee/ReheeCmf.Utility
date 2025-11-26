using System;
using System.Linq;

namespace ReheeCmf.Helpers
{
	public static class TypeHelper
	{
		/// <summary>
		/// Checks if the type is decorated with the specified attribute (generic version).
		/// </summary>
		/// <typeparam name="TAttribute">The attribute type to check for.</typeparam>
		/// <param name="type">The type to check.</param>
		/// <returns>True if the type has the attribute, false otherwise.</returns>
		public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
		{
			if (type == null)
			{
				return false;
			}

			return type.GetCustomAttributes(typeof(TAttribute), true).Any();
		}

		/// <summary>
		/// Checks if the type is decorated with the specified attribute (type parameter version).
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <param name="attributeType">The attribute type to check for.</param>
		/// <returns>True if the type has the attribute, false otherwise.</returns>
		public static bool HasAttribute(this Type type, Type attributeType)
		{
			if (type == null || attributeType == null)
			{
				return false;
			}

			if (!typeof(Attribute).IsAssignableFrom(attributeType))
			{
				return false;
			}

			return type.GetCustomAttributes(attributeType, true).Any();
		}

		/// <summary>
		/// Checks if the type implements the specified interface.
		/// Checks base classes layer by layer until no base class is found.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <param name="interfaceType">The interface type to check for.</param>
		/// <returns>True if the type implements the interface, false otherwise.</returns>
		public static bool ImplementsInterface(this Type type, Type interfaceType)
		{
			if (type == null || interfaceType == null)
			{
				return false;
			}

			if (!interfaceType.IsInterface)
			{
				return false;
			}

			var currentType = type;
			while (currentType != null)
			{
				if (currentType.GetInterfaces().Any(i => IsMatchingInterface(i, interfaceType)))
				{
					return true;
				}
				currentType = currentType.BaseType;
			}

			return false;
		}

		/// <summary>
		/// Helper method to check if an interface matches the target interface type.
		/// Handles both regular and generic interface matching.
		/// </summary>
		private static bool IsMatchingInterface(Type interfaceToCheck, Type targetInterface)
		{
			if (interfaceToCheck == targetInterface)
			{
				return true;
			}

			if (interfaceToCheck.IsGenericType && targetInterface.IsGenericType)
			{
				return interfaceToCheck.GetGenericTypeDefinition() == targetInterface.GetGenericTypeDefinition();
			}

			return false;
		}

		/// <summary>
		/// Checks if the type inherits from the specified class or implements the specified interface.
		/// Checks base classes layer by layer until no base class is found.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <param name="baseType">The base class or interface type to check for.</param>
		/// <returns>True if the type inherits from the class or implements the interface, false otherwise.</returns>
		public static bool InheritsFrom(this Type type, Type baseType)
		{
			if (type == null || baseType == null)
			{
				return false;
			}

			if (baseType.IsInterface)
			{
				return type.ImplementsInterface(baseType);
			}

			var currentType = type;
			while (currentType != null)
			{
				if (IsMatchingBaseType(currentType.BaseType, baseType))
				{
					return true;
				}
				currentType = currentType.BaseType;
			}

			return false;
		}

		/// <summary>
		/// Checks if a type inherits from the specified generic class or implements the specified interface.
		/// Checks base classes layer by layer until no base class is found.
		/// </summary>
		/// <typeparam name="T">The base class or interface type to check for.</typeparam>
		/// <param name="checkType">The type to check.</param>
		/// <returns>True if the type inherits from the class or implements the interface, false otherwise.</returns>
		public static bool InheritsFrom<T>(this Type checkType)
		{
			return checkType.InheritsFrom(typeof(T));
		}

		/// <summary>
		/// Helper method to check if a base type matches the target base type.
		/// Handles both regular and generic type matching.
		/// </summary>
		private static bool IsMatchingBaseType(Type? baseTypeToCheck, Type targetBaseType)
		{
			if (baseTypeToCheck == null)
			{
				return false;
			}

			if (baseTypeToCheck == targetBaseType)
			{
				return true;
			}

			if (baseTypeToCheck.IsGenericType && targetBaseType.IsGenericType)
			{
				return baseTypeToCheck.GetGenericTypeDefinition() == targetBaseType.GetGenericTypeDefinition();
			}

			return false;
		}

		/// <summary>
		/// Checks if the generic type implements the specified interface.
		/// Checks base classes layer by layer until no base class is found.
		/// </summary>
		/// <typeparam name="T">The type to check.</typeparam>
		/// <param name="interfaceType">The interface type to check for.</param>
		/// <returns>True if the type implements the interface, false otherwise.</returns>
		public static bool ImplementsInterface<T>(Type interfaceType)
		{
			return typeof(T).ImplementsInterface(interfaceType);
		}


	}
}
