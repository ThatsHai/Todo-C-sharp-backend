using MongoDB.Entities;

namespace Todolist.Models
{
    public class PersonMongo : Entity, ICreatedOn, IModifiedOn
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}