using User_Service.Exceptions;
using User_Service.Interfaces;
using User_Service.Models;

namespace User_Service.Services;

public class OrganisationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INatsService _natsService;

    public OrganisationService(IUnitOfWork unitOfWork, INatsService natsService)
    {
        _unitOfWork = unitOfWork;
        _natsService = natsService;
    }

    public IEnumerable<Organisation> GetAll()
    {
        return _unitOfWork.Organisations.GetAll();
    }

    public Organisation GetOrganization(string id)
    {
        var organization = _unitOfWork.Organisations.GetById(id);

        if (organization == null)
        {
            throw new NotFoundException($"Organization with id '{id}' doesn't exist.");
        }

        return organization;
    }

    public Organisation CreateOrganisation(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BadRequestException("Name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new BadRequestException("Id cannot be empty.");
        }

        var organization = new Organisation()
        {
            Name = name,
            Id = id
        };

        _unitOfWork.Organisations.Add(organization);
        _natsService.Publish("organization-created", "", organization);
        _natsService.Publish("th-logs", "", $"Organization created with ID: '{organization.Id}.'");

        _unitOfWork.Complete();

        return organization;
    }

    public Organisation UpdateOrganizationName(string id, string name)
    {
        var organization = _unitOfWork.Organisations.GetById(id);

        if (organization == null)
        {
            throw new NotFoundException($"Organization with id '{id}' doesn't exist.");
        }

        organization.Name = name;
        _unitOfWork.Organisations.Update(organization);
        _natsService.Publish("organization-updated", "", organization);
        _natsService.Publish("th-logs", "", $"Organization updated with ID: '{organization.Id}.'");

        _unitOfWork.Complete();

        return organization;
    }

    public void RemoveOrganization(string id)
    {
        var organization = _unitOfWork.Organisations.GetById(id);

        if (organization == null)
        {
            throw new NotFoundException($"Organization with id '{id}' doesn't exist.");
        }

        _unitOfWork.Organisations.Remove(organization);
        _natsService.Publish("organization-removed", "", organization);
        _natsService.Publish("th-logs", "", $"Organization removed with ID: '{organization.Id}.'");
        _unitOfWork.Complete();
    }
}
