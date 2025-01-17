namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Order1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderExchanges",
                c => new
                    {
                        OrderExchangeId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ExchangeType = c.String(),
                        ExchangeReason = c.String(),
                        ExchangeOtherReason = c.String(),
                    })
                .PrimaryKey(t => t.OrderExchangeId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderExchanges", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderExchanges", new[] { "OrderId" });
            DropTable("dbo.OrderExchanges");
        }
    }
}
