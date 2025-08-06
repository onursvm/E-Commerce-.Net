using E_Commerce.Business.Services.PropertyServices.Interfaces;
using E_Commerce.DataAccses.Entities.Properties;
using E_Commerce.DataAccses.Interfaces.Properties;

namespace E_Commerce.Business.Services.PropertyServices.Service
{
    public class PropertyStausService : IPropertyStatusService
    {
        private readonly IPropertyStatusRepository _statusRepository;
        public PropertyStausService(IPropertyStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }
        public async Task<bool> CanDeleteStatusAsync(int id)
        {
            var count = await _statusRepository.GetPropertyCountByStatusAsync(id);
            return count == 0;

        }

        public async Task<PropertyStatus> CreateAsync(string statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName))
                throw new ArgumentException("Status name cannot be empty");

            if (await _statusRepository.ExistsByNameAsync(statusName))
                throw new InvalidOperationException($"Status '{statusName}' already exists");

            var newStatus = new PropertyStatus { Name = statusName };
            return await _statusRepository.AddAsync(newStatus);
        }

        public async Task DeleteAsync(int id)
        {
            var status = await _statusRepository.GetByIdAsync(id);
            if (status == null)
                throw new KeyNotFoundException($"Status with ID {id} not found");

            if (!await CanDeleteStatusAsync(id))
                throw new InvalidOperationException("Cannot delete status in use");

            await _statusRepository.DeleteAsync(status);
        }

        public async Task<bool> ExistsByNameAsync(string statusName)
        {
            return await _statusRepository.ExistsByNameAsync(statusName);
        }

        public async Task<PropertyStatus> GetBYIdAsync(int id)
        {
            var status = await _statusRepository.GetByIdAsync(id);
            return status ?? throw new KeyNotFoundException($"Status with ID {id} not found");
        }

        public async Task<IEnumerable<PropertyStatus>> GetBYIdAsync()
        {
            return await _statusRepository.GetAllAsync();
        }

        public async Task<int> GetPropetyCountByStatusAsync(int statusId)
        {
            return await _statusRepository.GetPropertyCountByStatusAsync(statusId);
        }

        public async Task UpdateAsync(int id, string newStatusName)
        {
            if (string.IsNullOrWhiteSpace(newStatusName))
                throw new ArgumentException("Status name cannot be empty");

            var status = await _statusRepository.GetByIdAsync(id);
            if (status == null)
                throw new KeyNotFoundException($"Status with ID {id} not found");

            if (await _statusRepository.ExistsByNameAsync(newStatusName))
                throw new InvalidOperationException($"Status '{newStatusName}' already exists");

            status.Name = newStatusName;
            await _statusRepository.UpdateAsync(status);
        }
    }
}
