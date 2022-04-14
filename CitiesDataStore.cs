using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public List<CityDto> Cities { get; set; }

        public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public CitiesDataStore()
        {
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id=1,
                    Name="New York City",
                    Description = "The one with the big park.",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "Most visited park in USA"
                        },
                         new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Empire State Building",
                            Description = "A 102-story skycraper located in Manhattan"
                        },
                    }
                },
                new CityDto()
                {
                    Id=2,
                    Name="Antwerp",
                    Description = "The one with the unfinished cathedral.",
                     PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 3,
                            Name = "Antwerp Catedral",
                            Description = "Most visited park in Catedral"
                        },
                         new PointOfInterestDto()
                        {
                            Id = 4,
                            Name = "Antwerp Central station",
                            Description = "The finest example of railway architecture in Belgium"
                        },
                    }
                },
                new CityDto()
                {
                    Id=3,
                    Name="Paris",
                    Description = "The one with the big tower.",
                     PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 5,
                            Name = "Eiffel Tower",
                            Description = "Most visited Tower"
                        },
                         new PointOfInterestDto()
                        {
                            Id = 6,
                            Name = "The Louver",
                            Description = "The word's largest museum"
                        },
                    }
                }
            };
        }
    }
}
