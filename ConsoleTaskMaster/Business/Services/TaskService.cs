using Business.Interfaces;
using DataAccess.Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskItem = Models.TaskItem;
using TaskStatus = Models.TaskStatus;


namespace Business.Services
{
    public class TaskService : ITaskService
    {
        private readonly IRepository _repository;

        public TaskService(IRepository repository)
        {
            _repository = repository;
        }
        
        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await _repository.GetAllTasksAsync();
        }

        public async Task<TaskItem> GetTaskByIdAsync(int id)
        {
            return await _repository.GetTaskByIdAsync(id);
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            task.CreatedAt = DateTime.Now;
            await _repository.AddTaskAsync(task);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateTaskStatusAsync(int id, TaskStatus status)
        {
            var task = await _repository.GetTaskByIdAsync(id);
            if (task != null)
            {
                task.Status = status;
                task.UpdatedAt = DateTime.Now;
                await _repository.UpdateTaskAsync(task);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            await _repository.UpdateTaskAsync(task);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            await _repository.DeleteTaskAsync(id);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<TaskItem>> SearchTasksAsync(string keyword)
        {
            return await _repository.SearchTasksAsync(keyword);
        }
    }
}