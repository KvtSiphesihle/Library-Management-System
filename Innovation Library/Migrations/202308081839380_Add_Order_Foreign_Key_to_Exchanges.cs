namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Order_Foreign_Key_to_Exchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Exchanges", "OrderId", c => c.Int(nullable: false));
            CreateIndex("dbo.Exchanges", "OrderId");
            AddForeignKey("dbo.Exchanges", "OrderId", "dbo.Orders", "OrderId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Exchanges", "OrderId", "dbo.Orders");
            DropIndex("dbo.Exchanges", new[] { "OrderId" });
            DropColumn("dbo.Exchanges", "OrderId");
        }
    }
}
