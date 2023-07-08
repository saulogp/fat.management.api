using System.Security.Claims;
using System.Security.Cryptography;
using Domain.Model.Authentication;
using Infrastructure.Data.Entity;
using Infrastructure.Data.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Presentation.Controller
{
    public static class AuthenticationRouter
    {
        public static void ConfigureAuthenticationRouter(this WebApplication app)
        {
            var authGroup = app.MapGroup("api/v1");
            authGroup.MapPost("auth/login", Login);
            authGroup.MapPost("auth/register", Register);
            authGroup.MapPost("auth/changePassword", ChangePassword);
        }

        private static async Task<IResult> Login(UserAuthenticateModel request, [FromServices] IUserProfileRepository repository)
        {
            var user = await repository.GetUserAsync(request.Email);

            if (user is null) return Results.Unauthorized();

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return Results.Unauthorized();

            return Results.Ok(user.CreateToken());
        }

        private static async Task<IResult> ChangePassword(UserUpdatePasswordModel request, [FromServices] IUserProfileRepository repository)
        {
            var user = await repository.GetUserAsync(request.Email);

            if (user is null) return Results.BadRequest("User Not Found");

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return Results.BadRequest("Incorrect Password");

            CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await repository.UpdateAsync(user.Id, user);

            return Results.Ok();
        }

        private static async Task<IResult> Register(UserRegisterModel request, [FromServices] IUserProfileRepository repository)
        {
            var user = await repository.GetUserAsync(request.Email);
            if (user is not null) return Results.BadRequest("This email is already being used");

            var username = await repository.GetUsernameAsync(request.Username);
            if (username is not null) return Results.BadRequest("This username is already being used");

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            await repository.CreateAsync(new UserProfileEntity { Id = Guid.NewGuid(), UserName = request.Username, Email = request.Email, PasswordHash = passwordHash, PasswordSalt = passwordSalt, CreatedDate = DateTime.UtcNow, LastUpdatedDate = DateTime.UtcNow });

            return Results.Ok();
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private static string CreateToken(this UserProfileEntity user)
        {
            var issuer = "test.com";
            var audience = "test.com";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("&2f5t@dmR_8*79WtoSmCY1BzC"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes("&2f5t@dmR_8*79WtoSmCY1BzC");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(2),
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}