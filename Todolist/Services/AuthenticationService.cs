using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        public async Task<AuthResponse?> CreateUser(PersonMongo person, HttpContext httpContext)
        {
            if (string.IsNullOrEmpty(person.UserName) || string.IsNullOrEmpty(person.Password))
            {
                return null;
            }

            if (await DB.Instance().Find<PersonMongo>().Match(p => p.UserName == person.UserName).ExecuteFirstAsync() != null)
            {
                return null;
            }
            var user = new PersonMongo();
            user.UserName = person.UserName;
            user.FirstName = person.FirstName;
            user.LastName = person.LastName;
            user.Password = _passwordHasher.HashPassword(user, person.Password);
            user.RefreshTokens = new List<RefreshToken>();
            await DB.Instance().SaveAsync(user);
            string token = GenerateJwt(user.UserName);
            var (refreshToken, rawRefreshToken) = GenerateRefreshToken(user.ID);

            await DB.Instance().Update<PersonMongo>()
                .Match(p => p.ID == user.ID)
                .Modify(x => x.Push(p => p.RefreshTokens, refreshToken))
                .ExecuteAsync();

            httpContext.Response.Cookies.Append("refreshToken", rawRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = refreshToken.ExpiryDate
            });

            return new AuthResponse
            {
                AccessToken = token,
                RefreshToken = rawRefreshToken
            };
        }

        public async Task<AuthResponse?> Login(LoginRequest loginRequest, HttpContext httpContext)
        {
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
            // Generate refresh token and save to DB
            var (refreshToken, rawRefreshToken) = GenerateRefreshToken(person.ID);
            await DB.Instance().Update<PersonMongo>()
                .Match(p => p.ID == person.ID)
                .Modify(token => token.Push(x => x.RefreshTokens, refreshToken))
                .ExecuteAsync();

            // Set HttpOnly cookie for refresh token
            httpContext.Response.Cookies.Append("refreshToken", rawRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = refreshToken.ExpiryDate
            });

            return new AuthResponse
            {
                AccessToken = token,
                RefreshToken = rawRefreshToken
            };
        }

        public string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public (RefreshToken tokenEntity, string rawToken) GenerateRefreshToken(string userId)
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var rawToken = Convert.ToBase64String(randomNumber);
            var tokenHashed = HashToken(rawToken);

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                Token = tokenHashed,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };
            return (refreshToken, rawToken);
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

        public async Task RevokeRefreshToken(string token)
        {
            var hash = HashToken(token);
            var person = await DB.Instance().Find<PersonMongo>().Match(p => p.RefreshTokens.Any(rt => rt.Token == hash)).ExecuteFirstAsync();
            if (person == null)
            {
                throw new Exception("User not found");
            }
            var existingToken = person.RefreshTokens.FirstOrDefault(rt => rt.Token == hash);
            if (existingToken == null || existingToken.IsRevoked)
            {
                throw new Exception("Invalid refresh token");
            }
            existingToken.IsRevoked = true;
            await DB.Instance().Update<PersonMongo>()
                .Match(p => p.ID == person.ID)
                .Modify(token => token.Set(p => p.RefreshTokens, person.RefreshTokens))
                .ExecuteAsync();
        }

        public async Task<AuthResponse> RefreshJwt(string refreshToken)
        {
            var hashed = HashToken(refreshToken);

            var person = await DB.Instance().Find<PersonMongo>()
                .Match(p => p.RefreshTokens.Any(rt => rt.Token == hashed))
                .ExecuteFirstAsync();
            if (person == null)
            {
                throw new Exception("User not found");
            }
            var existingToken = person.RefreshTokens
                .FirstOrDefault(rt => rt.Token == hashed);
            if (existingToken == null || existingToken.IsRevoked || existingToken.ExpiryDate < DateTime.UtcNow)
            {
                throw new Exception("Invalid refresh token");
            }
            existingToken.IsRevoked = true;
            var (refreshTokenEntity, rawToken) = GenerateRefreshToken(person.ID);
            person.RefreshTokens ??= new List<RefreshToken>();
            person.RefreshTokens.Add(refreshTokenEntity);

            await DB.Instance().Update<PersonMongo>()
                .Match(p => p.ID == person.ID)
                .Modify(x => x.Set(p => p.RefreshTokens, person.RefreshTokens))
                .ExecuteAsync();

            var newJwt = GenerateJwt(person.UserName);
            return new AuthResponse
            {
                AccessToken = newJwt,
                RefreshToken = rawToken
            };
        }

    }
}
