using E_Commerce.Business.Services.IdentityServices.Interfaces;
using E_Commerce.Business.Services.PropertyServices.Interfaces;

namespace E_Commerce.Business.Manager
{
    public class ServiceManager: IServiceManager
    {
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IRoleService> _roleService;
        private readonly Lazy<IPropertyService> _propertyService;
        private readonly Lazy<IPropertyStatusService> _propertyStatusService;
        private readonly Lazy<IPropertyTypeService> _propertyTypeService;
        private readonly Lazy<IPropertyPhotoService> _propertyPhotoService;


        public ServiceManager(
            IUserService userService,
            IRoleService roleService,
            IPropertyService propertyService,
            IPropertyStatusService propertyStatusService,
            IPropertyTypeService propertyTypeService,
            IPropertyPhotoService propertyPhotoService
            )
            
        {
            _userService = new Lazy<IUserService>(() => userService);
            _roleService = new Lazy<IRoleService>(() => roleService);
            _propertyService = new Lazy<IPropertyService>(() => propertyService);
            _propertyStatusService = new Lazy<IPropertyStatusService>(() => propertyStatusService);
            _propertyTypeService = new Lazy<IPropertyTypeService>(() => propertyTypeService);
            _propertyPhotoService = new Lazy<IPropertyPhotoService>(() => propertyPhotoService);
            
        }
        public IUserService UserService => _userService.Value;
        public IRoleService RoleService => _roleService.Value;
        public IPropertyService PropertyService => _propertyService.Value;
        public IPropertyStatusService PropertyStatusService => _propertyStatusService.Value;
        public IPropertyTypeService PropertyTypeService => _propertyTypeService.Value;
        public IPropertyPhotoService PropertyPhotoService => _propertyPhotoService.Value;
        
    }
}
