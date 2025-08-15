using System;
using System.Collections.Generic;
using System.IO;

namespace Grading_System
{

    public class Student
    {
        public int Id;
        public string FullName;
        public int Score;

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }
    }

    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string path)
        {
            var list = new List<Student>();
            foreach (var line in File.ReadAllLines(path))
            {
                var parts = line.Split(',');

                if (!int.TryParse(parts[2], out int score))
                    throw new InvalidScoreFormatException($"Invalid score format in line: \"{line}\"");

                if (parts.Length != 3)
                    throw new MissingFieldException($"Invalid data format: \"{line}\" - missing fields.");

                if (!int.TryParse(parts[0], out int id))
                    throw new FormatException($"Invalid ID format in line: \"{line}\"");


                list.Add(new Student(id, parts[1], score));
            }
            return list;
        }

        public void WriteReportToFile(List<Student> students, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                foreach (var s in students)
                {
                    writer.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
                }
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            string input = "students.txt";

            // Create test data (includes one bad line to trigger exception)
            File.WriteAllLines(input, new[]
            {
                "111,Melvin,85",
                "121,Dave,SeventyTwo",
                "223,Charlie,60",
                "114,David"            
            });

            var processor = new StudentResultProcessor();

            try
            {
                var students = processor.ReadStudentsFromFile(input);
                processor.WriteReportToFile(students, "report.txt");
                Console.WriteLine(File.ReadAllText("report.txt"));
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error: " + ex.Message);
            }
        }
    }
}
