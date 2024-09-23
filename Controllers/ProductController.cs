using Microsoft.AspNetCore.Mvc;
using Static_crud.Models;
using System.Data.SqlClient;
using System.Data;
using Static_crud.BAL;

namespace Static_crud.Controllers
{
    [CheckAccess]
    public class ProductController : Controller
    {
        #region Configuration
        private IConfiguration configuration;

        public ProductController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region SelectAll
        public IActionResult Product()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[GetProducts]";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();
            return View(table);
        }
        #endregion  

        #region Delete
        public IActionResult ProductDelete(int ProductID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[DeleteProduct]";
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = "Cannot delete this product because it is associated with a user. Please delete the associated user first.";
            }
            return RedirectToAction("Product");
        }
        #endregion

        #region Add or Edit
        public IActionResult Add_Product(int? ProductID)
        {
            ProductModel modelProduct = new ProductModel();

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[PR_Product_SelectByPK]";
            command.Parameters.Add("@ProductID", SqlDbType.Int).Value = (object)ProductID ?? DBNull.Value;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows && ProductID != null)
            {
                reader.Read();
                modelProduct.ProductID = Convert.ToInt32(reader["ProductID"]);
                modelProduct.ProductName = reader["ProductName"].ToString();
                modelProduct.ProductPrice = Convert.ToDecimal(reader["ProductPrice"]);
                modelProduct.ProductCode = reader["ProductCode"].ToString();
                modelProduct.Description = reader["Description"].ToString();
                modelProduct.UserID = Convert.ToInt32(reader["UserID"]);
            }
            connection.Close();
            return View(modelProduct);
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(ProductModel modelProduct)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (modelProduct.ProductID == null)
            {
                command.CommandText = "[dbo].[InsertProduct]";
            }
            else
            {
                command.CommandText = "[dbo].[UpdateProduct]";
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = modelProduct.ProductID;
            }

            command.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = modelProduct.ProductName;
            command.Parameters.Add("@ProductPrice", SqlDbType.Decimal).Value = modelProduct.ProductPrice;
            command.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = modelProduct.ProductCode;
            command.Parameters.Add("@Description", SqlDbType.VarChar).Value = modelProduct.Description;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelProduct.UserID;

            if (command.ExecuteNonQuery() > 0)
            {
                TempData["ProductInsertMsg"] = modelProduct.ProductID == null ? "Record Inserted Successfully" : "Record Updated Successfully";
            }

            
            connection.Close();

            return RedirectToAction("Product");
        }
        #endregion
    }
}
