namespace LockerService.Application.Features.Auth.Models;

public class AccessTokenResponse
{
    public string AccessToken { get; private set; }

    public string RefreshToken { get; private set; }


    public AccessTokenResponse(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}