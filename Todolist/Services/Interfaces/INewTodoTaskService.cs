using Todolist.Models;
namespace Todolist.Services.Interfaces
{
    public interface INewTodoTaskService
    {
        public Task<NewTodoTask?> CreateNewTask(NewTodoTask task);
        public Task<IEnumerable<NewTodoTask>> GetAllNewTasks();
        public Task<NewTodoTask?> GetNewTaskById(string id);
        public Task ToggleNewTask(string id);
        public Task DeleteNewTask(string id);
        //public Task<IEnumerable<NewTodoTask>> GetNewTasks(
        //    string? status,
        //    string? taskName,
        //    int page = 1,
        //    int pageSize = 10
        //);
        public Task<IEnumerable<NewTodoTask>> GetNewTasks(TaskQueryRequest request);
    }
}
