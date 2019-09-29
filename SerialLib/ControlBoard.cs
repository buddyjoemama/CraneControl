using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerialLib
{
    public class ControlBoard
    {
        private static ControlBoard s_Instance = null;
        private SerialPort _port = null;

        private ControlBoard()
        {
            _port = FindControllerComPort();

            Task.Run(() =>
            {
                while(_port.IsOpen)
                {
                    WriteByte(0);
                    Thread.Sleep(1000);
                }
            });
        }

        public static ControlBoard Initialize()
        {
            s_Instance = new ControlBoard();

            return s_Instance;
        }

        ~ControlBoard()
        {
            _port.Close();
        }

        /// <summary>
        /// Write a byte to the current open port. Lock the port before writing
        /// to avoid threading issues.
        /// </summary>
        public void WriteByte(byte data)
        {
            WriteBytes(new byte[] { data });
        }

        public void WriteBytes(byte[] data)
        {
            if (!_port.IsOpen)
                throw new Exception("Port closed. Reinitialize ControlBoard object.");

            lock (s_Instance)
            {
                _port.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Find and open the main COM port.
        /// </summary>
        /// <returns></returns>
        private SerialPort FindControllerComPort()
        {
            foreach (var name in SerialPort.GetPortNames())
            {
                SerialPort port = new SerialPort(name);

                port.ReadTimeout = 500;
                port.Open();

                byte[] inBuffer = new byte[port.BytesToRead];
                port.Read(inBuffer, 0, port.BytesToRead);

                var val = Encoding.Default.GetString(inBuffer);

                if (String.Compare(val, "Welcome", true) == 0)
                {
                    return port;
                }
                else
                {
                    port.Close();
                }
            }

            throw new Exception("Unable to auto-discover COM port.");
        }
    }
}
