namespace GuardianTalesGuildRaidUpdater.Models.HttpResonses
{
    public class ResGoogleOAuthTokenWithRefreshToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        public ResGoogleOAuthTokenWithRefreshToken()
        {
            AccessToken = string.Empty;
            ExpiresIn = default;
            Scope = string.Empty;
            TokenType = string.Empty;
        }
    }
}
