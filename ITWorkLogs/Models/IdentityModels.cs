using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace ITWorkLogs.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        // Extend User Identity
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        
        [Required]
        public bool OpenDate { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("MySqlConnection", throwIfV1Schema: false)
        {
            //comment to not drop database..
           //Database.SetInitializer(new MySqlInitializer());
        }

        //Tables
        public DbSet<WorkLogs> workLogs { get; set; }
        public DbSet<Branches> branches { get; set; }
        public DbSet<Sync> sync { get; set; }
        public DbSet<ref_syncerrors> syncerrror { get; set; }
        public DbSet<sysOfflines> sysoffline { get; set; }
        public DbSet<ref_sysOfflines> ref_sysoffline { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}