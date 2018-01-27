﻿using Open.Nat;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerialLib
{
    class Program
    {
        static void Main(string[] args)
        {
            var discoverer = new NatDiscoverer();
            var device = discoverer.DiscoverDeviceAsync().Result;
            var ip = device.GetExternalIPAsync().Result;
            Console.WriteLine("The external IP Address is: {0} ", ip);

            using (SerialPort port = new SerialPort("COM11"))
            {
                port.Open();

                int b = 0;

                while(port.IsOpen)
                {
                    port.Write(new byte[] { (byte)((++b) % 255) }, 0, 1);

                    if (port.BytesToRead > 0)
                    {
                        byte[] buffer = new byte[port.BytesToRead];
                        port.Read(buffer, 0, buffer.Length);

                        foreach (var d in buffer)
                        {
                            Console.WriteLine(d);
                        }
                    }

                    Thread.Sleep(10);
                }
            }
        }
    }
}