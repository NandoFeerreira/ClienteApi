
using AutoMapper;
using ClienteApi.Application.Mappings;
using Xunit;

namespace ClienteApi.Tests.Unit.Mappings
{
    public class MappingProfileTests
    {
        [Fact]
        public void Should_Have_Valid_Configuration()
        {
            // Arrange
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            // Assert
            config.AssertConfigurationIsValid();
        }
    }
}
