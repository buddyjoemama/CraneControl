﻿
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
            magOperation = CraneOperation.GetByOpCode(CraneOperations.Magnet);

            _com = FindControllerComPort();

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
            using (SerialPort port = new SerialPort("COM5"))
            {
                port.Open();
                port.WriteLine("255");
                port.WriteLine(((int)action).ToString());
            }
        }

        public static void OperateCrane(ControlboardOperation op)
        {
            OperateCrane(new List<ControlboardOperation> { op });
        }

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

            OperatePVT(PvtActions.Off);
        }

        public static void Write(byte northChip, byte southChip)
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

        private static String FindControllerComPort()
        {
            foreach (var name in SerialPort.GetPortNames())
            {
                try
                {
                    using (SerialPort port = new SerialPort(name))
                    {
                        port.ReadTimeout = 500;
                        port.Open();

                        byte[] inBuffer = new byte[port.BytesToRead];
                        port.Read(inBuffer, 0, port.BytesToRead);

                        var val = Encoding.Default.GetString(inBuffer);

                        if (String.Compare(val, "Welcome", true) == 0)
                        {
                            return name;
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }

            throw new Exception("Unable to auto-discover COM port.");
        }
    }
}
