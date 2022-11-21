using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Web;
using User_Service.Controllers;
using User_Service.Dtos.PatientDto;
using User_Service.Dtos.PatientGroupDto;
using User_Service.Interfaces;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using User_Service.Services;
using Xunit;

namespace User_Service_Test.ServiceTests.ControllerTest
{
    public  class PatientGroupControllerTest
    {
        private readonly Mock<IOrganisationService> organisationServiceStub = new Mock<IOrganisationService>();
        private readonly Mock<IUserService> userServiceStub = new Mock<IUserService>();

        private readonly Mock<IPatientGroupService> _patientGroupServiceStub = new Mock<IPatientGroupService>();
        private readonly Mock<IHttpContextAccessor> _httpContextStub = new Mock<IHttpContextAccessor>();
        private readonly Mock<IHeaderConfiguration> _headerConfigStub = new Mock<IHeaderConfiguration>();

        private string organisationId = "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba";
        
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<INatsService> _natsService = new();
        private readonly Mock<IUserService> _userService = new();

        public PatientGroupControllerTest()
        {
            var patientGroup = new PatientGroup();

            _unitOfWork.Setup(x => x.PatientGroups.Add(patientGroup)).Returns(patientGroup);
            _unitOfWork.Setup(x => x.Complete()).Returns(0);
        }
        
        /*[Fact]
        public void GetPatientGroupByID_ReturnsPatientGroup2()
        {
            // Arrange
            var patientGroup = new PatientGroup
            {
                Id = "test-id",
                GroupName = "group1",
                Description = "description"
            };

            var organization = new Organisation
            {
                Id = "tenant"
            };
            
            _unitOfWork.Setup(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id)).Returns(patientGroup);
            var patientGroupService = new PatientGroupService(_unitOfWork.Object, _userService.Object);
            var patientGroupController = new PatientGroupController(patientGroupService);
            
            // Act
            var result = patientGroupController.GetPatientGroupById(patientGroup.Id);

            // Assert
            _unitOfWork.Verify(x => x.PatientGroups.GetByIdAndTenant(patientGroup.Id, organization.Id), Times.Once);

        }*/
        
        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetPatientGroupByID_ReturnsPatientGroup()
        {
            // Arrange 
            
            //Mock IHttpContextAccessor
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            var fakeTenantId = organisationId;
            context.Request.Headers["Tenant-ID"] = fakeTenantId;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            
            //Mock HeaderConfiguration
            var mockHeaderConfiguration = new Mock<IHeaderConfiguration>();
            mockHeaderConfiguration
                .Setup(_ => _.GetTenantId(It.IsAny<IHttpContextAccessor>()))
                .Returns(fakeTenantId);

            var expectedPatientGroup = CreateRandomPatientGroup();
            
            _patientGroupServiceStub.Setup(
                service => service.GetPatientGroupByIdandTenant(expectedPatientGroup.Id, fakeTenantId))
                .Returns(expectedPatientGroup);
            
            var patientGroupController = new PatientGroupController(_patientGroupServiceStub.Object, mockHttpContextAccessor.Object, mockHeaderConfiguration.Object);

            // Act           
            var result = patientGroupController.GetPatientGroupById(expectedPatientGroup.Id);

            // Assert
            var actualResult = Assert.IsType<ActionResult<ReadPatientGroupDto>>(result);
            var patientGroup = Assert.IsType<ReadPatientGroupDto>(actualResult.Value);
            Assert.Equal("Ward-A", patientGroup.GroupName);
        }

        //[Fact]
        //// UnitOfWork_StateUnderTest_ExpectedBehaviour
        //public void GetAllCareGiversInPatientGroup_ReturnsAllCaregviers()
        //{
        //    // Arrange 
        //    var expectedCaregivers = CreateRandomCaregivers();
        //    var expectedPatientGroup = CreateRandomPatientGroup();
        //    var expectedOrganisation = CreateRandomOrganisation();
        //    var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, httpContextStub.Object);

        //    patientGroupServiceStub.Setup(service => service.GetAllCaregiversInPatientGroup(expectedPatientGroup.Id, expectedOrganisation.Id))
        //        .Returns(expectedCaregivers);

        //    // Act           
        //    var result = patientGroupController.GetAllCaregiversInPatientGroup(expectedPatientGroup.Id);

        //    // Assert
        //    var actualResult = Assert.IsType<ActionResult<IEnumerable<ReadUserDto>>>(result);
        //    var okResult = Assert.IsType<OkObjectResult>(actualResult.Result);
        //    Assert.Equal(200, okResult.StatusCode);
        //}

        //[Fact]
        //// UnitOfWork_StateUnderTest_ExpectedBehaviour
        //public void PostPatientGroup_WithPatientGroupToCreate_ReturnsCreatedString()
        //{
        //    // Arrange
        //    var expectedPatientGroup = CreateRandomPatientGroup();
        //    var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, httpContextStub.Object);

        //    patientGroupServiceStub.Setup(service => service.Create("Ward-A", "Dementia group", "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba"))
        //        .Returns(expectedPatientGroup);

        //    var patientGroup = new CreatePatientGroupDto()
        //    {
        //        GroupName = "Ward-A",
        //        Description = "Dementia group",
        //    };

        //    // Act
        //    var result = patientGroupController.PostPatientGroup(patientGroup);

        //    // Assert
        //    // convert result
        //    var actualResult = Assert.IsType<ActionResult<ReadPatientGroupDto>>(result);
        //    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actualResult.Result);
        //    var stringResult = Assert.IsType<String>(createdAtActionResult.Value);
        //    Assert.Equal($"{patientGroup.GroupName} Added", stringResult);
        //}



