using System;
using System.Collections.Generic;
using System.Text;

namespace KommeOgGåSystem
{
    // Repræsenterer en medarbejder i HYDAC
    public class Employee
    {
        // Private felter for medarbejderen
        private int employeeID;
        private string employeeName;
        private bool presenceStatus;
        private bool systemAdmin;

        public Employee(int employeeID, string employeeName, bool presenceStatus = false, bool systemAdmin = false)
        {
            this.employeeID = employeeID;
            this.employeeName = employeeName;
            this.systemAdmin = systemAdmin;
            this.presenceStatus = presenceStatus;
        }

        // Laver en string med: id;navn;tilstedeværelse;admin?, hvor man kan splitte med ';' til gemning i Employees.txt
        public string MakeInfo()
        {
            return $"{employeeID};{employeeName};{presenceStatus};{systemAdmin}";
        }

        // Formaterer data pænt til visning i konsollen
        public string MakePresentableInfo()
        {
            string presence = presenceStatus ? "Tilstede" : "Fraværende";
            string admin = systemAdmin ? "Ja" : "Nej";

            return $"ID: {employeeID,-4} | Navn: {employeeName,-15} | Status: {presence,-12} | Admin: {admin,-3}";
        }

        // Properties for at tilgå og ændre medarbejderens data
        public int EmployeeID 
        { 
            get { return employeeID; }
            set { employeeID = value; }
        }

        public string EmployeeName
        {
            get { return employeeName; }
        }

        public bool PresenceStatus
        {
            get { return presenceStatus; }
            set { presenceStatus = value; }
        }

        public bool SystemAdmin
        {
            get { return systemAdmin; }
        }
    }
}
