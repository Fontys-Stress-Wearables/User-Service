using User_Service.Interfaces.IRepositories;

namespace User_Service.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DatabaseContext Context;

        public GenericRepository(DatabaseContext context)
        {
            Context = context;
        }

        public T Add(T entity)
        {
            return Context.Set<T>().Add(entity).Entity;
        }

        public IEnumerable<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }

        public T? GetById(string id)
        {
            return Context.Set<T>().Find(id);
        }

        public T Update(T entity)
        {
            return Context.Set<T>().Update(entity).Entity;
        }

        public void Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
        }
    }
}
