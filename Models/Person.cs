using System;

namespace SSASA.Services.Models
{
    // Base class
    public class Person
    {
        // Properties required by the instructions (DPI, Names, etc.)
        public string FullNames { get; set; }
        public string DPI { get; set; }
        public DateTime BirthDate { get; set; }
        public char Gender { get; set; }
        public string Address { get; set; }
        public string NIT { get; set; }

        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Year;
                if (BirthDate.Date > today.AddYears(-age))
                {
                    age--;
                }
                return age;
            }
        }
    }
}