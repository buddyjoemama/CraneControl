namespace CraneWeb.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CraneOperations",
                c => new
                    {
                        ActionsId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250),
                        BitPosition = c.Int(nullable: false),
                        ActionSource = c.Int(nullable: false),
                        OpCode = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ActionsId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CraneOperations");
        }
    }
}
