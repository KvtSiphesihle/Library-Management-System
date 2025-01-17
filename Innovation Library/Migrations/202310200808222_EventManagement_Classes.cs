namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventManagement_Classes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        EventID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        FullDate = c.DateTime(nullable: false),
                        Location = c.String(),
                        NumberOfSeats = c.Int(nullable: false),
                        ParticipantID = c.Int(nullable: false),
                        TicketID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EventID);
            
            CreateTable(
                "dbo.Participants",
                c => new
                    {
                        ParticipantID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        Event_EventID = c.Int(),
                    })
                .PrimaryKey(t => t.ParticipantID)
                .ForeignKey("dbo.Events", t => t.Event_EventID)
                .Index(t => t.Event_EventID);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        TicketID = c.Int(nullable: false, identity: true),
                        Address = c.String(),
                        Time = c.String(),
                        Date = c.String(),
                        TicketNumber = c.String(),
                        UserID = c.String(),
                        IsSoldOut = c.Boolean(nullable: false),
                        Event_EventID = c.Int(),
                    })
                .PrimaryKey(t => t.TicketID)
                .ForeignKey("dbo.Events", t => t.Event_EventID)
                .Index(t => t.Event_EventID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "Event_EventID", "dbo.Events");
            DropForeignKey("dbo.Participants", "Event_EventID", "dbo.Events");
            DropIndex("dbo.Tickets", new[] { "Event_EventID" });
            DropIndex("dbo.Participants", new[] { "Event_EventID" });
            DropTable("dbo.Tickets");
            DropTable("dbo.Participants");
            DropTable("dbo.Events");
        }
    }
}
