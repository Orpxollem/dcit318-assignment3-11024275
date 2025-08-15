using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Inventory_SYstem {

    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    public interface IInventoryEntity
    {
        int Id { get; }
    }

    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new();
        private string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll() => new List<T>(_log);

        public void SaveToFile()
        {
            try
            {
                using (var writer = new StreamWriter(_filePath))
                {
                    string json = JsonSerializer.Serialize(_log);
                    writer.Write(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("File not found.");
                    return;
                }

                using (var reader = new StreamReader(_filePath))
                {
                    string json = reader.ReadToEnd();
                    var items = JsonSerializer.Deserialize<List<T>>(json);
                    _log = items ?? new List<T>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
    }


    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(111, "Printer", 20, DateTime.Now));
            _logger.Add(new InventoryItem(221, "Scanner", 5, DateTime.Now));
            _logger.Add(new InventoryItem(301, "Camera", 50, DateTime.Now));
        }

        public void SaveData() => _logger.SaveToFile();

        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, DateAdded: {item.DateAdded}");
            }
        }
    }

    
    public class Program
    {
        public static void Main()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventory.txt");

            var app = new InventoryApp(filePath);

            
            app.SeedSampleData();
            app.SaveData();

            
            var newApp = new InventoryApp(filePath);

            newApp.LoadData();
            newApp.PrintAllItems();
        }
    }

}