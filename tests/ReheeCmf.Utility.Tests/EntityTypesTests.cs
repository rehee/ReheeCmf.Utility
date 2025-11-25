using System;
using Xunit;
using ReheeCmf.Entities;
using ReheeCmf.Users;
using ReheeCmf.Enums;
using ReheeCmf.Commons;

namespace ReheeCmf.Utility.Tests
{
  public class EntityTypesTests
  {
    [Fact]
    public void IId_CanBeImplemented()
    {
      var entity = new TestEntity { Id = Guid.NewGuid() };
      Assert.NotEqual(Guid.Empty, entity.Id);
    }

    [Fact]
    public void TokenDTO_CanBeCreated()
    {
      var token = new TokenDTO
      {
        TokenString = "test-token",
        UserId = "user123",
        UserName = "TestUser",
        UserEmail = "test@example.com",
        TenantID = Guid.NewGuid(),
        ExpireSecond = 3600
      };

      Assert.Equal("test-token", token.TokenString);
      Assert.Equal("user123", token.UserId);
      Assert.Equal("TestUser", token.UserName);
      Assert.Equal("test@example.com", token.UserEmail);
      Assert.NotEqual(Guid.Empty, token.TenantID);
      Assert.Equal((ulong)3600, token.ExpireSecond);
    }

    [Fact]
    public void Tenant_CanBeCreated()
    {
      var tenantId = Guid.NewGuid();
      var tenant = new Tenant
      {
        TenantID = tenantId,
        TenantName = "Test Tenant",
        TenantUrl = "https://test.example.com",
        TenantLevel = 1,
        MainConnectionString = "Server=localhost;Database=test;",
        IgnoreTenant = false
      };

      Assert.Equal(tenantId, tenant.TenantID);
      Assert.Equal("Test Tenant", tenant.TenantName);
      Assert.Equal("https://test.example.com", tenant.TenantUrl);
      Assert.Equal(1, tenant.TenantLevel);
      Assert.Equal("Server=localhost;Database=test;", tenant.MainConnectionString);
      Assert.False(tenant.IgnoreTenant);
    }

    [Fact]
    public void EnumEntityState_HasExpectedValues()
    {
      Assert.Equal(0, (int)EnumEntityState.NotSpecified);
      Assert.Equal(1, (int)EnumEntityState.Added);
      Assert.Equal(2, (int)EnumEntityState.Modified);
      Assert.Equal(3, (int)EnumEntityState.Deleted);
      Assert.Equal(4, (int)EnumEntityState.Unchanged);
    }

    [Fact]
    public void KeyValueItemDTO_CanBeCreated()
    {
      var item = new KeyValueItemDTO
      {
        Key = "testKey",
        Value = "testValue"
      };

      Assert.Equal("testKey", item.Key);
      Assert.Equal("testValue", item.Value);
    }

    private class TestEntity : IId<Guid>
    {
      public Guid Id { get; set; }
    }
  }
}
