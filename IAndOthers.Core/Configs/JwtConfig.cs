namespace IAndOthers.Core.Configs
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public int ExpirationInMinutes { get; set; }
    }
}
