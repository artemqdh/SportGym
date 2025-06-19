using System.Linq.Expressions;

namespace SportsGym.Services.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        bool Exists(Expression<Func<T, bool>> predicate);

        bool Any(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FindByIdAsync(int id);
        Task<T?> FindByLoginAsync(string login);
        Task<T?> FindByNameAsync(string name);
    }
}