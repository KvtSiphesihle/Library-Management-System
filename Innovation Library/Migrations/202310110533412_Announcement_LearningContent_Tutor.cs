namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Announcement_LearningContent_Tutor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnnouncementComments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        CommentText = c.String(),
                        AnnouncementId = c.Int(nullable: false),
                        Id = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Announcements", t => t.AnnouncementId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.AnnouncementId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Announcements",
                c => new
                    {
                        AnnouncementId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        AnnouncementText = c.String(),
                    })
                .PrimaryKey(t => t.AnnouncementId);
            
            CreateTable(
                "dbo.ContentComments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        CommentText = c.String(),
                        LearningContentId = c.Int(nullable: false),
                        Id = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.LearningContents", t => t.LearningContentId, cascadeDelete: true)
                .Index(t => t.LearningContentId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.LearningContents",
                c => new
                    {
                        LearningContentId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        YouTubeLink = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.LearningContentId);
            
            CreateTable(
                "dbo.Tutors",
                c => new
                    {
                        TutorId = c.Int(nullable: false, identity: true),
                        RegistrationNumber = c.String(),
                        EmailAddress = c.String(),
                        Name = c.String(),
                        PhoneNumber = c.String(),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.TutorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContentComments", "LearningContentId", "dbo.LearningContents");
            DropForeignKey("dbo.ContentComments", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AnnouncementComments", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AnnouncementComments", "AnnouncementId", "dbo.Announcements");
            DropIndex("dbo.ContentComments", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ContentComments", new[] { "LearningContentId" });
            DropIndex("dbo.AnnouncementComments", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AnnouncementComments", new[] { "AnnouncementId" });
            DropTable("dbo.Tutors");
            DropTable("dbo.LearningContents");
            DropTable("dbo.ContentComments");
            DropTable("dbo.Announcements");
            DropTable("dbo.AnnouncementComments");
        }
    }
}
