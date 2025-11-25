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
        if (currentType.GetInterfaces().Any(i => i == interfaceType || (i.IsGenericType && interfaceType.IsGenericType && i.GetGenericTypeDefinition() == interfaceType.GetGenericTypeDefinition())))
        {
          return true;
        }
        currentType = currentType.BaseType;
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
        if (currentType.BaseType == baseType || (currentType.BaseType != null && currentType.BaseType.IsGenericType && baseType.IsGenericType && currentType.BaseType.GetGenericTypeDefinition() == baseType.GetGenericTypeDefinition()))
        {
          return true;
        }
        currentType = currentType.BaseType;
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

    /// <summary>
    /// Checks if the generic type inherits from the specified class or implements the specified interface.
    /// Checks base classes layer by layer until no base class is found.
    /// </summary>
    /// <typeparam name="T">The type to check.</typeparam>
    /// <param name="baseType">The base class or interface type to check for.</param>
    /// <returns>True if the type inherits from the class or implements the interface, false otherwise.</returns>
    public static bool InheritsFrom<T>(Type baseType)
    {
      return typeof(T).InheritsFrom(baseType);
    }
  }
}
