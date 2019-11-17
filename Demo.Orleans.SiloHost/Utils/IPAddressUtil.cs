using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Demo.Orleans.SiloHost
{
    public static class IPAddressUtil
    {
        public static IPAddress GetLocalIPAddress(NetworkInterfaceType type = NetworkInterfaceType.Ethernet) =>
            NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(x => x.NetworkInterfaceType == type && x.OperationalStatus == OperationalStatus.Up)
                .SelectMany(x => x.GetIPProperties().UnicastAddresses)
                .FirstOrDefault(x => x.Address.AddressFamily == AddressFamily.InterNetwork)
                ?.Address;
    }
}
