using Business.Interfaces;
using Business.Services;
using DataAccess.Repositories;
using Models;
using System;
using System.Threading.Tasks;
using TaskStatus = Models.TaskStatus;

namespace ConsoleTaskMaster
{
    class Program
    {
        private static ITaskService _taskService;

        static async Task Main(string[] args)
        {
            // Dependency Injection (Basit bir şekilde)
            _taskService = new TaskService(new JsonRepository());

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                DisplayMenu();

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        await AddNewTask();
                        break;
                    case "2":
                        await ListAllTasks();
                        break;
                    case "3":
                        await UpdateTaskStatus();
                        break;
                    case "4":
                        await DeleteTask();
                        break;
                    case "5":
                        await SearchTasks();
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("Değişiklikler kaydediliyor...");
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim. Tekrar deneyin.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                    Console.ReadKey();
                }
            }
        }

        static void DisplayMenu()
        {
            Console.WriteLine("========== CONSOLE TASKMASTER ==========");
            Console.WriteLine("[1] Yeni Görev Ekle");
            Console.WriteLine("[2] Tüm Görevleri Listele");
            Console.WriteLine("[3] Görev Durumunu Güncelle");
            Console.WriteLine("[4] Görev Sil");
            Console.WriteLine("[5] Görev Ara");
            Console.WriteLine("[0] Çıkış (Değişiklikleri Kaydet ve Çık)");
            Console.WriteLine("========================================");
            Console.Write("Lütfen yapmak istediğiniz işlemin numarasını giriniz: ");
        }

        static async Task AddNewTask()
        {
            Console.WriteLine("\n--- Yeni Görev Ekle ---");

            Console.Write("Başlık: ");
            var title = Console.ReadLine();

            Console.Write("Açıklama: ");
            var description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Başlık boş olamaz!");
                return;
            }

            var newTask = new TaskItem
            {
                Title = title,
                Description = description,
                Status = TaskStatus.Pending
            };

            await _taskService.AddTaskAsync(newTask);
            Console.WriteLine("Görev başarıyla eklendi!");
        }

        static async Task ListAllTasks()
        {
            Console.WriteLine("\n--- Tüm Görevler ---");

            var tasks = await _taskService.GetAllTasksAsync();
            if (tasks.Count == 0)
            {
                Console.WriteLine("Henüz görev eklenmemiş.");
                return;
            }

            foreach (var task in tasks)
            {
                DisplayTask(task);
            }
        }

        static async Task UpdateTaskStatus()
        {
            Console.WriteLine("\n--- Görev Durumunu Güncelle ---");

            await ListAllTasks();
            Console.Write("\nGüncellemek istediğiniz görevin ID'sini girin: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            Console.WriteLine("\nDurum Seçenekleri:");
            Console.WriteLine("1 - Beklemede");
            Console.WriteLine("2 - Devam Ediyor");
            Console.WriteLine("3 - Tamamlandı");
            Console.Write("Yeni durum (1-3): ");

            var statusInput = Console.ReadLine();
            TaskStatus newStatus;

            switch (statusInput)
            {
                case "1":
                    newStatus = TaskStatus.Pending;
                    break;
                case "2":
                    newStatus = TaskStatus.InProgress;
                    break;
                case "3":
                    newStatus = TaskStatus.Completed;
                    break;
                default:
                    Console.WriteLine("Geçersiz durum seçeneği!");
                    return;
            }

            await _taskService.UpdateTaskStatusAsync(id, newStatus);
            Console.WriteLine("Görev durumu başarıyla güncellendi!");
        }

        static async Task DeleteTask()
        {
            Console.WriteLine("\n--- Görev Sil ---");

            await ListAllTasks();
            Console.Write("\nSilmek istediğiniz görevin ID'sini girin: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            await _taskService.DeleteTaskAsync(id);
            Console.WriteLine("Görev başarıyla silindi!");
        }

        static async Task SearchTasks()
        {
            Console.WriteLine("\n--- Görev Ara ---");
            Console.Write("Aranacak kelimeyi girin (başlık veya açıklamada): ");

            var keyword = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                Console.WriteLine("Arama kelimesi boş olamaz!");
                return;
            }

            var results = await _taskService.SearchTasksAsync(keyword);
            Console.WriteLine($"\nArama sonuçları ('{keyword}'):");

            if (results.Count == 0)
            {
                Console.WriteLine("Sonuç bulunamadı.");
                return;
            }

            foreach (var task in results)
            {
                DisplayTask(task);
            }
        }

        static void DisplayTask(TaskItem task)
        {
            Console.WriteLine($"\nID: {task.Id}");
            Console.WriteLine($"Başlık: {task.Title}");
            Console.WriteLine($"Açıklama: {task.Description}");
            Console.WriteLine($"Durum: {task.Status}");
            Console.WriteLine($"Oluşturulma: {task.CreatedAt:g}");
            if (task.UpdatedAt.HasValue)
            {
                Console.WriteLine($"Güncellenme: {task.UpdatedAt.Value:g}");
            }
            Console.WriteLine("---------------------");
        }
    }
}