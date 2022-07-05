using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {

        public List<CityDTO> Cities { get; set; }
        public static CitiesDataStore Current { get; } = new CitiesDataStore();


        public CitiesDataStore()
        {
            Cities = new List<CityDTO>()
            {

                new CityDTO()
                {
                    Id = 1,
                    Name = "New York City",
                    Description = "The one with the big park.",
                    PointsOfInterest=new List< PointOfInterestDTO>()

                    {
                        new PointOfInterestDTO()
                        {
                            Id= 1,
                            Name="Central Park",
                            Description ="A 102-story skyscrapes located in MidTown Manhattan"
                        },
                      new PointOfInterestDTO()
                        {
                            Id= 2,
                            Name="Central Park",
                            Description ="A 102-story skyscrapes located in MidTown Manhattan"
                        },

                    }
                },
                new CityDTO()
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "The one with the cathedral that was never really finished.",
                      PointsOfInterest=new List< PointOfInterestDTO>()

                    {
                        new PointOfInterestDTO()
                        {
                            Id= 3,
                            Name="Cathedral Of Our Lady",
                            Description ="A Gothic Cathedral, conceived by architects Jan and Pietre."
                        },
                              new PointOfInterestDTO()
                        {
                            Id= 4,
                            Name="Antwerp Central Station",
                            Description ="The finest example of railway architecture in Belgium."
                        },

                    }
                },
                new CityDTO()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "The one with the big tower.",
                      PointsOfInterest=new List< PointOfInterestDTO>()

                    {
                        new PointOfInterestDTO()
                        {
                            Id= 5,
                            Name="Eiffel Tower",
                            Description ="A wrought iron lattice tower on the campp de Mars, named after"
                        },
                              new PointOfInterestDTO()
                        {
                            Id= 6,
                            Name="The Louvre",
                            Description ="The world's largest museum."
                        },

                    }
                }
            };
        }

    }
}
