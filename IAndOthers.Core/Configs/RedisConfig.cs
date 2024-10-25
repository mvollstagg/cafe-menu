namespace IAndOthers.Core.Configs
{
    public class RedisConfig
    {
        public string Host { get; set; }
        public string Password { get; set; }
        public string User { get; set; }
        public bool Ssl { get; set; }
        public string SslHost { get; set; }
        public bool AbortOnConnectFail { get; set; }
    }
}