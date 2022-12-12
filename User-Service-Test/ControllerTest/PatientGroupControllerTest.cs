using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using User_Service.Controllers;
using User_Service.Dtos.PatientGroupDto;
using User_Service.Interfaces;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using Xunit;

namespace User_Service_Test.ControllerTest
{
    public  class PatientGroupControllerTest
    {
        private readonly Mock<IPatientGroupService> _patientGroupServiceStub = new();

        private string organisationId = "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba";
        
        private readonly Mock<IUnitOfWork> _unitOfWork = new();

        public PatientGroupControllerTest()
        {
            var patientGroup = new PatientGroup();

            _unitOfWork.Setup(x => x.PatientGroups.Add(patientGroup)).Returns(patientGroup);
            _unitOfWork.Setup(x => x.Complete()).Returns(0);
        }

        [Fact]
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
