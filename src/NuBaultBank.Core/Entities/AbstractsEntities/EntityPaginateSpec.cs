using NuBaultBank.SharedKernel;
using NuBaultBank.SharedKernel.Interfaces;
using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.AbstractsEntities;
public class EntityPaginateSpec<T> : Specification<T> where T : EntityBase, IAggregateRoot
{
  public EntityPaginateSpec(int pageNumber, int pageSize)
  {
    Query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize);
  }
}
