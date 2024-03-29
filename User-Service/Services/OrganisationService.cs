﻿using User_Service.Interfaces;
using User_Service.Interfaces.IServices;
using User_Service.Models;

namespace User_Service.Services;

public class OrganisationService : IOrganisationService
{
    private readonly IUnitOfWork _unitOfWork;
    //private readonly INatsService _natsService;

    public OrganisationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IEnumerable<Organisation> GetAll()
    {
        return _unitOfWork.Organisations.GetAll();
    } 

    public Organisation GetOrganisationByID(string id)
    {
        var organization = _unitOfWork.Organisations.GetById(id);

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

        _unitOfWork.Organisations.Remove(organization);
        //_natsService.Publish("organization-removed", "", organization);
        //_natsService.Publish("th-logs", "", $"Organization removed with ID: '{organization.Id}.'");
        _unitOfWork.Complete();
    }
}
