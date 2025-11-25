using System;
using System.Linq;
using Xunit;
using ReheeCmf.Entities;
using ReheeCmf.Profiles;

namespace ReheeCmf.Utility.Tests
{
  public class ProfileTests
  {
    // Test enum for testing Profile<T>
    private enum TestProfileType
    {
      Type1 = 1,
      Type2 = 2,
      Type3 = 3
    }

    // Concrete implementation for testing Profile base class
    private class TestProfile : Profile
    {
      public override Type KeyType => typeof(int);
    }

    // Concrete implementation for testing Profile<T>
    private class TestGenericProfile : Profile<TestProfileType>
    {
      private readonly TestProfileType _key;

      public TestGenericProfile(TestProfileType key)
      {
        _key = key;
      }

      public override TestProfileType Key => _key;
    }

    // Concrete implementation for testing ProfileContainer
    private class TestProfileContainer : ProfileContainer
    {
    }

    // Concrete implementation for testing ProfileContainer<T>
    private class TestGenericProfileContainer : ProfileContainer<TestProfileType, TestGenericProfile>
    {
    }

    [Fact]
    public void IWithName_Profile_HasNameAndDescription()
    {
      var profile = new TestProfile
      {
        Name = "Test Profile",
        Description = "Test Description"
      };

      Assert.Equal("Test Profile", profile.Name);
      Assert.Equal("Test Description", profile.Description);
    }

    [Fact]
    public void Profile_GetEffectiveKey_WithNonZeroKeyValue_ReturnsKeyValue()
    {
      var profile = new TestProfile
      {
        KeyValue = 123,
        StringKeyValue = "test"
      };

      Assert.Equal("123", profile.GetEffectiveKey());
    }

    [Fact]
    public void Profile_GetEffectiveKey_WithZeroKeyValueAndStringKey_ReturnsStringKey()
    {
      var profile = new TestProfile
      {
        KeyValue = 0,
        StringKeyValue = "custom-key"
      };

      Assert.Equal("custom-key", profile.GetEffectiveKey());
    }

    [Fact]
    public void Profile_GetEffectiveKey_WithZeroKeyValueAndNullStringKey_ReturnsZero()
    {
      var profile = new TestProfile
      {
        KeyValue = 0,
        StringKeyValue = null
      };

      Assert.Equal("0", profile.GetEffectiveKey());
    }

    [Fact]
    public void ProfileGeneric_KeyType_ReturnsCorrectType()
    {
      var profile = new TestGenericProfile(TestProfileType.Type1);

      Assert.Equal(typeof(TestProfileType), profile.KeyType);
    }

    [Fact]
    public void ProfileGeneric_Key_ReturnsCorrectValue()
    {
      var profile = new TestGenericProfile(TestProfileType.Type2);

      Assert.Equal(TestProfileType.Type2, profile.Key);
      Assert.Equal(2, profile.KeyValue);
    }

    [Fact]
    public void ProfileGeneric_KeyStringValue_ReturnsStringRepresentation()
    {
      var profile = new TestGenericProfile(TestProfileType.Type3);

      Assert.Equal("Type3", profile.KeyStringValue);
    }

    [Fact]
    public void ProfileContainer_AddProfile_AddsProfileSuccessfully()
    {
      var container = new TestProfileContainer();
      var profile = new TestProfile
      {
        Name = "Profile1",
        KeyValue = 1
      };

      container.AddProfile(profile);

      var retrieved = container.GetProfile("1");
      Assert.NotNull(retrieved);
      Assert.Equal("Profile1", retrieved?.Name);
    }

    [Fact]
    public void ProfileContainer_AddProfile_WithNullProfile_ThrowsArgumentNullException()
    {
      var container = new TestProfileContainer();

      Assert.Throws<ArgumentNullException>(() => container.AddProfile(null!));
    }

    [Fact]
    public void ProfileContainer_GetProfile_WithNonExistentKey_ReturnsNull()
    {
      var container = new TestProfileContainer();

      var retrieved = container.GetProfile("non-existent");

      Assert.Null(retrieved);
    }

    [Fact]
    public void ProfileContainer_RemoveProfile_RemovesProfileSuccessfully()
    {
      var container = new TestProfileContainer();
      var profile = new TestProfile 
      { 
        Name = "Profile1",
        KeyValue = 1
      };
      container.AddProfile(profile);

      var removed = container.RemoveProfile("1");

      Assert.True(removed);
      Assert.Null(container.GetProfile("1"));
    }

    [Fact]
    public void ProfileContainer_RemoveProfile_WithNonExistentKey_ReturnsFalse()
    {
      var container = new TestProfileContainer();

      var removed = container.RemoveProfile("non-existent");

      Assert.False(removed);
    }

    [Fact]
    public void ProfileContainer_RemoveProfile_WithNullKey_ReturnsFalse()
    {
      var container = new TestProfileContainer();

      var removed = container.RemoveProfile(null!);

      Assert.False(removed);
    }

    [Fact]
    public void ProfileContainer_GetAllProfiles_ReturnsAllProfiles()
    {
      var container = new TestProfileContainer();
      var profile1 = new TestProfile { Name = "Profile1", KeyValue = 1 };
      var profile2 = new TestProfile { Name = "Profile2", KeyValue = 2 };

      container.AddProfile(profile1);
      container.AddProfile(profile2);

      var allProfiles = container.GetAllProfiles();

      Assert.Equal(2, allProfiles.Count());
    }

    [Fact]
    public void ProfileContainerGeneric_AddProfile_AddsTypedProfileSuccessfully()
    {
      var container = new TestGenericProfileContainer();
      var profile = new TestGenericProfile(TestProfileType.Type1)
      {
        Name = "GenericProfile1"
      };

      container.AddProfile(profile);

      var retrieved = container.GetProfile("1");
      Assert.NotNull(retrieved);
      Assert.Equal("GenericProfile1", retrieved?.Name);
      Assert.Equal(TestProfileType.Type1, retrieved?.Key);
    }

    [Fact]
    public void ProfileContainerGeneric_GetAllProfiles_ReturnsTypedProfiles()
    {
      var container = new TestGenericProfileContainer();
      var profile1 = new TestGenericProfile(TestProfileType.Type1) { Name = "Profile1" };
      var profile2 = new TestGenericProfile(TestProfileType.Type2) { Name = "Profile2" };

      container.AddProfile(profile1);
      container.AddProfile(profile2);

      var allProfiles = container.GetAllProfiles();

      Assert.Equal(2, allProfiles.Count());
      Assert.All(allProfiles, p => Assert.IsType<TestGenericProfile>(p));
    }
  }
}
