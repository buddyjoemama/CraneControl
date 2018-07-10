using Common.Configuration;
using Newtonsoft.Json;
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

            IPAddress externalIp = IPAddress.None;
            var discoverer = new NatDiscoverer();
            var device = await discoverer.DiscoverDeviceAsync();

            while (true)
            {
                var ip = await device.GetExternalIPAsync();

                if (externalIp != ip)
                {
                    externalIp = ip;

                   // StorageHelper.StoreSetting(StorageHelper.StorageKeys.RouterPublicIPAddress, ip.ToString());
                   // StorageHelper.StoreSetting(StorageHelper.StorageKeys.ServerPrivateIPAddress, GetLocalIPAddress());

                    var ports = JsonConvert.DeserializeAnonymousType(ConfigurationManager.AppSettings["ServerSettings"],
                        new[]
                        {
                            new
                            {
                                PortName = "",
                                Value = 0
                            }
                        });

                    foreach (var port in ports)
                    {
                        try
                        {
                            await device.DeletePortMapAsync(new Mapping(Protocol.Tcp, port.Value, port.Value));
                        }
                        catch { }

                        try
                        {
                            var mapping = new Mapping(Protocol.Tcp, port.Value, port.Value, "port_mapping");
                            await device.CreatePortMapAsync(mapping);
                        }
                        catch { }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(delay));
            }
        }
    }
}
