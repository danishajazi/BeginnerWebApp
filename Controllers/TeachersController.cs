using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
//using WebApiDemo2.DTO;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;
        public TeachersController(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new SqlConnection(_Configuration.GetConnectionString("dbcs").ToString());
        }

        [HttpGet]
        [Route("GetTeachers")]
        public string GetTeachers()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Teachers", sqlConnection);
            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dataTable);
            }
            else
            {
                return "Teachers Data Not Found";
            }
        }

        [HttpGet]
        [Route("GetTeachersCount")]
        public string GetTeachersCount()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Teachers", sqlConnection);
            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows.Count.ToString();
            }
            else
            {
                return "0";
            }
        }

        [HttpGet]
        [Route("GetTeacherDetail/{teacherId}")]
        public string GetTeacherDetail(int teacherId)
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Teachers WHERE Id =" + teacherId, sqlConnection);
            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dataTable);
            }
            else
            {
                return "Teacher Not Found";
            }
        }

        [HttpGet]
        [Route("GetTeacherDetail/{teacherName}/{department}/{teachersName}")]
        public string GetTeacherDetail(string department, string teacherName, string teachersName)
        {
            string query = $"SELECT * FROM Teachers WHERE (FullName Like '%{teacherName}%' And Department Like '%{department}%') OR (FullName Like '%{teachersName}%')";
            SqlDataAdapter sqlDataAdapter = new(query, sqlConnection);
            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dataTable);
            }
            else
            {
                return "Teacher Detail Not Found";
            }
        }

        [HttpGet]
        [Route("GetTeacherBySalaryRange/{minimumSalary}/{maximumSalary}")]
        public string GetTeacherBySalaryRange(int minimumSalary, int maximumSalary)
        {
            string query = $@" SELECT * FROM Teachers 
                                    WHERE Salary BETWEEN {minimumSalary} AND {maximumSalary}
                                    ORDER BY Salary ";
            SqlDataAdapter sqlDataAdapter = new(query, sqlConnection);
            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dataTable);
            }
            else
            {
                return "Teacher Not Found";
            }
        }
    }
}
