namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Edit_Driver_Id_To_Exchanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Exchanges", "DriverId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Exchanges", "DriverId", c => c.Int(nullable: false));
        }
    }
}
