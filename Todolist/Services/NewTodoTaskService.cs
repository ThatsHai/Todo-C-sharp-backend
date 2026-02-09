using Todolist.Models;
using Todolist.Services.Interfaces;

namespace Todolist.Services
{
    public class NewTodoTaskService : INewTodoTaskService
    {
        private readonly List<NewTodoTask> singleList = new()
        {
            new NewTodoTask
            {
                Id = "1",
                TaskName = "Learn Angular",
                TaskUserId = "001"
            },
            new NewTodoTask
            {
                Id = "2",
                TaskName = "Fix N+1 Query",
                TaskUserId = "001"
            },
            new NewTodoTask
            {
                Id = "3",
                TaskName = "Dockerize API",
                TaskUserId = "002"
            }

    };
        public Task<NewTodoTask?> CreateNewTask(NewTodoTask task)
        {
            var nextId = singleList
                .Select(t => int.TryParse(t.Id, out var id) ? id : 0)
                .DefaultIfEmpty(0)
                .Max() + 1;
            task.Id = nextId.ToString();
            singleList.Add(task);
            return Task.FromResult<NewTodoTask?>(task);
        }

        public Task DeleteNewTask(string id)
        {
            var todo = singleList.FirstOrDefault(t => t.Id == id);
            if (todo is null)
            {
                Console.Write("id error");
                return Task.CompletedTask;
                throw new BadHttpRequestException("Task not found", StatusCodes.Status404NotFound);
            }

            singleList.Remove(todo);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<NewTodoTask>> GetAllNewTasks()
        {
            return Task.FromResult<IEnumerable<NewTodoTask>>(singleList);
        }

        public Task<NewTodoTask?> GetNewTaskById(string id)
        {
            var todo = singleList.FirstOrDefault(todo => todo.Id == id);
            return Task.FromResult(todo);
        }

        public Task<IEnumerable<NewTodoTask>> GetTasksByUserId(string userId)
        {
            var todos = singleList.Where(todo => todo.TaskUserId == userId);
            return Task.FromResult<IEnumerable<NewTodoTask>>(todos);
        }

        public Task ToggleNewTask(string id)
        {
            var task = singleList.FirstOrDefault(todo => todo.Id == id);

            if (task is null)
                throw new KeyNotFoundException();

            task.TaskCompleted = !task.TaskCompleted;
            return Task.CompletedTask;
        }

        public Task<PagedResult<NewTodoTask>> GetPagedTasks(int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var totalItems = singleList.Count;

            var items = singleList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult(new PagedResult<NewTodoTask>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            });
        }

        public IEnumerable<NewTodoTask> GetNewTasks(
            string? status,
            int page = 1,
            int pageSize = 10
        )
        {
            var query = status?.ToLowerInvariant() switch
            {
                "completed" => singleList.Where(t => t.TaskCompleted),
                "active" => singleList.Where(t => !t.TaskCompleted),
                _ => singleList
            };

            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        //public Task<PagedResult<NewTodoTask>> GetPagedTasks(string status, int page, int pageSize)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
