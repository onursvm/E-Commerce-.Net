using E_Commerce.DataAccses.Entities.Properties;

namespace E_Commerce.Business.Services.PropertyServices.Interfaces
{
    public interface IPropertyStatusService
    {
        //Crud
        Task<PropertyStatus>GetBYIdAsync(int id);
        Task<IEnumerable<PropertyStatus>> GetBYIdAsync();
        Task<IEnumerable<PropertyStatus>> GetAllAsync();

        Task<PropertyStatus> CreateAsync(string statusName);
        Task UpdateAsync(int id,string newStatusName);
        Task DeleteAsync(int id);
        
        //Sorgular

        Task<bool>ExistsByNameAsync(string statusName);
        Task<int>GetPropetyCountByStatusAsync(int statusId);

        //logic

        Task<bool> CanDeleteStatusAsync(int id); //kontrol silinebiliyormu

    }
}
