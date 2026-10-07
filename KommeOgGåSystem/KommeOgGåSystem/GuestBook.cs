using System;
using System.IO;

namespace KommeOgGåSystem
{
    public class GuestBook
    {
        // Arrays til at holde data i hukommelsen under kørsel
        private GuestVisit[] visits;
        private Employee[] employees;
        private MeetingRoom[] meetingRooms;

        public GuestBook()
        {
            // 1. Indlæs alle medarbejdere fra tekstfilen, så de kan vælges som værter for gæsterne
            DataHandler employeeHandler = new DataHandler("Employees.txt");
            employees = employeeHandler.LoadPersons();

            // 2. Hent listen over HYDACs standard mødelokaler
            meetingRooms = MeetingRoom.GetDefaultRooms();

            // 3. Indlæs alle tidligere og nuværende gæstebesøg fra filen
            DataHandler guestHandler = new DataHandler("Guests.txt");
            visits = guestHandler.LoadGuestVisits();

            // 4. Hvis tekstfilen ikke eksisterede eller var tom, opretter vi start-data (mock data)
            if (visits.Length == 0)
            {
                CreateInitialVisits();
                guestHandler.SaveGuestVisits(visits);
            }
        }

        // Opretter start-eksempler baseret på den fysiske formular fra HYDAC
        private void CreateInitialVisits()
        {
            // Måde at undgå at lave en if. Vi tjekker om længden er større end 0, hvis den er tager vi employee[0] og hvis ikke opretter en ny
            Employee host1 = employees.Length > 0 ? employees[0] : new Employee(1, "Egon");
            Employee host2 = employees.Length > 1 ? employees[1] : new Employee(2, "Jens");

            DateTime today = DateTime.Today;

            visits =
            [
                new GuestVisit(new Guest("Mikkel Holst", "Murr Elektronik"), host1, meetingRooms[0], new TimeSlot(today.AddHours(9).AddMinutes(30), today.AddHours(12).AddMinutes(50)), today.AddHours(9).AddMinutes(30), true, today.AddHours(12).AddMinutes(50)),
                new GuestVisit(new Guest("Simon", "Murr Elektronik"), host1, meetingRooms[0], new TimeSlot(today.AddHours(9).AddMinutes(30), today.AddHours(12).AddMinutes(50)), today.AddHours(9).AddMinutes(30), true, today.AddHours(12).AddMinutes(50)),
                new GuestVisit(new Guest("Kasper Edal", "Nidec"), host2, meetingRooms[1], new TimeSlot(today.AddHours(12), today.AddHours(12).AddMinutes(45)), today.AddHours(12), false, today.AddHours(12).AddMinutes(45)),
                new GuestVisit(new Guest("Alexander Møller", "Micro Technic"), host2, meetingRooms[2], new TimeSlot(today.AddHours(13), today.AddHours(17)), today.AddHours(13), false, null) // null = stadig i huset
            ];
        }

        // Viser hovedmenuen for Gæstebogen og styrer brugerens navigation
        public void ShowMenu()
        {
            do
            {
                Console.Clear();
                string line = Logbog.GetConsoleDash();
                Console.WriteLine("HYDAC - Gæstebog\n");
                Console.WriteLine(line);
                Console.WriteLine("Gæster i huset lige nu:");
                Console.WriteLine(line);

                // Find og vis kun de gæster, som ikke er tjekket ud endnu (IsPresent == true)
                int presentCount = 0;
                for (int i = 0; i < visits.Length; i++)
                {
                    if (visits[i].IsPresent)
                    {
                        Console.WriteLine($"[{i + 1}] {visits[i].MakePresentableInfo()}");
                        presentCount++;
                    }
                }

                if (presentCount == 0)
                {
                    Console.WriteLine("Der er i øjeblikket ingen aktive gæster i huset.");
                }

                Console.WriteLine(line);
                Console.WriteLine();
                Console.WriteLine("1. Registrer ny gæst (Tjek ind)");
                Console.WriteLine("2. Tjek gæst ud");
                Console.WriteLine("3. Se historik over alle gæster");
                Console.WriteLine("4. Se mødelokale-status");
                Console.WriteLine();
                Console.WriteLine("(Skriv 0 for at gå tilbage til hovedmenuen)\n");

                Console.Write("Vælg menupunkt (0-4): ");
                int choice = 0;
                while (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > 4)
                {
                    Console.Write("Ugyldigt valg. Skriv et tal mellem 0 og 4: ");
                }

                // 0 betyder gå tilbage til hovedmenuen
                if (choice == 0)
                {
                    break;
                }

                if (choice == 1)
                {
                    CheckInGuest();
                }
                else if (choice == 2)
                {
                    CheckOutGuest();
                }
                else if (choice == 3)
                {
                    ShowAllGuests();
                }
                else if (choice == 4)
                {
                    ShowMeetingRooms();
                }

            } while (true);
        }

