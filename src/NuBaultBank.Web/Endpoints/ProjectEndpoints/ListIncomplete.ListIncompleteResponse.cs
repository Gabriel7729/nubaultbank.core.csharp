namespace NuBaultBank.Web.Endpoints.ProjectEndpoints;

  public class ListIncompleteResponse
  {
      public ListIncompleteResponse(Guid projectId, List<ToDoItemRecord> incompleteItems)
      {
          ProjectId = projectId;
          IncompleteItems = incompleteItems;
      }
      public Guid ProjectId { get; set; }
      public List<ToDoItemRecord> IncompleteItems { get; set; }
  }
