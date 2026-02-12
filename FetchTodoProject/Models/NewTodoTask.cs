namespace Todolist.Models
{
    public class NewTodoTask
    {
        //public required string Id { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string TaskUserId { get; set; } = string.Empty;
        public bool TaskCompleted { get; set; } = false;
    }
}
