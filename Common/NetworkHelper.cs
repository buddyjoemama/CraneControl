using Open.Nat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class NetworkHelper
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static async Task<String> GetExternalIPAddress()
        {
            IPAddress externalIp = IPAddress.None;
            var discoverer = new NatDiscoverer();
            var device = await discoverer.DiscoverDeviceAsync();

            var res = await device.GetExternalIPAsync();

            return res.ToString();
        }
    }
}
