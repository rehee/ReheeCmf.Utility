using System;
using ReheeCmf.Users;

namespace ReheeCmf.Contexts
{
	public interface ICrossTenant
	{
		Guid? CrossTenantID { get; }

		Tenant? CrossTenant { get; }

		void SetCrossTenant(Tenant? tenant);
	}
}
