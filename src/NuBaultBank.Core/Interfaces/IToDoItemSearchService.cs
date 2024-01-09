using Ardalis.Result;
using NuBaultBank.Core.ProjectAggregate;

namespace NuBaultBank.Core.Interfaces;

  public interface IToDoItemSearchService
  {
      Task<Result<ToDoItem>> GetNextIncompleteItemAsync(Guid projectId);
      Task<Result<List<ToDoItem>>> GetAllIncompleteItemsAsync(Guid projectId, string searchString);
  }
