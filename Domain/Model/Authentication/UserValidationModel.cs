namespace Domain.Model.Authentication
{
    public class UserValidationModel
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
