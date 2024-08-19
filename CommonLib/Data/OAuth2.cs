using CommonLib.Data.Databases;

namespace CommonLib.Data
{
    public class OAuth2 : IEntity
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public uint ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
    }
}
