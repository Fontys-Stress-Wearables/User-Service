using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using User_Service.Dtos.OrganisationDto;
using User_Service.Dtos.PatientGroupDto;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using User_Service.Services;

namespace User_Service.Controllers
{
    [Route("patientgroups")]
    [ApiController]
    public class PatientGroupController : ControllerBase
    {
        private IPatientGroupService _patientGroupService;

        public PatientGroupController(IPatientGroupService patientGroupService)
        {
            this._patientGroupService = patientGroupService;
        }

        [HttpGet("{id}")]
        public ActionResult<ReadPatientGroupDto> GetPatientGroupById(string patientGroupID)
        {
            //var patientGroup = _patientGroupService.GetPatientGroupByIdandTenant(patientGroupID, HttpContext.User.GetTenantId());
            var patientGroup = _patientGroupService.GetPatientGroupByIdandTenant(patientGroupID, "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba");

            if (patientGroup is null)
            {
                return NotFound();
            }

            return patientGroup.AsPatientGroupDto();
        }

        [HttpPost]
        public ActionResult<ReadPatientGroupDto> PostPatientGroup(CreatePatientGroupDto createPatientGroupDto)
        {
            //var patientGroup = _patientGroupService.Create(createPatientGroupDto.GroupName, createPatientGroupDto.Description, HttpContext.User.GetTenantId()!);

            var patientGroup = _patientGroupService.Create(createPatientGroupDto.GroupName, createPatientGroupDto.Description, "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba");

            // this is the return result which produces the id in the response 
            // i.e https;//localhost:7113/items/985398y3843y43

            // if the item is created its Id will be gotten back
            // https;//localhost:7113/items/{id of created item}
            return CreatedAtAction(nameof(GetPatientGroupById), new { id = patientGroup.Id }, patientGroup.GroupName + "Added");
        }
    }
}
