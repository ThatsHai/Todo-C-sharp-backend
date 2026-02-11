using MongoDB.Entities;
using StackExchange.Redis;
using System.Text.Json;
using Todolist.Models;
using Todolist.Services.Interfaces;

namespace Todolist.Services
{
    public class NewTodoTaskService : INewTodoTaskService
    {
        private IConnectionMultiplexer _cache;
        public NewTodoTaskService(IConnectionMultiplexer cache)
        {
            this._cache = cache;
        }
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

        public async Task<NewTodoTask?> GetNewTaskById(string id)
        {
            var db = _cache.GetDatabase();
            var cachedValue = await db.StringGetAsync(id);

            if (!cachedValue.IsNull)
            {
                // Found in cache
                return JsonSerializer.Deserialize<NewTodoTask>(cachedValue.ToString());
            }

            // 2️⃣ Not in cache → Fetch from Mongo
            var mongoResult = await DB.Instance()
                                      .Find<NewTodoTask>()
                                      .OneAsync(id);

            if (mongoResult is null)
                return null;

            // 3️⃣ Store in Redis for next time
            var serialized = JsonSerializer.Serialize(mongoResult);
            await db.StringSetAsync(id, serialized, TimeSpan.FromMinutes(10));

            return mongoResult;
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
            var db = _cache.GetDatabase();

            var cacheKey = $"todo:list:{status}:{taskName}:{page}:{pageSize}";

            var cacheValue = await db.StringGetAsync(cacheKey);

            if (!cacheValue.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<IEnumerable<NewTodoTask>>(
                    cacheValue.ToString()
                ) ?? Enumerable.Empty<NewTodoTask>();
            }

            // =========================

            // Build query
            var query = DB.Instance().Find<NewTodoTask, NewTodoTask>();

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
                query = query.Match(t =>
                    t.TaskName.ToLower().Contains(taskName.ToLower())
                );
            }

            var result = await query
                .Sort(t => t.ID, MongoDB.Entities.Order.Ascending)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ExecuteAsync();

            // Save to cache
            await db.StringSetAsync(
                cacheKey,
                JsonSerializer.Serialize(result),
                TimeSpan.FromMinutes(10)
            );

            return result;
        }
    }
}
