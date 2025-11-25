using System;
using ReheeCmf.Entities;

namespace ReheeCmf.Profiles
{
  public abstract class Profile : IWithName
  {
    public string? Name { get; set; }
    public string? Description { get; set; }

    public abstract Type KeyType { get; }
    
    public int KeyValue { get; set; }
    
    public string? StringKeyValue { get; set; }

    public string GetEffectiveKey()
    {
      if (KeyValue == 0 && !string.IsNullOrEmpty(StringKeyValue))
      {
        return StringKeyValue;
      }
      return KeyValue.ToString();
    }
  }
}
