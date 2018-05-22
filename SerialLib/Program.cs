using CraneWeb.Data;
using Open.Nat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerialLib
{
    public enum CraneActions
    {
        Off = 0,
        On = 1
    }

    public enum NorthChipActions
    {
        Deactivate = -1,

        CabCW = 0,
        CabCCW = 1,

        BoomUp = 2,
        BoomDown = 3
    }

    public enum SouthChipActions
    {
        Deactivate = -1,

        HookUp = 0,
        HookDown = 1,

        PlatformEast = 2,
        PlatformWest = 3,

        PlatformNorth = 4,
        PlatformSouth = 5
    }

    public enum MagActions
    {
        Off = 0,
        On = 4
    }

    public static class Driver
    {
        private static Dictionary<ActionSource, byte> s_chipActions = new Dictionary<ActionSource, byte>();
        public static string _com;
        public static object locker = new object();
        public static CraneOperation magOperation;

        static Driver()
        {
            s_chipActions.Add(ActionSource.NorthChip, 0);
            s_chipActions.Add(ActionSource.SouthChip, 0);
            _com = FindControllerComPort();
            magOperation = CraneOperation.GetByOpCode(CraneOperations.Magnet);
        }

        public static void OperateCrane(List<ControlboardOperation> operations)
        {
            foreach(var op in operations)
            {
                if (op.Action == CraneOperationAction.Off)
                {
                    s_chipActions[op.Operation.ActionSource] = 0;
                    continue;
                }
                else
                {
                    s_chipActions[op.Operation.ActionSource] |= (byte)(1 << (int)op.Operation.BitPosition);
                }
            }

            WriteAll();
        }

        public static void OperateCrane(ControlboardOperation op)
        {
            OperateCrane(new List<ControlboardOperation> { op });
        }

        public static void Off()
        {
            s_chipActions[ActionSource.SouthChip] = 0;
            s_chipActions[ActionSource.NorthChip] = 0;
            WriteAll();
        }

        public static void HardResetBoard()
        {

        }

        private static void Write(byte northChip, byte southChip)
        {
            if (_com == null)
                _com = FindControllerComPort();

            lock (locker)
            {
                if (_com == null)
                    _com = FindControllerComPort();

                using (SerialPort port = new SerialPort(_com))
                {
                    port.Open();
                    port.Write(new byte[] { northChip, southChip }, 0, 2);
                }
            }
        }

        private static void WriteAll()
        {
            var actions = s_chipActions.Values.Select(s => s).ToArray();
            Write(actions[0], actions[1]);
        }

        public static bool TestPort(string portName)
        {
            try
            {
                String testPort = SerialPort.GetPortNames().SingleOrDefault(s => portName.Contains(s));
                if (testPort != null)
                {
                    using (SerialPort port = new SerialPort(testPort))
                    {
                        port.ReadTimeout = 2500;
                        port.Open();
                        port.Write(new byte[] { 0, 0 }, 0, 2);

                        byte[] inBuffer = new byte[port.BytesToRead];
                        port.Read(inBuffer, 0, port.BytesToRead);

                        var val = Encoding.Default.GetString(inBuffer);

                        return String.Compare(val, "ok", true) == 0;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public static String FindControllerComPort()
        {
            foreach (var name in SerialPort.GetPortNames())
            {
                try
                {
                    using (SerialPort port = new SerialPort(name))
                    {
                        port.ReadTimeout = 500;
                        port.Open();
                        port.Write(new byte[] { 0, 0 }, 0, 2);

                        Thread.Sleep(250);
                        byte[] inBuffer = new byte[port.BytesToRead];
                        port.Read(inBuffer, 0, port.BytesToRead);

                        var val = Encoding.Default.GetString(inBuffer);

                        if (String.Compare(val, "ok", true) == 0)
                            return name;
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        public static void ActivateMagnet(bool on)
        {
            if (on)
                s_chipActions[ActionSource.NorthChip] |= (byte)(1 << (int)magOperation.BitPosition);
            else
                s_chipActions[ActionSource.NorthChip] &= 15;

            WriteAll();
        }
    }
}
