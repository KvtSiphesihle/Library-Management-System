namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Exchanges_Add_IsCollected : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Exchanges", "IsCollected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Exchanges", "IsCollected");
        }
    }
}
