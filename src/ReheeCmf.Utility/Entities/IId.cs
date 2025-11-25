using System;

namespace ReheeCmf.Entities
{
  public interface IId<T> where T : IEquatable<T>
  {
    T Id { get; set; }
  }
}
