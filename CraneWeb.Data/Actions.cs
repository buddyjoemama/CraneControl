using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraneWeb.Data
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
        [Key]
        public int ActionsId { get; set; }

        [MaxLength(250)]
        public String Name { get; set; }

        public int BitPosition { get; set; }

        public ActionSource ActionSource { get; set; }

        public CraneOperations OpCode { get; set; }
    }

    public class ControlboardOperation
    {
        public CraneOperationAction Action { get; set; }
        public CraneOperation Operation { get; set; }
    }
}
