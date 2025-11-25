using System;

namespace ReheeCmf.Contexts
{
  public interface ICrossTenant
  {
    Guid? CrossTenantID { get; }

    Users.Tenant? CrossTenant { get; }

    void SetCrossTenant(Users.Tenant? tenant);
  }
}
