namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Edit_Comments : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AnnouncementComments", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ContentComments", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.AnnouncementComments", "Id");
            DropColumn("dbo.ContentComments", "Id");
            RenameColumn(table: "dbo.AnnouncementComments", name: "ApplicationUser_Id", newName: "Id");
            RenameColumn(table: "dbo.ContentComments", name: "ApplicationUser_Id", newName: "Id");
            AlterColumn("dbo.AnnouncementComments", "Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.ContentComments", "Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AnnouncementComments", "Id");
            CreateIndex("dbo.ContentComments", "Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ContentComments", new[] { "Id" });
            DropIndex("dbo.AnnouncementComments", new[] { "Id" });
            AlterColumn("dbo.ContentComments", "Id", c => c.Int(nullable: false));
            AlterColumn("dbo.AnnouncementComments", "Id", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.ContentComments", name: "Id", newName: "ApplicationUser_Id");
            RenameColumn(table: "dbo.AnnouncementComments", name: "Id", newName: "ApplicationUser_Id");
            AddColumn("dbo.ContentComments", "Id", c => c.Int(nullable: false));
            AddColumn("dbo.AnnouncementComments", "Id", c => c.Int(nullable: false));
            CreateIndex("dbo.ContentComments", "ApplicationUser_Id");
            CreateIndex("dbo.AnnouncementComments", "ApplicationUser_Id");
        }
    }
}
