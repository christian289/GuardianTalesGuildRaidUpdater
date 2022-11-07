namespace GuardianTalesGuildRaidUpdater.Models
{
    [DataContract]
    public class AuthInfo
    {
        [DataMember]
        public string RefreshToken { get; set; }

        [DataMember]
        public string AccessToken { get; set; }

        [DataMember]
        public int ExpiresIn { get; set; }

        [DataMember]
        public string Scope { get; set; }

        [DataMember]
        public string TokenType { get; set; }
    }
}
