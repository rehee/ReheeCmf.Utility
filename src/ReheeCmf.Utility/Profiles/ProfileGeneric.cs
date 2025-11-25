using System;

namespace ReheeCmf.Profiles
{
  public abstract class Profile<T> : Profile where T : Enum
  {
    public override Type KeyType => typeof(T);

    public abstract T Key { get; }

    public override string? StringKeyValue => Key?.ToString() ?? string.Empty;

    public override int KeyValue => Key != null ? Convert.ToInt32(Key) : 0;
  }
}
