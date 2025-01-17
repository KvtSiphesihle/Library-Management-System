using Innovation_Library.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Innovation_Library.Startup))]
namespace Innovation_Library
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ApplicationDbContext _context = new ApplicationDbContext();

            var Email = "admin@inno.com";
            var Password = "Admin@56";
            var StudentName = "Admin";
            var Contact = "0602265169";

            var user = new ApplicationUser { UserName = Email, Email = Email, StudentName = StudentName, ContactNo = Contact };
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));

            var ExistingUser = UserManager.FindByEmail(Email);

            if (ExistingUser == null)
            {
                var result = UserManager.Create(user, Password);
                if (result.Succeeded)
                {
                    if (!roleManager.RoleExists("Admin"))
                    {
                        var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                        {
                            Name = "Admin"
                        };
                        roleManager.Create(role);
                        UserManager.AddToRole(user.Id, "Admin");
                    }
                    else
                    {
                        UserManager.AddToRole(user.Id, "Admin");
                    }
                }
            }
            ConfigureAuth(app);
        }
    }
}
