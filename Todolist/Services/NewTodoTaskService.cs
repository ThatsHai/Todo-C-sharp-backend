using MongoDB.Entities;
using Todolist.Models;
using Todolist.Services.Interfaces;

namespace Todolist.Services
{
    public class NewTodoTaskService : INewTodoTaskService
    {
        public async Task<NewTodoTask?> CreateNewTask(NewTodoTask task)
        {
            await DB.Instance().SaveAsync(task);
            return task;
        }

        public async Task DeleteNewTask(string id)
        {
            await DB.Instance().DeleteAsync<NewTodoTask>(id);
        }

        public async Task<IEnumerable<NewTodoTask>> GetAllNewTasks()
        {
            return await DB.Instance().Find<NewTodoTask>().ExecuteAsync();
        }

        public Task<NewTodoTask?> GetNewTaskById(string id)
        {
            return DB.Instance().Find<NewTodoTask>().OneAsync(id);
        }

        public async Task ToggleNewTask(string id)
        {
            var task = await DB.Instance().Find<NewTodoTask>().OneAsync(id);
            if (task == null) return;

            task.TaskCompleted = !task.TaskCompleted;
            await DB.Instance().SaveAsync(task);
        }

        public async Task<IEnumerable<NewTodoTask>> GetNewTasks(
            string? status,
            string? taskName,
            int page = 1,
            int pageSize = 10
        )
        {
            var query = DB.Instance().Find<NewTodoTask, NewTodoTask>();

            // filtering
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = status.ToLowerInvariant() switch
                {
                    "completed" => query.Match(t => t.TaskCompleted),
                    "active" => query.Match(t => !t.TaskCompleted),
                    _ => query
                };
            }

            if (!string.IsNullOrWhiteSpace(taskName))
            {
                query = query.Match(t => t.TaskName.ToLower().Contains(taskName.ToLower()));
            }

            // paging + ordering
            return await query
                .Sort(t => t.ID, Order.Ascending)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ExecuteAsync();
        }
    }
}
