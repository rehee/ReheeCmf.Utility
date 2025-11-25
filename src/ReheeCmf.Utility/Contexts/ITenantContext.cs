using ReheeCmf.Users;

namespace ReheeCmf.Contexts
{
	public interface ITenantContext : IWithTenant, ICrossTenant
	{
		Tenant? ThisTenant { get; }

		bool? ReadOnly { get; }

		bool IgnoreTenant { get; }

		void SetTenant(Tenant tenant);

		void SetReadOnly(bool readOnly);

		void SetIgnoreTenant(bool ignore);

		void UseDefaultConnection();

		void UseTenantConnection();
	}
}
