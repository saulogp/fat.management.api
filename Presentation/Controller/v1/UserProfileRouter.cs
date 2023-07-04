using Domain.Model.UserProfile;
using Infrastructure.Data.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controller
{
    public static class UserProfileRouter
    {
        public static void ConfigureUserProfileRouter(this WebApplication app)
        {
            var userGroup = app.MapGroup("api/v1");
            userGroup.MapGet("user/{id}", GetUserById);
            userGroup.MapPut("user/{id}", UpdateUserById);
            userGroup.MapDelete("user/{id}", DeleteUserById);
        }

        private static async Task<IResult> GetUserById(Guid id, [FromServices] IUserProfileRepository repository)
        {
            var user = await repository.GetUserAsync(id);

            if (user is null) return Results.BadRequest("User Not Found");

            return Results.Ok(user);
        }

        private static async Task<IResult> UpdateUserById(Guid id, UserProfileModel userRequest, [FromServices] IUserProfileRepository repository)
        {
            var user = await repository.GetUserAsync(id);
            if (user is null) return Results.BadRequest("User Not Found");

            user.LastUpdatedDate = DateTime.UtcNow;
            user.Email = userRequest.Email;
            user.UserName = userRequest.UserName;
            user.ImgUrl = userRequest.ImgUrl;

            await repository.UpdateAsync(id, user);

            return Results.Ok(user.Id);
        }

        private static async Task<IResult> DeleteUserById(Guid id, [FromServices] IUserProfileRepository repository)
        {
            var user = await repository.GetUserAsync(id);
            if (user is null) return Results.BadRequest("User Not Found");

            await repository.RemoveAsync(id);

            return Results.Ok(id);
        }
    }
}