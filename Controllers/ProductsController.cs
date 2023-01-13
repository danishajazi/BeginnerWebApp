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
using WebApiDemo1.DTO.InputDTO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;
        public ProductsController(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new SqlConnection(_Configuration.GetConnectionString("ProductDBCConnection").ToString());
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Products", sqlConnection);
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
        [Route("GetProductsCount")]
        public IActionResult GetProductsCount()
        {
            string getQuery = "SELECT COUNT(*) FROM Products";

            var sqlCommand = new SqlCommand(getQuery, sqlConnection);
           
            sqlConnection.Open();
            int productCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(productCount);           
        }

        [HttpGet]
        [Route("GetBrandNameById/{productId}")]
        public IActionResult GetBrandNameById(int productId)
        {
            string stringQuery = @"SELECT BrandName FROM Products where id = @productId";

            var sqlCommand = new SqlCommand(stringQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@productId", productId);

            sqlConnection.Open();
            string productBrandNane = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(productBrandNane);
        }

        [HttpGet]
        [Route("GetProductsDetail/{brandName}/{productName}")]
        public IActionResult GetProductsDetailByProductNameBrandName(string brandName, string productName)
        {
            string query = $"SELECT * FROM Products WHERE BrandName Like '%{brandName}%' ";
            if (!string.IsNullOrEmpty(productName))
            {
                query = query + "AND ProductName Like '%{productName}%' ";
            }

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
        [Route("GetProductByPriceRange/{minimumPrice}/{maximumPrice}")]
        public IActionResult GetProductByPriceRange(int minimumPrice, int maximumPrice)
        {
            string stringQuery = $@" SELECT * FROM Products 
                                    WHERE Price BETWEEN {minimumPrice} AND {maximumPrice}
                                    ORDER BY Price ";
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
        [Route("ProductAdd")]
        public IActionResult ProductAdd([FromBody] ProductDto product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string stringQuery = $@"
                    INSERT INTO Products(ProductName, BrandName, Size, Color, Fit, Fabric, Category, Discount, Price)
                    VALUES (@ProductName, @BrandName, @Size, @Color, @Fit, @Fabric, @Category, @Discount, @Price)
                    Select Scope_Identity() ";

                    var sqlCommand = new SqlCommand(stringQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@ProductName", product.ProductName);
                    sqlCommand.Parameters.AddWithValue("@BrandName", product.BrandName);
                    sqlCommand.Parameters.AddWithValue("@Size", product.Size);
                    sqlCommand.Parameters.AddWithValue("@Color", product.Color);
                    sqlCommand.Parameters.AddWithValue("@Fit", product.Fit);
                    sqlCommand.Parameters.AddWithValue("@Fabric", product.Fabric);
                    sqlCommand.Parameters.AddWithValue("@Category", product.Category);
                    sqlCommand.Parameters.AddWithValue("@Discount", product.Discount);
                    sqlCommand.Parameters.AddWithValue("@Price", product.Price);

                    sqlConnection.Open();
                    product.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                    sqlConnection.Close();

                    return Ok(product.Id);
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
