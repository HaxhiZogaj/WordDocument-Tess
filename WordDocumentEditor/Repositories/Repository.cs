using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WordDocumentEditor.Repositories.Interfaces;

namespace WordDocumentEditor.Repositories
{
    public class Repository<T, TContext> : IRepository<T> where T : class where TContext : DbContext
    {
        protected readonly TContext context;
        protected readonly DbSet<T> dbSet;

        public Repository(TContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        // Add an entity to the DbSet and save changes immediately
        public async Task AddAsync(T entity)
        {
            try
            {
                await dbSet.AddAsync(entity);
                await context.SaveChangesAsync(); // Commit changes immediately
            }
            catch (Exception ex)
            {
                // Handle exception or log
                throw new InvalidOperationException("Error adding entity", ex);
            }
        }

        // Save changes after any update
        public async Task SaveChangesAsync()
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exception or log
                throw new InvalidOperationException("Error saving changes", ex);
            }
        }

        // Update an entity and save changes immediately
        public async Task UpdateAsync(T entity)
        {
            try
            {
                dbSet.Update(entity);
                await context.SaveChangesAsync(); // Commit changes immediately
            }
            catch (Exception ex)
            {
                // Handle exception or log
                throw new InvalidOperationException("Error updating entity", ex);
            }
        }

        // Remove an entity and save changes immediately
        public async Task RemoveAsync(T entity)
        {
            try
            {
                dbSet.Remove(entity);
                await context.SaveChangesAsync(); // Commit changes immediately
            }
            catch (Exception ex)
            {
                // Handle exception or log
                throw new InvalidOperationException("Error removing entity", ex);
            }
        }

        // Select all entities without any filter
        public async Task<IEnumerable<T>> SelectAllAsync()
        {
            try
            {
                return await dbSet.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle exception or log
                throw new InvalidOperationException("Error retrieving all entities", ex);
            }
        }

        // Select all entities with an optional filter
        public async Task<IEnumerable<T>> SelectAllAsync(Expression<Func<T, bool>> filter)
        {
            try
            {
                return filter == null
                    ? await dbSet.AsNoTracking().ToListAsync()
                    : await dbSet.AsNoTracking().Where(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle exception or log
                throw new InvalidOperationException("Error retrieving filtered entities", ex);
            }
        }

        // Get an entity by its primary key
        public async Task<T?> GetByIdAsync(params object[] keyValues)
        {
            try
            {
                if (keyValues == null || keyValues.Length == 0)
                {
                    throw new ArgumentException("Key values must be provided.", nameof(keyValues));
                }

                return await dbSet.FindAsync(keyValues);
            }
            catch (Exception ex)
            {
                // Handle exception or log
                throw new InvalidOperationException("Error retrieving entity by ID", ex);
            }
        }
    }
}
