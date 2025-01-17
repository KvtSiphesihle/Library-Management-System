namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Order_Arrival_Date : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ArrivalDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "ArrivalDate");
        }
    }
}
