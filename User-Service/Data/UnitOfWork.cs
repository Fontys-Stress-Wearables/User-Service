using User_Service.Interfaces;
using User_Service.Interfaces.IRepositories;

namespace User_Service.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public IOrganisationRepository Organisations { get; }
        public IUserRepository Users { get; }
        public IPatientGroupRepository PatientGroups { get; }
        private readonly DatabaseContext _context;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
            Organisations = new OrganisationRepository(_context);
            Users = new UserRepository(_context);
            PatientGroups = new PatientGroupRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }
    }
}
