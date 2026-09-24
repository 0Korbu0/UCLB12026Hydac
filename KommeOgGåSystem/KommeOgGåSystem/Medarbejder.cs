using System;
using System.Collections.Generic;
using System.Text;

namespace KommeOgGåSystem
{
    public class Medarbejder
    {
        private string medarbejderID;
        private string medarbejderNavn;
        private bool systemAdmin;


        public Medarbejder(string medarbejderID , string medarbejderNavn , bool systemAdmin)
        {
            this.medarbejderID = medarbejderID;
            this.medarbejderNavn = medarbejderNavn;
            this.systemAdmin = systemAdmin;
        }


        
    }
}
