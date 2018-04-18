using Common.Configuration;
using Open.Nat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace UPnPAgent
{
    public partial class Agent : ServiceBase
    {
        public Agent()
        {
            InitializeComponent();
        }

        protected override async void OnStart(string[] args)
        {
            await Run(); 
        }

        protected override void OnStop()
        {
        }

        public async Task Run()
        {
            int.TryParse(ConfigurationManager.AppSettings["DelayMinutes"], out int delay);
            int.TryParse(ConfigurationManager.AppSettings["ServerPublicPort"], out int serverPublicPort);
            int.TryParse(ConfigurationManager.AppSettings["ServerPrivatePort"], out int serverPrivatePort);

            IPAddress externalIp = IPAddress.None;
            var discoverer = new NatDiscoverer();
            var device = await discoverer.DiscoverDeviceAsync();
            

            while (true)
            {
                var ip = await device.GetExternalIPAsync();

                if (externalIp != ip)
                {
                    externalIp = ip;

                    StorageHelper.StoreSetting(StorageHelper.StorageKeys.IPAddress, ip.ToString());
                    StorageHelper.StoreSetting(StorageHelper.StorageKeys.PublicPort, serverPublicPort.ToString());
                    StorageHelper.StoreSetting(StorageHelper.StorageKeys.PrivatePort, serverPrivatePort.ToString());
                    StorageHelper.StoreSetting(StorageHelper.StorageKeys.PrivateIP, GetLocalIPAddress());

                    foreach (var m in await device.GetAllMappingsAsync())
                    {
                        try
                        {
                            await device.DeletePortMapAsync(m);
                        }
                        catch { }
                    }

                    try
                    {
                        var mapping = new Mapping(Protocol.Tcp, serverPrivatePort, serverPublicPort, "testing");
                        await device.CreatePortMapAsync(mapping);
                    }
                    catch { }
                }

                await Task.Delay(TimeSpan.FromMinutes(delay));
            }
        }

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
    }
}
