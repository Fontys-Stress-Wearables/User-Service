using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using User_Service;
using User_Service.Controllers;
using User_Service.Dtos.OrganisationDto;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using Xunit;

namespace User_Service_Test.ServiceTests.ControllerTest
{
    public class OrganisationControllerTest
    {
        private readonly Mock<IOrganisationService> serviceStub = new Mock<IOrganisationService>();

        private readonly Mock<ILogger<OrganisationController>> loggerStub = new Mock<ILogger<OrganisationController>>();
        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetOrganisationByID_WithUnExisitingOrganisation_ReturnsNotFound()
        {
            // Arrange
            // setup of GetOrganisationById method to return null
            // we want to test the controller takes care of null values

            // whenever the controller invokes GetOrganisationById method create any Guid for its arguement and return a null value
            serviceStub.Setup(service => service.GetOrganisationByID(It.IsAny<Guid>().ToString())).Returns((Organisation)null);

            var controller = new OrganisationController(serviceStub.Object, loggerStub.Object);


            // Act
            var actualResult = controller.GetOrganisationByID(Guid.NewGuid().ToString());

            // Assert
            // actualReult.Result 
            // actual Result.Expected Result
            Assert.IsType<NotFoundObjectResult>(actualResult.Result);
        }

        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetOrganisationByID_WithExisitingOrganisation_ReturnsNotFound()
        {
            // Arrange 
            var expectedOrgnaisation = CreateRandomOrganisation();
            // This is where we set the method we want to test and what it should return 
            // forcing the GetOrganisationById method to return the newly created Organisation
            var controller = new OrganisationController(serviceStub.Object, loggerStub.Object);
            serviceStub.Setup(service => service.GetOrganisationByID("123456789")).Returns(expectedOrgnaisation);

            // Act           
            var result = controller.GetOrganisationByID("123456789");

            // Assert
            // this checks if the values of the actual Result(object) is equal to the expectedOrgnisation values
            var actualResult = Assert.IsType<ActionResult<ReadOrganisationDto>>(result);
            var okResult = Assert.IsType<ReadOrganisationDto>(actualResult.Value);
            Assert.Equal("Tue medical", okResult.Name);
        }

        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void PostOrganisation_WithOrganisationToCreate_ReturnsCreatedItem()
        {
            // Arrange 
            var organisationToCreate = new CreateOrganisationDto()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Fontys medical"
            };

            var controller = new OrganisationController(serviceStub.Object, loggerStub.Object);

            // Act 
            var result = controller.PostOrganisation(organisationToCreate);

            // Assert
            // convert result
            var actualResult = Assert.IsType<ActionResult<ReadOrganisationDto>>(result);
            var okResult = Assert.IsType<CreatedAtActionResult>(actualResult.Result);
            var organisation = Assert.IsType<Organisation>(okResult.Value);

            Assert.Equal("Fontys medical", organisation.Name);
        }

        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void UpdateOrganisation_WithOrganisationToUpdate_ReturnsUpdateItem()
        {
            // Arrange 
            // to update an organisation we need to create an organisation
            string updatedOrganisationName = "Fontys medical ward";
            var existingOrganisation = CreateRandomOrganisation();
            var existingOrganisationId = existingOrganisation.Id;

            var organisationToUpdate = new UpdateOrganisationDto()
            {
                Name = updatedOrganisationName
            };

            var controller = new OrganisationController(serviceStub.Object, loggerStub.Object);
            serviceStub.Setup(service => service.GetOrganisationByID("123456789"))
                .Returns(existingOrganisation);


            serviceStub.Setup(service => service.UpdateOrganisationName(existingOrganisationId, updatedOrganisationName))
                .Returns(UpdateOrganisation(updatedOrganisationName));

            // Act 
            var result = controller.UpdateOrganisation(existingOrganisationId, organisationToUpdate);

            // Assert
            var actualResult = Assert.IsType<ActionResult<ReadOrganisationDto>>(result);
            var organisation = Assert.IsType<ReadOrganisationDto>(actualResult.Value);

            Assert.Equal(updatedOrganisationName, organisation.Name);
        }

        private Organisation CreateRandomOrganisation()
        {
            return new()
            {
                Id = "123456789",
                Name = "Tue medical",
                User = null,
                PatientGroups = null
            };
        }

        private Organisation UpdateOrganisation(string organisationName)
        {
            return new()
            {
                Id = CreateRandomOrganisation().Id,
                Name = organisationName,
                User = CreateRandomOrganisation().User,
                PatientGroups = CreateRandomOrganisation().PatientGroups
            };
        }
    }
}