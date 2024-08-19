using Backoffice.Models;
using CommonLib.Data;

namespace Backoffice.Utils
{
    public static class DtoExtension
    {
        public static AccessTokenDto ToDto(this OAuth2 oauth2) => new()
        {
            AccessToken = oauth2.AccessToken,
            TokenType = oauth2.TokenType,
            ExpiresIn = oauth2.ExpiresIn,
            RefreshToken = oauth2.RefreshToken,
            Scope = oauth2.Scope
        };

        public static SilentstormUser ToEntity(this DiscordUserDto user) => new()
        {
            Id = 0,
            Username = user.Username!,
            DiscordId = user.Id
        };
    }
}
