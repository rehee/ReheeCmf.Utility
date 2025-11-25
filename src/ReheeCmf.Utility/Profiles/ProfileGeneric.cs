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
        return Key.ToString();
      }
    }

    public override int KeyValue
    {
      get
      {
        return Convert.ToInt32(Key);
      }
      set
      {
        throw new NotSupportedException("KeyValue cannot be set directly. It is computed from the Key property.");
      }
    }
  }
}
