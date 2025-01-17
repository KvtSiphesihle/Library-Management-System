namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsCancelled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "IsCancelled");
        }
    }
}
