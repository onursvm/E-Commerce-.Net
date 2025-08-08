using E_Commerce.Business.AuthServices.Interfaces;
using E_Commerce.Business.Services.IdentityServices.Interfaces;
using E_Commerce.Business.Services.PropertyServices.Interfaces;

namespace E_Commerce.Business.Manager
{
    public interface IServiceManager
    {
        IUserService UserService { get; }
        IRoleService RoleService { get; }
        IPropertyService PropertyService { get; }
        IPropertyStatusService PropertyStatusService { get; }
        IPropertyTypeService PropertyTypeService { get; }
        IPropertyPhotoService PropertyPhotoService { get; }
        IAuthService AuthService { get; }
       

    }
}
