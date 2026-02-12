namespace FetchTodoProject.Models
{
    public class JsonPlaceholderTodo
    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public bool completed { get; set; }
    }
}
