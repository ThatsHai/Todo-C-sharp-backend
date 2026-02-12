namespace Todolist.Models
{
    public class TaskQueryRequest
    {
        public string? Status { get; set; }
        public string? TaskName { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
