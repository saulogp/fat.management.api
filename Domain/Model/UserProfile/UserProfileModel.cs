namespace Domain.Model.UserProfile
{
    public sealed class UserProfileModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
    }
}