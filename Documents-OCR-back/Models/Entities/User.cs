using System.Reflection.Metadata;

namespace Documents_OCR_back.Models.Entities
{
    public enum Role
    {
        User,
        Admin
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }

        public ICollection<Document> Documents { get; set; }
    }
}
