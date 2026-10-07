using System;
using System.Collections.Generic;
using System.Text;

namespace KommeOgGåSystem
{
    // Repræsenterer gæsten som person og hvilket firma de kommer fra
    public class Guest
    {
        // Private felter
        private string guestName;
        private string guestCompany;

        public Guest(string guestName, string guestCompany)
        {
            this.guestName = guestName;
            this.guestCompany = guestCompany;
        }

        // Properties til at tilgå og ændre gæstens oplysninger
        public string GuestName
        {
            get { return guestName; }
            set { guestName = value; }
        }

        public string GuestCompany
        {
            get { return guestCompany; }
            set { guestCompany = value; }
        }
    }
}
