using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskStatus = Models.TaskStatus;
namespace Business.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetAllTasksAsync();
        Task<TaskItem> GetTaskByIdAsync(int id);
        Task AddTaskAsync(TaskItem task);
        Task UpdateTaskStatusAsync(int id, TaskStatus status);
        Task UpdateTaskAsync(TaskItem task);
        Task DeleteTaskAsync(int id);
        Task<List<TaskItem>> SearchTasksAsync(string keyword);
    }
}