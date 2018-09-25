using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;

namespace YawcamServiceController
{
    class Program
    {
        static void Main(string[] args)
        {
            String path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".yawcam");
            String settingsFile = path + @"\yawcam_settings.xml";

            var root = XElement.Parse(File.ReadAllText(settingsFile));

            int startPort = 8100;
            int camNum = 1;
            List<int> ports = new List<int>();

            foreach(var service in ServiceController.GetServices().Where(s=>s.DisplayName.ToLower().StartsWith("yawcam")))
            {
                var port = root.Descendants().Where(s => s.Attribute("property")?.Value == "http_port")
                    .Elements()
                    .First();

                port.Value = startPort.ToString();

                // auto start 
                var prop = root.Descendants().Where(s => s.Attribute("property")?.Value == "s_http_o").SingleOrDefault();
                if (prop == null)
                {
                    var newProp = new XElement("void", new XAttribute("property", "s_http_o"));
                    newProp.Add(new XElement("boolean", "true"));

                    root.Element("object").Add(newProp);
                }

                // Cam
                var cam = root.Descendants().Where(s => s.Attribute("property")?.Value == "cam").SingleOrDefault();
                if(cam == null)
                {
                    var newCam = new XElement("void", new XAttribute("property", "cam"));
                    newCam.Add(new XElement("int", camNum));

                    root.Element("object").Add(newCam);

                    camNum += 1;
                }
                else
                {
                    cam.Element("int").SetValue(camNum);
                    camNum += 1;
                }

                root.Save(settingsFile);

                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    Console.WriteLine("Stopping service...");

                    while(service.Status != ServiceControllerStatus.Stopped)
                    {
                        service.Refresh();
                        Thread.Sleep(1000);
                    }

                    Console.WriteLine("Service stopped!");
                }

                Console.WriteLine("Starting service...");

                service.Start();
                while (service.Status != ServiceControllerStatus.Running)
                {
                    service.Refresh();
                    Thread.Sleep(1000);
                }

                Console.WriteLine("Service started");

                ports.Add(startPort);

                startPort += 1;                
            }

            Console.WriteLine("Updating ports file");
            File.WriteAllText(@"C:\ports.json", JsonConvert.SerializeObject(ports));
        }
    }
}

// 