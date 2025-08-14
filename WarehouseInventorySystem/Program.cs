using System;
using System.Collections.Generic;

// Marker interface
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// Electronic product
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int qty, string brand, int warrantyMonths)
    {
        Id = id; Name = name; Quantity = qty; Brand = brand; WarrantyMonths = warrantyMonths;
    }

    public override string ToString() =>
        $"[Electronic] {Name} (ID:{Id}) Qty:{Quantity}, Brand:{Brand}, Warranty:{WarrantyMonths} months";
}

// Grocery product
public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int qty, DateTime expiry)
    {
        Id = id; Name = name; Quantity = qty; ExpiryDate = expiry;
    }

    public override string ToString() =>
        $"[Grocery] {Name} (ID:{Id}) Qty:{Quantity}, Exp:{ExpiryDate:yyyy-MM-dd}";
}

// Custom exceptions
public class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) { } }
public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) { } }
public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) { } }

// Generic inventory repo
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item ID {id} not found.");
        return _items[id];
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException($"Item ID {id} not found.");
    }

    public List<T> GetAllItems() => new List<T>(_items.Values);

    public void UpdateQuantity(int id, int newQty)
    {
        if (newQty < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
        if (!_items.ContainsKey(id)) throw new ItemNotFoundException($"Item ID {id} not found.");
        _items[id].Quantity = newQty;
    }
}

// Manager class
public class WareHouseManager
{
    public InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    public InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Wireless Mouse", 15, "Logitech", 24));
        _electronics.AddItem(new ElectronicItem(2, "USB-C Charger", 30, "Anker", 12));

        _groceries.AddItem(new GroceryItem(101, "Rice 5kg", 25, DateTime.Now.AddMonths(12)));
        _groceries.AddItem(new GroceryItem(102, "Olive Oil 1L", 10, DateTime.Now.AddMonths(6)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
            Console.WriteLine(item);
    }
}

class Program
{
    static void Main()
    {
        var manager = new WareHouseManager();
        manager.SeedData();

        Console.WriteLine("Groceries:");
        manager.PrintAllItems(manager._groceries);

        Console.WriteLine("\nElectronics:");
        manager.PrintAllItems(manager._electronics);

        // Test error handling
        try
        {
            manager._groceries.AddItem(new GroceryItem(101, "Extra Rice", 5, DateTime.Now.AddMonths(3)));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        try
        {
            manager._electronics.UpdateQuantity(999, 5);
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
