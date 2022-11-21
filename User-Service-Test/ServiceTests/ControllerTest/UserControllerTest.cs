using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User_Service.Controllers;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using Xunit;

namespace User_Service_Test.ServiceTests.ControllerTest
{
    public class UserControllerTest
    {
        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetUserByID_WithInexisitingUser_ReturnsNotFound()
        {
            // Arrange
            // this is to create a fake instance of the organisation service class for testing
            var serviceStub = new Mock<IUserService>();

            var organisationServiceStub = new Mock<IOrganisationService>();

            // setup of GetOrganisationById method to return null
            // we want to test the controller takes care of null values

            // whenever the controller invokes GetOrganisationById method create any Guid for its arguement and return a null value
            serviceStub.Setup(service => service.GetUser(It.IsAny<Guid>().ToString(), It.IsAny<Guid>().ToString())).Returns((User)null);

            organisationServiceStub.Setup(service => service.GetOrganisationByID(It.IsAny<Guid>().ToString())).Returns((Organisation)null);


            var controller = new UserController(serviceStub.Object, organisationServiceStub.Object);


            // Act
            var actualResult = controller.GetUsersById(Guid.NewGuid().ToString());

            // Assert
            // actualReult.Result 
            // actual Result.Expected Result
            Assert.IsType<NotFoundResult>(actualResult.Result);
        }
    }
}
