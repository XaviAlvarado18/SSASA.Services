using System;
using System.Collections.Generic;
using System.Web.Services;
using SSASA.Services.Data;
using SSASA.Services.Models;

namespace SSASA.Services
{
    // Clase que implementa el Servicio Web SOAP (.asmx)
    // Requisito: Servicio Web (SOAP o REST) en C# .NET Framework [cite: 24]
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // [System.Web.Script.Services.ScriptService] // Descomenta esto si lo vas a llamar desde AJAX
    public class EmployeeService : WebService
    {
        private readonly DatabaseLogic _db = new DatabaseLogic();

        [WebMethod]
        public string HelloWorld()
        {
            return "Backend Service is Running";
        }

        [WebMethod(Description = "Creates or Updates an Employee")]
        public bool SaveEmployee(Employee emp)
        {
            if (string.IsNullOrEmpty(emp.DPI) || emp.DPI.Length != 13)
                return false;

            return _db.SaveEmployee(emp);
        }

        [WebMethod(Description = "Returns the list of employees with calculated tenure")]
        public List<Employee> GetAllEmployees()
        {
            return _db.GetEmployees();
        }

        [WebMethod(Description = "Creates or Updates a Department")]
        public bool SaveDepartment(Department dept)
        {
            return _db.SaveDepartment(dept);
        }

        [WebMethod]
        public Employee GetEmployeeById(int id)
        {
            return _db.GetEmployeeById(id);
        }

        [WebMethod]
        public bool DeleteEmployee(int employeeId)
        {
            return _db.DeleteEmployee(employeeId);
        }
    }
}