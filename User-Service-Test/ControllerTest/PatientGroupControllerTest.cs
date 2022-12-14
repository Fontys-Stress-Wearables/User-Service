using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using User_Service.Controllers;
using User_Service.Dtos.PatientGroupDto;
using User_Service.Dtos.UserDto;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using Xunit;

namespace User_Service_Test.ControllerTest
{
    public  class PatientGroupControllerTest
    {
        private readonly Mock<IPatientGroupService> _patientGroupServiceStub = new();
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();

        private const string OrganisationId = "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba";
        
        [Fact]
        public void GetAllPatientsInPatientGroup_ReturnsAllPatients()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IPatientGroupService
            var expectedPatients = CreateRandomPatients();
            var expectedPatientGroup = CreateRandomPatientGroup();
            var expectedOrganisation = CreateRandomOrganisation();
            _patientGroupServiceStub.Setup(service => service.GetAllCaregiversInPatientGroup(expectedPatientGroup.Id, expectedOrganisation.Id))
                .Returns(expectedPatients);

            var patientGroupController = new PatientGroupController(_patientGroupServiceStub.Object, _mockHttpContextAccessor.Object);

            // Act
            var result = patientGroupController.GetAllCaregiversInPatientGroup(expectedPatientGroup.Id);

            // Assert
            var actualResult = Assert.IsType<ActionResult<IEnumerable<ReadUserDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actualResult.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void GetPatientGroupByID_ReturnsPatientGroup()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            var fakeTenantId = OrganisationId;
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            
            //Mock IPatientGroupService
            var expectedPatientGroup = CreateRandomPatientGroup();
            _patientGroupServiceStub.Setup(
                service => service.GetPatientGroupByIdandTenant(expectedPatientGroup.Id, fakeTenantId))
                .Returns(expectedPatientGroup);

            var patientGroupController = new PatientGroupController(_patientGroupServiceStub.Object, _mockHttpContextAccessor.Object);

            // Act           
            var result = patientGroupController.GetPatientGroupById(expectedPatientGroup.Id);

            // Assert
            var actualResult = Assert.IsType<ActionResult<ReadPatientGroupDto>>(result);
            var patientGroup = Assert.IsType<ReadPatientGroupDto>(actualResult.Value);
            Assert.Equal("Ward-A", patientGroup.GroupName);
        }

        [Fact]
        public void GetAllCareGiversInPatientGroup_ReturnsAllCaregivers()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IPatientGroupService
            var expectedCaregivers = CreateRandomCaregivers();
            var expectedPatientGroup = CreateRandomPatientGroup();
            var expectedOrganisation = CreateRandomOrganisation();
            _patientGroupServiceStub.Setup(service => service.GetAllCaregiversInPatientGroup(expectedPatientGroup.Id, expectedOrganisation.Id))
                .Returns(expectedCaregivers);

            var patientGroupController = new PatientGroupController(_patientGroupServiceStub.Object, _mockHttpContextAccessor.Object);

            // Act           
            var result = patientGroupController.GetAllCaregiversInPatientGroup(expectedPatientGroup.Id);

            // Assert
            var actualResult = Assert.IsType<ActionResult<IEnumerable<ReadUserDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actualResult.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void PostPatientGroup_WithPatientGroupToCreate_ReturnsCreatedString()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IPatientGroupService
            var expectedPatientGroup = CreateRandomPatientGroup();

            _patientGroupServiceStub.Setup(service => service.Create("Ward-A", "Dementia group", "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba"))
                .Returns(expectedPatientGroup);

            var patientGroupController = new PatientGroupController(_patientGroupServiceStub.Object, _mockHttpContextAccessor.Object);

            var patientGroup = new CreatePatientGroupDto()
            {
                GroupName = "Ward-A",
                Description = "Dementia group",
            };

            // Act
            var result = patientGroupController.PostPatientGroup(patientGroup);

            // Assert
            var actualResult = Assert.IsType<ActionResult<ReadPatientGroupDto>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actualResult.Result);
            var stringResult = Assert.IsType<String>(createdAtActionResult.Value);
            Assert.Equal($"{patientGroup.GroupName} Added", stringResult);
        }

        [Fact]
        public void GetPatientPatientGroups_ReturnsPatientGroup()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IPatientGroupService
            var expectedOrganisation = CreateRandomOrganisation();
            var expectedPatient = CreateRandomPatient();
            _patientGroupServiceStub.Setup(service => service.GetForPatient(expectedPatient.Id, expectedOrganisation.Id))
                .Returns(expectedPatient.PatientGroups);

            var patientGroupController = new PatientGroupController(_patientGroupServiceStub.Object, _mockHttpContextAccessor.Object);

            // Act           
            var result = patientGroupController.GetPatientPatientGroups(expectedPatient.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetCaregiverPatientGroups_ReturnsPatientGroup()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IPatientGroupService
            var expectedOrganisation = CreateRandomOrganisation();
            var expectedCaregiver = CreateRandomCaregiver();
            _patientGroupServiceStub.Setup(service => service.GetForPatient(expectedCaregiver.Id, expectedOrganisation.Id))
                .Returns(expectedCaregiver.PatientGroups);


            var patientGroupController = new PatientGroupController(_patientGroupServiceStub.Object, _mockHttpContextAccessor.Object);

            // Act           
            var result = patientGroupController.GetPatientPatientGroups(expectedCaregiver.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdatePatientGroup_WithPatientGroupToUpdate_ReturnsUpdatePatientGroup()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IUserService
            var existingPatientGroup = CreateRandomPatientGroup();
            var expectedOrgnaisation = CreateRandomOrganisation();
            var existingPatientGroupId = existingPatientGroup.Id;

            var patientGroupToUpdate = new UpdatePatientGroupDto()
            {
                GroupName = "Ward-B",
                Description = "Dementia group",
            };

            var updatedUser = UpdatePatientGroup(patientGroupToUpdate.GroupName, patientGroupToUpdate.Description);

            _patientGroupServiceStub.Setup(service => service.GetPatientGroupByIdandTenant(existingPatientGroupId, expectedOrgnaisation.Id))
                .Returns(existingPatientGroup);
            _patientGroupServiceStub.Setup(service => service.Update(existingPatientGroupId, patientGroupToUpdate.GroupName, patientGroupToUpdate.Description, expectedOrgnaisation.Id))
                .Returns(updatedUser);

            var patientGroupController = new PatientGroupController(_patientGroupServiceStub.Object, _mockHttpContextAccessor.Object);

            // Act 
            var result = patientGroupController.UpdatePatientGroup(existingPatientGroupId, patientGroupToUpdate);

            // Assert
            var actualResult = Assert.IsType<ReadPatientGroupDto>(result);
            Assert.Equal(patientGroupToUpdate.GroupName, actualResult.GroupName);
            Assert.Equal(patientGroupToUpdate.Description, actualResult.Description);
        }

        private ClaimsPrincipal GenerateAuthenticatedUser()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                                        new(ClaimTypes.Name, "gunnar@somecompany.com"),
                                        new("http://schemas.microsoft.com/identity/claims/tenantid", OrganisationId)
                                        // other required and custom claims
                                   }, "TestAuthentication"));
            return user;
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

        private PatientGroup UpdatePatientGroup(string? groupName, string? description)
        {
            var existingPatientGroup = CreateRandomPatientGroup();
            return new()
            {
                Id = existingPatientGroup.Id,
                GroupName = groupName,
                Description = description,
                Users = existingPatientGroup.Users,
                Organisation = existingPatientGroup.Organisation,
            };
        }
    }
}
