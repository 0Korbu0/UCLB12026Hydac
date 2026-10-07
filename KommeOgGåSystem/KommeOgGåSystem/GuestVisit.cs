using System;

namespace KommeOgGåSystem
{
    // Repræsenterer et enkelt gæstebesøg (svarende til én række på HYDAC-formularen)
    public class GuestVisit
    {
        // Private felter for besøget (Indkapsling / Encapsulation)
        private Guest guest;
        private Employee host;
        private MeetingRoom meetingRoom;
        
        // Nullable typer (?): Værdier med '?' kan enten indeholde et objekt/værdi eller være 'null'.
        // 1. meetingTime er 'null', hvis mødet foregår på værtens eget kontor (ikke et fælleslokale).
        // 2. departureTime er 'null', så længe gæsten stadig befinder sig i bygningen (ikke tjekket ud endnu).
        private TimeSlot? meetingTime;
        private DateTime arrivalTime;
        private DateTime? departureTime;
        private bool safetyFolderDelivered;

        public GuestVisit(Guest guest, Employee host, MeetingRoom meetingRoom, TimeSlot? meetingTime, DateTime arrivalTime, bool safetyFolderDelivered, DateTime? departureTime = null)
        {
            this.guest = guest;
            this.host = host;
            this.meetingRoom = meetingRoom;
            this.meetingTime = meetingTime;
            this.arrivalTime = arrivalTime;
            this.safetyFolderDelivered = safetyFolderDelivered;
            this.departureTime = departureTime;
        }

        // Properties for at få adgang til og ændre felterne udefra
        public Guest Guest
        {
            get { return guest; }
            set { guest = value; }
        }

        public Employee Host
        {
            get { return host; }
            set { host = value; }
        }

        public MeetingRoom MeetingRoom
        {
            get { return meetingRoom; }
            set { meetingRoom = value; }
        }

        public TimeSlot? MeetingTime
        {
            get { return meetingTime; }
            set { meetingTime = value; }
        }

        public DateTime ArrivalTime
        {
            get { return arrivalTime; }
            set { arrivalTime = value; }
        }

        public DateTime? DepartureTime
        {
            get { return departureTime; }
            set { departureTime = value; }
        }

        public bool SafetyFolderDelivered
        {
            get { return safetyFolderDelivered; }
            set { safetyFolderDelivered = value; }
        }

        // Computed Property (beregnet egenskab): Har intet felt, men returnerer dynamisk 'true', så længe afgangstidspunktet er null
        public bool IsPresent
        {
            get { return departureTime == null; }
        }

        // Danner en semikolon-separeret streng (CSV-format) til lagring i tekstfilen
        public string MakeInfo()
        {
            // Hvis et tidsrum eller afgangstid er null, gemmer vi en bindestreg '-' som pladsholder
            string meetingStart = meetingTime != null ? meetingTime.StartTime.ToString("dd/MM/yyyy HH:mm") : "-";
            string meetingEnd = meetingTime != null ? meetingTime.EndTime.ToString("dd/MM/yyyy HH:mm") : "-";
            string departureString = departureTime.HasValue ? departureTime.Value.ToString("dd/MM/yyyy HH:mm") : "-";

            return $"{guest.GuestName};{guest.GuestCompany};{host.EmployeeID};{meetingRoom.MeetingRoomName};{meetingStart};{meetingEnd};{arrivalTime:dd/MM/yyyy HH:mm};{departureString};{safetyFolderDelivered}";
        }

        // Formaterer data pænt til konsollen i snorlige kolonner
        public string MakePresentableInfo()
        {
            // Note om formatering: Tallene med minus (f.eks. ,-16 og ,-32) angiver fast kolonnebredde og venstrejustering.
            // Eksempel: $"{guest.GuestName,-16}" reserverer altid 16 tegn, så tabellen ikke skrider.
            string date = arrivalTime.ToString("dd/MM/yyyy");
            string arr = arrivalTime.ToString("HH:mm");
            string dep = departureTime.HasValue ? departureTime.Value.ToString("HH:mm") : "I huset";
            string folder = safetyFolderDelivered ? "Ja" : "Nej";
            string meeting = meetingTime != null ? meetingTime.ToString() : "Eget kontor";

            return $"Dato: {date,-10} | Gæst: {guest.GuestName,-16} | Firma: {guest.GuestCompany,-15} | Vært: {host.EmployeeName,-12} \n    Lokale: {meetingRoom.MeetingRoomName,-33} | Mødetid: {meeting,-13} | Ankomst: {arr,-5} | Afgang: {dep,-8} | Folder: {folder}";
        }
    }
}
