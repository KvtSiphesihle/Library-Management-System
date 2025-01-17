namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Slide_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Slides",
                c => new
                    {
                        SlideID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        SlideTitle = c.String(),
                        SlideUrl = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SlideID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Slides");
        }
    }
}
