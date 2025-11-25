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
      None = 0,
      Type1 = 1,
      Type2 = 2,
      Type3 = 3
    }

    // Concrete implementation for testing Profile base class
    private class TestProfile : Profile
    {
      public override Type KeyType => typeof(int);
      
      private int _keyValue;
      private string? _stringKeyValue;
      
      public override int KeyValue => _keyValue;
      
      public override string? StringKeyValue => _stringKeyValue;
      
      public void SetKeyValue(int value)
      {
        _keyValue = value;
      }
      
      public void SetStringKeyValue(string? value)
      {
        _stringKeyValue = value;
      }
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
    public void Profile_EffectiveKey_WithNonZeroKeyValue_ReturnsStringKeyValue()
    {
      var profile = new TestProfile();
      profile.SetKeyValue(123);
      profile.SetStringKeyValue("test");

      Assert.Equal("test", profile.EffectiveKey);
    }

    [Fact]
    public void Profile_EffectiveKey_WithZeroKeyValueAndStringKeyValueOverride_ReturnsOverride()
    {
      var profile = new TestProfile
      {
        StringKeyValueOverride = "custom-key"
      };
      profile.SetKeyValue(0);
      profile.SetStringKeyValue(null);

      Assert.Equal("custom-key", profile.EffectiveKey);
    }

    [Fact]
    public void Profile_EffectiveKey_WithZeroKeyValueAndNullOverride_ReturnsNull()
    {
      var profile = new TestProfile();
      profile.SetKeyValue(0);
      profile.SetStringKeyValue(null);

      Assert.Null(profile.EffectiveKey);
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
    public void ProfileGeneric_StringKeyValue_ReturnsStringRepresentation()
    {
      var profile = new TestGenericProfile(TestProfileType.Type3);

      Assert.Equal("Type3", profile.StringKeyValue);
    }

    [Fact]
    public void ProfileContainer_AddProfile_AddsProfileSuccessfully()
    {
      var container = new TestProfileContainer();
      var profile = new TestProfile
      {
        Name = "Profile1"
      };
      profile.SetKeyValue(1);
      profile.SetStringKeyValue("1");

      container.AddProfile(profile);

      var retrieved = container.GetProfile("1");
      Assert.NotNull(retrieved);
      Assert.Equal("Profile1", retrieved?.Name);
    }

    [Fact]
    public void ProfileContainer_AddProfile_WithNullProfile_DoesNotThrow()
    {
      var container = new TestProfileContainer();

      // Should not throw, just returns early
      container.AddProfile(null!);
      
      var allProfiles = container.GetAllProfiles();
      Assert.Empty(allProfiles);
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
        Name = "Profile1"
      };
      profile.SetKeyValue(1);
      profile.SetStringKeyValue("1");
      container.AddProfile(profile);

      var removed = container.RemoveProfile("1", out var removedProfile);

      Assert.True(removed);
      Assert.NotNull(removedProfile);
      Assert.Equal("Profile1", removedProfile?.Name);
      Assert.Null(container.GetProfile("1"));
    }

    [Fact]
    public void ProfileContainer_RemoveProfile_WithNonExistentKey_ReturnsFalse()
    {
      var container = new TestProfileContainer();

      var removed = container.RemoveProfile("non-existent", out var value);

      Assert.False(removed);
      Assert.Null(value);
    }

    [Fact]
    public void ProfileContainer_RemoveProfile_WithNullKey_ReturnsFalse()
    {
      var container = new TestProfileContainer();

      var removed = container.RemoveProfile(null!, out var value);

      Assert.False(removed);
      Assert.Null(value);
    }

    [Fact]
    public void ProfileContainer_GetAllProfiles_ReturnsAllProfiles()
    {
      var container = new TestProfileContainer();
      var profile1 = new TestProfile { Name = "Profile1" };
      profile1.SetKeyValue(1);
      profile1.SetStringKeyValue("1");
      var profile2 = new TestProfile { Name = "Profile2" };
      profile2.SetKeyValue(2);
      profile2.SetStringKeyValue("2");

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

      var retrieved = container.GetProfile("Type1");
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

    [Fact]
    public void ProfileContainerGeneric_GetProfileByEnum_RetrievesCorrectProfile()
    {
      var container = new TestGenericProfileContainer();
      var profile = new TestGenericProfile(TestProfileType.Type1)
      {
        Name = "EnumProfile"
      };

      container.AddProfile(profile);

      var retrieved = container.GetProfile(TestProfileType.Type1, "");
      Assert.NotNull(retrieved);
      Assert.Equal("EnumProfile", retrieved?.Name);
      Assert.Equal(TestProfileType.Type1, retrieved?.Key);
    }

    [Fact]
    public void ProfileContainer_GetProfileByEnum_WithZeroValueAndOverride_UsesOverride()
    {
      var container = new TestProfileContainer();
      var profile = new TestProfile
      {
        Name = "OverrideProfile",
        StringKeyValueOverride = "custom-key"
      };
      profile.SetKeyValue(0);
      profile.SetStringKeyValue(null);

      container.AddProfile(profile);

      // Use the defined enum value None (0)
      var retrieved = container.GetProfile(TestProfileType.None, "custom-key");
      Assert.NotNull(retrieved);
      Assert.Equal("OverrideProfile", retrieved?.Name);
    }

    [Fact]
    public void ProfileContainer_GetProfileByEnum_WithNonZeroValue_UsesEnumName()
    {
      var container = new TestGenericProfileContainer();
      var profile = new TestGenericProfile(TestProfileType.Type2)
      {
        Name = "EnumNameProfile"
      };

      container.AddProfile(profile);

      // Override parameter is not used for non-zero enum values
      var retrieved = container.GetProfile(TestProfileType.Type2, "");
      Assert.NotNull(retrieved);
      Assert.Equal("EnumNameProfile", retrieved?.Name);
    }
  }
}
