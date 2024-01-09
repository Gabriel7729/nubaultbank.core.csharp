
namespace NuBaultBank.Web.Endpoints.ProjectEndpoints;

  public class GetProjectByIdResponse
  {
      public GetProjectByIdResponse(Guid id, string name, List<ToDoItemRecord> items)
      {
          Id = id;
          Name = name;
          Items = items;
      }

      public Guid Id { get; set; }
      public string Name { get; set; }
      public List<ToDoItemRecord> Items { get; set; } = new();
  }