        //// UnitOfWork_StateUnderTest_ExpectedBehaviour
        //[Fact]
        //// UnitOfWork_StateUnderTest_ExpectedBehaviour
        //public void PostUserToPatientGroup_WithUserToCreate_ReturnsLoggerInfo()
        //{
        //    // Arrange
        //    var expectedOrgnaisation = CreateRandomOrganisation();
        //    var expecteduser = CreateRandomUser();
        //    var expectedPatientGroup = CreateRandomPatientGroup();
        //    var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, httpContextStub.Object);

        //    patientGroupServiceStub.Setup(service => service.GetPatientGroupByIdandTenant(expectedPatientGroup.Id, expectedOrgnaisation.Id))
        //        .Returns(expectedPatientGroup);

        //    // Act
        //    var result = patientGroupController.PostUserToPatientGroup(expectedPatientGroup.Id, expecteduser.Id);

        //    // Assert
        //    // ToDO
        //}

        //[Fact]
        //// UnitOfWork_StateUnderTest_ExpectedBehaviour
        //public void GetPatientPatientGroups_ReturnsPatientGroup()
        //{
        //    // Arrange 
        //    var expectedPatientGroup = CreateRandomPatientGroup();
        //    var expectedOrganisation = CreateRandomOrganisation();
        //    var expectedPatient = CreateRandomPatient();
        //    var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, httpContextStub.Object);

        //    patientGroupServiceStub.Setup(service => service.GetForPatient(expectedPatient.Id, expectedOrganisation.Id))
        //        .Returns(expectedPatient.PatientGroups);

        //    // Act           
        //    var result = patientGroupController.GetPatientPatientGroups(expectedPatient.Id);

        //    // Assert
        //    Assert.NotNull(result);
        //}

        //[Fact]
        //// UnitOfWork_StateUnderTest_ExpectedBehaviour
        //public void GetCaregiverPatientGroups_ReturnsPatientGroup()
        //{
        //    // Arrange 
        //    var expectedPatientGroup = CreateRandomPatientGroup();
        //    var expectedOrganisation = CreateRandomOrganisation();
        //    var expectedCaregiver = CreateRandomCaregiver();
        //    var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, httpContextStub.Object);

        //    patientGroupServiceStub.Setup(service => service.GetForPatient(expectedCaregiver.Id, expectedOrganisation.Id))
        //        .Returns(expectedCaregiver.PatientGroups);

        //    // Act           
        //    var result = patientGroupController.GetPatientPatientGroups(expectedCaregiver.Id);

        //    // Assert
        //    Assert.NotNull(result);
        //}

        private User CreateRandomUser()
        {
            return new()
            {
                Id = "123456789",
                FirstName = "John",
                LastName = "Doe",
                Birthdate = DateTime.Now,
                IsActive = true,
                Organisation = CreateRandomOrganisation(),
                Role = "Patient",
                PatientGroups = null
            };
        }

        private User CreateRandomPatient()
        {
            return new()
            {
                Id = "123456789",
                FirstName = "John",
                LastName = "Doe",
                Birthdate = DateTime.Now,
                IsActive = true,
                Organisation = CreateRandomOrganisation(),
                Role = "Patient",
                PatientGroups = new List<PatientGroup>
                {
                    CreateRandomPatientGroup()
                }
            };
        }

        private User CreateRandomCaregiver()
        {
            return new()
            {
                Id = "123456789",
                FirstName = "John",
                LastName = "Doe",
                Birthdate = DateTime.Now,
                IsActive = true,
                Organisation = CreateRandomOrganisation(),
                Role = "Caregiver",
                PatientGroups = new List<PatientGroup>
                {
                    CreateRandomPatientGroup()
                }
            };
        }

        private Organisation CreateRandomOrganisation()
        {
            return new()
            {
                Id = "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba",
                Name = "Fontys medical",
                Users = null,
                PatientGroups = null
            };
        }

        private PatientGroup CreateRandomPatientGroup()
        {
            return new()
            {
                Id = "2345678910",
                GroupName = "Ward-A",
                Description = "Dementia group",
                Users = new List<User>(),
                Organisation = CreateRandomOrganisation()
            };
        }


        private List<User> CreateRandomCaregivers()
        {
            List<User> caregivers = new List<User>();

            var caregiver1 = new User()
            {
                Id = "1234567",
                FirstName = "John",
                LastName = "Doe",
                Birthdate = DateTime.Now,
                IsActive = true,
                Organisation = CreateRandomOrganisation(),
                Role = "Caregiver"
            };

            var caregiver2 = new User()
            {
                Id = "2345678",
                FirstName = "Carian",
                LastName = "Giverian",
                Birthdate = DateTime.Now,
                IsActive = true,
                Organisation = CreateRandomOrganisation(),
                Role = "Caregiver"
            };

            caregivers.Add(caregiver1);
            caregivers.Add(caregiver2);
            return caregivers;
        }

        private List<User> CreateRandomPatients()
        {
            List<User> patients = new List<User>();

            var patient1 = new User()
            {
                Id = "1234567",
                FirstName = "John",
                LastName = "Doe",
                Birthdate = DateTime.Now,
                IsActive = true,
                Organisation = CreateRandomOrganisation(),
                Role = "Patient"
            };

            var patient2 = new User()
            {
                Id = "2345678",
                FirstName = "Carian",
                LastName = "Giverian",
                Birthdate = DateTime.Now,
                IsActive = true,
                Organisation = CreateRandomOrganisation(),
                Role = "Patient"
            };

            patients.Add(patient1);
            patients.Add(patient2);
            return patients;
        }
    }
}
