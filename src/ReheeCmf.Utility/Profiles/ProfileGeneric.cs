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
        // KeyValue is derived from Key, so setting it has no effect
        // but we need this setter to override the base property
      }
    }
  }
}
