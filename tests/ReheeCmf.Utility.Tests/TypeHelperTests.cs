using System;
using Xunit;
using ReheeCmf.Helpers;
using ReheeCmf.Attributes;

namespace ReheeCmf.Utility.Tests
{
  public class TypeHelperTests
  {
    [Pooled]
    private class ClassWithAttribute
    {
    }

    private class ClassWithoutAttribute
    {
    }

    private interface ITestInterface
    {
    }

    private class ClassImplementingInterface : ITestInterface
    {
    }

    private class DerivedClass : ClassImplementingInterface
    {
    }

    private class BaseClass
    {
    }

    private class InheritedClass : BaseClass
    {
    }

    private class DoublyInheritedClass : InheritedClass
    {
    }

    [Fact]
    public void HasAttribute_Generic_WithAttribute_ReturnsTrue()
    {
      // Arrange
      var type = typeof(ClassWithAttribute);

      // Act
      var result = type.HasAttribute<PooledAttribute>();

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void HasAttribute_Generic_WithoutAttribute_ReturnsFalse()
    {
      // Arrange
      var type = typeof(ClassWithoutAttribute);

      // Act
      var result = type.HasAttribute<PooledAttribute>();

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void HasAttribute_Generic_WithNullType_ReturnsFalse()
    {
      // Arrange
      Type? type = null;

      // Act
      var result = type.HasAttribute<PooledAttribute>();

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void HasAttribute_TypeParameter_WithAttribute_ReturnsTrue()
    {
      // Arrange
      var type = typeof(ClassWithAttribute);
      var attributeType = typeof(PooledAttribute);

      // Act
      var result = type.HasAttribute(attributeType);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void HasAttribute_TypeParameter_WithoutAttribute_ReturnsFalse()
    {
      // Arrange
      var type = typeof(ClassWithoutAttribute);
      var attributeType = typeof(PooledAttribute);

      // Act
      var result = type.HasAttribute(attributeType);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void HasAttribute_TypeParameter_WithNullType_ReturnsFalse()
    {
      // Arrange
      Type? type = null;
      var attributeType = typeof(PooledAttribute);

      // Act
      var result = type.HasAttribute(attributeType);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void HasAttribute_TypeParameter_WithNullAttributeType_ReturnsFalse()
    {
      // Arrange
      var type = typeof(ClassWithAttribute);
      Type? attributeType = null;

      // Act
      var result = type.HasAttribute(attributeType);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void HasAttribute_TypeParameter_WithNonAttributeType_ReturnsFalse()
    {
      // Arrange
      var type = typeof(ClassWithAttribute);
      var attributeType = typeof(string);

      // Act
      var result = type.HasAttribute(attributeType);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void ImplementsInterface_DirectImplementation_ReturnsTrue()
    {
      // Arrange
      var type = typeof(ClassImplementingInterface);
      var interfaceType = typeof(ITestInterface);

      // Act
      var result = type.ImplementsInterface(interfaceType);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void ImplementsInterface_InheritedImplementation_ReturnsTrue()
    {
      // Arrange
      var type = typeof(DerivedClass);
      var interfaceType = typeof(ITestInterface);

      // Act
      var result = type.ImplementsInterface(interfaceType);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void ImplementsInterface_NoImplementation_ReturnsFalse()
    {
      // Arrange
      var type = typeof(ClassWithoutAttribute);
      var interfaceType = typeof(ITestInterface);

      // Act
      var result = type.ImplementsInterface(interfaceType);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void ImplementsInterface_WithNullType_ReturnsFalse()
    {
      // Arrange
      Type? type = null;
      var interfaceType = typeof(ITestInterface);

      // Act
      var result = type.ImplementsInterface(interfaceType);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void ImplementsInterface_WithNullInterfaceType_ReturnsFalse()
    {
      // Arrange
      var type = typeof(ClassImplementingInterface);
      Type? interfaceType = null;

      // Act
      var result = type.ImplementsInterface(interfaceType);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void ImplementsInterface_WithNonInterfaceType_ReturnsFalse()
    {
      // Arrange
      var type = typeof(ClassImplementingInterface);
      var interfaceType = typeof(string);

      // Act
      var result = type.ImplementsInterface(interfaceType);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void InheritsFrom_DirectInheritance_ReturnsTrue()
    {
      // Arrange
      var type = typeof(InheritedClass);
      var baseType = typeof(BaseClass);

      // Act
      var result = type.InheritsFrom(baseType);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void InheritsFrom_MultiLevelInheritance_ReturnsTrue()
    {
      // Arrange
      var type = typeof(DoublyInheritedClass);
      var baseType = typeof(BaseClass);

      // Act
      var result = type.InheritsFrom(baseType);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void InheritsFrom_NoInheritance_ReturnsFalse()
    {
      // Arrange
      var type = typeof(ClassWithoutAttribute);
      var baseType = typeof(BaseClass);

      // Act
      var result = type.InheritsFrom(baseType);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void InheritsFrom_Interface_ReturnsTrue()
    {
      // Arrange
      var type = typeof(ClassImplementingInterface);
      var interfaceType = typeof(ITestInterface);

      // Act
      var result = type.InheritsFrom(interfaceType);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void InheritsFrom_WithNullType_ReturnsFalse()
    {
      // Arrange
      Type? type = null;
      var baseType = typeof(BaseClass);

      // Act
      var result = type.InheritsFrom(baseType);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void InheritsFrom_WithNullBaseType_ReturnsFalse()
    {
      // Arrange
      var type = typeof(InheritedClass);
      Type? baseType = null;

      // Act
      var result = type.InheritsFrom(baseType);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void ImplementsInterface_Generic_WithInterface_ReturnsTrue()
    {
      // Arrange
      var interfaceType = typeof(ITestInterface);

      // Act
      var result = TypeHelper.ImplementsInterface<ClassImplementingInterface>(interfaceType);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void ImplementsInterface_Generic_WithoutInterface_ReturnsFalse()
    {
      // Arrange
      var interfaceType = typeof(ITestInterface);

      // Act
      var result = TypeHelper.ImplementsInterface<ClassWithoutAttribute>(interfaceType);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void InheritsFrom_Generic_WithBaseClass_ReturnsTrue()
    {
      // Arrange
      var baseType = typeof(BaseClass);

      // Act
      var result = TypeHelper.InheritsFrom<InheritedClass>(baseType);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void InheritsFrom_Generic_WithoutBaseClass_ReturnsFalse()
    {
      // Arrange
      var baseType = typeof(BaseClass);

      // Act
      var result = TypeHelper.InheritsFrom<ClassWithoutAttribute>(baseType);

      // Assert
      Assert.False(result);
    }
  }
}
