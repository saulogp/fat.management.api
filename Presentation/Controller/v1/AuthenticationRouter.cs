using System.Security.Claims;
using System.Security.Cryptography;
using Domain.Model.Authentication;
using Infrastructure.Data.Entity;
using Infrastructure.Data.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Presentation.Controller
{
    public static class AuthenticationRouter
    {
        public static void ConfigureAuthenticationRouter(this WebApplication app)
        {
            var authGroup = app.MapGroup("api/v1");
            authGroup.MapPost("auth/login", Login);
            authGroup.MapPost("auth/register", Register);
            authGroup.MapPost("auth/changePassword", ChangePassword).RequireAuthorization();
        }

        private static async Task<IResult> Login(UserAuthenticateModel request, [FromServices] IUserProfileRepository repository)
        {
            var user = await repository.GetUserAsync(request.Email);

            if (user == null) return Results.BadRequest("Not Allow");

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return Results.BadRequest("Not Allow");

            var token = CreateToken(user);
            return Results.Ok(token);
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

            return Results.Ok("Allow");
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

        private static string CreateToken(UserProfileEntity user){
            List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("çkljasdçfkjaçsdfkj"));
            
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            return jwt;
        }
    }
}