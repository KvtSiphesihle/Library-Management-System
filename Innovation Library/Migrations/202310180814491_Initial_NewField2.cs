namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_NewField2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LearningContents", "Id", c => c.String());
            AlterColumn("dbo.LearningContents", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.LearningContents", "UserId");
            AddForeignKey("dbo.LearningContents", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LearningContents", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.LearningContents", new[] { "UserId" });
            AlterColumn("dbo.LearningContents", "UserId", c => c.String());
            DropColumn("dbo.LearningContents", "Id");
        }
    }
}
