using System.Linq.Expressions;

namespace WordDocumentEditor.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task SaveChangesAsync();
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
        Task<IEnumerable<T>> SelectAllAsync();
        Task<IEnumerable<T>> SelectAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T?> GetByIdAsync(params object[] keyValues);
    }
}
