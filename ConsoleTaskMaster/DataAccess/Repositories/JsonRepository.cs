using DataAccess.Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class JsonRepository : IRepository
    {
        private readonly string _filePath;
        private List<TaskItem> _tasks;
        private readonly object _lock = new object();

        public JsonRepository(string filePath = "tasks.json")
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            _tasks = new List<TaskItem>();
            LoadTasksAsync().Wait();
        }

        private async Task LoadTasksAsync()
        {
            if (!File.Exists(_filePath))
            {
                _tasks = new List<TaskItem>();
                return;
            }

            try
            {
                string json;
                lock (_lock)
                {
                    json = File.ReadAllText(_filePath);
                }

                _tasks = await Task.Run(() =>
                    JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>());
            }
            catch (Exception)
            {
                _tasks = new List<TaskItem>();
            }
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await Task.FromResult(_tasks.OrderBy(t => t.Id).ToList());
        }

        public async Task<TaskItem> GetTaskByIdAsync(int id)
        {
            return await Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            await Task.Run(() =>
            {
                task.Id = _tasks.Any() ? _tasks.Max(t => t.Id) + 1 : 1;
                task.CreatedAt = DateTime.Now;
                _tasks.Add(task);
            });
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            await Task.Run(() =>
            {
                var existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id);
                if (existingTask != null)
                {
                    existingTask.Title = task.Title;
                    existingTask.Description = task.Description;
                    existingTask.Status = task.Status;
                    existingTask.UpdatedAt = DateTime.Now;
                }
            });
        }

        public async Task DeleteTaskAsync(int id)
        {
            await Task.Run(() =>
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task != null)
                {
                    _tasks.Remove(task);
                }
            });
        }

        public async Task<List<TaskItem>> SearchTasksAsync(string keyword)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return new List<TaskItem>();

                var lowerKeyword = keyword.ToLower();
                return _tasks.Where(t =>
                    t.Title.ToLower().Contains(lowerKeyword) ||
                    t.Description.ToLower().Contains(lowerKeyword)
                ).ToList();
            });
        }

        public async Task SaveChangesAsync()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_tasks, options);

            await Task.Run(() =>
            {
                lock (_lock)
                {
                    File.WriteAllText(_filePath, json);
                }
            });
        }
    }
}