namespace GuardianTalesGuildRaidUpdater.Models.HttpRequests
{
    public class ReqGoogleOAuthTokenWithRefreshToken
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
        [JsonProperty("grant_type")]
        public string GrantType { get; init; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        public ReqGoogleOAuthTokenWithRefreshToken()
        {
            ClientId = string.Empty;
            ClientSecret = string.Empty;
            GrantType = "refresh_token";
            RefreshToken = string.Empty;
        }
    }
}
