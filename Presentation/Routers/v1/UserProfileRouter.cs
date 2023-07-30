using Domain.Model.UserProfile;
using Infrastructure.Data.Interface;

namespace Presentation.Routers
{
    public static class UserProfileRouter
    {
        public static void ConfigureUserProfileRouter(this WebApplication app)
        {
            var group = app.MapGroup("api/v1").RequireAuthorization();
            group.MapGet("/user/{id}", GetUser);
            group.MapPut("/user/{id}", UpdateUser);
            group.MapDelete("/user/{id}", DeleteUser);
        }

        private static async Task<IResult> GetUser(Guid id, IUserProfileRepository repository)
        {
            try
            {
                if (id.Equals(Guid.Empty)) return Results.BadRequest("Invalid user ID");

                var foundUser = await repository.GetUserAsync(id);

                if (foundUser is null) return Results.BadRequest("User Not Found");

                return Results.Ok(foundUser);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching user by ID: " + ex.Message);
            }
        }

        private static async Task<IResult> UpdateUser(Guid id, UserProfileModel userRequest, IUserProfileRepository repository)
        {
            try
            {
                if (id.Equals(Guid.Empty)) return Results.BadRequest("Invalid user ID");

                if (userRequest is null) return Results.BadRequest("Invalid user data");

                var foundUser = await repository.GetUserAsync(id);

                if (foundUser is null) return Results.BadRequest("User Not Found");

                foundUser.LastUpdatedDate = DateTime.UtcNow;
                foundUser.Email = userRequest.Email;
                foundUser.UserName = userRequest.UserName;
                foundUser.ImgUrl = userRequest.ImgUrl;

                await repository.UpdateAsync(id, foundUser);

                return Results.Ok(foundUser.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user profile: " + ex.Message);
            }
        }

        private static async Task<IResult> DeleteUser(Guid id, IUserProfileRepository repository)
        {
            try
            {
                if (id.Equals(Guid.Empty)) return Results.BadRequest("Invalid user ID");

                var foundUser = await repository.GetUserAsync(id);

                if (foundUser is null) return Results.BadRequest("User Not Found");

                await repository.RemoveAsync(id);

                return Results.Ok(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting user profile: " + ex.Message);
            }
        }
    }
}