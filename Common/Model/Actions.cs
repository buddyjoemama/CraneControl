using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
    public enum ActionSource
    {
        NorthChip,
        SouthChip
    };

    public enum CraneOperations
    {
        Off,
        CabCW,
        CabCCW,
        BoomUp,
        BoomDown,
        HookUp,
        HookDown,
        PlatformEast,
        PlatformWest,
        PlatformNorth,
        PlatformSouth,
        Magnet
    };

    public enum CraneOperationAction
    {
        Off,
        On
    };

    public class CraneOperation
    {
        public int ActionsId { get; set; }

        public String Name { get; set; }

        public int BitPosition { get; set; }

        public ActionSource ActionSource { get; set; }

        public CraneOperations OpCode { get; set; }

        [JsonProperty("on")]
        public bool On => false;

        public bool SupportsPulse { get; set; }

        public static CraneOperation GetByOpCode(CraneOperations op)
        {
            return GetAll().Single(s => s.OpCode == op);
        }

        public static List<CraneOperation> GetAll()
        {
            CraneOperation cabCW = new CraneOperation { Name = "Cab CW", ActionSource = ActionSource.NorthChip, BitPosition = 0, OpCode = CraneOperations.CabCW, SupportsPulse = true };
            CraneOperation cabCCW = new CraneOperation { Name = "Cab CCW", ActionSource = ActionSource.NorthChip, BitPosition = 1, OpCode = CraneOperations.CabCCW, SupportsPulse = true };
            CraneOperation boomUp = new CraneOperation { Name = "Boom Up", ActionSource = ActionSource.NorthChip, BitPosition = 2, OpCode = CraneOperations.BoomUp };
            CraneOperation boomDown = new CraneOperation { Name = "BoomDown", ActionSource = ActionSource.NorthChip, BitPosition = 3, OpCode = CraneOperations.BoomDown };

            CraneOperation hookUp = new CraneOperation { Name = "Hook Up", ActionSource = ActionSource.SouthChip, BitPosition = 0, OpCode = CraneOperations.HookUp };
            CraneOperation hookDown = new CraneOperation { Name = "HookDown", ActionSource = ActionSource.SouthChip, BitPosition = 1, OpCode = CraneOperations.HookDown };
            CraneOperation platformEast = new CraneOperation { Name = "Platform East", ActionSource = ActionSource.SouthChip, BitPosition = 2, OpCode = CraneOperations.PlatformEast };
            CraneOperation platformWest = new CraneOperation { Name = "Platform West", ActionSource = ActionSource.SouthChip, BitPosition = 3, OpCode = CraneOperations.PlatformWest };
            CraneOperation platformNorth = new CraneOperation { Name = "Platform North", ActionSource = ActionSource.SouthChip, BitPosition = 4, OpCode = CraneOperations.PlatformNorth };
            CraneOperation platformSouth = new CraneOperation { Name = "Platform South", ActionSource = ActionSource.SouthChip, BitPosition = 5, OpCode = CraneOperations.PlatformSouth };

            CraneOperation magOn = new CraneOperation { Name = "Magnet", ActionSource = ActionSource.SouthChip, BitPosition = 6, OpCode = CraneOperations.Magnet };

            return new List<CraneOperation>
            {
                cabCW,
                cabCCW,
                boomUp,
                boomDown,
                hookUp,
                hookDown,
                platformEast,
                platformWest,
                platformNorth,
                platformSouth,
                magOn
            };
        }
    }

    public class ControlboardOperation
    {
        public CraneOperationAction Action { get; set; }
        public CraneOperation Operation { get; set; }
    }
}
