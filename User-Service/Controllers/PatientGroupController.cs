using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using User_Service.Dtos.PatientGroupDto;
using User_Service.Dtos.UserDto;
using User_Service.Interfaces.IServices;
using User_Service.Models;

namespace User_Service.Controllers
{
    [Authorize]
    [ApiController]
    [Route("patientgroups")]
    public class PatientGroupController : ControllerBase
    {
        private readonly IPatientGroupService _patientGroupService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientGroupController(IPatientGroupService patientGroupService, IHttpContextAccessor httpContextAccessor)
        {
            _patientGroupService = patientGroupService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpGet("/patientGroups")]
        public ActionResult<IEnumerable<PatientGroup>> GetAllPatientGroups()
        {
            var patientGroups = _patientGroupService.GetAll(_httpContextAccessor.HttpContext.User.GetTenantId());
           
            if (patientGroups is null)
            {
                return NotFound($"No patient groups found for tenantId:'{_httpContextAccessor.HttpContext.User.GetTenantId()}'");
            }

            return Ok(patientGroups);
        }

        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpGet("{patientGroupID}")]
        public ActionResult<ReadPatientGroupDto> GetPatientGroupById(string patientGroupID)
        {
            var patientGroup = _patientGroupService.GetPatientGroupByIdandTenant(patientGroupID, _httpContextAccessor.HttpContext.User.GetTenantId());

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
            
            return CreatedAtAction(nameof(GetPatientGroupById), new { patientGroupID = patientGroup.Id }, $"{patientGroup.GroupName} Added");
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpPost("{patientGroupID}/user")]
        public async Task PostUserToPatientGroup(string patientGroupID, [FromBody] string userId )
        {
            await _patientGroupService.AddUserToPatientGroup(patientGroupID, userId, _httpContextAccessor.HttpContext.User.GetTenantId()!);
        }


        // PatientGroups a Patient is in
        [Authorize(Roles = "Organization.Admin")]
        [HttpGet("patients/{userId}")]
        public IEnumerable<ReadPatientGroupDto> GetPatientPatientGroups(string userId)
        {
            var groups = _patientGroupService.GetForPatient(userId, _httpContextAccessor.HttpContext.User.GetTenantId()!)
                .Select(patientGroup => patientGroup.AsPatientGroupDto());
            
            return groups; 
        }

        // PatientGroups a Caregiver is in
        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpGet("caregivers/{userId}")]
        public IEnumerable<ReadPatientGroupDto> GetCaregiverPatientGroups(string userId)
        {
            var groups = _patientGroupService.GetForCareGivers(userId, _httpContextAccessor.HttpContext.User.GetTenantId()!)
                .Select(patientGroup => patientGroup.AsPatientGroupDto());
            
            return groups;
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpDelete("{id}")]
        public void DeletePatientGroup(string id)
        {
            _patientGroupService.Delete(id, _httpContextAccessor.HttpContext.User.GetTenantId()!);
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpPut("{id}")]
        public ReadPatientGroupDto UpdatePatientGroup(string id, [FromBody] UpdatePatientGroupDto patientGroup)
        {
            var updatedGroup = _patientGroupService.Update(id, patientGroup.GroupName, patientGroup.Description, _httpContextAccessor.HttpContext.User.GetTenantId()!);
            
            return updatedGroup.AsPatientGroupDto();
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpDelete("{id}/user")]
        public void RemovePatientFromPatientGroup(string id, [FromBody] string userId)
        {
            _patientGroupService.RemoveUserFromPatientGroup(id, userId, _httpContextAccessor.HttpContext.User.GetTenantId()!);
        }
    }
}
