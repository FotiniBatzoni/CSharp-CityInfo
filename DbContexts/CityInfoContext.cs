
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        //= null! --- null forgiving operator
        public DbSet<City> Cities { get; set; } = null!;

        public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

        //constructor
        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
        {

        }

       // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       //{
       //     optionsBuilder.UseSqlite("connectionstring");
       //     base.OnConfiguring(optionsBuilder);
       // }

    }
}


