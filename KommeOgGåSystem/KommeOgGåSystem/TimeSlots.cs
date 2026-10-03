using System;

namespace KommeOgGåSystem
{
    // Repræsenterer et tidsrum (TimeSlot) for et mødelokale med start- og sluttidspunkt
    public class TimeSlot
    {
        private DateTime startTime;
        private DateTime endTime;

        public TimeSlot(DateTime startTime, DateTime endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        // Tjekker om mødet finder sted lige nu
        public bool IsActiveNow()
        {
            DateTime now = DateTime.Now;

            // Hvis det nuværende tidspunkt er efter/på starttidspunktet OG før/på sluttidspunktet
            if (now >= startTime && now <= endTime)
            {
                return true; // Mødet er i gang lige nu
            }
            else
            {
                return false; // Mødet er enten ikke begyndt endnu eller er allerede afsluttet
            }
        }

        // Tjekker om dette tidsrum overlapper med et andet tidsrum
        public bool OverlapsWith(TimeSlot other)
        {
            // To tidsrum overlapper, hvis dette møde starter før det andet slutter, 
            // OG dette møde slutter efter det andet starter.
            if (startTime < other.endTime && endTime > other.startTime)
            {
                return true; // Der er et overlap (tidsrummene rammer hinanden)
            }
            else
            {
                return false; // Ingen overlap (lokalet er frit)
            }
        }

        // Udskriver tidsrummet pænt, f.eks. "10:00 - 12:00"
        public override string ToString()
        {
            return $"{startTime:HH:mm} - {endTime:HH:mm}";
        }
    }
}
