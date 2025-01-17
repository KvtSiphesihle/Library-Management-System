namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_NewField1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Announcements", "AnnouncementTitle", c => c.String());
            AddColumn("dbo.Announcements", "PostedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.LearningContents", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LearningContents", "Title");
            DropColumn("dbo.Announcements", "PostedDate");
            DropColumn("dbo.Announcements", "AnnouncementTitle");
        }
    }
}
