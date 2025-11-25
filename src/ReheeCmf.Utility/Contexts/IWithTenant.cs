using System;

namespace ReheeCmf.Contexts
{
	public interface IWithTenant
	{
		Guid? TenantID { get; set; }
	}
}
