using Microsoft.AspNetCore.Mvc;
using Static_crud.Models;
using System.Data.SqlClient;
using System.Data;

namespace Static_crud.Controllers
{
    public class OrderController : Controller
    {
        private  IConfiguration configuration;

        public OrderController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region GetOrders
        public IActionResult Order()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[GetOrders]";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();
            return View(table);
        }
        #endregion

        #region DeleteOrder
        public IActionResult OrderDelete(int OrderID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[DeleteOrder]";
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = "Cannot delete this Order because it is associated with a user. Please delete the associated user first.";
            }
            return RedirectToAction("Order");
        }
        #endregion

        #region AddOrder
        public IActionResult Add_Order(int? OrderID = null)
        {
            OrderModel modelOrder = new OrderModel();
            string connectionString = configuration.GetConnectionString("ConnectionString");

            #region Dropdown
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = CommandType.StoredProcedure;
            command1.CommandText = "PR_Customer_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader1);
            List<CustomerDropDownModel> customerList = new List<CustomerDropDownModel>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                CustomerDropDownModel customerDropDownModel = new CustomerDropDownModel
                {
                    CustomerID = Convert.ToInt32(dataRow["CustomerID"]),
                    CustomerName = dataRow["CustomerName"].ToString()
                };
                customerList.Add(customerDropDownModel);
            }
            ViewBag.CustomerList = customerList;
            connection1.Close();
            #endregion

            #region AddEdit
            if (OrderID != null)
            {
                SqlConnection connection2 = new SqlConnection(connectionString);
                connection2.Open();
                SqlCommand command2 = connection2.CreateCommand();
                command2.CommandType = CommandType.StoredProcedure;
                command2.CommandText = "[dbo].[PR_Order_SelectByPK]";
                command2.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;

                SqlDataReader reader2 = command2.ExecuteReader();
                if (reader2.HasRows)
                {
                    reader2.Read();
                    modelOrder.OrderID = Convert.ToInt32(reader2["OrderID"]);
                    modelOrder.Ordernumber = Convert.ToInt32(reader2["Ordernumber"]);

                    modelOrder.OrderDate = Convert.ToDateTime(reader2["OrderDate"]);
                    modelOrder.CustomerID = Convert.ToInt32(reader2["CustomerID"]);
                    modelOrder.PaymentMode = reader2["PaymentMode"].ToString();
                    modelOrder.TotalAmount = Convert.ToDecimal(reader2["TotalAmount"]);
                    modelOrder.ShippingAddress = reader2["ShippingAddress"].ToString();
                    modelOrder.UserID = Convert.ToInt32(reader2["UserID"]);
                }
                connection2.Close();
            }
            return View(modelOrder);
            #endregion
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(OrderModel modelOrder)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (modelOrder.OrderID == 0)
            {
                command.CommandText = "[dbo].[InsertOrder]";
            }
            else
            {
                command.CommandText = "[dbo].[UpdateOrder]";
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = modelOrder.OrderID;
            }

            command.Parameters.Add("@Ordernumber", SqlDbType.Int).Value = modelOrder.Ordernumber;
            command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = modelOrder.OrderDate;
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = modelOrder.CustomerID;
            command.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = modelOrder.PaymentMode;
            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = modelOrder.TotalAmount;
            command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = modelOrder.ShippingAddress;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelOrder.UserID;

            int result = command.ExecuteNonQuery();
            if (result > 0)
            {
                TempData["OrderInsertMsg"] = modelOrder.OrderID == null ? "Record Inserted Successfully" : "Record Updated Successfully";
            }
            connection.Close();

            return RedirectToAction("Order");
        }
        #endregion
    }
}
