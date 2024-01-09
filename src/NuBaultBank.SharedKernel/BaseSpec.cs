﻿using Ardalis.Specification;

namespace NuBaultBank.SharedKernel;
public class BaseSpec<TEntity> : Specification<TEntity>, ISingleResultSpecification where TEntity : EntityBase
{
  public BaseSpec(Guid entityId)
  {
    Query.Where(entity => entity.Id == entityId);
  }
}