        // Tjekker en ny gæst ind ved at spørge om alle nødvendige oplysninger
        public void CheckInGuest()
        {
            Console.Clear();
            string line = Logbog.GetConsoleDash();
            Console.WriteLine("HYDAC - Tjek ny gæst ind\n");
            Console.WriteLine(line);

            // 1. Indtast gæstens fulde navn
            Console.Write("Indtast gæstens navn (eller 0 for at annullere): ");
            string guestName = Console.ReadLine() ?? "";
            if (guestName == "" || guestName == "0") return; // Afbryd hvis tomt eller 0

            // 2. Indtast firma
            Console.Write("Indtast firma: ");
            string guestCompany = Console.ReadLine() ?? "";

            // 3. Vælg hvilken medarbejder der er vært for gæsten
            Console.WriteLine("\nVælg ansvarlig vært (medarbejder):");
            for (int i = 0; i < employees.Length; i++)
            {
                // Pluser 1, da int i starter på 0, derfor vi siger - 1 nede ved selectedHost også, så det tilsvare til det rigtige index
                Console.WriteLine($"  {i + 1}. {employees[i].EmployeeName} (ID: {employees[i].EmployeeID})");
            }
            Console.WriteLine("  0. Annuller");
            Console.Write($"Vælg vært (0-{employees.Length}): ");
            int hostChoice;
            while (!int.TryParse(Console.ReadLine(), out hostChoice) || hostChoice < 0 || hostChoice > employees.Length)
            {
                Console.Write($"Skriv et tal mellem 0 og {employees.Length}: ");
            }
            if (hostChoice == 0) return; // 0 = afbryd
            Employee selectedHost = employees[hostChoice - 1];

            // 4. Vælg hvilket mødelokale gæsten og værten skal bruge
            Console.WriteLine("\nVælg mødelokale:");
            for (int i = 0; i < meetingRooms.Length; i++)
            {
                // Forklaring med + 1 lidt længere oppe
                Console.WriteLine($"  {i + 1}. {meetingRooms[i].MeetingRoomName}");
            }
            Console.WriteLine($"  {meetingRooms.Length + 1}. Eget kontor");
            Console.WriteLine("  0. Annuller");

            MeetingRoom selectedRoom;
            TimeSlot? selectedSlot = null;

            do
            {
                Console.Write($"Vælg lokale (0-{meetingRooms.Length + 1}): ");
                int roomChoice;
                while (!int.TryParse(Console.ReadLine(), out roomChoice) || roomChoice < 0 || roomChoice > meetingRooms.Length + 1)
                {
                    Console.Write($"Skriv et tal mellem 0 og {meetingRooms.Length + 1}: ");
                }

                if (roomChoice == 0) return; // 0 = afbryd

                // Hvis brugeren vælger det sidste menupunkt, er det "Eget kontor"
                if (roomChoice == meetingRooms.Length + 1)
                {
                    selectedRoom = new MeetingRoom("Eget kontor");
                    selectedSlot = null; // Eget kontor kræver ikke TimeSlot-booking
                    break;
                }
                else
                {
                    selectedRoom = meetingRooms[roomChoice - 1];

                    // Spørg om tidsrum (TimeSlot) for mødelokalet
                    DateTime meetingStart = DateTime.Now;
                    Console.Write($"\nIndtast møde starttidspunkt (Tryk Enter for nu: {meetingStart:HH:mm}): ");
                    string startInput = Console.ReadLine() ?? "";
                    while (startInput != "" && !DateTime.TryParse(startInput, out meetingStart))
                    {
                        Console.Write("Ugyldigt format. Skriv f.eks. '10:00' (eller Enter for nu): ");
                        startInput = Console.ReadLine() ?? "";
                    }

                    DateTime meetingEnd = meetingStart.AddHours(1);
                    Console.Write($"Indtast møde sluttidspunkt (Tryk Enter for om 1 time: {meetingEnd:HH:mm}): ");
                    string endInput = Console.ReadLine() ?? "";
                    while (endInput != "" && !DateTime.TryParse(endInput, out meetingEnd))
                    {
                        Console.Write("Ugyldigt format. Skriv f.eks. '11:30' (eller Enter for standard): ");
                        endInput = Console.ReadLine() ?? "";
                    }

                    selectedSlot = new TimeSlot(meetingStart, meetingEnd);

                    // Tjek om det valgte fælleslokale allerede er optaget i det ønskede TimeSlot
                    if (selectedRoom.IsOccupiedInSlot(selectedSlot, visits))
                    {
                        Console.WriteLine($"\nAdvarsel: {selectedRoom.MeetingRoomName} er allerede booket i tidsrummet {selectedSlot}! Vælg venligst et andet lokale eller tidsrum.");
                        continue;
                    }

                    break;
                }
            } while (true);

            // 5. Dato og ankomsttidspunkt for gæsten til HYDAC (standard er DateTime.Now)
            DateTime arrivalTime = DateTime.Now;
            Console.Write($"\nIndtast gæstens ankomst (Tryk Enter for nu: {arrivalTime:dd/MM/yyyy HH:mm}): ");
            string timeInput = Console.ReadLine() ?? "";
            while (timeInput != "" && !DateTime.TryParse(timeInput, out arrivalTime))
            {
                Console.Write("Ugyldigt format. Skriv f.eks. '15/05/2026 13:00' (eller Enter for nu): ");
                timeInput = Console.ReadLine() ?? "";
            }

            // 6. Registrer om sikkerhedsfolder er udleveret i receptionen
            Console.Write("Er sikkerhedsfolder udleveret? (j/n): ");
            string folderInput = (Console.ReadLine() ?? "").ToLower();
            bool folder = folderInput == "j" || folderInput == "ja" || folderInput == "y";

            // 7. Opret selve Guest og GuestVisit-objekterne
            Guest newGuest = new Guest(guestName, guestCompany);
            GuestVisit newVisit = new GuestVisit(newGuest, selectedHost, selectedRoom, selectedSlot, arrivalTime, folder, null);

            // 8. Udvider arrayet med 1 ekstra plads via Array.Resize (vi har ikke haft om Array.Resize, men fundet det online)
            // Da array-indekser starter ved 0, er den nye tomme plads altid på index: Length - 1
            Array.Resize(ref visits, visits.Length + 1);
            visits[visits.Length - 1] = newVisit;

            // 9. Gem den opdaterede liste i tekstfilen
            DataHandler handler = new DataHandler("Guests.txt");
            handler.SaveGuestVisits(visits);

            Console.WriteLine("\nGæsten er oprettet og tjekket ind!");
            Console.WriteLine("\nTryk en tast for at fortsætte...");
            Console.ReadKey();
        }

