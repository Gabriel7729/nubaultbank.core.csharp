using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.UserAggregate.Specs;
public class GetPaginateUsersSpec : Specification<User>
{
  public GetPaginateUsersSpec(int pageNumber, int pageSize)
  {
    //TODO : Agregar logica para obtener los usuarios, menos los del backoffice
    Query
      .Skip((pageNumber - 1) * pageSize)
      .Take(pageSize);
  }
}
