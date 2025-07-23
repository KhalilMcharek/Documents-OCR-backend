namespace Documents_OCR_back.Models.DTOs
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
