using System;
using System.Collections.Generic;
using System.Text;

namespace KommeOgGåSystem
{
    // Håndterer medarbejdernes logbog (visning af tilstedeværelse samt tjek ind / tjek ud)
    public class Logbog
    {
        private Employee[] hydacEmployees { get; set; }

        public Logbog()
        {
            // Indlæs alle medarbejdere fra tekstfilen ved opstart
            DataHandler file = new DataHandler("Employees.txt");
            hydacEmployees = file.LoadPersons();

            // Hvis tekstfilen ikke eksisterer eller er tom, opretter vi start-medarbejdere
            if (hydacEmployees.Length == 0)
            {
                CreateInitialEmployees();
                file.SaveEmployees(hydacEmployees);
            }
        }

        // Opretter start-medarbejdere (mock data)
        private void CreateInitialEmployees()
        {
            hydacEmployees =
            [
                new Employee(1, "Egon", false, false),
                new Employee(2, "Jens", true, false),
                new Employee(3, "Mathilde", false, false),
                new Employee(4, "Jesper", false, true),
                new Employee(5, "Emma", false, false),
                new Employee(6, "Freja", false, false)
            ];
        }

        // Viser hovedmenuen for Logbogen med et do-while loop
        public void ShowMenu()
        {
            do
            {
                Console.Clear();
                string line = GetConsoleDash();
                Console.WriteLine("HYDAC - Logbog (Medarbejdere)\n");
                Console.WriteLine(line);
                Console.WriteLine("Medarbejdere til stede lige nu:");
                Console.WriteLine(line);

                // Gennemgå alle medarbejdere og udskriv kun dem, der er tjekket ind
                int presentCount = 0;
                for (int i = 0; i < hydacEmployees.Length; i++)
                {
                    if (hydacEmployees[i].PresenceStatus)
                    {
                        Console.WriteLine($"| {hydacEmployees[i].MakePresentableInfo()} |");
                        Console.WriteLine(line);
                        presentCount++;
                    }
                }

                if (presentCount == 0)
                {
                    Console.WriteLine("Ingen medarbejdere er i øjeblikket tjekket ind.");
                    Console.WriteLine(line);
                }

                Console.WriteLine();
                Console.WriteLine("1. Tjek ind / Tjek ud");
                Console.WriteLine("2. Se alle medarbejdere");
                Console.WriteLine();
                Console.WriteLine("(Skriv 0 for at gå tilbage til hovedmenuen)\n");

                Console.Write("Vælg menupunkt (0-2): ");
                int choice = 0;
                while (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > 2)
                {
                    Console.Write("Ugyldigt valg. Skriv et tal mellem 0 og 2: ");
                }

                // 0 = Gå tilbage til hovedmenuen
                if (choice == 0)
                {
                    break;
                }

                if (choice == 1)
                {
                    CheckInOutMenu();
                }
                else if (choice == 2)
                {
                    ShowAllEmployees();
                }

            } while (true);
        }

        // Menu til at en medarbejder kan tjekke sig selv ind eller ud
        public void CheckInOutMenu()
        {
            string line = GetConsoleDash();
            Console.Clear();
            Console.WriteLine("HYDAC - LogBog Tjek ind/ud\n");
            Console.WriteLine(line);
            Console.Write("Indtast dit medarbejderID: ");

            // Validering af brugerens input til ID
            int employeeID = 0;
            while (!int.TryParse(Console.ReadLine(), out employeeID) || employeeID < 0)
            {
                Console.WriteLine($"Du skal skrive dit medarbejderID. Skriv 0 for at anullere indtastningen.");
            }
            if (employeeID == 0) return; // 0 = afbryd

            // Find medarbejderen ud fra ID via DataHandlerens hjælper
            var employee = DataHandler.GetEmployeeById(employeeID, hydacEmployees);
            if (employee == null)
            {
                Console.WriteLine("\nMedarbejder ikke fundet.");
                Console.WriteLine("\nTryk en tast for at gå tilbage...");
                Console.ReadKey();
                return;
            }

            do
            {
                Console.Clear();
                Console.WriteLine("HYDAC - LogBog Tjek ind/ud\n");
                Console.WriteLine(line);
                Console.WriteLine("Din nuværrende status:");
                Console.WriteLine(employee.MakePresentableInfo());
                Console.WriteLine(line);
                Console.WriteLine();
                Console.WriteLine("1. Tjek ind");
                Console.WriteLine("2. Tjek ud");
                Console.WriteLine();
                Console.WriteLine("(Skriv 0 for at gå tilbage)\n");

                // Valg af 1 eller 2
                Console.Write("Vælg menupunkt (0-2): ");
                int input = 0;
                while (!int.TryParse(Console.ReadLine(), out input) || input < 0 || input > 2)
                {
                    Console.Write("Ugyldigt valg. Skriv et tal mellem 0 og 2: ");
                }
                if (input == 0) return;

                // Opdater tilstedeværelse
                if (input == 1) 
                { 
                    employee.PresenceStatus = true; 
                }
                else if (input == 2) 
                { 
                    employee.PresenceStatus = false; 
                }

                // Gem ændringerne tilbage til tekstfilen
                DataHandler file = new DataHandler("Employees.txt");
                file.SaveEmployees(hydacEmployees);

            } while (true);
        }

        // Viser listen over alle medarbejdere i systemet uanset tilstedeværelse
        public void ShowAllEmployees()
        {
            Console.Clear();
            string line = GetConsoleDash();
            Console.WriteLine("HYDAC - Alle Medarbejdere\n");
            Console.WriteLine(line);

            for (int i = 0; i < hydacEmployees.Length; i++)
            {
                Console.WriteLine($"| {hydacEmployees[i].MakePresentableInfo()} |");
                Console.WriteLine(line);
            }

            Console.WriteLine("\nTryk en tast for at gå tilbage...");
            Console.ReadKey();
        }

        // Laver en vandret bindestreg-linje på tværs af konsolvinduets bredde
        public static string GetConsoleDash()
        {
            int width = Math.Max(10, Console.WindowWidth - 1);
            string line = new string('-', width);
            return line;
        }
    }
}
