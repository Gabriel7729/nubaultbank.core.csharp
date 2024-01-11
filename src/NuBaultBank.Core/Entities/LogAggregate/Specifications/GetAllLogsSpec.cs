using Ardalis.Specification;
using NuBaultBank.Core.Models;

namespace NuBaultBank.Core.Entities.LogAggregate.Specifications;
public class GetAllLogsSpec : Specification<Log>
{
  public GetAllLogsSpec(FilterLogQuery query)
  {
    if (query.PageNumber.HasValue && query.PageSize.HasValue)
    {
      Query
          .Where(x => query.UserId == null || x.UserId == query.UserId)
          .Where(x => query.ActionStatus == null || x.ActionStatus == query.ActionStatus)
          .Where(x => query.Criteria == null
              || x.UserName.ToLower().Contains(query.Criteria.ToLower())
              || x.Message.ToLower().Contains(query.Criteria.ToLower()))
          .Where(x => query.StartDate == null || x.CreatedDate >= query.StartDate)
          .Where(x => query.EndDate == null || x.CreatedDate <= query.EndDate)
          .OrderByDescending(x => x.CreatedDate)
          .Skip((query.PageNumber.Value - 1) * query.PageSize.Value)
          .Take(query.PageSize.Value);
    }
    else
    {
      Query
          .Where(x => query.UserId == null || x.UserId == query.UserId)
          .Where(x => query.ActionStatus == null || x.ActionStatus == query.ActionStatus)
          .Where(x => query.Criteria == null
              || x.UserName.ToLower().Contains(query.Criteria.ToLower())
              || x.Message.ToLower().Contains(query.Criteria.ToLower()))
          .Where(x => query.StartDate == null || x.CreatedDate >= query.StartDate)
          .Where(x => query.EndDate == null || x.CreatedDate <= query.EndDate)
          .OrderByDescending(x => x.CreatedDate);
    }

  }
}
