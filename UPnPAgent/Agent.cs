using Open.Nat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            IPAddress externalIp = IPAddress.None;

            while (true)
            {
                var discoverer = new NatDiscoverer();
                var device = await discoverer.DiscoverDeviceAsync();
                var ip = await device.GetExternalIPAsync();

                if (externalIp != ip)
                {
                    externalIp = ip;
                    var mapping = await device.GetSpecificMappingAsync(Protocol.Tcp, 88);
                    if (mapping == null)
                    {
                        mapping = new Mapping(Protocol.Tcp, 8180, 88);
                        device.CreatePortMapAsync(mapping).Wait();
                    }
                }

                await Task.Delay(TimeSpan.FromHours(1));
            }
        }
    }
}
