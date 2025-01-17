namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_NewField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AnnouncementComments", "CommentedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Announcements", "id", c => c.String());
            AddColumn("dbo.ContentComments", "CommentedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.LearningContents", "SessionCover", c => c.String());
            AddColumn("dbo.LearningContents", "StartTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.LearningContents", "EndTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Announcements", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Announcements", "UserId");
            AddForeignKey("dbo.Announcements", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Announcements", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Announcements", new[] { "UserId" });
            AlterColumn("dbo.Announcements", "UserId", c => c.String());
            DropColumn("dbo.LearningContents", "EndTime");
            DropColumn("dbo.LearningContents", "StartTime");
            DropColumn("dbo.LearningContents", "SessionCover");
            DropColumn("dbo.ContentComments", "CommentedAt");
            DropColumn("dbo.Announcements", "id");
            DropColumn("dbo.AnnouncementComments", "CommentedAt");
        }
    }
}
