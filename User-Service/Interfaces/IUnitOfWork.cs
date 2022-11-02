namespace User_Service.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IOrganisationRepository Organisations { get; }
        IUserRepository Users { get; }
        int Complete();
    }
}
