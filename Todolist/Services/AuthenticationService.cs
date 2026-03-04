using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Todolist.Models;
using Todolist.Services.Interfaces;

namespace Todolist.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<PersonMongo> _passwordHasher;

        public AuthenticationService(IConfiguration config, IPasswordHasher<PersonMongo> passwordHasher)

        {
            _config = config;
            _passwordHasher = passwordHasher;
        }

        public async Task<string> createUser(PersonMongo person)
        {
            if (string.IsNullOrEmpty(person.UserName) || string.IsNullOrEmpty(person.Password))
            {
                throw new ArgumentException("Username and password cannot be empty");
            }
            var user = new PersonMongo();
            user.UserName = person.UserName;
            user.FirstName = person.FirstName;
            user.LastName = person.LastName;
            user.Password = _passwordHasher.HashPassword(user, person.Password);
            await DB.Instance().SaveAsync(user);
            string token = GenerateJwt(user.UserName);
            return token;
        }

        public async Task<string?> Login(LoginRequest loginRequest)
        {
            //var passwordUnhash = _passwordHasher.VerifyHashedPassword(loginRequest.Password, )
            //var person = await DB.Instance().Find<PersonMongo>().Match(p => p.UserName == loginRequest.Username && p.Password == loginRequest.Password).ExecuteFirstAsync();
            var person = await DB.Instance().Find<PersonMongo>().Match(p => p.UserName == loginRequest.Username).ExecuteFirstAsync();
            if (person == null)
            {
                return null;
            }
            var result = _passwordHasher.VerifyHashedPassword(
                person,
                person.Password,
                loginRequest.Password
                );
            if (result != PasswordVerificationResult.Success)
            {
                return null;
            }
            string token = GenerateJwt(person.UserName);
            return token;
        }

        public string GenerateJwt(string username)
        {
            var role = username == "admin" ? "Admin" : "User";

            var key = _config["Jwt:Key"] ?? throw new Exception("JWT Key missing");
            var expireHours = int.Parse(_config["Jwt:ExpireHours"] ?? "1");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("CustomClaim", "CustomValue"),
                new Claim("Domain", "IT")
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expireHours),
                signingCredentials: creds
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
