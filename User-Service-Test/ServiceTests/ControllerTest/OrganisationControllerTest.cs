using AutoMapper;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using User_Service.Controllers;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using Xunit;

namespace User_Service_Test.ServiceTests.ControllerTest
{
    public class OrganisationControllerTest
    {
        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetOrganisationByID_WithInexisitingOrganisation_ReturnsNotFound()
        {
            // Arrange
            // this is to create a fake instance of the organisation service class for testing
            var serviceStub = new Mock<IOrganisationService>();

            // setup of GetOrganisationById method to return null
            // we want to test the controller takes care of null values

            // whenever the controller invokes GetOrganisationById method create any Guid for its arguement and return a null value
            serviceStub.Setup(service => service.GetOrganisationByID(It.IsAny<Guid>().ToString())).Returns((Organisation)null);

            var loggerStub = new Mock<ILogger<OrganisationController>>();

            var controller = new OrganisationController(serviceStub.Object, loggerStub.Object);


            // Act
            var actualResult = controller.GetOrganisationByID(Guid.NewGuid().ToString());

            // Assert
            // actualReult.Result 
            // actual Result.Expected Result
            Assert.IsType<NotFoundResult>(actualResult.Result);
        }
    }
}