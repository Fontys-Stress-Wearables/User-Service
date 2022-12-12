using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using User_Service.Controllers;
using User_Service.Dtos.OrganisationDto;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using Xunit;

namespace User_Service_Test.ControllerTest
{
    public class OrganisationControllerTest
    {
        private readonly Mock<IOrganisationService> _serviceStub = new();
        private readonly Mock<ILogger<OrganisationController>> _loggerStub = new();
        
        [Fact]
        public void GetOrganisationByID_WithNonexistentOrganisation_ReturnsNotFound()
        {
            // Arrange
            _serviceStub.Setup(service => service.GetOrganisationByID(It.IsAny<Guid>().ToString())).Returns((Organisation)null);
            var controller = new OrganisationController(_serviceStub.Object, _loggerStub.Object);
            
            // Act
            var actualResult = controller.GetOrganisationByID(Guid.NewGuid().ToString());

            // Assert
            Assert.IsType<NotFoundObjectResult>(actualResult.Result);
        }

        [Fact]
        public void GetOrganisationByID_WithExistingOrganisation_ReturnsOrganisation()
        {
            // Arrange 
            var expectedOrganisation = CreateRandomOrganisation();

            var controller = new OrganisationController(_serviceStub.Object, _loggerStub.Object);
            _serviceStub.Setup(service => service.GetOrganisationByID("123456789")).Returns(expectedOrganisation);

            // Act           
            var result = controller.GetOrganisationByID("123456789");

            // Assert
            var actualResult = Assert.IsType<ActionResult<ReadOrganisationDto>>(result);
            var okResult = Assert.IsType<ReadOrganisationDto>(actualResult.Value);
            Assert.Equal("Tue medical", okResult.Name);
        }
        
        [Fact]
        public void PostOrganisation_WithOrganisationToCreate_ReturnsCreatedItem()
        {
            // Arrange 
            var organisationToCreate = new CreateOrganisationDto()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Fontys medical"
            };

            var controller = new OrganisationController(_serviceStub.Object, _loggerStub.Object);

            // Act 
            var result = controller.PostOrganisation(organisationToCreate);

            // Assert
            var actualResult = Assert.IsType<ActionResult<ReadOrganisationDto>>(result);
            var okResult = Assert.IsType<CreatedAtActionResult>(actualResult.Result);
            var organisation = Assert.IsType<Organisation>(okResult.Value);

            Assert.Equal("Fontys medical", organisation.Name);
        }

        [Fact]
        public void UpdateOrganisation_WithOrganisationToUpdate_ReturnsUpdateItem()
        {
            // Arrange 
            string updatedOrganisationName = "Fontys medical ward";
            var existingOrganisation = CreateRandomOrganisation();
            var existingOrganisationId = existingOrganisation.Id;

            var organisationToUpdate = new UpdateOrganisationDto()
            {
                Name = updatedOrganisationName
            };

            var controller = new OrganisationController(_serviceStub.Object, _loggerStub.Object);
            _serviceStub.Setup(service => service.GetOrganisationByID("123456789"))
                .Returns(existingOrganisation);
            _serviceStub.Setup(service => service.UpdateOrganisationName(existingOrganisationId, updatedOrganisationName))
                .Returns(UpdateOrganisation(updatedOrganisationName));

            // Act 
            var result = controller.UpdateOrganisation(existingOrganisationId, organisationToUpdate);

            // Assert
            var actualResult = Assert.IsType<ActionResult<ReadOrganisationDto>>(result);
            var organisation = Assert.IsType<ReadOrganisationDto>(actualResult.Value);
            Assert.Equal(updatedOrganisationName, organisation.Name);
        }

        [Fact]
        public void RemoveOrganisation_WithOrganisationToDelete_ReturnsOk()
        {
            // Arrange 
            string toBeDeleteOrganisationId = "123456789";
            var existingOrganisation = CreateRandomOrganisation();
            var existingOrganisationId = existingOrganisation.Id;

            var controller = new OrganisationController(_serviceStub.Object, _loggerStub.Object);
            _serviceStub.Setup(service => service.GetOrganisationByID(toBeDeleteOrganisationId))
                .Returns(existingOrganisation);
            
            // Act 
            var result = controller.RemoveOrganisation(existingOrganisationId);

            // Assert
            var actualResult = Assert.IsType<ActionResult<HttpResponse>>(result);
            var httpResponse = Assert.IsType<OkObjectResult>(actualResult.Result);
            Assert.Equal($"Organisation with Id:'{existingOrganisationId}' has been deleted.", httpResponse.Value);
        }
        
        private Organisation CreateRandomOrganisation()
        {
            return new()
            {
                Id = "123456789",
                Name = "Tue medical",
                Users = null,
                PatientGroups = null
            };
        }

        private Organisation UpdateOrganisation(string organisationName)
        {
            return new()
            {
                Id = CreateRandomOrganisation().Id,
                Name = organisationName,
                Users = CreateRandomOrganisation().Users,
                PatientGroups = CreateRandomOrganisation().PatientGroups
            };
        }
    }
}