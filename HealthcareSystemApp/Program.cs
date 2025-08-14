using System;
using System.Collections.Generic;
using System.Linq;

public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => new List<T>(items);
    public T? GetById(Func<T, bool> match) => items.FirstOrDefault(match);
    public bool Remove(Func<T, bool> match)
    {
        var found = items.FirstOrDefault(match);
        if (found == null) return false;
        return items.Remove(found);
    }
}

public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id; Name = name; Age = age; Gender = gender;
    }

    public override string ToString() => $"{Name} (ID:{Id}, Age:{Age}, Gender:{Gender})";
}

public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int pid, string med, DateTime dateIssued)
    {
        Id = id; PatientId = pid; MedicationName = med; DateIssued = dateIssued;
    }

    public override string ToString() => $"{MedicationName} (ID:{Id}) - {DateIssued:yyyy-MM-dd}";
}

public class HealthSystemApp
{
    private Repository<Patient> _patients = new Repository<Patient>();
    private Repository<Prescription> _prescriptions = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _map = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        _patients.Add(new Patient(1, "Alice Mensah", 29, "Female"));
        _patients.Add(new Patient(2, "Kofi Asante", 45, "Male"));
        _patients.Add(new Patient(3, "Esi Boateng", 33, "Female"));

        _prescriptions.Add(new Prescription(1, 1, "Amoxicillin 500mg", DateTime.Now.AddDays(-10)));
        _prescriptions.Add(new Prescription(2, 1, "Paracetamol 500mg", DateTime.Now.AddDays(-2)));
        _prescriptions.Add(new Prescription(3, 2, "Atorvastatin 10mg", DateTime.Now.AddDays(-30)));
        _prescriptions.Add(new Prescription(4, 3, "Metformin 500mg", DateTime.Now.AddDays(-7)));
        _prescriptions.Add(new Prescription(5, 2, "Amlodipine 5mg", DateTime.Now.AddDays(-1)));
    }

    public void BuildPrescriptionMap()
    {
        _map.Clear();
        foreach (var p in _prescriptions.GetAll())
        {
            if (!_map.ContainsKey(p.PatientId))
                _map[p.PatientId] = new List<Prescription>();
            _map[p.PatientId].Add(p);
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("Patients:");
        foreach (var p in _patients.GetAll())
            Console.WriteLine($" - {p}");
        Console.WriteLine();
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        if (_map.ContainsKey(id))
        {
            Console.WriteLine($"Prescriptions for Patient {id}:");
            foreach (var pres in _map[id])
                Console.WriteLine($" - {pres}");
        }
        else
        {
            Console.WriteLine("No prescriptions for this patient.");
        }
    }
}

class Program
{
    static void Main()
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        app.PrintPrescriptionsForPatient(2);
    }
}
