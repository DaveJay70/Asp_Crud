using Microsoft.AspNetCore.Mvc;
using Static_crud.Models;
using System.Data.SqlClient;
using System.Data;

namespace Static_crud.Controllers
{
    public class BillsController : Controller
    {
        private  IConfiguration _configuration;

        public BillsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region GetAll
        public IActionResult Bills()
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[GetBills]";

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
        public IActionResult DeleteBill(int billID)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[DeleteBill]";
            command.Parameters.Add("@BillID", SqlDbType.Int).Value = billID;

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = "Cannot delete this Bill because it is associated with a user. Please delete the associated user first.";
            }
            finally
            {
                connection.Close();
            }

            return RedirectToAction("Bills");
        }
        #endregion

        #region AddEdit
        public IActionResult AddBills(int? billID)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = CommandType.StoredProcedure;
            command1.CommandText = "PR_Order_DropDown";

            List<OrderDropDownModel> orderList = new List<OrderDropDownModel>();

            try
            {
                connection1.Open();
                SqlDataReader reader1 = command1.ExecuteReader();
                DataTable dataTable1 = new DataTable();
                dataTable1.Load(reader1);

                foreach (DataRow dataRow in dataTable1.Rows)
                {
                    OrderDropDownModel orderDropDownModel = new OrderDropDownModel
                    {
                        OrderID = Convert.ToInt32(dataRow["OrderID"])
                    };
                    orderList.Add(orderDropDownModel);
                }
                ViewBag.orderList = orderList;
            }
            finally
            {
                connection1.Close();
            }

            BillsModel modelBills = new BillsModel();
            SqlConnection connection2 = new SqlConnection(connectionString);
            SqlCommand command2 = connection2.CreateCommand();
            command2.CommandType = CommandType.StoredProcedure;
            command2.CommandText = "[dbo].[PR_Bills_SelectByPK]";
            command2.Parameters.Add("@BillID", SqlDbType.Int).Value = (object)billID ?? DBNull.Value;
            try
            {
                connection2.Open();
                SqlDataReader reader2 = command2.ExecuteReader();

                if (reader2.HasRows && billID != null)
                {
                    reader2.Read();
                    modelBills.BillID = Convert.ToInt32(reader2["BillID"]);
                    modelBills.BillNumber = reader2["BillNumber"].ToString();
                    modelBills.BillDate = Convert.ToDateTime(reader2["BillDate"]);
                    modelBills.OrderID = Convert.ToInt32(reader2["OrderID"]);
                    modelBills.TotalAmount = Convert.ToDecimal(reader2["TotalAmount"]);
                    modelBills.Discount = Convert.ToDecimal(reader2["Discount"]);
                    modelBills.NetAmount = Convert.ToDecimal(reader2["NetAmount"]);
                    modelBills.UserID = Convert.ToInt32(reader2["UserID"]);
                }
            }
            finally
            {
                connection2.Close();
            }

            return View(modelBills);
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(BillsModel modelBills)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (modelBills.BillID == null)
            {
                command.CommandText = "[dbo].[InsertBill]";
            }
            else
            {
                command.CommandText = "[dbo].[UpdateBill]";
                command.Parameters.Add("@BillID", SqlDbType.Int).Value = modelBills.BillID;
            }

            command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = modelBills.BillNumber;
            command.Parameters.Add("@BillDate", SqlDbType.DateTime).Value = modelBills.BillDate;
            command.Parameters.Add("@OrderID", SqlDbType.Int).Value = modelBills.OrderID;
            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = modelBills.TotalAmount;
            command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = modelBills.Discount;
            command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = modelBills.NetAmount;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelBills.UserID;

            try
            {
                connection.Open();
                if (command.ExecuteNonQuery() > 0)
                {
                    TempData["BillInsertMsg"] = modelBills.BillID == null ? "Record Inserted Successfully" : "Record Updated Successfully";
                }
            }
            finally
            {
                connection.Close();
            }

            return RedirectToAction("Bills");
        }
        #endregion
    }
}
