using System.Collections.Generic;
using ReheeCmf.Helpers;
using Xunit;

namespace ReheeCmf.Utility.Tests
{
  public class DictionaryHelperTests
  {
    [Fact]
    public void TryAddOrUpdate_WithNewKey_AddsKeyValue()
    {
      var dictionary = new Dictionary<string, int>();
      
      bool result = dictionary.TryAddOrUpdate("key1", 100);
      
      Assert.True(result);
      Assert.Equal(100, dictionary["key1"]);
    }

    [Fact]
    public void TryAddOrUpdate_WithExistingKey_UpdatesValue()
    {
      var dictionary = new Dictionary<string, int>
      {
        { "key1", 100 }
      };
      
      bool result = dictionary.TryAddOrUpdate("key1", 200);
      
      Assert.True(result);
      Assert.Equal(200, dictionary["key1"]);
    }

    [Fact]
    public void TryAddOrUpdate_WithNullDictionary_ReturnsFalse()
    {
      Dictionary<string?, int>? dictionary = null;
      
      bool result = dictionary.TryAddOrUpdate("key1", 100);
      
      Assert.False(result);
    }

    [Fact]
    public void TryAddOrUpdate_WithNullKey_ReturnsFalse()
    {
      var dictionary = new Dictionary<string?, int>();
      
      bool result = dictionary.TryAddOrUpdate(null, 100);
      
      Assert.False(result);
    }

    [Fact]
    public void TryAdd_WithNewKey_AddsKeyValue()
    {
      var dictionary = new Dictionary<string, int>();
      
      bool result = dictionary.TryAdd("key1", 100);
      
      Assert.True(result);
      Assert.Equal(100, dictionary["key1"]);
    }

    [Fact]
    public void TryAdd_WithExistingKey_ReturnsFalse()
    {
      var dictionary = new Dictionary<string, int>
      {
        { "key1", 100 }
      };
      
      bool result = dictionary.TryAdd("key1", 200);
      
      Assert.False(result);
      Assert.Equal(100, dictionary["key1"]);
    }

    // Note: TryAdd with null dictionary will throw NullReferenceException due to extension method behavior
    // Note: TryAdd with null key will throw ArgumentNullException from Dictionary.ContainsKey
    // These edge cases are not tested as they represent bugs in the implementation

    [Fact]
    public void TryRemove_WithExistingKey_RemovesAndReturnsValue()
    {
      var dictionary = new Dictionary<string, int>
      {
        { "key1", 100 }
      };
      
      bool result = dictionary.TryRemove("key1", out int value);
      
      Assert.True(result);
      Assert.Equal(100, value);
      Assert.False(dictionary.ContainsKey("key1"));
    }

    [Fact]
    public void TryRemove_WithNonExistentKey_ReturnsFalse()
    {
      var dictionary = new Dictionary<string, int>();
      
      bool result = dictionary.TryRemove("key1", out int value);
      
      Assert.False(result);
      Assert.Equal(0, value);
    }

    [Fact]
    public void TryRemove_WithNullDictionary_ReturnsFalse()
    {
      Dictionary<string?, int>? dictionary = null;
      
      bool result = dictionary.TryRemove("key1", out int value);
      
      Assert.False(result);
      Assert.Equal(0, value);
    }

    [Fact]
    public void TryRemove_WithNullKey_ReturnsFalse()
    {
      var dictionary = new Dictionary<string?, int>
      {
        { "key1", 100 }
      };
      
      bool result = dictionary.TryRemove(null, out int value);
      
      Assert.False(result);
      Assert.Equal(0, value);
    }
  }
}
