using Microsoft.EntityFrameworkCore;
using NZWalks.Models.Domain;

namespace NZWalks.data
{
    public class NZWalksDbContext : DbContext
    {
        //if we have multiple dbcotext then applicatin will fail by givig type should be same for
        //dbcotext for this we need to rpvide type of dbcontext while injecting in program.cs so that
        //applicatin will check for specific required type like DbContextOptions<NZWalkAuthDbContext>
        public NZWalksDbContext(DbContextOptions<NZWalksDbContext> dbContextOptions): base(dbContextOptions)
        {
            
        }

        public DbSet<Difficulty> Difficulties { get; set; }

        public DbSet<Region> regions { get; set; }

        public DbSet<Walk> walks { get; set; }

        public DbSet<Image> images { get; set; }
    }
}
