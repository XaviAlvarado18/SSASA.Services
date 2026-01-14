using SSASA.Services.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SSASA.Services.Data
{
    public class DatabaseLogic
    {
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        // -------------------------
        // EMPLOYEE
        // -------------------------

        public bool SaveEmployee(Employee emp)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_SaveEmployee", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = emp.EmployeeId;

                // ✅ NOMBRE CORRECTO DEL PARAMETRO: @FullName
                cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 150).Value =
                    string.IsNullOrWhiteSpace(emp.FullNames) ? (object)DBNull.Value : emp.FullNames;

                // ✅ DPI es 13
                cmd.Parameters.Add("@DPI", SqlDbType.NVarChar, 13).Value =
                    string.IsNullOrWhiteSpace(emp.DPI) ? (object)DBNull.Value : emp.DPI;

                cmd.Parameters.Add("@BirthDate", SqlDbType.Date).Value = emp.BirthDate;
                cmd.Parameters.Add("@Gender", SqlDbType.Char, 1).Value = emp.Gender;
                cmd.Parameters.Add("@HireDate", SqlDbType.Date).Value = emp.HireDate;

                cmd.Parameters.Add("@Address", SqlDbType.NVarChar, -1).Value =
                    string.IsNullOrWhiteSpace(emp.Address) ? (object)DBNull.Value : emp.Address;

                cmd.Parameters.Add("@NIT", SqlDbType.NVarChar, 15).Value =
                    string.IsNullOrWhiteSpace(emp.NIT) ? (object)DBNull.Value : emp.NIT;

                // ✅ NOMBRE CORRECTO DEL PARAMETRO: @IsActive
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = emp.IsActive;

                cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = emp.DepartmentId;

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public List<Employee> GetEmployees(int? deptId = null)
        {
            var list = new List<Employee>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeReport", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (deptId.HasValue)
                    cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = deptId.Value;

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(MapEmployeeFromReport(dr));
                    }
                }
            }
            return list;
        }

        public Employee GetEmployeeById(int employeeId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        // Aquí puedes mapear más campos (Email/Teléfono/etc si existen)
                        return new Employee
                        {
                            EmployeeId = employeeId,
                            FullNames = dr["FullNames"].ToString(),
                            DPI = dr["DPI"].ToString(),
                            BirthDate = SafeGetDate(dr, "BirthDate"),
                            Gender = SafeGetChar(dr, "Gender"),
                            HireDate = SafeGetDate(dr, "HireDate"),
                            Address = SafeGetString(dr, "Address"),
                            NIT = SafeGetString(dr, "NIT"),
                            IsActive = SafeGetBool(dr, "EmployeeStatus"),
                            DepartmentId = SafeGetInt(dr, "DepartmentId"),
                            DepartmentName = SafeGetString(dr, "DepartmentName")
                        };
                    }
                }
            }
            return null;
        }

        public bool DeleteEmployee(int employeeId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_DeleteEmployee", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DoesEmployeeExist(string dpi)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Employees WHERE DPI = @DPI";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DPI", dpi);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }


        // Si prefieres buscar en SQL (mejor para miles de registros)
        public List<Employee> SearchEmployees(string term, int? deptId = null)
        {
            // Ojo: puedes implementar un SP sp_SearchEmployees.
            // Si no quieres SP, se puede filtrar en memoria en la página.
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_SearchEmployees", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Term", SqlDbType.NVarChar, 150).Value = term ?? "";
                if (deptId.HasValue)
                    cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = deptId.Value;

                var list = new List<Employee>();
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        list.Add(MapEmployeeFromReport(dr));
                }
                return list;
            }
        }

        // -------------------------
        // DEPARTMENT
        // -------------------------

        public bool SaveDepartment(Department dept)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_SaveDepartment", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = dept.DepartmentId;
                cmd.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, 100).Value =
                    string.IsNullOrWhiteSpace(dept.Name) ? (object)DBNull.Value : dept.Name;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = dept.IsActive;

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public List<Department> GetDepartments()
        {
            var list = new List<Department>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetDepartments", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new Department
                        {
                            DepartmentId = SafeGetInt(dr, "DepartmentId"),
                            Name = SafeGetString(dr, "Name"),
                            IsActive = SafeGetBool(dr, "IsActive")
                        });
                    }
                }
            }
            return list;
        }

        // -------------------------
        // DASHBOARD / KPI / CHARTS
        // -------------------------

        public (int Activos, int NuevosEsteMes, int Vacantes) GetDashboardStats()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetDashboardStats", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return (
                            Activos: SafeGetInt(dr, "ActiveEmployees"),
                            NuevosEsteMes: SafeGetInt(dr, "NewHiresThisMonth"),
                            Vacantes: SafeGetInt(dr, "OpenPositions")
                        );
                    }
                }
            }
            return (0, 0, 0);
        }

        public List<(string DepartmentName, int Total)> GetDepartmentCounts()
        {
            var list = new List<(string, int)>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetDepartmentCounts", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add((
                            DepartmentName: SafeGetString(dr, "DepartmentName"),
                            Total: SafeGetInt(dr, "TotalEmployees")
                        ));
                    }
                }
            }
            return list;
        }

        // -------------------------
        // MAPPERS + SAFE READERS
        // -------------------------

        private Employee MapEmployeeFromReport(SqlDataReader dr)
        {
            return new Employee
            {
                EmployeeId = SafeGetInt(dr, "EmployeeId"),
                FullNames = SafeGetString(dr, "FullNames"),
                DPI = SafeGetString(dr, "DPI"),
                BirthDate = SafeGetDate(dr, "BirthDate"),
                Gender = SafeGetChar(dr, "Gender"),
                HireDate = SafeGetDate(dr, "HireDate"),
                IsActive = SafeGetBool(dr, "EmployeeStatus"),
                DepartmentName = SafeGetString(dr, "DepartmentName"),
                DepartmentId = SafeGetInt(dr, "DepartmentId"),
                Tenure = SafeGetString(dr, "Tenure")
            };
        }

        public Department GetDepartmentById(int departmentId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_GetDepartmentById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = departmentId;

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new Department
                        {
                            DepartmentId = SafeGetInt(dr, "DepartmentId"),
                            Name = SafeGetString(dr, "Name"),
                            IsActive = SafeGetBool(dr, "IsActive")
                        };
                    }
                }
            }

            return null;
        }



        private static string SafeGetString(SqlDataReader dr, string col)
            => dr[col] == DBNull.Value ? null : dr[col].ToString();

        private static int SafeGetInt(SqlDataReader dr, string col)
            => dr[col] == DBNull.Value ? 0 : Convert.ToInt32(dr[col]);

        private static bool SafeGetBool(SqlDataReader dr, string col)
            => dr[col] != DBNull.Value && Convert.ToBoolean(dr[col]);

        private static DateTime SafeGetDate(SqlDataReader dr, string col)
            => dr[col] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr[col]);

        private static char SafeGetChar(SqlDataReader dr, string col)
            => dr[col] == DBNull.Value ? '\0' : Convert.ToChar(dr[col]);



    }

}
