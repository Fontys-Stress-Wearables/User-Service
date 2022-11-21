using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using User_Service.Dtos.OrganisationDto;
using User_Service.Dtos.PatientDto;
using User_Service.Dtos.PatientGroupDto;
using User_Service.Interfaces;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using User_Service.Services;

namespace User_Service.Controllers
{
    [Authorize]
    [ApiController]
    [Route("patientgroups")]
    public class PatientGroupController : ControllerBase
    {
        private string _tenantId;
        private IPatientGroupService _patientGroupService;
        private IHttpContextAccessor _httpContextAccessor;

        public PatientGroupController(IPatientGroupService patientGroupService, IHttpContextAccessor httpContextAccessor, IHeaderConfiguration headerConfiguration)
        {
            _httpContextAccessor = httpContextAccessor;
            _patientGroupService = patientGroupService;
            
            _tenantId = headerConfiguration.GetTenantId(_httpContextAccessor);

        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpGet("/patientGroups")]
        public ActionResult<IEnumerable<PatientGroup>> GetAllPatientGroups()
        {
            var patientGroups = _patientGroupService.GetAll(_tenantId);
            //var patientGroup = _patientGroupService.GetPatientGroupByIdandTenant(patientGroupID, "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba");

            if (patientGroups is null)
            {
                return NotFound($"No patient groups found for tenantId:'{_tenantId}'");
            }

            return Ok(patientGroups);
        }

        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpGet("{patientGroupID}")]
        public ActionResult<ReadPatientGroupDto> GetPatientGroupById(string patientGroupID)
        {
            var patientGroup = _patientGroupService.GetPatientGroupByIdandTenant(patientGroupID, _tenantId);

            if (patientGroup is null)
            {
                return NotFound();
            }

            return patientGroup.AsPatientGroupDto();
        }

        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpGet("{patientGroupID}/patients")]
        public ActionResult<IEnumerable<ReadUserDto>> GetAllPatientsInPatientGroup(string patientGroupID)
        {
            var usersInPatientGroup = _patientGroupService.GetAllPatientsInPatientGroup(patientGroupID, _httpContextAccessor.HttpContext.User.GetTenantId());
            //var usersInPatientGroup = _patientGroupService.GetAllPatientsInPatientGroup(patientGroupID, "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba");

            if (usersInPatientGroup is null)
            {
                return NotFound();
            }
            var patients = usersInPatientGroup
                .Select(item => item.AsUserDto());
            return Ok(patients);
        }

        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpGet("{patientGroupID}/caregivers")]
        public ActionResult<IEnumerable<ReadUserDto>> GetAllCaregiversInPatientGroup(string patientGroupID)
        {
            var usersInPatientGroup = _patientGroupService.GetAllCaregiversInPatientGroup(patientGroupID, _httpContextAccessor.HttpContext.User.GetTenantId());
            //var usersInPatientGroup = _patientGroupService.GetAllCaregiversInPatientGroup(patientGroupID, "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba");

            if (usersInPatientGroup is null)
            {
                return NotFound();
            }
            var caregivers = usersInPatientGroup
                .Select(item => item.AsUserDto());
            return Ok(caregivers);
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpPost]
        public ActionResult<ReadPatientGroupDto> PostPatientGroup(CreatePatientGroupDto createPatientGroupDto)
        {
            var patientGroup = _patientGroupService.Create(createPatientGroupDto.GroupName, createPatientGroupDto.Description, _httpContextAccessor.HttpContext.User.GetTenantId()!);

            //var patientGroup = _patientGroupService.Create(createPatientGroupDto.GroupName, createPatientGroupDto.Description, "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba");

            // this is the return result which produces the id in the response 
            // i.e https;//localhost:7113/items/985398y3843y43

            // if the item is created its Id will be gotten back
            // https;//localhost:7113/items/{id of created item}
            return CreatedAtAction(nameof(GetPatientGroupById), new { patientGroupID = patientGroup.Id }, $"{patientGroup.GroupName} Added");
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpPost("{patientGroupID}/user")]
        public async Task PostUserToPatientGroup(string patientGroupID, [FromBody] string userId )
        {
            await _patientGroupService.AddUserToPatientGroup(patientGroupID, userId, _httpContextAccessor.HttpContext.User.GetTenantId()!);

            //await _patientGroupService.AddUserToPatientGroup(patientGroupID, userId, "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba");
        }


        // all patient groups a patient is in
        [Authorize(Roles = "Organization.Admin")]
        [HttpGet("patients/{userId}")]
        public IEnumerable<ReadPatientGroupDto> GetPatientPatientGroups(string userId)
        {
            var groups = _patientGroupService.GetForPatient(userId, _httpContextAccessor.HttpContext.User.GetTenantId()!)
                .Select(patientGroup => patientGroup.AsPatientGroupDto()); 

            //var groups = _patientGroupService.GetForPatient(userId, "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba")
            //    .Select(patientGroup => patientGroup.AsPatientGroupDto());
            return groups; 
        }

        // all patient groups a patient is in
        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpGet("caregivers/{userId}")]
        public IEnumerable<ReadPatientGroupDto> GetCaregiverPatientGroups(string userId)
        {
            var groups = _patientGroupService.GetForCareGivers(userId, _httpContextAccessor.HttpContext.User.GetTenantId()!)
                .Select(patientGroup => patientGroup.AsPatientGroupDto());

            //var groups = _patientGroupService.GetForCareGivers(userId, "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba")
            //    .Select(patientGroup => patientGroup.AsPatientGroupDto());
            return groups;
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpDelete("{id}")]
        public void DeletePatientGroup(string id)
        {
            _patientGroupService.Delete(id, _httpContextAccessor.HttpContext.User.GetTenantId()!);

            //_patientGroupService.Delete(id, "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba");
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpPut("{id}")]
        public ReadPatientGroupDto UpdatePatientGroup(string id, [FromBody] UpdatePatientGroupDto patientGroup)
        {
            var updatedGroup = _patientGroupService.Update(id, patientGroup.GroupName, patientGroup.Description, _httpContextAccessor.HttpContext.User.GetTenantId()!);

            //var updatedGroup = _patientGroupService.Update(id, patientGroup.GroupName, patientGroup.Description, "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba");

            return updatedGroup.AsPatientGroupDto();
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpDelete("{id}/user")]
        public void RemovePatientFromPatientGroup(string id, [FromBody] string userId)
        {
            _patientGroupService.RemoveUserFromPatientGroup(id, userId, _httpContextAccessor.HttpContext.User.GetTenantId()!);
            //_patientGroupService.RemoveUserFromPatientGroup(id, userId, "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba");
        }

    }
}
