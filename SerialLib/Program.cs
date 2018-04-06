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
        PlatformNorth = 1,
        PlatformSouth = 2,
        PlatformEast = 3,
        PlatformWest = 4,

        CabCW = 5,
        CabCCW = 6,

        HookUp = 7,
        HookDown = 8,

        BoomUp = 9,
        BoomDown = 10,

        MagOn = 11
    }

    public static class Driver
    {
        public static void Control(CraneActions action, String com = "COM9")
        {
            byte northChip = 0;
            byte southChip = 0;

            if (action == CraneActions.PlatformNorth)
                southChip = 1 << 4;
            else if (action == CraneActions.PlatformSouth)
                southChip = 1 << 5;
            else if (action == CraneActions.PlatformEast)
                southChip = 1 << 3;
            else if (action == CraneActions.PlatformWest)
                southChip = 1 << 2;
            else if (action == CraneActions.HookUp)
                southChip = 1 << 1;
            else if (action == CraneActions.HookDown)
                southChip = 1;

            if (action == CraneActions.CabCW)
                northChip = 1;
            else if (action == CraneActions.CabCCW)
                northChip = 1 << 1;
            else if (action == CraneActions.BoomUp)
                northChip = 1 << 2;
            else if (action == CraneActions.BoomDown)
                northChip = 1 << 3;

            if (action == CraneActions.MagOn)
                northChip |= 1 << 4;

            using (SerialPort port = new SerialPort(com))
            {
                port.Open();
                port.Write(new byte[] { northChip, southChip }, 0, 2);
            }
        }

        public static String[] EnumeratePorts()
        {
            return SerialPort.GetPortNames();
        }
    }
}
