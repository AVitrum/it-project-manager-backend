namespace OAuthService;

public class GoogleOAuthException : Exception
{
    public GoogleOAuthException(string message) : base(message) { }

    public GoogleOAuthException(string message, Exception innerException) : base(message, innerException) { }
}