namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Driver_Id_To_Exchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Exchanges", "DriverId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Exchanges", "DriverId");
        }
    }
}
