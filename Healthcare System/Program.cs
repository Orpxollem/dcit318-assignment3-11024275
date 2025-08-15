using System;
using System.Collections.Generic;

namespace Healthcare_System
{
    public class Repository<T>
    {
        private List<T> items = new List<T>();

        public void Add(T item) => items.Add(item);

        public List<T> GetAll() => items;

        public T GetById(Func<T, bool> predicate) => items.Find(x => predicate(x));

        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.Find(x => predicate(x));
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }

    public class Patient
    {
        public int Id;
        public string Name;
        public int Age;
        public string Gender;

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }
    }

    public class Prescription
    {
        public int Id;
        public int PatientId;
        public string MedicationName;
        public decimal Price;
        public DateTime DateIssued;

        public Prescription(int id, int patientId, string medicationName, decimal price, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            Price = price;
            DateIssued = dateIssued;
        }
    }


    public class HealthSystemApp
    {
        private Dictionary<int, List<Prescription>> prescriptionMap = new Dictionary<int, List<Prescription>>();
        private Repository<Patient> patientRepo = new Repository<Patient>();
        private Repository<Prescription> prescriptionRepo = new Repository<Prescription>();

        public void SeedData()
        {
            patientRepo.Add(new Patient(101, "Melvin King", 21, "Male"));
            patientRepo.Add(new Patient(201, "Abena Smith", 45, "Female"));

            prescriptionRepo.Add(new Prescription(111, 101, "Vitamin C", 55, DateTime.Now.AddDays(-12)));
            prescriptionRepo.Add(new Prescription(112, 101, "Folic Acid", 12, DateTime.Now.AddDays(-30)));
            prescriptionRepo.Add(new Prescription(113, 201, "Paracetamol", 10, DateTime.Now.AddDays(-22)));
        }

        
        public void BuildPrescriptionMap()
        {
            prescriptionMap.Clear();
            foreach (var p in prescriptionRepo.GetAll())
            {
                if (!prescriptionMap.ContainsKey(p.PatientId))
                    prescriptionMap[p.PatientId] = new List<Prescription>();
                prescriptionMap[p.PatientId].Add(p);
            }
        }

        
        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (prescriptionMap.ContainsKey(patientId))
            {
                return prescriptionMap[patientId];
            }
            return new List<Prescription>();
        }

        public void PrintAllPatients()
        {
            foreach (var p in patientRepo.GetAll())
                Console.WriteLine(p.Id + " - Name: " + p.Name + "\n      Age: " + p.Age + "\n      Gender: " + p.Gender + "\n");
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            if (prescriptionMap.ContainsKey(id))
            {
                foreach (var p in prescriptionMap[id])
                    Console.WriteLine(p.MedicationName + " - Price: GHS" + p.Price + "\n            Date: " + p.DateIssued.ToShortDateString() + "\n");
            }
            else
            {
                Console.WriteLine("No prescriptions found.");
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();

            Console.WriteLine("Patients:");
            app.PrintAllPatients();

            Console.WriteLine("\nDisplaying prescriptions for patient 101:");
            app.PrintPrescriptionsForPatient(101);

        }
    }
}
