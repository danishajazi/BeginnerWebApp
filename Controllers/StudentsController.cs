using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;

        public StudentsController(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new(_Configuration.GetConnectionString("TestDBConnection").ToString());
        }

        [HttpGet]
        [Route("GetAllStudents")]
        public IActionResult GetAllStudents()
        {
            SqlDataAdapter sqlDataAdapter = new(@"SELECT Students.Name, Students.EnrollmentId, 
                                                Students.RollNumber, Colleges.CollegeName, 
                                                Courses.CourceName 
                                                FROM Students 
                                                INNER JOIN Courses ON Students.CourseId = Courses.Id INNER JOIN
                                                Colleges ON Students.CollegeId = Colleges.Id", sqlConnection);
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
        [Route("GetStudentDetailByEnrollmentId/{enrollmentId}")]
        public IActionResult GetStudentDetailByEnrollmentId(int enrollmentId)
        {
            SqlDataAdapter sqlDataAdapter = new(@"SELECT Students.Name, Students.EnrollmentId, 
                                                Students.RollNumber, Colleges.CollegeName, 
                                                Courses.CourceName 
                                                FROM Students 
                                                INNER JOIN Courses ON Students.CourseId = Courses.Id INNER JOIN
                                                Colleges ON Students.CollegeId = Colleges.Id
                                                WHERE EnrollmentId = " + enrollmentId, sqlConnection);
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
        [Route("GetStudentCollegeNameByEnrollmentId/{enrollmentId}")]
        public IActionResult GetStudentCollegeNameByEnrollmentId(int enrollmentId)
        {
            string stringQuery = @"SELECT Colleges.CollegeName 
                                   FROM Students
                                   INNER JOIN Colleges ON Students.CollegeId = Colleges.Id	
                                   WHERE EnrollmentId = @enrollmentId";

            var sqlCommand = new SqlCommand(stringQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@enrollmentId", enrollmentId);

            sqlConnection.Open();
            string collegeName = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(collegeName);
        }

        [HttpGet]
        [Route("GetStudentCourseNameByEnrollmentId/{enrollmentId}")]
        public IActionResult GetStudentCourseNameByEnrollmentId(int enrollmentId)
        {
            string stringQuery = @"SELECT Courses.CourceName 
                                   FROM Students
                                   INNER JOIN Courses ON Students.CourseId = Courses.Id	
                                   WHERE EnrollmentId = @enrollmentId";

            var sqlCommand = new SqlCommand(stringQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@enrollmentId", enrollmentId);

            sqlConnection.Open();
            string courseName = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(courseName);
        }

        [HttpGet]
        [Route("GetStudentsNameListByCourseId/{courseId}")]
        public IActionResult GetStudentsNameListByCourseId(int courseId)
        {
            SqlDataAdapter sqlDataAdapter = new(@"SELECT Students.Name FROM Students
                                                  INNER JOIN Courses ON Students.CourseId = Courses.Id
                                                  WHERE CourseId =" + courseId, sqlConnection);
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
        [Route("GetStudentsNameListByCourseId2/{courseId}")]
        public IActionResult GetStudentsNameListByCourseId2(int courseId)
        {
            SqlDataAdapter sqlDataAdapter = new(@"SELECT Students.Name FROM Students
                                                  INNER JOIN Courses ON Students.CourseId = Courses.Id
                                                  WHERE CourseId =" + courseId, sqlConnection);
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
    }
}

