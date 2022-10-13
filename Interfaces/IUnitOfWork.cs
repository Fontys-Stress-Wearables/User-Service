namespace User_Service.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IOrganisationRepository Organisations { get; }
        int Complete();
    }
}
