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
    [Route("patient-groups")]
    public class PatientGroupController : ControllerBase
    {
        private readonly IPatientGroupService _patientGroupService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public PatientGroupController(IPatientGroupService patientGroupService, IHttpContextAccessor httpContextAccessor)
        {
            _patientGroupService = patientGroupService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpGet]
        public ActionResult<IEnumerable<PatientGroup>> GetAllPatientGroups()
        {
            var patientGroups = _patientGroupService.GetAll(httpContextAccessor.HttpContext.User.GetTenantId());

            if (patientGroups is null)
            {
                return NotFound($"No patient groups found for tenantId:'{httpContextAccessor.HttpContext.User.GetTenantId()}'");
            }

            return Ok(patientGroups);
        }

        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpGet("{patientGroupId}")]
        public ActionResult<ReadPatientGroupDto> GetPatientGroupById(string patientGroupId)
        {
            var patientGroup = _patientGroupService.GetPatientGroupByIdAndTenant(patientGroupId, httpContextAccessor.HttpContext.User.GetTenantId());

            if (patientGroup is null)
            {
                return NotFound();
            }

            return patientGroup.AsPatientGroupDto();
        }

        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpGet("{patientGroupId}/patients")]
        public ActionResult<IEnumerable<ReadUserDto>> GetAllPatientsInPatientGroup(string patientGroupId)
        {
            var usersInPatientGroup = _patientGroupService.GetAllPatientsInPatientGroup(patientGroupId, httpContextAccessor.HttpContext.User.GetTenantId());

            if (usersInPatientGroup is null)
            {
                return NotFound();
            }
            var patients = usersInPatientGroup
                .Select(item => item.AsUserDto());
            return Ok(patients);
        }

        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpGet("{patientGroupId}/caregivers")]
        public ActionResult<IEnumerable<ReadUserDto>> GetAllCaregiversInPatientGroup(string patientGroupId)
        {
            var usersInPatientGroup = _patientGroupService.GetAllCaregiversInPatientGroup(patientGroupId, httpContextAccessor.HttpContext.User.GetTenantId());

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
            var patientGroup = _patientGroupService.Create(createPatientGroupDto.GroupName, createPatientGroupDto.Description, httpContextAccessor.HttpContext.User.GetTenantId()!);
            
            return CreatedAtAction(nameof(GetPatientGroupById), new { patientGroupId = patientGroup.Id }, patientGroup);
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpPost("{patientGroupId}/user")]
        public async Task PostUserToPatientGroup(string patientGroupId, [FromBody] string userId )
        {
            await _patientGroupService.AddUserToPatientGroup(patientGroupId, userId, httpContextAccessor.HttpContext.User.GetTenantId()!);
        }


        // PatientGroups a Patient is in
        [Authorize(Roles = "Organization.Admin")]
        [HttpGet("patients/{userId}")]
        public IEnumerable<ReadPatientGroupDto> GetPatientPatientGroups(string userId)
        {
            var groups = _patientGroupService.GetForPatient(userId, httpContextAccessor.HttpContext.User.GetTenantId()!)
                .Select(patientGroup => patientGroup.AsPatientGroupDto()); 

            return groups; 
        }

        // PatientGroups a Caregiver is in
        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpGet("caregivers/{userId}")]
        public IEnumerable<ReadPatientGroupDto> GetCaregiverPatientGroups(string userId)
        {
            var groups = _patientGroupService.GetForCareGivers(userId, httpContextAccessor.HttpContext.User.GetTenantId()!)
                .Select(patientGroup => patientGroup.AsPatientGroupDto());

            return groups;
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpDelete("{id}")]
        public void DeletePatientGroup(string id)
        {
            _patientGroupService.Delete(id, httpContextAccessor.HttpContext.User.GetTenantId()!);
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpPut("{id}")]
        public ReadPatientGroupDto UpdatePatientGroup(string id, [FromBody] UpdatePatientGroupDto patientGroup)
        {
            var updatedGroup = _patientGroupService.Update(id, patientGroup.GroupName, patientGroup.Description, httpContextAccessor.HttpContext.User.GetTenantId()!);

            return updatedGroup.AsPatientGroupDto();
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpDelete("{groupId}/user")]
        public void RemoveUserFromPatientGroup(string groupId, [FromBody] string userId)
        {
            _patientGroupService.RemoveUserFromPatientGroup(groupId, userId, httpContextAccessor.HttpContext.User.GetTenantId()!);
        }
    }
}
