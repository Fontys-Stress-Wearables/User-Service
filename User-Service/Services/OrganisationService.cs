using System.Runtime.CompilerServices;
using User_Service.Exceptions;
using User_Service.Interfaces;
using User_Service.Interfaces.IServices;
using User_Service.Models;

namespace User_Service.Services;

public class OrganisationService : IOrganisationService
{
    private readonly IUnitOfWork _unitOfWork;
    //private readonly INatsService _natsService;

    // testing sake
    public OrganisationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    //public OrganisationService(IUnitOfWork unitOfWork, INatsService natsService)
    //{
    //    _unitOfWork = unitOfWork;
    //    _natsService = natsService;
    //}


    public IEnumerable<Organisation> GetAll()
    {
        return _unitOfWork.Organisations.GetAll();
    } 

    public Organisation GetOrganisationByID(string id)
    {
        var organization = _unitOfWork.Organisations.GetById(id);

        if (organization == null)
        {
            throw new NotFoundException($"Organization with id '{id}' doesn't exist.");
        }

        return organization;
    }

    public void CreateOrganisation(Organisation organisation)
    {
        _unitOfWork.Organisations.Add(organisation);

        //_natsService.Publish("organization-created", "", organisation);
        //_natsService.Publish("th-logs", "", $"Organization created with ID: '{organisation.Id}.'");

        _unitOfWork.Complete();
    }
    
    public Organisation UpdateOrganisationName(string id, string name)
    {
        var organisation = _unitOfWork.Organisations.GetById(id);

        if (organisation == null)
        {
            throw new NotFoundException($"Organization with id '{id}' doesn't exist.");
        }

        organisation.Name = name;
        _unitOfWork.Organisations.Update(organisation);
        //_natsService.Publish("organization-updated", "", organisation);
        //_natsService.Publish("th-logs", "", $"Organization updated with ID: '{organisation.Id}.'");

        _unitOfWork.Complete();

        return organisation;
    }

    public void RemoveOrganisation(string id)
    {
        var organization = _unitOfWork.Organisations.GetById(id);

        if (organization == null)
        {
            throw new NotFoundException($"Organization with id '{id}' doesn't exist.");
        }

        _unitOfWork.Organisations.Remove(organization);
        //_natsService.Publish("organization-removed", "", organization);
        //_natsService.Publish("th-logs", "", $"Organization removed with ID: '{organization.Id}.'");
        _unitOfWork.Complete();
    }
}
