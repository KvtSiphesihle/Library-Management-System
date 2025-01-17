namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Refund : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Refunds",
                c => new
                    {
                        RefundID = c.Int(nullable: false, identity: true),
                        AccountHolderName = c.String(),
                        AccountNumber = c.String(),
                        RefundAmount = c.Double(nullable: false),
                        UserID = c.String(),
                    })
                .PrimaryKey(t => t.RefundID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Refunds");
        }
    }
}