        // Tjekker en gæst ud når de forlader HYDAC
        public void CheckOutGuest()
        {
            Console.Clear();
            string line = Logbog.GetConsoleDash();
            Console.WriteLine("HYDAC - Tjek gæst ud\n");
            Console.WriteLine(line);

            // Vis kun de gæster, som i øjeblikket er til stede i huset
            int presentCount = 0;
            for (int i = 0; i < visits.Length; i++)
            {
                if (visits[i].IsPresent)
                {
                    Console.WriteLine($"  {i + 1}. {visits[i].Guest.GuestName} ({visits[i].Guest.GuestCompany}) - Vært: {visits[i].Host.EmployeeName}");
                    presentCount++;
                }
            }

            if (presentCount == 0)
            {
                Console.WriteLine("Ingen gæster at tjekke ud.");
                Console.WriteLine("\nTryk en tast for at gå tilbage...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("  0. Annuller");
            Console.Write("\nIndtast nummeret på gæsten: ");
            int selectedNumber;
            while (!int.TryParse(Console.ReadLine(), out selectedNumber) || selectedNumber < 0 || selectedNumber > visits.Length || (selectedNumber > 0 && !visits[selectedNumber - 1].IsPresent))
            {
                Console.Write("Ugyldigt nummer. Prøv igen: ");
            }

            if (selectedNumber == 0) return;

            // Find det valgte besøg (index er selectedNumber - 1)
            GuestVisit visitToCheckout = visits[selectedNumber - 1];

            // Sæt afgangstidspunktet (standard er DateTime.Now)
            DateTime departureTime = DateTime.Now;
            Console.Write($"Indtast afgangstidspunkt (Tryk Enter for nu: {departureTime:HH:mm}): ");
            string depInput = Console.ReadLine() ?? "";
            while (depInput != "" && !DateTime.TryParse(depInput, out departureTime))
            {
                Console.Write("Ugyldigt format. Skriv f.eks. '15:30' (eller Enter for nu): ");
                depInput = Console.ReadLine() ?? "";
            }

            // Opdater gæstens afgangstid og gem til filen
            visitToCheckout.DepartureTime = departureTime;

            DataHandler handler = new DataHandler("Guests.txt");
            handler.SaveGuestVisits(visits);

            Console.WriteLine($"\n{visitToCheckout.Guest.GuestName} er tjekket ud kl. {departureTime:HH:mm}!");
            Console.WriteLine("\nTryk en tast for at fortsætte...");
            Console.ReadKey();
        }

        // Viser den komplette historik over alle gæster (både nuværende og tidligere)
        public void ShowAllGuests()
        {
            Console.Clear();
            string line = Logbog.GetConsoleDash();
            Console.WriteLine("HYDAC - Historik over alle gæster\n");
            Console.WriteLine(line);

            if (visits.Length == 0)
            {
                Console.WriteLine("Der er ingen registrerede gæster.");
            }
            else
            {
                for (int i = 0; i < visits.Length; i++)
                {
                    Console.WriteLine($"[{ i + 1}] {visits[i].MakePresentableInfo()}");
                }
            }

            Console.WriteLine(line);
            Console.WriteLine("\nTryk en tast for at gå tilbage...");
            Console.ReadKey();
        }

        // Viser en oversigt over alle mødelokaler og om de er [LEDIG] eller [OPTAGET]
        public void ShowMeetingRooms()
        {
            Console.Clear();
            string line = Logbog.GetConsoleDash();
            Console.WriteLine("HYDAC - Mødelokale Oversigt\n");
            Console.WriteLine(line);

            // Gennemgå alle lokaler ét for ét
            for (int r = 0; r < meetingRooms.Length; r++)
            {
                MeetingRoom room = meetingRooms[r];
                GuestVisit? activeMeeting = null;

                // Tjek om der er et møde i gang lige nu i dette lokale (TimeSlot.IsActiveNow)
                for (int v = 0; v < visits.Length; v++)
                {
                    GuestVisit visit = visits[v];
                    if (visit != null &&
                        visit.MeetingRoom.MeetingRoomName == room.MeetingRoomName &&
                        visit.MeetingTime?.IsActiveNow() == true)
                    {
                        activeMeeting = visit;
                        break;
                    }
                }

                // Udskriv status for lokalet
                if (activeMeeting != null && activeMeeting.MeetingTime != null)
                {
                    Console.WriteLine($"[OPTAGET ({activeMeeting.MeetingTime})] {room.MeetingRoomName,-35} -> Gæst: {activeMeeting.Guest.GuestName} | Vært: {activeMeeting.Host.EmployeeName}");
                }
                else
                {
                    Console.WriteLine($"[LEDIG]                  {room.MeetingRoomName,-35}");
                }
            }

            Console.WriteLine(line);
            Console.WriteLine("\nTryk en tast for at gå tilbage...");
            Console.ReadKey();
        }
    }
}
