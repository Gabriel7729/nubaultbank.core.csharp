using Ardalis.Specification;

namespace NuBaultBank.Core.ProjectAggregate.Specifications;

public class ProjectByIdWithItemsSpec : Specification<Project>, ISingleResultSpecification
  {
      public ProjectByIdWithItemsSpec(Guid projectId)
      {
          Query
              .Where(project => project.Id == projectId)
              .Include(project => project.Items);
      }
  }
