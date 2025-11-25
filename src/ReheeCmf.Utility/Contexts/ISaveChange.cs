using System.Threading;
using System.Threading.Tasks;

namespace ReheeCmf.Contexts
{
  public interface ISaveChange
  {
    int SaveChanges(Users.TokenDTO? user = null);

    Task<int> SaveChangesAsync(Users.TokenDTO? user = null, CancellationToken ct = default(CancellationToken));
  }
}
