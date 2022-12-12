using User_Service.Interfaces.IRepositories;

namespace User_Service.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IOrganisationRepository Organisations { get; }
        IUserRepository Users { get; }
        IPatientGroupRepository PatientGroups { get; }
        int Complete();
    }
}
