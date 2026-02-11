using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace Todolist.Models
{
    public class NewTodoTask : Entity
    {
        //public required string Id { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string TaskUserId { get; set; } = string.Empty;
        public bool TaskCompleted { get; set; } = false;
    }
}
