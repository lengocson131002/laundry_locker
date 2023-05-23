namespace LockerService.Application.Auth.Models;

public class TokenResponse
{
    public string AccessToken { get; private set; }

    public string RefreshToken { get; private set; }


    public TokenResponse(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}