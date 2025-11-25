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
        return GetValidatedKey().ToString();
      }
    }

    public override int KeyValue
    {
      get
      {
        return Convert.ToInt32(GetValidatedKey());
      }
      set
      {
        throw new NotSupportedException("KeyValue cannot be set directly. It is computed from the Key property.");
      }
    }

    private T GetValidatedKey()
    {
      var key = Key;
      if (key == null)
      {
        throw new InvalidOperationException("Key property must not return null.");
      }
      return key;
    }
  }
}
