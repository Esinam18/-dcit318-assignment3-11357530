using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker interface
public interface IInventoryEntity { int Id { get; } }

// Immutable record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private readonly string _filePath;

    public InventoryLogger(string path) { _filePath = path; }

    public void Add(T item) => _log.Add(item);
    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Save error: " + ex.Message);
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath)) return;
            var json = File.ReadAllText(_filePath);
            var items = JsonSerializer.Deserialize<List<T>>(json);
            _log = items ?? new List<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Load error: " + ex.Message);
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
        _logger.Add(new InventoryItem(1, "Perfume - Bloom", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Perfume - Night", 5, DateTime.Now.AddDays(-2)));
        _logger.Add(new InventoryItem(3, "Body Lotion", 20, DateTime.Now.AddDays(-10)));
    }

    public void SaveData() => _logger.SaveToFile();
    public void LoadData() => _logger.LoadFromFile();
    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        if (items.Count == 0) Console.WriteLine("No items found.");
        foreach (var i in items) Console.WriteLine(i);
    }
}

class Program
{
    static void Main()
    {
        string filePath = "inventory.json";
        var app = new InventoryApp(filePath);

        app.SeedSampleData();
        app.SaveData();

        var newApp = new InventoryApp(filePath);
        newApp.LoadData();
        newApp.PrintAllItems();
    }
}
