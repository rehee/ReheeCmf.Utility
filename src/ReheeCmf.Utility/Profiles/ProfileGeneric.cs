using System;

namespace ReheeCmf.Profiles
{
  public abstract class Profile<T> : Profile where T : Enum
  {
    public override Type KeyType => typeof(T);

    public T? Key { get; set; }

    public void SetKey(T key)
    {
      Key = key;
      KeyValue = Convert.ToInt32(key);
    }

    public T? GetKey()
    {
      return Key;
    }
  }
}
