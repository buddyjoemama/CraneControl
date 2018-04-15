namespace CraneWeb.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CraneWeb.Data.CraneDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CraneWeb.Data.CraneDbContext context)
        {
            CraneOperation cabCW = new CraneOperation { Name = "Cab CW", ActionSource = ActionSource.NorthChip, BitPosition = 0, OpCode = CraneOperations.CabCW };
            CraneOperation cabCCW = new CraneOperation { Name = "Cab CCW", ActionSource = ActionSource.NorthChip, BitPosition = 1, OpCode = CraneOperations.CabCCW };
            CraneOperation boomUp = new CraneOperation { Name = "Boom Up", ActionSource = ActionSource.NorthChip, BitPosition = 2, OpCode = CraneOperations.BoomUp };
            CraneOperation boomDown = new CraneOperation { Name = "BoomDown", ActionSource = ActionSource.NorthChip, BitPosition = 3, OpCode = CraneOperations.BoomDown };

            CraneOperation hookUp = new CraneOperation { Name = "Hook Up", ActionSource = ActionSource.SouthChip, BitPosition = 0, OpCode = CraneOperations.HookUp };
            CraneOperation hookDown = new CraneOperation { Name = "HookDown", ActionSource = ActionSource.SouthChip, BitPosition = 1,  OpCode = CraneOperations.HookDown };
            CraneOperation platformEast = new CraneOperation { Name = "Platform East", ActionSource = ActionSource.SouthChip, BitPosition = 2, OpCode = CraneOperations.PlatformEast };
            CraneOperation platformWest = new CraneOperation { Name = "Platform West", ActionSource = ActionSource.SouthChip, BitPosition = 3, OpCode = CraneOperations.PlatformWest };
            CraneOperation platformNorth = new CraneOperation { Name = "Platform North", ActionSource = ActionSource.SouthChip, BitPosition = 4, OpCode = CraneOperations.PlatformNorth };
            CraneOperation platformSouth = new CraneOperation { Name = "Platform South", ActionSource = ActionSource.SouthChip, BitPosition = 5,  OpCode = CraneOperations.PlatformSouth };

            CraneOperation magOn = new CraneOperation { Name = "Magnet", ActionSource = ActionSource.NorthChip, BitPosition = 4, OpCode = CraneOperations.Magnet };

            context.CraneOperations.AddOrUpdate(s => s.OpCode, cabCW);
            context.CraneOperations.AddOrUpdate(s => s.OpCode, cabCCW);
            context.CraneOperations.AddOrUpdate(s => s.OpCode, boomUp);
            context.CraneOperations.AddOrUpdate(s => s.OpCode, boomDown);
            context.CraneOperations.AddOrUpdate(s => s.OpCode, hookUp);
            context.CraneOperations.AddOrUpdate(s => s.OpCode, hookDown);
            context.CraneOperations.AddOrUpdate(s => s.OpCode, platformEast);
            context.CraneOperations.AddOrUpdate(s => s.OpCode, platformWest);
            context.CraneOperations.AddOrUpdate(s => s.OpCode, platformNorth);
            context.CraneOperations.AddOrUpdate(s => s.OpCode, platformSouth);
            context.CraneOperations.AddOrUpdate(s => s.OpCode, magOn);

            context.SaveChanges();
        }
    }
}
