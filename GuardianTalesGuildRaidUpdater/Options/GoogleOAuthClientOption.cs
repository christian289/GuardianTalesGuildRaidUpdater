namespace GuardianTalesGuildRaidUpdater.Options
{
    public class GoogleOAuthClientOption
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class GoogleApiEndPointsOption
    {
        public string Authorization { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserInfo { get; set; }
    }
}
