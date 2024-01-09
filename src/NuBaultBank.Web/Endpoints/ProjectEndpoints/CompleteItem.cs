using Ardalis.ApiEndpoints;
using NuBaultBank.Core.ProjectAggregate;
using NuBaultBank.Core.ProjectAggregate.Specifications;
using NuBaultBank.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.ProjectEndpoints;
public class CompleteItem : EndpointBaseAsync
        .WithRequest<CompleteItemRequest>
        .WithoutResult
{
  private readonly IRepository<Project> _repository;

  public CompleteItem(IRepository<Project> repository)
  {
    _repository = repository;
  }
  // PATCH: api/Projects/{projectId}/complete/{itemId}
  [HttpPatch(CompleteItemRequest.Route)]
  [SwaggerOperation(
            Summary = "Change Item status",
            Description = "Change Item status",
            OperationId = "ToDoItem.Patch",
            Tags = new[] { "ProjectEndpoints" })
        ]
  public override async Task<IActionResult> HandleAsync([FromRoute] CompleteItemRequest request, CancellationToken cancellationToken = default)
  {
    var projectSpec = new ProjectByIdWithItemsSpec(request.ProjectId);
    var project = await _repository.GetBySpecAsync(projectSpec);
    if (project == null) return NotFound("No such project");

    var toDoItem = project.Items.FirstOrDefault(item => item.Id == request.ItemId);
    if (toDoItem == null) return NotFound("No such item.");

    toDoItem.MarkComplete();
    await _repository.UpdateAsync(project);

    return Ok();
  }
}
