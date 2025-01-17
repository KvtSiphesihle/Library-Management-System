namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Exchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Exchanges", "IsPickedUp", c => c.Boolean(nullable: false));
            AddColumn("dbo.Exchanges", "IsArrived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Exchanges", "IsArrived");
            DropColumn("dbo.Exchanges", "IsPickedUp");
        }
    }
}
