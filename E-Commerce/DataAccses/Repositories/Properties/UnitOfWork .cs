using E_Commerce.DataAccses.Context;
using E_Commerce.DataAccses.Interfaces;
using E_Commerce.DataAccses.Interfaces.Identity;
using E_Commerce.DataAccses.Interfaces.Properties;
using E_Commerce.DataAccses.Repositories.Identity;
using E_Commerce.DataAccses.Repositories.Properties;

namespace E_Commerce.DataAccses.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CommerceDbContext _context;
        private IUserRepository _users;
        private IRoleRepository _roles;
        private IPropertyRepository _properties;
        private IPropertyTypeRepository _propertyTypes;
        private IPropertyStatusRepository _propertyStatus;
        private IPropertyPhotoRepository _propertyPhotos;

        public UnitOfWork(CommerceDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users =>
            _users ??= new UserRepository(_context);

        public IRoleRepository Roles =>
            _roles ??= new RoleRepository(_context);

        public IPropertyRepository Properties =>
            _properties ??= new PropertyRepository(_context);

        public IPropertyTypeRepository PropertiesType =>
            _propertyTypes ??= new PropertyTypeRepository(_context);

        public IPropertyStatusRepository PropertiesStatus =>
            _propertyStatus ??= new PropertyStatusRepository(_context);

        public IPropertyPhotoRepository PropertyPhotos =>
            _propertyPhotos ??= new PropertyPhotoRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}