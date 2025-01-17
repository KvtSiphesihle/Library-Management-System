namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventManagement_Classes__AddIsTerminated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "IsTerminated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "IsTerminated");
        }
    }
}
