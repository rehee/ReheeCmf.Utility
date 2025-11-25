using System;

namespace ReheeCmf.Users
{
  public class Tenant
  {
    public Guid? TenantID { get; set; }

    public string? TenantName { get; set; }

    public string? TenantSubDomain { get; set; }

    public string? TenantUrl { get; set; }

    public int? TenantLevel { get; set; }

    public string? MainConnectionString { get; set; }

    public string[]? ReadConnectionStrings { get; set; }

    public bool? IgnoreTenant { get; set; }
  }
}
