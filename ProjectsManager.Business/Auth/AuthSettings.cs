namespace ProjectsManager.Business.Auth;

public class AuthSettings
{
    public TimeSpan Expires { get; init; }
    public string SecretKey { get; init; }
}