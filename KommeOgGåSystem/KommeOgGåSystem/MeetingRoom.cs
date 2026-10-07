using System;
using System.Collections.Generic;
using System.Text;

namespace KommeOgGåSystem
{
    // Repræsenterer et mødelokale i HYDAC-bygningen
    public class MeetingRoom
    {
        private string meetingRoomName;

        public MeetingRoom(string meetingRoomName)
        {
            this.meetingRoomName = meetingRoomName;
        }

        public string MeetingRoomName
        {
            get { return meetingRoomName; }
            set { meetingRoomName = value; }
        }

        // Returnerer lokalets navn når objektet udskrives i tekst
        public override string ToString()
        {
            return meetingRoomName;
        }

        // Standard liste over HYDACs fysiske mødelokaler (fra billedet i opgaven)
        public static MeetingRoom[] GetDefaultRooms()
        {
            return
            [
                new MeetingRoom("#DK-LGS_Lokale_The_Aquarium"),
                new MeetingRoom("#DK-LGS_Lokale_The_Bridge-East"),
                new MeetingRoom("#DK-LGS_Lokale_The_Bridge-West"),
                new MeetingRoom("#DK-LGS_Lokale_The_Station-Platform_9¾"),
                new MeetingRoom("#DK-LGS_Lokale_The_Station-The_Library"),
                new MeetingRoom("#DK-LGS_Lokale_The_Training_Center"),
                new MeetingRoom("#DK-LGS_Lokale_Stilling_Kantine"),
                new MeetingRoom("#DK-LGS_Lokale_lille_Stue")
            ];
        }

        // 1. Tjekker om dette lokale er optaget LIGE NU (dvs. et aktivt TimeSlot lige nu)
        public bool IsOccupied(GuestVisit[] visits)
        {
            // "Eget kontor" betragtes aldrig som optaget for andre værter
            if (meetingRoomName == "Eget kontor")
            {
                return false;
            }

            for (int i = 0; i < visits.Length; i++)
            {
                GuestVisit visit = visits[i];
                // Tjekker om besøget bruger dette lokale, og om mødet er i gang lige nu
                if (visit != null &&
                    visit.MeetingRoom.MeetingRoomName == meetingRoomName &&
                    visit.MeetingTime?.IsActiveNow() == true)
                {
                    return true;
                }
            }
            return false;
        }

        // Overload: Tjekker om lokalet er optaget lige nu ved at indlæse direkte fra filen
        public bool IsOccupied(string fileName = "Guests.txt")
        {
            DataHandler guestHandler = new DataHandler(fileName);
            GuestVisit[] visits = guestHandler.LoadGuestVisits();
            return IsOccupied(visits);
        }

        // 2. Tjekker om dette lokale er optaget i et specifikt tidsrum (TimeSlot)
        public bool IsOccupiedInSlot(TimeSlot requestedSlot, GuestVisit[] visits)
        {
            if (meetingRoomName == "Eget kontor")
            {
                return false;
            }

            for (int i = 0; i < visits.Length; i++)
            {
                GuestVisit visit = visits[i];
                // Tjekker om der findes et andet møde i samme lokale, som tidsmæssigt overlapper
                if (visit != null &&
                    visit.MeetingRoom.MeetingRoomName == meetingRoomName &&
                    visit.MeetingTime?.OverlapsWith(requestedSlot) == true)
                {
                    return true;
                }
            }
            return false;
        }

        // Overload: Tjekker om lokalet er optaget i et tidsrum ud fra filen
        public bool IsOccupiedInSlot(TimeSlot requestedSlot, string fileName = "Guests.txt")
        {
            DataHandler guestHandler = new DataHandler(fileName);
            GuestVisit[] visits = guestHandler.LoadGuestVisits();
            return IsOccupiedInSlot(requestedSlot, visits);
        }
    }
}
