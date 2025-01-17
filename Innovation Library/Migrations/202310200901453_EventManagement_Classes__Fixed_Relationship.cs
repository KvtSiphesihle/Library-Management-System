namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventManagement_Classes__Fixed_Relationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Participants", "Event_EventID", "dbo.Events");
            DropForeignKey("dbo.Tickets", "Event_EventID", "dbo.Events");
            DropIndex("dbo.Participants", new[] { "Event_EventID" });
            DropIndex("dbo.Tickets", new[] { "Event_EventID" });
            RenameColumn(table: "dbo.Participants", name: "Event_EventID", newName: "EventID");
            RenameColumn(table: "dbo.Tickets", name: "Event_EventID", newName: "EventID");
            AlterColumn("dbo.Participants", "EventID", c => c.Int(nullable: false));
            AlterColumn("dbo.Tickets", "EventID", c => c.Int(nullable: false));
            CreateIndex("dbo.Participants", "EventID");
            CreateIndex("dbo.Tickets", "EventID");
            AddForeignKey("dbo.Participants", "EventID", "dbo.Events", "EventID", cascadeDelete: true);
            AddForeignKey("dbo.Tickets", "EventID", "dbo.Events", "EventID", cascadeDelete: true);
            DropColumn("dbo.Events", "ParticipantID");
            DropColumn("dbo.Events", "TicketID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "TicketID", c => c.Int(nullable: false));
            AddColumn("dbo.Events", "ParticipantID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Tickets", "EventID", "dbo.Events");
            DropForeignKey("dbo.Participants", "EventID", "dbo.Events");
            DropIndex("dbo.Tickets", new[] { "EventID" });
            DropIndex("dbo.Participants", new[] { "EventID" });
            AlterColumn("dbo.Tickets", "EventID", c => c.Int());
            AlterColumn("dbo.Participants", "EventID", c => c.Int());
            RenameColumn(table: "dbo.Tickets", name: "EventID", newName: "Event_EventID");
            RenameColumn(table: "dbo.Participants", name: "EventID", newName: "Event_EventID");
            CreateIndex("dbo.Tickets", "Event_EventID");
            CreateIndex("dbo.Participants", "Event_EventID");
            AddForeignKey("dbo.Tickets", "Event_EventID", "dbo.Events", "EventID");
            AddForeignKey("dbo.Participants", "Event_EventID", "dbo.Events", "EventID");
        }
    }
}
