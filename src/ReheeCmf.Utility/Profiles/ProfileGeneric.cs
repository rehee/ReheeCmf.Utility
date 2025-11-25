using System;

namespace ReheeCmf.Profiles
{
  public abstract class Profile<T> : Profile where T : Enum
  {
    public override Type KeyType => typeof(T);

    public abstract T Key { get; }

    public string KeyStringValue
    {
      get
      {
        return Key?.ToString() ?? string.Empty;
      }
    }

    public override int KeyValue
    {
      get
      {
        return Key != null ? Convert.ToInt32(Key) : 0;
      }
      set
      {
        throw new NotSupportedException("KeyValue cannot be set directly. It is computed from the Key property.");
      }
    }
  }
}
