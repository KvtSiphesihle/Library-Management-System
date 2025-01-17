namespace Innovation_Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        StudentId = c.String(),
                        Duration = c.Int(nullable: false),
                        StartDate = c.String(),
                        StartTime = c.String(),
                        DelayTime = c.String(),
                        RoomId = c.Int(nullable: false),
                        Status = c.String(),
                        SigningCode = c.String(),
                        IsSessionExpired = c.Boolean(nullable: false),
                        IsSignedIn = c.Boolean(nullable: false),
                        IsSignedOut = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId);
            
            CreateTable(
                "dbo.BookRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Author = c.String(),
                        BookTitle = c.String(nullable: false),
                        PublicationYear = c.String(),
                        ISBN = c.String(),
                        UserId = c.String(),
                        RequestDate = c.String(),
                        Status = c.String(),
                        IsRequestApproved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        BookId = c.Int(nullable: false, identity: true),
                        BookTitle = c.String(nullable: false),
                        BookPrice = c.Double(nullable: false),
                        NoOfPages = c.String(),
                        ShelfNo = c.String(),
                        Row = c.String(),
                        Quantity = c.Int(nullable: false),
                        Publisher = c.String(),
                        PublishedDate = c.String(),
                        Views = c.Int(nullable: false),
                        Author = c.String(),
                        ISBN = c.String(),
                        Category = c.String(),
                        Description = c.String(),
                        Image = c.String(),
                        PurchaseCount = c.Int(nullable: false),
                        ContentType = c.String(),
                    })
                .PrimaryKey(t => t.BookId);
            
            CreateTable(
                "dbo.Borrowers",
                c => new
                    {
                        BorrowerId = c.Int(nullable: false, identity: true),
                        StudentId = c.String(),
                        StudentName = c.String(),
                        Email = c.String(nullable: false),
                        ContactNo = c.String(nullable: false),
                        BorrowedDate = c.DateTime(),
                        ReturnedDate = c.String(),
                        ExpectedReturnDate = c.DateTime(),
                        Status = c.String(),
                        AllocatedBookId = c.Int(nullable: false),
                        Author = c.String(),
                        BookTitle = c.String(),
                        ISBN = c.String(),
                        BillStatus = c.String(),
                        IsPayed = c.Boolean(nullable: false),
                        IsReturned = c.Boolean(nullable: false),
                        IsAccepted = c.Boolean(nullable: false),
                        IsOverDue = c.Boolean(nullable: false),
                        BillAmount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.BorrowerId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.Coapons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CoaponCode = c.String(),
                        IsExpired = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ExpiryDate = c.DateTime(nullable: false),
                        Discount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Drivers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RegistrationNumber = c.String(),
                        EmailAddress = c.String(),
                        Name = c.String(),
                        PhoneNumber = c.String(),
                        AssignedOrders = c.Int(nullable: false),
                        IsAvailable = c.Boolean(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Exchanges",
                c => new
                    {
                        ExchangeId = c.Int(nullable: false, identity: true),
                        ExchangeKey = c.String(),
                        ExchangeTime = c.String(),
                        Status = c.String(),
                        UserId = c.String(),
                        AmountDue = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ExchangeId);
            
            CreateTable(
                "dbo.ReturnItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BookId = c.Int(nullable: false),
                        ExchangeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Exchanges", t => t.ExchangeId, cascadeDelete: true)
                .Index(t => t.BookId)
                .Index(t => t.ExchangeId);
            
            CreateTable(
                "dbo.HireItems",
                c => new
                    {
                        ItemId = c.Int(nullable: false, identity: true),
                        ItemName = c.String(),
                        ItemPrice = c.Double(nullable: false),
                        Image = c.String(),
                        ItemQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ItemId);
            
            CreateTable(
                "dbo.Hirings",
                c => new
                    {
                        HiringId = c.Int(nullable: false, identity: true),
                        StudentId = c.String(),
                        StudentEmail = c.String(),
                        HireDate = c.String(),
                        NoOfDays = c.String(),
                        ExpectedReturn = c.String(),
                        HireTotalAmount = c.Double(nullable: false),
                        ItemId = c.Int(nullable: false),
                        BillStatus = c.String(),
                        ReferenceNo = c.String(),
                        IsPayed = c.Boolean(nullable: false),
                        IsAccepted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.HiringId);
            
            CreateTable(
                "dbo.Inspections",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BillAmount = c.String(),
                        Notes = c.String(),
                        IsReturnedOnTime = c.Boolean(nullable: false),
                        IsOriginalPackaged = c.Boolean(nullable: false),
                        IsWorking = c.Boolean(nullable: false),
                        IsScratched = c.Boolean(nullable: false),
                        IsWaterDamaged = c.Boolean(nullable: false),
                        HiringId = c.Int(nullable: false),
                        UserID = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ItemExchanges",
                c => new
                    {
                        ItemExchangeID = c.Int(nullable: false, identity: true),
                        ItemID = c.Int(nullable: false),
                        ItemName = c.String(),
                        ItemPrice = c.Double(nullable: false),
                        OrderNo = c.String(),
                        ExchangeType = c.String(),
                        ExchangeReason = c.String(),
                        ExchangeOtherReason = c.String(),
                    })
                .PrimaryKey(t => t.ItemExchangeID);
            
            CreateTable(
                "dbo.LibrarySignIns",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StudentEmail = c.String(),
                        Name = c.String(),
                        SignInTime = c.String(),
                        SignOutTime = c.String(),
                        SessionDuration = c.Int(nullable: false),
                        SignInKey = c.String(),
                        BookingID = c.Int(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        OrderDetailsId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                        ProductPrice = c.Double(),
                        Quantity = c.Int(nullable: false),
                        ProductName = c.String(),
                    })
                .PrimaryKey(t => t.OrderDetailsId)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        OrderNo = c.String(),
                        CustomerId = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        City = c.String(),
                        OrderDate = c.String(),
                        Address = c.String(),
                        Total = c.Double(nullable: false),
                        Status = c.String(),
                        isPayed = c.Boolean(nullable: false),
                        IsPicked = c.Boolean(nullable: false),
                        IsOnRoute = c.Boolean(nullable: false),
                        isDelivered = c.Boolean(nullable: false),
                        IsDeliveryConfirmed = c.Boolean(nullable: false),
                        IsAssigned = c.Boolean(nullable: false),
                        AssignedDriverId = c.String(),
                    })
                .PrimaryKey(t => t.OrderId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        RoomId = c.Int(nullable: false, identity: true),
                        RoomName = c.String(),
                        Capacity = c.Int(nullable: false),
                        Status = c.String(),
                        TableDesk = c.Int(nullable: false),
                        Whiteboard = c.Int(nullable: false),
                        Projector = c.Int(nullable: false),
                        isAvailable = c.Boolean(nullable: false),
                        Image = c.String(),
                    })
                .PrimaryKey(t => t.RoomId);
            
            CreateTable(
                "dbo.Shelves",
                c => new
                    {
                        ShelfId = c.Int(nullable: false, identity: true),
                        ShelfNo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ShelfId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        StudentId = c.Int(nullable: false, identity: true),
                        ProfilePic = c.String(),
                        StudentGuid = c.String(),
                        StudentName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        ContactNo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.StudentId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        StudentName = c.String(),
                        ContactNo = c.String(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderDetails", "BookId", "dbo.Books");
            DropForeignKey("dbo.ReturnItems", "ExchangeId", "dbo.Exchanges");
            DropForeignKey("dbo.ReturnItems", "BookId", "dbo.Books");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.OrderDetails", new[] { "BookId" });
            DropIndex("dbo.OrderDetails", new[] { "OrderId" });
            DropIndex("dbo.ReturnItems", new[] { "ExchangeId" });
            DropIndex("dbo.ReturnItems", new[] { "BookId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Students");
            DropTable("dbo.Shelves");
            DropTable("dbo.Rooms");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.LibrarySignIns");
            DropTable("dbo.ItemExchanges");
            DropTable("dbo.Inspections");
            DropTable("dbo.Hirings");
            DropTable("dbo.HireItems");
            DropTable("dbo.ReturnItems");
            DropTable("dbo.Exchanges");
            DropTable("dbo.Drivers");
            DropTable("dbo.Coapons");
            DropTable("dbo.Categories");
            DropTable("dbo.Borrowers");
            DropTable("dbo.Books");
            DropTable("dbo.BookRequests");
            DropTable("dbo.Bookings");
        }
    }
}
