namespace ReheeCmf.Contexts
{
  public interface ITokenDTOContext
  {
    Users.TokenDTO? User { get; }

    void SetUser(Users.TokenDTO? user);
  }
}
