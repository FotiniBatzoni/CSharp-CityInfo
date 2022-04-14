
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new City("New York City")
                {
                    Id = 1,
                    Description = "The one with the big park."
                },
                  new City("Antwerp")
                  {
                      Id = 2,
                      Description = "The one with the unfinished cathedral."
                  },
                    new City("Paris")
                    {
                        Id = 3,
                        Description = "The one with the big tower."
                    }
                );

            modelBuilder.Entity<PointOfInterest>().HasData(
           new PointOfInterest("Central Park")
           {
               Id = 1,
               CityId = 1,
               Description = "Most visited park in USA"
           },
             new PointOfInterest("Empire State Building")
             {
                 Id = 2,
                 CityId = 1,
                 Description = "A 102-story skycraper located in Manhattan"
             },
               new PointOfInterest("Cathedral")
               {
                   Id = 3,
                   CityId = 2,
                   Description = "The one with the unfinished cathedral."
               },
               new PointOfInterest("Central station")
               {
                   Id = 4,
                   CityId = 2,
                   Description = "The finest example of railway architecture in Belgium"
               },
                new PointOfInterest("Eiffel Tower")
                {
                    Id = 5,
                    CityId = 3,
                    Description = "Most visited Tower."
                },
               new PointOfInterest("The Louver")
               {
                   Id = 6,
                   CityId = 3,
                   Description = "The word's largest museum"
               }
           );
            base.OnModelCreating(modelBuilder);
        }

     
       
     

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //     optionsBuilder.UseSqlite("connectionstring");
        //     base.OnConfiguring(optionsBuilder);
        // }

    }
}


