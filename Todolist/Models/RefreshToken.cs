namespace Todolist.Models
{
    public class RefreshToken
    {
        public required string Token { get; set; }
        public required string UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
