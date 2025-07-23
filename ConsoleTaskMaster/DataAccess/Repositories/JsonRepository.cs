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

        public JsonRepository()
        {
            // Uygulamanın ÇALIŞTIĞI ana dizini bul (bin/Debug içi değil!)
            string appRootPath = AppDomain.CurrentDomain.BaseDirectory;

            // Solution/Proje kök dizinine çık (Debug klasöründen kurtul)
            string projectRootPath = Directory.GetParent(appRootPath).Parent.Parent.Parent.FullName;

            _filePath = Path.Combine(projectRootPath, "tasks.json"); // Ana dizinde oluştur
            _tasks = new List<TaskItem>();
            LoadTasksAsync().Wait(); // Verileri yükle
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
                string json = await Task.Run(() => File.ReadAllText(_filePath));
                _tasks = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosya okuma hatası: {ex.Message}");
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
            try
            {
                string json = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions { WriteIndented = true });
                await Task.Run(() => File.WriteAllText(_filePath, json)); // Use synchronous WriteAllText wrapped in Task.Run
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosya yazma hatası: {ex.Message}");
            }
        }
    }
}