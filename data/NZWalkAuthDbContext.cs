using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace NZWalks.data
{
    public class NZWalkAuthDbContext : IdentityDbContext
    {
        //if we have multiple dbcotext then applicatin will fail by givig type should be same for
        //dbcotext for this we need to rpvide type of dbcontext while injecting in program.cs so that
        //applicatin will check for specific required type like DbContextOptions<NZWalkAuthDbContext>
        public NZWalkAuthDbContext(DbContextOptions<NZWalkAuthDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
            
        }
        //Override OnModelCreating method
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var readerRoleId = "9f1e8e78-3b07-444f-81eb-d1dbf9cd8fc9";
            var writeRoleId = "a7a50fcd-2fe5-4404-a3e3-8fff66c63aba";

            var role = new List<IdentityRole>
            { 
                new IdentityRole 
                { 
                    Id = readerRoleId,
                    ConcurrencyStamp=readerRoleId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper(),
                },
                  new IdentityRole
                {
                    Id = writeRoleId,
                    ConcurrencyStamp=writeRoleId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper(),
                }

            };

            // this it will seed role to database if it's not there in DB
            builder.Entity<IdentityRole>().HasData(role);
        }


    }
}
