using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Data;
using WebApiDemo1.DTO.InputDTO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;
        public EmployeesController(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new SqlConnection(_Configuration.GetConnectionString("EmployeesDBConnection").ToString());
        }

        [HttpGet]
        [Route("GetAllEmployees")]
        public IActionResult GetAllEmployees()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Employees", sqlConnection);
            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return Ok(JsonConvert.SerializeObject(dataTable));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("GetEmployeesCount")]
        public IActionResult GetEmployeesCount()
        {
            string insertQuery = "SELECT COUNT(*) FROM Employees ";

            var sqlCommand = new SqlCommand(insertQuery, sqlConnection);

            sqlConnection.Open();
            int employeeCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(employeeCount);
        }

        [HttpGet]
        [Route("GetEmployeeFullNameById/{EmployeeId}")]
        public IActionResult GetEmployeeFullNameById(int employeeId)
        {
            string stringQuery = "SELECT FullName FROM Employee WHERE Id = @EmployeeId";

            var sqlCommand = new SqlCommand(stringQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Employee", employeeId);

            sqlConnection.Open();
            string employeeFullName = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(employeeFullName);
        }

        [HttpGet]
        [Route("GetEmployeeDetail/{gender}/{salary}")]
        public IActionResult GetEmployeeDetailByGenderBySalary(string gender, int salary)
        {
            string query = $"SELECT * FROM Employees WHERE Gender = '{gender}' AND Salary > '{salary}' ";
            SqlDataAdapter sqlDataAdapter = new(query, sqlConnection);
            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return Ok(JsonConvert.SerializeObject(dataTable));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("GetEmployeeBySalaryRange/{minimumSalary}/{maximumSalary}")]
        public IActionResult GetEmployeeBySalaryRange(int minimumSalary, int maximumSalary)
        {
            string query = $@" SELECT * FROM Employees 
                                    WHERE Salary BETWEEN {minimumSalary} AND {maximumSalary}
                                    ORDER BY Salary ";
            SqlDataAdapter sqlDataAdapter = new(query, sqlConnection);
            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return Ok(JsonConvert.SerializeObject(dataTable));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("EmployeeRegister")]
        public IActionResult EmployeeRegister([FromBody] EmployeeDto employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string insertQuery = $@"
                    INSERT INTO Employees(FullName, Email, Gender, Salary)
                    VALUES (@FullName, @Email, @Gender, @Salary)
                    Select Scope_Identity() ";

                    var sqlCommand = new SqlCommand(insertQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@FullName", employee.FullName);
                    sqlCommand.Parameters.AddWithValue("@Email", employee.Email);
                    sqlCommand.Parameters.AddWithValue("@Gender", employee.Gender);
                    sqlCommand.Parameters.AddWithValue("@Salary", employee.Salary);

                    sqlConnection.Open();
                    employee.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                    sqlConnection.Close();

                    return Ok(employee.Id);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", @"Unable to save changes. 
                    Try again, and if the problem persists 
                    see your system administrator.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

