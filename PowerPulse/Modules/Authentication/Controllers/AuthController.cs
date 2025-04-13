using Microsoft.AspNetCore.Mvc;
using PowerPulse.Modules.Authentication.Models;
using PowerPulse.Modules.Authentication.Services;

namespace PowerPulse.Modules.Authentication.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        try
        {
            var existingUsers = await _authService.GetUsersByUsernameAndEmail(model.Username, model.Email);
            if (existingUsers.Any(u => u.Username == model.Username))
            {
                return BadRequest("Username already exists");
            }
            if (existingUsers.Any(u => u.Email == model.Email))
            {
                return BadRequest("Email already exists");
            }
            var user = await _authService.Register(model.Username, model.Email, model.Password);
            return Ok(new { user.Id, user.Username });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        try
        {
            var token = await _authService.Login(model.Username, model.Password);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}