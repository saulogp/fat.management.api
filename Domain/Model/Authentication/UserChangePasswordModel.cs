namespace Domain.Model.Authentication
{
    public sealed class UserChangePasswordModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NewPassword { get; set;} = string.Empty;
    }
}