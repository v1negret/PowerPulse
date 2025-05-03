using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PowerPulse.Core.Entities;
using PowerPulse.Infrastructure.Data;
using PowerPulse.Modules.Email.Services;

namespace PowerPulse.Modules.Authentication.Services;

public class AuthService
    {
        private readonly EnergyDbContext _context;
        private readonly IConfiguration _config;
        private readonly EmailSenderService _emailSenderService;
        public AuthService(EnergyDbContext context, IConfiguration config, EmailSenderService emailSenderService)
        {
            _context = context;
            _config = config;
            _emailSenderService = emailSenderService;
        }

        public async Task<User> Register(string username, string email, string password)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                IsEmailConfirmed = false,
                EmailConfirmationToken = Guid.NewGuid().ToString(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var confirmationLink = $"{_config["ApiBaseUrl"]}/api/auth/confirm-email?token={user.EmailConfirmationToken}";
            var message = $"Пожалуйста, подтвердите вашу электронную почту, перейдя по ссылке: <a href='{confirmationLink}'>Подтвердить почту</a>";
            await _emailSenderService.SendEmailAsync(user.Email, "Подтверждение электронной почты", message);
            return user;
        }

        public async Task<string> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Неверный логин или пароль");
            if (!user.IsEmailConfirmed)
                throw new Exception("Перед входом необходимо подтвердить электронную почту");

            return GenerateJwtToken(user);
        }
        
        public async Task<List<User>> GetUsersByUsernameAndEmail(string username, string email)
        {
            var users = await _context.Users.Where(u => u.Username == username || email.Equals(u.Email)).ToListAsync();
            return users;
        }

        public async Task<Guid?> GetUserUidByUsername(string? username)
        {
            var uid = await _context.Users.Where(u => u.Username.Equals(username)).Select(u => u.Id)
                .FirstOrDefaultAsync();
            return uid;
        } 
        public async Task<bool> ConfirmEmailAsync(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
            if (user == null)
            {
                return false;
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null; // Очищаем токен после подтверждения
            await _context.SaveChangesAsync();

            return true;
        }
        

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
    }