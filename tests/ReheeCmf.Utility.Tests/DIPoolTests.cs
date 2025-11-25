using System;
using Xunit;
using ReheeCmf.DIContainers;
using ReheeCmf.Profiles;
using ReheeCmf.ProfileContainers;

namespace ReheeCmf.Utility.Tests
{
  public class DIPoolTests : IDisposable
  {
    public enum TestProfileKey
    {
      Default = 0,
      First = 1,
      Second = 2
    }

    public class TestProfile : Profile<TestProfileKey>
    {
      private readonly TestProfileKey _key;

      public TestProfile() : this(TestProfileKey.First)
      {
      }

      public TestProfile(TestProfileKey key)
      {
        _key = key;
        Name = $"TestProfile_{key}";
        Description = $"Test profile with key {key}";
      }

      public override TestProfileKey Key => _key;
    }

    public class TestProfileContainer : ProfileContainer<TestProfileKey, TestProfile>
    {
    }

    public class AnotherProfile : Profile<TestProfileKey>
    {
      private readonly TestProfileKey _key;

      public AnotherProfile() : this(TestProfileKey.Second)
      {
      }

      public AnotherProfile(TestProfileKey key)
      {
        _key = key;
        Name = $"AnotherProfile_{key}";
      }

      public override TestProfileKey Key => _key;
    }

    public DIPoolTests()
    {
      // Reset the DIPool before each test
      DIPool.Reset();
    }

    public void Dispose()
    {
      // Reset the DIPool after each test
      DIPool.Reset();
    }

    [Fact]
    public void Initialize_CalledOnce_SetsInitializedToTrue()
    {
      // Act
      DIPool.Initialize();

      // Assert
      Assert.True(DIPool.IsInitialized);
    }

    [Fact]
    public void Initialize_CalledTwice_OnlyInitializesOnce()
    {
      // Arrange
      int actionCallCount = 0;
      Action<Type> action = (type) => actionCallCount++;

      // Act
      DIPool.Initialize(action);
      DIPool.Initialize(action);

      // Assert
      Assert.True(DIPool.IsInitialized);
      // The action count should indicate initialization happened only once
      // Note: The exact count depends on how many Profile types are discovered
      var firstCount = actionCallCount;
      
      // Reset and initialize again
      DIPool.Reset();
      actionCallCount = 0;
      DIPool.Initialize(action);
      
      // Should be the same count if same types are discovered
      Assert.Equal(firstCount, actionCallCount);
    }

    [Fact]
    public void GetProfile_ByString_WithoutInitialization_ReturnsEmpty()
    {
      // Act
      var profiles = DIPool.GetProfile("First");

      // Assert
      Assert.Empty(profiles);
    }

    [Fact]
    public void GetProfile_ByEnum_WithoutInitialization_ReturnsEmpty()
    {
      // Act
      var profiles = DIPool.GetProfile(TestProfileKey.First);

      // Assert
      Assert.Empty(profiles);
    }

    [Fact]
    public void GetProfile_Generic_WithoutInitialization_ReturnsEmpty()
    {
      // Act
      var profiles = DIPool.GetAllProfiles<TestProfile>();

      // Assert
      Assert.Empty(profiles);
    }

    [Fact]
    public void Initialize_ScansAndCreatesProfiles()
    {
      // Act
      DIPool.Initialize();

      // Assert
      var profiles = DIPool.GetAllProfiles<TestProfile>();
      Assert.NotEmpty(profiles);
    }

    [Fact]
    public void GetProfile_ByStringKey_ReturnsCorrectProfile()
    {
      // Arrange
      DIPool.Initialize();

      // Act
      var profiles = DIPool.GetProfile("First");

      // Assert
      Assert.NotEmpty(profiles);
      Assert.All(profiles, p => Assert.IsAssignableFrom<Profile>(p));
    }

    [Fact]
    public void GetProfile_ByEnumKey_ReturnsCorrectProfile()
    {
      // Arrange
      DIPool.Initialize();

      // Act
      var profiles = DIPool.GetProfile(TestProfileKey.First);

      // Assert
      Assert.NotEmpty(profiles);
      Assert.All(profiles, p => Assert.IsAssignableFrom<Profile>(p));
    }

    [Fact]
    public void GetProfile_Generic_ReturnsCorrectProfile()
    {
      // Arrange
      DIPool.Initialize();

      // Act
      var profiles = DIPool.GetAllProfiles<TestProfile>();

      // Assert
      Assert.NotEmpty(profiles);
      var profile = profiles.FirstOrDefault();
      Assert.NotNull(profile);
      Assert.IsType<TestProfile>(profile);
      Assert.Equal(TestProfileKey.First, profile.Key);
    }

    [Fact]
    public void GetAllProfiles_Generic_ReturnsAllMatchingProfiles()
    {
      // Arrange
      DIPool.Initialize();

      // Act
      var profiles = DIPool.GetAllProfiles<TestProfile>();

      // Assert
      Assert.NotNull(profiles);
      Assert.NotEmpty(profiles);
    }

    [Fact]
    public void Initialize_WithActions_ExecutesActionsOnProfileTypes()
    {
      // Arrange
      int actionCallCount = 0;
      Type? capturedType = null;
      Action<Type> action = (type) =>
      {
        actionCallCount++;
        if (type == typeof(TestProfile))
        {
          capturedType = type;
        }
      };

      // Act
      DIPool.Initialize(action);

      // Assert
      Assert.True(actionCallCount > 0);
      Assert.Equal(typeof(TestProfile), capturedType);
    }

    [Fact]
    public void Initialize_WithMultipleActions_ExecutesAllActions()
    {
      // Arrange
      int action1CallCount = 0;
      int action2CallCount = 0;
      Action<Type> action1 = (type) => action1CallCount++;
      Action<Type> action2 = (type) => action2CallCount++;

      // Act
      DIPool.Initialize(action1, action2);

      // Assert
      Assert.True(action1CallCount > 0);
      Assert.True(action2CallCount > 0);
      Assert.Equal(action1CallCount, action2CallCount);
    }

    [Fact]
    public void GetAllProfilesByKeyType_ReturnsCorrectProfiles()
    {
      // Arrange
      DIPool.Initialize();

      // Act
      var profiles = DIPool.GetAllProfilesByKeyType<TestProfileKey>();

      // Assert
      Assert.NotNull(profiles);
      Assert.NotEmpty(profiles);
    }

    [Fact]
    public void GetProfile_WithNullKey_ReturnsEmpty()
    {
      // Arrange
      DIPool.Initialize();

      // Act
      var profiles = DIPool.GetProfile(default(string));

      // Assert
      Assert.Empty(profiles);
    }

    [Fact]
    public void GetProfile_WithEmptyKey_ReturnsEmpty()
    {
      // Arrange
      DIPool.Initialize();

      // Act
      var profiles = DIPool.GetProfile("");

      // Assert
      Assert.Empty(profiles);
    }

    [Fact]
    public void Reset_ClearsInitialization()
    {
      // Arrange
      DIPool.Initialize();
      Assert.True(DIPool.IsInitialized);

      // Act
      DIPool.Reset();

      // Assert
      Assert.False(DIPool.IsInitialized);
    }
  }
}
