using E_Commerce.DataAccses.Entities.Properties;
using System.Runtime.Intrinsics.Wasm;

namespace E_Commerce.Business.Services.PropertyServices.Interfaces
{
    public interface IPropertyTypeService
    {
        //Crud

        Task<PropertyType> GetTypeIdAsync(int id);
        Task<IEnumerable<PropertyType>> GetAllAsync();

        Task<PropertyType> CreateAsync(string typeName);
        Task UpdateAsync(int id, string newTypeName);
        Task DeleteAsync(int id);
        //Sorgular
        Task<bool> ExistsByNameAsync(string typeName);
        Task<int> GetPropertyCountByTypeAsync(int typeId);
        Task<IEnumerable<PropertyType>> SearchTypeAsync(string keyword);
        //logic
        Task<bool> IsTypeInUseAsync(int typeId);
        Task<bool>ValidateTypeName(string typeName);
    }
}
