namespace TrainingCenterSystem.Models.Entities
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }
    }
}
