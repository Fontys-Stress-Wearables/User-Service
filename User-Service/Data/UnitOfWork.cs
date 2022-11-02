using User_Service.Interfaces;

namespace User_Service.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
            Organisations = new OrganisationRepository(_context);
            Users = new UserRepository(_context);
        }

        public IOrganisationRepository Organisations { get; }

        public IUserRepository Users { get; }

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
