using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TesteStefanini.Application.DTOs;
using TesteStefanini.Application.Interfaces;

namespace TesteStefanini.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
                return Unauthorized(new { message = "Usuário ou senha inválidos" });

            return Ok(result);
        }
    }
}
