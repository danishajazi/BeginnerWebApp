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
    public class OrdersController : ControllerBase
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;
        public OrdersController(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new SqlConnection(_Configuration.GetConnectionString("OrderDBCConnection").ToString());
        }

        [HttpGet]
        [Route("GetAllOrders")]
        public IActionResult GetAllOrders()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Orders", sqlConnection);
            var dataTable = new DataTable();
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
        [Route("GetOrdersCount")]
        public IActionResult GetOrdersCount()
        {
            string stringQuery = "SELECT COUNT(*) FROM Orders";

            var sqlCommand = new SqlCommand(stringQuery, sqlConnection);

            sqlConnection.Open();
            int orderCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(orderCount);
        }

        [HttpGet]
        [Route("GetProductNameById/{orderId}")]
        public IActionResult GetProductNameById(int orderId)
        {
            string stringQuery = @"SELECT ProductName FROM Orders where id = @orderId";

            var sqlCommand = new SqlCommand(stringQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@orderId", orderId);

            sqlConnection.Open();
            string productName = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(productName);
        }

        [HttpGet]
        [Route("GetOrderByAmountRange/{minimumAmount}/{maximumAmount}")]
        public IActionResult GetOrderByAmountRange(int minimumAmount, int maximumAmount)
        {
            string stringQuery = $@" SELECT * FROM Orders 
                                    WHERE Amount BETWEEN {minimumAmount} AND {maximumAmount}
                                    ORDER BY Amount ";
            SqlDataAdapter sqlDataAdapter = new(stringQuery, sqlConnection);
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
        [Route("CreateNewOrders")]
        public IActionResult CreateNewOrders([FromBody] OrderDto order)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string stringQuery = $@"
                    INSERT INTO Orders(CustomerId, OrderDate, Amount, ProductName)
                    VALUES (@CustomerId, @OrderDate, @Amount, @ProductName)
                    Select Scope_Identity() ";

                    var sqlCommand = new SqlCommand(stringQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@CustomerId", order.CustomerId);
                    sqlCommand.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                    sqlCommand.Parameters.AddWithValue("@Amount", order.Amount);
                    sqlCommand.Parameters.AddWithValue("@ProductName", order.ProductName);

                    sqlConnection.Open();
                    order.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                    sqlConnection.Close();

                    return Ok(order.Id);
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
