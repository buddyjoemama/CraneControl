
using Common.Data;
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

    public enum PvtActions
    {
        Up = 9,
        Down = 8,
        Left = 16,
        Right = 10,
        Off = 255
    }

    public static class Driver
    {
        private static Dictionary<ActionSource, byte> s_chipActions = new Dictionary<ActionSource, byte>();
        public static string _com;
        public static string _pvtCom;
        public static object locker = new object();
        public static CraneOperation magOperation;
        static DateTime? lastStartTime = null;

        static Driver()
        {
            s_chipActions.Add(ActionSource.NorthChip, 0);
            s_chipActions.Add(ActionSource.SouthChip, 0);
            _com = "COM7";
            magOperation = CraneOperation.GetByOpCode(CraneOperations.Magnet);

            Task.Run(() =>
            {
                while (true)
                {
                    if(lastStartTime != null && DateTime.UtcNow.Subtract(lastStartTime.Value).Minutes >= 2)
                    {
                        Driver.Off();
                        lastStartTime = null;
                    }

                    Thread.Sleep(1000);
                }
            });
        }

        public static void OperatePVT(PvtActions action)
        {
            using (SerialPort port = new SerialPort("COM12"))
            {
                port.Open();
                port.WriteLine("255");
                port.WriteLine(((int)action).ToString());
            }
        }

        public static void OperateCrane(ControlboardOperation op)
        {
            if (op.Operation.SupportsPulse && _pulseThread == null && op.Action == CraneOperationAction.On)
            {
                _cancelTokenSource = new CancellationTokenSource();
                CancellationToken token = _cancelTokenSource.Token;

                token.Register(() =>
                {
                    _pulseThread = null;
                    return;
                });

                _pulseThread = Task.Run(() =>
                {
                    if (!token.IsCancellationRequested)
                    {
                        while (!token.IsCancellationRequested)
                        {
                            if (op.Action == CraneOperationAction.On)
                                op.Action = CraneOperationAction.Off;
                            else
                                op.Action = CraneOperationAction.On;

                            OperateCrane(new List<ControlboardOperation> { op });
                            Thread.Sleep(5);
                        }
                    }
                }, token);
            }
            else if(op.Operation.SupportsPulse && op.Action == CraneOperationAction.Off)
            {
                _cancelTokenSource.Cancel();
                OperateCrane(new List<ControlboardOperation> { op });
            }
            else
            {
                OperateCrane(new List<ControlboardOperation> { op });
            }
        }

        private static Task _pulseThread = null;
        private static CancellationTokenSource _cancelTokenSource = null;

        public static void OperateCrane(List<ControlboardOperation> operations)
        {
            lastStartTime = DateTime.UtcNow;
            CraneOperation magOp = CraneOperation.GetByOpCode(CraneOperations.Magnet);
            
            foreach(var op in operations)
            {
                if (op.Action == CraneOperationAction.Off)
                {
                    s_chipActions[op.Operation.ActionSource] &= (byte)~(1 << (int)op.Operation.BitPosition);
                    continue;
                }
                else
                {
                    s_chipActions[op.Operation.ActionSource] |= (byte)(1 << (int)op.Operation.BitPosition);
                }
            }

            WriteAll();
        }

        public static void Off()
        {
            s_chipActions[ActionSource.SouthChip] = 0;
            s_chipActions[ActionSource.NorthChip] = 0;
            WriteAll();

            using (SerialPort port = new SerialPort("COM12"))
            {
                port.Open();
                port.WriteLine("255");
            }
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
                        {
                            return name;
                        }
                    }
                }
                catch
                {
                }
            }

            return "COM7";
        }

        public static String FindPvtPort()
        {
            return "";
        }
    }
}
