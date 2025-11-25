namespace ReheeCmf.Contexts
{
  public interface ITenantContext : IWithTenant, ICrossTenant
  {
    Users.Tenant? ThisTenant { get; }

    bool? ReadOnly { get; }

    bool IgnoreTenant { get; }

    void SetTenant(Users.Tenant tenant);

    void SetReadOnly(bool readOnly);

    void SetIgnoreTenant(bool ignore);

    void UseDefaultConnection();

    void UseTenantConnection();
  }
}
