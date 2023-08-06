using Domain.Model.Authentication;
using Infrastructure.Data.Entity;
using Infrastructure.Data.Interface;
using Domain.Extensions;

namespace Presentation.Routers
{
    public static class AuthenticationRouter
    {
        public static void ConfigureAuthenticationRouter(this WebApplication app)
        {
            var group = app.MapGroup("api/v1");
            group.MapPost("/auth/login", Login);
            group.MapPost("/auth/account", Register);
            group.MapPost("/auth/account/validate", AccountValidation);
            group.MapPost("/auth/account/changePassword", ChangePassword);
        }

        private static async Task<IResult> Login(UserAuthenticateModel request, IUserProfileRepository repository)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                    return Results.BadRequest("Invalid email or password");

                var user = await repository.GetUserAsync(request.Email);

                if (user is null) return Results.NoContent();

                if (!user.IsEmailConfirmed) return Results.BadRequest("E-mail not confirmed");

                if (!AuthMetods.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                    return Results.BadRequest("Incorrect Password");

                return Results.Ok(user.CreateToken());
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login: " + ex.Message);
            }
        }

        private static async Task<IResult> ChangePassword(UserChangePasswordModel request, IUserProfileRepository repository)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.NewPassword))
                    return Results.BadRequest("Invalid email, current password, or new password");

                var user = await repository.GetUserAsync(request.Email);

                if (user is null) return Results.BadRequest("User Not Found");

                if (!AuthMetods.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                    return Results.BadRequest("Incorrect Password");

                AuthMetods.CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                await repository.UpdateAsync(user.Id, user);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                throw new Exception("Error changing password: " + ex.Message);
            }
        }

        private static async Task<IResult> Register(UserRegisterModel request, IUserProfileRepository repository)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                    return Results.BadRequest("Invalid email, username, or password");

                var userEmail = await repository.GetUserAsync(request.Email);
                if (userEmail is not null) return Results.BadRequest("This email is already being used");

                var username = await repository.GetUsernameAsync(request.Username);
                if (username is not null) return Results.BadRequest("This username is already being used");

                AuthMetods.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                
                await repository.CreateAsync(new UserProfileEntity
                {
                    Id = Guid.NewGuid(),
                    UserName = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
                    IsEmailConfirmed = false,
                    EmailConfirmationCode = RegisterMetods.GenerateValidationCode(5)
                });

                return Results.Ok();
            }
            catch (Exception ex)
            {
                throw new Exception("Error registering user: " + ex.Message);
            }
        }
    
        private static async Task<IResult> AccountValidation(UserValidationModel request, IUserProfileRepository repository)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Code))
                    return Results.BadRequest("Invalid email or code");
                
                var user = await repository.GetUserAsync(request.Email);

                if (user is null) return Results.NoContent();

                if (!user.EmailConfirmationCode.Equals(request.Code.ToUpper()))
                    return Results.BadRequest("Invalid code");

                user.IsEmailConfirmed = true;
                await repository.UpdateAsync(user.Id, user);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating account: " + ex.Message);
            }
        }
    }
}