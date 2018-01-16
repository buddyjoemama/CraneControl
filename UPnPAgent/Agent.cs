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
            int.TryParse(ConfigurationManager.AppSettings["DelaySeconds"], out int delay);

            IPAddress externalIp = IPAddress.None;
            var discoverer = new NatDiscoverer();
            var device = await discoverer.DiscoverDeviceAsync();

            while (true)
            {
                var ip = await device.GetExternalIPAsync();

                if (externalIp != ip)
                {
                    externalIp = ip;
                    foreach (var portConfig in ConfigurationManager.AppSettings["Ports"].Split(new char[] { ',' }))
                    {
                        var info = portConfig.Split(new char[] { ':' });

                        Protocol protocol = (Protocol)Enum.Parse(typeof(Protocol), info[0]);
                        int port = int.Parse(info[1]);

                        try
                        {
                            var mapping = await device.GetSpecificMappingAsync(protocol, port);
                            if (mapping == null)
                            {
                                mapping = new Mapping(protocol, port, port);
                                await device.CreatePortMapAsync(mapping);
                            }
                        }
                        catch { }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(delay));
            }
        }
    }
}
