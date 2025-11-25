using ReheeCmf.Users;

namespace ReheeCmf.Contexts
{
	public interface ITokenDTOContext
	{
		TokenDTO? User { get; }

		void SetUser(TokenDTO? user);
	}
}
