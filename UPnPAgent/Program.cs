using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace UPnPAgent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                Agent agent = new Agent();
                agent.Run().Wait();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new Agent()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
//https://azurecraneappfunctions.azurewebsites.net/api/Forwarder