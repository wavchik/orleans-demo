using System;

namespace Demo.SiloHost.Client.Configuration
{
    public class ConnectionConfig
    {
        public int ConnectionRetriesCount { get; set; }

        public TimeSpan ConnectionRetriesTimeout { get; set; }
    }
}
