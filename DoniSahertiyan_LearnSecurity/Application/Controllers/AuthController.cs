using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(IConfiguration configuration)
        {
            _jwtTokenService = new JwtTokenService(configuration["Jwt:Key"], configuration["Jwt:Issuer"]);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest.Username == "admin" && loginRequest.Password == "admin")
            {
                var tokenResponse = _jwtTokenService.GenerateToken(loginRequest.Username, "Admin");
                return Ok(tokenResponse);
            }
            else if (loginRequest.Username == "user" && loginRequest.Password == "user")
            {
                var tokenResponse = _jwtTokenService.GenerateToken(loginRequest.Username, "User");
                return Ok(tokenResponse);
            }

            return Unauthorized();
        }
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (_jwtTokenService.ValidateRefreshToken(request.Username, request.RefreshToken))
            {
                var tokenResponse = _jwtTokenService.GenerateToken(request.Username, "User");
                return Ok(tokenResponse);
            }

            return Unauthorized();
        }

        public class RefreshTokenRequest
        {
            public string Username { get; set; }
            public string RefreshToken { get; set; }
        }
        [HttpPost("logout")]
        public IActionResult Logout([FromBody] LogoutRequest request)
        {
            _jwtTokenService.RevokeRefreshToken(request.Username);
            return Ok();
        }

        public class LogoutRequest
        {
            public string Username { get; set; }
        }

    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
