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
        private static byte s_NorthChip = (byte)CraneActions.Off;
        private static byte s_SouthChip = (byte)CraneActions.Off;
        public static string _com = "COM9";

        public static void Control(CraneActions craneActions, NorthChipActions north, SouthChipActions south, bool magOn = false)
        {
            if (north != NorthChipActions.Deactivate)
                s_NorthChip = (byte)(1 << (int)north);
            else
                s_NorthChip = 0;

            if (south != SouthChipActions.Deactivate)
                s_SouthChip = (byte)(1 << (int)south);
            else
                s_SouthChip = 0;

            if (magOn)
                s_NorthChip |= (byte)(1 << ((int)MagActions.On));

            if (craneActions == CraneActions.Off)
                s_NorthChip = s_SouthChip = 0;

            Write();
        }

        public static void Activate(CraneActions action)
        {
            if (action == CraneActions.Off)
                s_NorthChip = s_SouthChip = 0;

            Write();
        }

        public static String[] EnumeratePorts()
        {
            return SerialPort.GetPortNames();
        }

        private static void Write()
        {
            using (SerialPort port = new SerialPort(_com))
            {
                port.Open();
                port.Write(new byte[] { s_NorthChip, s_SouthChip }, 0, 2);
            }
        }
    }
}
