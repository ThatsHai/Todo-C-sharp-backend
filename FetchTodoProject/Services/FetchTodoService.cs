using System.Text.Json;
using Todolist.Models;

namespace FetchTodoProject.Services
{
    public class FetchTodoService
    {
        IHttpClientFactory _httpClientFactory;

        public FetchTodoService(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }
        //public async Task<JsonPlaceholderTodo[]> FetchTodos()
        //{
        //    HttpClient client = _httpClientFactory.CreateClient();
        //    JsonPlaceholderTodo[] todos = await client.GetFromJsonAsync<JsonPlaceholderTodo[]>(
        //        $"https://jsonplaceholder.typicode.com/todos", new JsonSerializerOptions(JsonSerializerDefaults.Web));
        //    return todos ?? [];
        //}
        public async Task<NewTodoTask[]> FetchTodos()
        {
            HttpClient client = _httpClientFactory.CreateClient();
            NewTodoTask[] todos = await client.GetFromJsonAsync<NewTodoTask[]>(
                $"https://localhost:7081/newTasks?page=1&pageSize=4", new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return todos ?? [];
        }
    }
}
