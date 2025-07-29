namespace OneID.Data.Redis
{
#nullable disable
    public sealed class RedisOptions
    {
        public string EndPoint { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool Ssl { get; set; }
        public int ConnectRetry { get; set; }
        public int ReconnectRetryDelay { get; set; }
        public int ConnectTimeout { get; set; }
        public int SyncTimeout { get; set; }
        public int AsyncTimeout { get; set; }
        public int KeepAlive { get; set; }

    }
}
