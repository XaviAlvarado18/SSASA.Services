using SSASA.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSASA.Services.Models
{
    [Serializable]
    public class Employee : Person
    {
        public int EmployeeId { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public int DepartmentId { get; set; }

        // Report specific properties
        public string DepartmentName { get; set; }
        public string Tenure { get; set; }

        public int Age { get; set; }

        public bool DepartmentIsActive { get; set; }
    }
}