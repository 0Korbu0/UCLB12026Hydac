using System;
using System.IO;

namespace KommeOgGåSystem
{
    // Håndterer læsning og skrivning til/fra tekstfiler (persistering af data)
    public class DataHandler
    {
        private string dataFileName;

        public DataHandler(string dataFileName)
        {
            this.dataFileName = dataFileName;
        }

        public string DataFileName
        {
            get { return dataFileName; }
        }

        // Gemmer hele arrayet af medarbejdere til filen
        public void SaveEmployees(Employee[] employees)
        {
            var sw = new StreamWriter(dataFileName);
            foreach (Employee p in employees)
            {
                string title = p.MakeInfo();
                sw.WriteLine(title);
            }
            sw.Close();
        }

        // Indlæser alle medarbejdere fra filen og returnerer dem som et Employee array
        public Employee[] LoadPersons()
        {
            // Hvis filen ikke findes, returnerer vi et tomt array (så mock-data oprettes)
            if (!File.Exists(dataFileName))
            {
                return [];
            }

            string[] lines = File.ReadAllLines(dataFileName);

            Employee[] persons = new Employee[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                // Splitter linjen ved semikolon: id;navn;tilstedeværelse;admin
                string[] personInfo = lines[i].Split(';');
                int personID = int.Parse(personInfo[0]);
                string personName = personInfo[1];
                bool personPresenceStatus = bool.Parse(personInfo[2]);
                bool personSystemAdmin = bool.Parse(personInfo[3]);

                persons[i] = new Employee(personID, personName, personPresenceStatus, personSystemAdmin);
            }

            return persons;
        }

        // Gemmer alle gæstebesøg til filen
        public void SaveGuestVisits(GuestVisit[] visits)
        {
            var sw = new StreamWriter(dataFileName);
            foreach (GuestVisit v in visits)
            {
                string info = v.MakeInfo();
                sw.WriteLine(info);
            }
            sw.Close();
        }

        // Indlæser alle gæstebesøg fra filen og returnerer dem som et GuestVisit array
        public GuestVisit[] LoadGuestVisits()
        {
            // Hvis filen ikke findes, returnerer vi et tomt array (så mock-data oprettes)
            if (!File.Exists(dataFileName))
            {
                return [];
            }

            // Henter medarbejderne, så vi kan koble værten på hvert besøg ud fra ID
            DataHandler employeeHandler = new DataHandler("Employees.txt");
            Employee[] employees = employeeHandler.LoadPersons();

            string[] lines = File.ReadAllLines(dataFileName);

            GuestVisit[] visits = new GuestVisit[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                // Splitter linjen: navn;firma;værtID;lokale;mødeStart;mødeSlut;ankomst;afgang;folder
                string[] info = lines[i].Split(';');
                string guestName = info[0];
                string guestCompany = info[1];
                int hostId = int.Parse(info[2]);
                string roomName = info[3];

                // Håndtering af mødelokalets TimeSlot (kan være null hvis eget kontor er valgt)
                // I tekstfilen gemmes mødetid som "-" hvis der intet tidsrum er tilknyttet
                TimeSlot? meetingTime = null;
                if (info[4] != "-" && info[5] != "-")
                {
                    DateTime meetingStart = DateTime.Parse(info[4]);
                    DateTime meetingEnd = DateTime.Parse(info[5]);
                    meetingTime = new TimeSlot(meetingStart, meetingEnd);
                }

                // Gæstens ankomst og afgang til bygningen
                DateTime arrivalTime = DateTime.Parse(info[6]);

                // Hvis gæsten stadig er i huset, står der "-" i filen, og vi sætter departureTime til null.
                // Ellers parses tidspunktet til en DateTime
                DateTime? departureTime = info[7] == "-" ? null : DateTime.Parse(info[7]);

                // Om sikkerhedsfolderen er udleveret (true/false)
                bool safetyFolder = bool.Parse(info[8]);

                Guest guest = new Guest(guestName, guestCompany);

                // Find den rigtige medarbejder ud fra hostId vha. den statiske hjælper
                Employee? host = GetEmployeeById(hostId, employees);

                // Opretter en Ukendt Employee, hvis der er sket en eller anden fejl og vi ikke kan finde den rigtige medarbejder
                if (host == null)
                {
                    host = new Employee(hostId, "Ukendt");
                }

                MeetingRoom room = new MeetingRoom(roomName);

                visits[i] = new GuestVisit(guest, host, room, meetingTime, arrivalTime, safetyFolder, departureTime);
            }

            return visits;
        }

        // 1. Statisk hjælper: Søger efter en medarbejder i et eksisterende Employee array i hukommelsen
        // - 'static': Betyder metoden tilhører klassen DataHandler og kan kaldes uden 'new DataHandler()'
        // - 'Employee?': Returtypen er nullable, da metoden returnerer 'null' hvis medarbejderen ikke findes
        public static Employee? GetEmployeeById(int id, Employee[] employees)
        {
            for (int i = 0; i < employees.Length; i++)
            {
                // employees[i] != null sikrer, at vi ikke får en NullReferenceException hvis en plads i arrayet er tom
                // Vi tjekker alle medarbejdere igennem om deres id matcher det søgte id
                if (employees[i] != null && employees[i].EmployeeID == id)
                {
                    return employees[i];
                }
            }
            return null; // Hvis ID'et ikke findes i listen
        }

        // 2. Statisk hjælper (Metode-overload): Samme metodenavn, men indlæser selv filen automatisk
        // - 'fileName = "Employees.txt"': En standardparameter (default parameter), så filnavnet kan udelades ved kald
        public static Employee? GetEmployeeById(int id, string fileName = "Employees.txt")
        {
            DataHandler handler = new DataHandler(fileName);
            Employee[] employees = handler.LoadPersons();
            return GetEmployeeById(id, employees);
        }
    }
}
