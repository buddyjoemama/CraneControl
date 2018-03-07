using Open.Nat;
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
            using (SerialPort port = new SerialPort("COM5"))
            {
                port.Open();

                int b = 0;

                while (port.IsOpen)
                {
                    port.Write(new byte[] { 1, 1 }, 0, 2);

                    //if (port.BytesToRead > 0)
                    //{
                    //    byte[] buffer = new byte[port.BytesToRead];
                    //    port.Read(buffer, 0, buffer.Length);

                    //    foreach (var d in buffer)
                    //    {
                    //        Console.WriteLine(d);
                    //    }
                    //}

                    Thread.Sleep(10);
                    port.Write(new byte[] { 0, 0 }, 0, 2);
                }
            }
        }
    }
}
