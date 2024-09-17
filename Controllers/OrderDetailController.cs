using Microsoft.AspNetCore.Mvc;
using Static_crud.Models;
using System.Data.SqlClient;
using System.Data;

namespace Static_crud.Controllers
{
    public class OrderDetailController : Controller
    {
        private IConfiguration _configuration;

        public OrderDetailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region GetAll
        public IActionResult OrderDetail()
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[GetOrderDetails]";
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
        public IActionResult Delete_OrderDetail(int orderDetailID)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[DeleteOrderDetail]";
            command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = orderDetailID;
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = "Cannot delete this Detail of Order because it is associated with a user. Please delete the associated user first.";
            }
            finally
            {
                connection.Close();
            }

            return RedirectToAction("OrderDetail");
        }
        #endregion

        #region PopulateDropDowns
        private void PopulateDropDowns()
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            List<ProductDropDownModel> productList = new List<ProductDropDownModel>();
            List<OrderDropDownModel> orderList = new List<OrderDropDownModel>();

            #region DropdownProductID
            SqlConnection connectionProduct = new SqlConnection(connectionString);
            SqlCommand commandProduct = new SqlCommand("PR_Product_DropDown", connectionProduct);
            commandProduct.CommandType = CommandType.StoredProcedure;

            try
            {
                connectionProduct.Open();
                SqlDataReader readerProduct = commandProduct.ExecuteReader();
                DataTable dataTableProduct = new DataTable();
                dataTableProduct.Load(readerProduct);

                foreach (DataRow row in dataTableProduct.Rows)
                {
                    ProductDropDownModel productDropDownModel = new ProductDropDownModel
                    {
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        ProductName = row["ProductName"].ToString()
                    };
                    productList.Add(productDropDownModel);
                }
            }
            finally
            {
                connectionProduct.Close();
            }
            #endregion

            #region DropdownOrderID
            SqlConnection connectionOrder = new SqlConnection(connectionString);
            SqlCommand commandOrder = new SqlCommand("PR_Order_DropDown", connectionOrder);
            commandOrder.CommandType = CommandType.StoredProcedure;

            try
            {
                connectionOrder.Open();
                SqlDataReader readerOrder = commandOrder.ExecuteReader();
                DataTable dataTableOrder = new DataTable();
                dataTableOrder.Load(readerOrder);

                foreach (DataRow row in dataTableOrder.Rows)
                {
                    OrderDropDownModel orderDropDownModel = new OrderDropDownModel
                    {
                        OrderID = Convert.ToInt32(row["OrderID"]),
                        Ordernumber = Convert.ToInt32(row["Ordernumber"])
                    };
                    orderList.Add(orderDropDownModel);
                }
            }
            finally
            {
                connectionOrder.Close();
            }
            #endregion

            ViewBag.ProductList = productList;
            ViewBag.OrderList = orderList;
        }
        #endregion

        #region Add or Edit
        public IActionResult Add_OrderDetail(int? orderDetailID)
        {
            PopulateDropDowns();
            OrderDetailModel orderDetailModel = new OrderDetailModel();

            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[PR_OrderDetail_SelectByPK]";
            command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = (object)orderDetailID ?? DBNull.Value;

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows && orderDetailID != null)
                {
                    reader.Read();
                    orderDetailModel.OrderDetailID = Convert.ToInt32(reader["OrderDetailID"]);
                    orderDetailModel.OrderID = Convert.ToInt32(reader["OrderID"]);
                    orderDetailModel.ProductID = Convert.ToInt32(reader["ProductID"]);
                    orderDetailModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                    orderDetailModel.Amount = Convert.ToDecimal(reader["Amount"]);
                    orderDetailModel.TotalAmount = Convert.ToDecimal(reader["TotalAmount"]);
                    orderDetailModel.UserID = Convert.ToInt32(reader["UserID"]);
                }
            }
            finally
            {
                connection.Close();
            }

            return View(orderDetailModel);
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(OrderDetailModel modelOrderDetail)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (modelOrderDetail.OrderDetailID != 0 && modelOrderDetail.OrderDetailID != null)
            {
                command.CommandText = "[dbo].[UpdateOrderDetail]";
                command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = modelOrderDetail.OrderDetailID;
            }
            else
            {
                command.CommandText = "[dbo].[InsertOrderDetail]";
            }

            command.Parameters.Add("@OrderID", SqlDbType.Int).Value = modelOrderDetail.OrderID;
            command.Parameters.Add("@ProductID", SqlDbType.Int).Value = modelOrderDetail.ProductID;
            command.Parameters.Add("@Quantity", SqlDbType.Int).Value = modelOrderDetail.Quantity;
            command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = modelOrderDetail.Amount;
            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = modelOrderDetail.TotalAmount;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelOrderDetail.UserID;

            try
            {
                connection.Open();
                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    TempData["OrderDetailInsertMsg"] = modelOrderDetail.OrderDetailID == null ? "Record Inserted Successfully" : "Record Updated Successfully";
                }
            }
            finally
            {
                connection.Close();
            }

            return RedirectToAction("OrderDetail");
        }
        #endregion
    }
}
