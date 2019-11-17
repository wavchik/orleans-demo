using System;

namespace Demo.Orleans.Client.Configuration
{
    public class ConnectionConfig
    {
        public int ConnectionRetriesCount { get; set; }

        public TimeSpan ConnectionRetriesTimeout { get; set; }
    }
}
