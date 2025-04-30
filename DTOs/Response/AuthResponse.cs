using CartolaLigas.Models;

namespace CartolaLigas.DTOs.Response
{
    public class AuthResponse
    {
        public string token { get; set; }
        public User record { get; set; }
    }
}