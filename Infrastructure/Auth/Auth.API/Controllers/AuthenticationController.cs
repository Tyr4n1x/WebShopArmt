using Auth.API.DTOs;
using Auth.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(
                    UserManager<ApplicationUser> _userManager,
                    IConfiguration _config
        ) : ControllerBase

    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Ensure email is provided
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email is required.");

            // Ensure password and confirmation password are provided
            if (string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
                return BadRequest("Password is required.");

            // Ensure password and confirmation pasword match
            if (request.Password != request.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            // Use email as username if not provided
            var userName = string.IsNullOrWhiteSpace(request.UserName) ? request.Email : request.UserName;

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = request.Email,
                FirstName = request.FirstName ?? string.Empty,
                LastName = request.LastName ?? string.Empty,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            return result.Succeeded ? Ok("User registered successfully.") : BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Entry)
                        ?? await _userManager.FindByEmailAsync(request.Entry);

            if (user == null)
                return Unauthorized("Invalid username or email.");

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                return Unauthorized("Invalid password.");

            var token = await GenerateJwtTokenAsync(user);
            return Ok(new { token });
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new InvalidOperationException("JWT secret key is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"] ?? "Auth.API",
                audience: _config["Jwt:Audience"] ?? "BlazorUI",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
