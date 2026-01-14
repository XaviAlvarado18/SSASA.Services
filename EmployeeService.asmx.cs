using System;
using System.Collections.Generic;
using System.Web.Services;
using SSASA.Services.Data;
using SSASA.Services.Models;

namespace SSASA.Services
{
    // SOAP Web Service (.asmx) implementation
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // [System.Web.Script.Services.ScriptService] // Uncomment if calling from AJAX
    public class EmployeeService : WebService
    {
        private readonly DatabaseLogic _db = new DatabaseLogic();

        [WebMethod(Description = "Health check endpoint. Returns a short message confirming the service is running.")]
        public string HelloWorld()
        {
            return "Backend Service is Running";
        }

        [WebMethod(Description = "Creates a new employee or updates an existing employee based on EmployeeId. DPI is required and must be exactly 13 characters.")]
        public bool SaveEmployee(Employee emp)
        {
            if (emp == null) return false;

            // Basic validation: DPI must exist and be 13 characters
            if (string.IsNullOrEmpty(emp.DPI) || emp.DPI.Length != 13)
                return false;

            return _db.SaveEmployee(emp);
        }

        [WebMethod(Description = "Returns the full list of employees (may include computed fields such as tenure).")]
        public List<Employee> GetAllEmployees()
        {
            return _db.GetEmployees();
        }

        [WebMethod(Description = "Creates a new department or updates an existing department based on DepartmentId. If a department is disabled, employees assigned to it are set to inactive.")]
        public bool SaveDepartment(Department dept)
        {
            if (dept == null) return false;
            return _db.SaveDepartment(dept);
        }

        [WebMethod(Description = "Returns a single employee by EmployeeId. Returns null if the employee does not exist.")]
        public Employee GetEmployeeById(int id)
        {
            return _db.GetEmployeeById(id);
        }

        [WebMethod(Description = "Deletes an employee by EmployeeId. Returns true if the employee was deleted.")]
        public bool DeleteEmployee(int employeeId)
        {
            return _db.DeleteEmployee(employeeId);
        }

        [WebMethod(Description = "Returns the full list of departments.")]
        public List<Department> GetAllDepartments()
        {
            return _db.GetDepartments();
        }

        [WebMethod(Description = "Returns a single department by DepartmentId. Returns null if the department does not exist.")]
        public Department GetDepartmentById(int departmentId)
        {
            return _db.GetDepartmentById(departmentId);
        }

        [WebMethod(Description = "Returns the employee report with optional filters: DepartmentId, employee status, and hire date range. Includes computed fields such as updated age and tenure.")]
        public List<Employee> GetEmployeeReport(int? departmentId, bool? status, DateTime? startDate, DateTime? endDate)
        {
            return _db.GetEmployeeReport(departmentId, status, startDate, endDate);
        }
    }
}
