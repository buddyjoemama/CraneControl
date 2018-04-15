using CraneWeb.Data;
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
        public static string _com = "COM9";

        static Driver()
        {
            s_chipActions.Add(ActionSource.NorthChip, 0);
            s_chipActions.Add(ActionSource.SouthChip, 0);
        }

        public static void OperateCrane(params ControlboardOperation[] operations)
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

        public static void Off()
        {
            s_chipActions[ActionSource.SouthChip] = 0;
            s_chipActions[ActionSource.NorthChip] = 0;
            WriteAll();
        }

        public static void HardResetBoard()
        {

        }

        public static String[] EnumeratePorts()
        {
            return SerialPort.GetPortNames();
        }

        private static void Write(byte northChip, byte southChip)
        {
            using (SerialPort port = new SerialPort(_com))
            {
                port.Open();
                port.Write(new byte[] { northChip, southChip }, 0, 2);
            }
        }

        private static void WriteAll()
        {
            var actions = s_chipActions.Values.Select(s => s).ToArray();
            Write(actions[0], actions[1]);
        }
    }
}
