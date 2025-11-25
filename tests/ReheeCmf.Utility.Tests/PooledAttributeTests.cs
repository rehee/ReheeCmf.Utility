using System;
using Xunit;
using ReheeCmf.Attributes;

namespace ReheeCmf.Utility.Tests
{
  public class PooledAttributeTests
  {
    [Fact]
    public void PooledAttribute_CanBeAppliedToClass()
    {
      // Arrange & Act
      var attribute = new PooledAttribute();

      // Assert
      Assert.NotNull(attribute);
    }

    [Fact]
    public void PooledAttribute_IsAttribute()
    {
      // Arrange & Act
      var attribute = new PooledAttribute();

      // Assert
      Assert.IsAssignableFrom<Attribute>(attribute);
    }

    [Pooled]
    private class TestClass
    {
    }

    [Fact]
    public void PooledAttribute_CanBeAppliedToTestClass()
    {
      // Arrange
      var type = typeof(TestClass);

      // Act
      var attributes = type.GetCustomAttributes(typeof(PooledAttribute), true);

      // Assert
      Assert.NotEmpty(attributes);
      Assert.Single(attributes);
    }
  }
}
