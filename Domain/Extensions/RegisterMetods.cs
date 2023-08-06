namespace Domain.Extensions;
public static class RegisterMetods
{
    private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public static string GenerateValidationCode(int length)
    {
        var random = new Random();
        var code = new char[length];

        for (int i = 0; i < length; i++)
        {
            code[i] = AllowedChars[random.Next(AllowedChars.Length)];
        }

        return new string(code);
    }

}
