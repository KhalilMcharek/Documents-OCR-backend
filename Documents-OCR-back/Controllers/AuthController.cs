using Documents_OCR_back.Helpers;
using Documents_OCR_back.Models.DTOs;
using Documents_OCR_back.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Documents_OCR_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = await _authService.Register(request.Name , request.Email, request.Password );
            return Ok(new { message = "compte crée avec succes", email = user.Email});
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.Login(request.Email, request.Password);
            var token = JwtHelper.GenerateJwtToken(user, _configuration);
            return Ok(new AuthResponse { Token = token, Email = user.Email,
                Name = user.Name,
                Role = user.Role.ToString()
            });
        }
    }

}
