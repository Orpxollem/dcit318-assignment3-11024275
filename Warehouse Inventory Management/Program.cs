using System;
using System.Collections.Generic;

namespace Warehouse_Inventory_Management {

    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }
    }

    
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }
    }


    
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException("An item with this ID exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException("Item not found.");
            return _items[id];
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException("Item not found.");
        }

        public List<T> GetAllItems() => _items.Values.ToList();

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity must be a positive value.");
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException("Item not found.");
            _items[id].Quantity = newQuantity;
        }
    }


    public class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) { } }
    public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) { } }
    public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) { } }

    
    public class WareHouseManager
    {
        private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(111, "Laptop", 5, "Asus", 50));
            _electronics.AddItem(new ElectronicItem(222, "Printer", 10, "Samsung", 150));

            _groceries.AddItem(new GroceryItem(101, "Bread", 40, DateTime.Now.AddDays(10)));
            _groceries.AddItem(new GroceryItem(102, "Cereals", 90, DateTime.Now.AddDays(33)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine($"{item.Id} - Name: {item.Name}, Quantity: {item.Quantity}");
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine("Stock updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine("Item removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
        public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
    }

    
    public class Program
    {
        public static void Main()
        {
            var manager = new WareHouseManager();
            manager.SeedData();

            Console.WriteLine("Grocery items:");
            manager.PrintAllItems(manager.GroceriesRepo);

            Console.WriteLine("\nElectronic items:");
            manager.PrintAllItems(manager.ElectronicsRepo);

            Console.WriteLine("\nAdding a duplicate electronic item:");
            try
            {
                manager.ElectronicsRepo.AddItem(new ElectronicItem(111, "Laptop", 5, "Asus", 50));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.WriteLine("\nRemoving Grocery ID 2:");
            manager.RemoveItemById(manager.GroceriesRepo, 2);

            Console.WriteLine("\nRemoving a non-existent grocery item with _D 405:");
            manager.RemoveItemById(manager.GroceriesRepo, 405);

        }
    }
}