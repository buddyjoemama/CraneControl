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
            int.TryParse(ConfigurationManager.AppSettings["ServerPrivatePort"], out int serverPrivatePort);
            int.TryParse(ConfigurationManager.AppSettings["ServerPublicPort"], out int serverPublicPort);

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

                    try
                    {
                        var mapping = await device.GetSpecificMappingAsync(Protocol.Tcp, serverPrivatePort);
                        if (mapping == null)
                        {
                            mapping = new Mapping(Protocol.Tcp, serverPrivatePort, serverPublicPort);
                            await device.CreatePortMapAsync(mapping);
                        }
                    }
                    catch { }
                }

                await Task.Delay(TimeSpan.FromMinutes(delay));
            }
        }
    }
}
