namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Slide_Table_New_Slide : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Slides", "SlideUrl", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Slides", "SlideUrl", c => c.Int(nullable: false));
        }
    }
}
