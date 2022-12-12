﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Identity.Web;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using User_Service.Controllers;
using User_Service.Dtos.PatientGroupDto;
using User_Service.Dtos.UserDto;
using User_Service.Interfaces;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using Xunit;

namespace User_Service_Test.ServiceTests.ControllerTest
{
    public  class PatientGroupControllerTest
    {
        // PostUserToPatientGroup, DeletePatientGroup and RemovePatientFromPatientGroup methods not yet tested
        // due to no return and response values

        private readonly Mock<IOrganisationService> organisationServiceStub = new Mock<IOrganisationService>();

        private readonly Mock<IUserService> userServiceStub = new Mock<IUserService>();

        private readonly Mock<IPatientGroupService> patientGroupServiceStub = new Mock<IPatientGroupService>();
        private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        private string organisationId = "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba";


        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetAllPatientsInPatientGroup_ReturnsAllPatients()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            var fakeTenantId = organisationId;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IPatientGroupService
            var expectedPatients = CreateRandomPatients();
            var expectedPatientGroup = CreateRandomPatientGroup();
            var expectedOrganisation = CreateRandomOrganisation();
            patientGroupServiceStub.Setup(service => service.GetAllCaregiversInPatientGroup(expectedPatientGroup.Id, expectedOrganisation.Id))
                .Returns(expectedPatients);

            var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, mockHttpContextAccessor.Object);

            // Act
            var result = patientGroupController.GetAllCaregiversInPatientGroup(expectedPatientGroup.Id);

            // Assert
            var actualResult = Assert.IsType<ActionResult<IEnumerable<ReadUserDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actualResult.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetPatientGroupByID_ReturnsPatientGroup()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            var fakeTenantId = organisationId;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);


            //Mock IPatientGroupService
            var expectedPatientGroup = CreateRandomPatientGroup();
            patientGroupServiceStub.Setup(
                service => service.GetPatientGroupByIdandTenant(expectedPatientGroup.Id, fakeTenantId))
                .Returns(expectedPatientGroup);

            var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, mockHttpContextAccessor.Object);

            // Act           
            var result = patientGroupController.GetPatientGroupById(expectedPatientGroup.Id);

            // Assert
            var actualResult = Assert.IsType<ActionResult<ReadPatientGroupDto>>(result);
            var patientGroup = Assert.IsType<ReadPatientGroupDto>(actualResult.Value);
            Assert.Equal("Ward-A", patientGroup.GroupName);
        }

        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetAllCareGiversInPatientGroup_ReturnsAllCaregviers()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            var fakeTenantId = organisationId;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IPatientGroupService
            var expectedCaregivers = CreateRandomCaregivers();
            var expectedPatientGroup = CreateRandomPatientGroup();
            var expectedOrganisation = CreateRandomOrganisation();
            patientGroupServiceStub.Setup(service => service.GetAllCaregiversInPatientGroup(expectedPatientGroup.Id, expectedOrganisation.Id))
                .Returns(expectedCaregivers);

            var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, mockHttpContextAccessor.Object);

            // Act           
            var result = patientGroupController.GetAllCaregiversInPatientGroup(expectedPatientGroup.Id);

            // Assert
            var actualResult = Assert.IsType<ActionResult<IEnumerable<ReadUserDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actualResult.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void PostPatientGroup_WithPatientGroupToCreate_ReturnsCreatedString()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            var fakeTenantId = organisationId;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IPatientGroupService
            var expectedPatientGroup = CreateRandomPatientGroup();

            patientGroupServiceStub.Setup(service => service.Create("Ward-A", "Dementia group", "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba"))
                .Returns(expectedPatientGroup);

            var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, mockHttpContextAccessor.Object);

            var patientGroup = new CreatePatientGroupDto()
            {
                GroupName = "Ward-A",
                Description = "Dementia group",
            };

            // Act
            var result = patientGroupController.PostPatientGroup(patientGroup);

            // Assert
            // convert result
            var actualResult = Assert.IsType<ActionResult<ReadPatientGroupDto>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actualResult.Result);
            var stringResult = Assert.IsType<String>(createdAtActionResult.Value);
            Assert.Equal($"{patientGroup.GroupName} Added", stringResult);
        }

        //ToDo
        //[Fact]
        //// UnitOfWork_StateUnderTest_ExpectedBehaviour
        //public void PostUserToPatientGroup_WithUserToCreate_ReturnsLoggerInfo()
        //{
        //    // Arrange 
        //    var user = GenerateAuthenticatedUser();

        //    //Mock IHttpContextAccessor
        //    var context = new DefaultHttpContext { User = user };
        //    var fakeTenantId = organisationId;
        //    mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

        //    //Mock IPatientGroupService
        //    var expectedOrgnaisation = CreateRandomOrganisation();
        //    var expecteduser = CreateRandomUser();
        //    var expectedPatientGroup = CreateRandomPatientGroup();
        //    patientGroupServiceStub.Setup(service => service.GetPatientGroupByIdandTenant(expectedPatientGroup.Id, expectedOrgnaisation.Id))
        //        .Returns(expectedPatientGroup);

        //    var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, mockHttpContextAccessor.Object);

        //    // Act
        //    var result = patientGroupController.PostUserToPatientGroup(expectedPatientGroup.Id, expecteduser.Id);

        //    // Assert
        //    // ToDO
        //}

        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetPatientPatientGroups_ReturnsPatientGroup()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            var fakeTenantId = organisationId;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IPatientGroupService
            var expectedPatientGroup = CreateRandomPatientGroup();
            var expectedOrganisation = CreateRandomOrganisation();
            var expectedPatient = CreateRandomPatient();
            patientGroupServiceStub.Setup(service => service.GetForPatient(expectedPatient.Id, expectedOrganisation.Id))
                .Returns(expectedPatient.PatientGroups);

            var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, mockHttpContextAccessor.Object);

            // Act           
            var result = patientGroupController.GetPatientPatientGroups(expectedPatient.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetCaregiverPatientGroups_ReturnsPatientGroup()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            var fakeTenantId = organisationId;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IPatientGroupService
            var expectedPatientGroup = CreateRandomPatientGroup();
            var expectedOrganisation = CreateRandomOrganisation();
            var expectedCaregiver = CreateRandomCaregiver();
            patientGroupServiceStub.Setup(service => service.GetForPatient(expectedCaregiver.Id, expectedOrganisation.Id))
                .Returns(expectedCaregiver.PatientGroups);


            var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, mockHttpContextAccessor.Object);

            // Act           
            var result = patientGroupController.GetPatientPatientGroups(expectedCaregiver.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void UpdatePatientGroup_WithpatientGroupToUpdate_ReturnsUpdatePatientGroup()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            var fakeTenantId = organisationId;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

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

            patientGroupServiceStub.Setup(service => service.GetPatientGroupByIdandTenant(existingPatientGroupId, expectedOrgnaisation.Id))
                .Returns(existingPatientGroup);
            patientGroupServiceStub.Setup(service => service.Update(existingPatientGroupId, patientGroupToUpdate.GroupName, patientGroupToUpdate.Description, expectedOrgnaisation.Id))
                .Returns(updatedUser);

            var patientGroupController = new PatientGroupController(patientGroupServiceStub.Object, mockHttpContextAccessor.Object);

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
                                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                                        new Claim(ClaimTypes.Name, "gunnar@somecompany.com"),
                                        new Claim("http://schemas.microsoft.com/identity/claims/tenantid", organisationId)
                                        // other required and custom claims
                                   }, "TestAuthentication"));
            return user;
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
