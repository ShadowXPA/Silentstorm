using Backoffice.Models;
using Backoffice.Utils;
using CommonLib.Data;
using CommonLib.Data.Databases;
using CommonLib.Services;
using CommonLib.Utils;

namespace Backoffice.Services
{
    public class OAuth2Service
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PropertiesService _propertiesService;
        private readonly SilentstormDatabase _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OAuth2Service(IHttpClientFactory httpClientFactory,
            PropertiesService propertiesService,
            SilentstormDatabase database,
            IWebHostEnvironment webHostEnvironment)
        {
            _httpClientFactory = httpClientFactory;
            _propertiesService = propertiesService;
            _db = database;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<SilentstormUser?> GetCurrentUserAsync(OAuth2 oauth2)
        {
            var apiVersion = _propertiesService.GetProperty("discord.api-version");
            var apiEndpoint = $@"https://discord.com/api{(string.IsNullOrWhiteSpace(apiVersion) ? "" : $@"/{apiVersion}")}/users/@me";

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);

            httpRequest.Headers.Add("Authorization", $@"{oauth2.TokenType} {oauth2.AccessToken}");

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponse = await httpClient.SendAsync(httpRequest);

            if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized
                && oauth2.TokenType == "Bearer")
            {
                oauth2 = await RefreshAccessTokenAsync(oauth2);
                httpRequest = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);

                httpRequest.Headers.Add("Authorization", $@"{oauth2.TokenType} {oauth2.AccessToken}");
                httpResponse = await httpClient.SendAsync(httpRequest);
            }

            if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                await _db.OAuth2.RemoveAsync(oauth2.AccessToken);
                return null;
            }

            var result = await httpResponse.Content.ReadAsStringAsync();
            var discordUser = result.Deserialize<DiscordUserDto>()!;

            var silentStormUser = await _db.SilentstormUsers.FindByUsernameAsync(discordUser.Username!);

            if (silentStormUser == null)
            {
                _ = await _db.SilentstormUsers.AddAsync(discordUser.ToEntity());
                silentStormUser = await _db.SilentstormUsers.FindByUsernameAsync(discordUser.Username!);
            }

            return silentStormUser!;
        }

        public async Task<OAuth2?> GetAccessTokenAsync(string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://discord.com/api/oauth2/token");
            var body = new List<KeyValuePair<string, string>>()
            {
                new("client_id", _propertiesService.GetPropertyOrDefault("silentstorm.client_id", string.Empty)),
                new("client_secret", _propertiesService.GetPropertyOrDefault("silentstorm.client_secret", string.Empty)),
                new("grant_type", "authorization_code"),
                new("code", code),
                new("redirect_uri", _propertiesService.GetPropertyOrDefault($@"{(_webHostEnvironment.IsDevelopment() ? "dev." : "")}silentstorm.redirect_uri", string.Empty))
            };

            httpRequest.Content = new FormUrlEncodedContent(body);

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponse = await httpClient.SendAsync(httpRequest);

            if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK) return null;

            var result = await httpResponse.Content.ReadAsStringAsync();
            var oauth2 = result.Deserialize<OAuth2>()!;
            _ = await _db.OAuth2.AddAsync(oauth2);
            _ = await GetCurrentUserAsync(oauth2);

            return oauth2;
        }

        public async Task<OAuth2> RefreshAccessTokenAsync(OAuth2 oauth2)
        {
            var newOAuth2 = await RefreshAccessTokenAsync(oauth2.RefreshToken);

            if (newOAuth2 == null) return oauth2;

            _ = await _db.OAuth2.RemoveAsync(oauth2.AccessToken);
            _ = await _db.OAuth2.AddAsync(newOAuth2);

            return newOAuth2;
        }

        public async Task<bool> RevokeAccessTokenAsync(string? accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken)) return false;

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://discord.com/api/oauth2/token/revoke");
            var body = new List<KeyValuePair<string, string>>()
            {
                new("client_id", _propertiesService.GetPropertyOrDefault("silentstorm.client_id", string.Empty)),
                new("client_secret", _propertiesService.GetPropertyOrDefault("silentstorm.client_secret", string.Empty)),
                new("token", accessToken)
            };

            httpRequest.Content = new FormUrlEncodedContent(body);

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponse = await httpClient.SendAsync(httpRequest);
            var removed = await _db.OAuth2.RemoveAsync(accessToken);

            return httpResponse.StatusCode == System.Net.HttpStatusCode.OK && removed;
        }

        private async Task<OAuth2?> RefreshAccessTokenAsync(string refreshToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://discord.com/api/oauth2/token");
            var body = new List<KeyValuePair<string, string>>()
            {
                new("client_id", _propertiesService.GetPropertyOrDefault("silentstorm.client_id", string.Empty)),
                new("client_secret", _propertiesService.GetPropertyOrDefault("silentstorm.client_secret", string.Empty)),
                new("grant_type", "refresh_token"),
                new("refresh_token", refreshToken)
            };

            httpRequest.Content = new FormUrlEncodedContent(body);

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponse = await httpClient.SendAsync(httpRequest);

            if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK) return null;

            var result = await httpResponse.Content.ReadAsStringAsync();

            return result.Deserialize<OAuth2>();
        }
    }
}
