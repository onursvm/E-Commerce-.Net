using E_Commerce.DataAccses.Entities.Properties;
using System.Linq.Expressions;
namespace E_Commerce.DataAccses.Interfaces.Properties
{
    public interface IPropertyRepository
    {
        Task<Property> GetByIdAsync(int id);
        Task<IEnumerable<Property>> GetAllAsync();
        Task<IEnumerable<Property>> GetFilteredAsync(Expression<Func<Property, bool>> filter);
        Task AddAsync(Property entitiy);
        Task UpdateAsync(Property entitiy);
        Task DeleteAsync(int id);
        Task<bool> ExitsAsync(int id);
        Task<bool> CountAsync(Expression<Func<Property, bool>> filter = null);
        Task<IEnumerable<Property>> GetPagedAsync(int pageNumber, int pageSize,
            Expression<Func<Property, bool>> filter = null,
            Func<IQueryable<Property>, IOrderedQueryable<Property>> orderBy = null);
    }
}
