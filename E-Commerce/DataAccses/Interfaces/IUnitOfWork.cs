using E_Commerce.DataAccses.Interfaces.Identity;
using E_Commerce.DataAccses.Interfaces.Properties;

namespace E_Commerce.DataAccses.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPropertyRepository Properties { get; }
        IPropertyTypeRepository PropertiesType { get; }
        IPropertyStatusRepository PropertiesStatus { get; }
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        Task<int> SaveChangesAsync();
    }
}
