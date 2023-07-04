using Infrastructure.Data.Entity;

namespace Infrastructure.Data.Interface
{
    public interface IUserProfileRepository
    {
        Task CreateAsync(UserProfileEntity user);

        Task<UserProfileEntity> GetUserAsync(string email);

        Task<UserProfileEntity> GetUsernameAsync(string username);
        
        Task<UserProfileEntity> GetUserAsync(Guid id);

        Task UpdateAsync(Guid id, UserProfileEntity user);

        Task RemoveAsync(Guid id);
        
    }
}