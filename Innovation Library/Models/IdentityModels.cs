using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Innovation_Library.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string StudentName { get; set; }
        [Required]
        public string ContactNo { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet <BookRequest> BookRequests { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Shelves> Shelves { get; set; }
        public DbSet<Borrower> Borrowers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet <Room> Rooms { get; set; }
        public DbSet <HireItems> HireItems { get; set; }
        public DbSet <Booking> Bookings { get; set; }
        public DbSet <Hiring> Hirings { get; set; }
        public DbSet <LibrarySignIn> LibrarySignIns { get; set; }
        public DbSet <Inspection> Inspections { get; set; }
        public DbSet <Coapon> Coapons { get; set; }
        public DbSet <Driver> Drivers { get; set; }
        public DbSet <ItemExchange> ItemExchanges { get; set; }
        public DbSet <OrderExchange> OrderExchanges { get; set; }
        public DbSet <Exchanges> Exchanges { get; set; }
        public DbSet <ReturnItems> ReturnItems { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet <Tutor> Tutors { get; set; }
        public DbSet <LearningContent> LearningContents { get; set; }
        public DbSet <Announcement> Announcements { get; set; }
        public DbSet <AnnouncementComment> AnnouncementComments { get; set; }
        public DbSet <ContentComment> ContentComments { get; set; }
        public DbSet <Slides> Slidess { get; set; }
        public DbSet <Ticket> Tickets { get; set; }
        public DbSet <Event> Events { get; set; }
        public DbSet <Participant> Participants { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }    
    }
}