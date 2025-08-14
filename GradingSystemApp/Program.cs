using System;
using System.Collections.Generic;
using System.IO;

public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string name, int score)
    {
        Id = id; FullName = name; Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }

    public override string ToString() =>
        $"{FullName} (ID:{Id}): Score = {Score}, Grade = {GetGrade()}";
}

// Exceptions
public class InvalidScoreFormatException : Exception { public InvalidScoreFormatException(string msg) : base(msg) { } }
public class MissingFieldException : Exception { public MissingFieldException(string msg) : base(msg) { } }

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string filePath)
    {
        var list = new List<Student>();
        using var sr = new StreamReader(filePath);
        string? line;
        int lineNum = 0;

        while ((line = sr.ReadLine()) != null)
        {
            lineNum++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length != 3)
                throw new MissingFieldException($"Line {lineNum}: Missing data.");

            if (!int.TryParse(parts[0].Trim(), out int id))
                throw new InvalidScoreFormatException($"Line {lineNum}: Invalid ID.");

            if (!int.TryParse(parts[2].Trim(), out int score))
                throw new InvalidScoreFormatException($"Line {lineNum}: Invalid score.");

            list.Add(new Student(id, parts[1].Trim(), score));
        }

        return list;
    }

    public void WriteReportToFile(List<Student> students, string outputPath)
    {
        using var sw = new StreamWriter(outputPath);
        foreach (var s in students)
            sw.WriteLine(s);
    }
}

class Program
{
    static void Main()
    {
        var processor = new StudentResultProcessor();
        string input = "students.txt"; // file must exist
        string output = "report.txt";

        try
        {
            var students = processor.ReadStudentsFromFile(input);
            processor.WriteReportToFile(students, output);
            Console.WriteLine("Report generated successfully.");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: Input file not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
