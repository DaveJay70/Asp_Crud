using Microsoft.AspNetCore.Mvc;
using Static_crud.Models;
using System.Data.SqlClient;
using System.Data;
using Static_crud.BAL;

namespace Static_crud.Controllers
{
    [CheckAccess]
    public class CustomerController : Controller
    {
        private  IConfiguration _configuration;

        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get
        public IActionResult Customer()
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[GetCustomers]";

            DataTable table = new DataTable();

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                table.Load(reader);
            }
            finally
            {
                connection.Close();
            }

            return View(table);
        }
        #endregion

        #region Delete
        public IActionResult DeleteCustomer(int customerID)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[DeleteCustomer]";
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = "Cannot delete this Customer because it is associated with a user. Please delete the associated user first.";
            }
            finally
            {
                connection.Close();
            }

            return RedirectToAction("Customer");
        }
        #endregion

        #region AddEdit
        public IActionResult Add_Customer(int? customerID)
        {
            CustomerModel modelCustomer = new CustomerModel();
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[PR_Customer_SelectByPK]";
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = (object)customerID ?? DBNull.Value;

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows && customerID != null)
                {
                    reader.Read();
                    modelCustomer.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                    modelCustomer.CustomerName = reader["CustomerName"].ToString();
                    modelCustomer.HomeAddress = reader["HomeAddress"].ToString();
                    modelCustomer.Email = reader["Email"].ToString();
                    modelCustomer.MobileNo = reader["MobileNo"].ToString();
                    modelCustomer.GST_NO = reader["GST_NO"].ToString();
                    modelCustomer.CityName = reader["CityName"].ToString();
                    modelCustomer.PinCode = reader["PinCode"].ToString();
                    modelCustomer.NetAmount = Convert.ToDecimal(reader["NetAmount"]);
                    modelCustomer.UserID = Convert.ToInt32(reader["UserID"]);
                }
            }
            finally
            {
                connection.Close();
            }

            return View(modelCustomer);
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(CustomerModel modelCustomer)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (modelCustomer.CustomerID == null || modelCustomer.CustomerID == 0)
            {
                command.CommandText = "[dbo].[InsertCustomer]";
            }
            else
            {
                command.CommandText = "[dbo].[UpdateCustomer]";
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = modelCustomer.CustomerID;
            }

            command.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = modelCustomer.CustomerName;
            command.Parameters.Add("@HomeAddress", SqlDbType.VarChar).Value = modelCustomer.HomeAddress;
            command.Parameters.Add("@Email", SqlDbType.VarChar).Value = modelCustomer.Email;
            command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = modelCustomer.MobileNo;
            command.Parameters.Add("@GST_NO", SqlDbType.VarChar).Value = modelCustomer.GST_NO;
            command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = modelCustomer.CityName;
            command.Parameters.Add("@PinCode", SqlDbType.VarChar).Value = modelCustomer.PinCode;
            command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = modelCustomer.NetAmount;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelCustomer.UserID;

            try
            {
                connection.Open();
                if (command.ExecuteNonQuery() > 0)
                {
                    TempData["CustomerInsertMsg"] = modelCustomer.CustomerID == null || modelCustomer.CustomerID == 0 ? "Record Inserted Successfully" : "Record Updated Successfully";
                }
            }
            finally
            {
                connection.Close();
            }

            return RedirectToAction("Customer");
        }
        #endregion
    }
}
