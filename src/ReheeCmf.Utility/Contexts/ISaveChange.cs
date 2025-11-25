using System.Threading;
using System.Threading.Tasks;
using ReheeCmf.Users;

namespace ReheeCmf.Contexts
{
	public interface ISaveChange
	{
		int SaveChanges(TokenDTO? user = null);

		Task<int> SaveChangesAsync(TokenDTO? user = null, CancellationToken ct = default(CancellationToken));
	}
}
