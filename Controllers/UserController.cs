using Microsoft.AspNetCore.Mvc;
using Static_crud.Models;
using System.Data.SqlClient;
using System.Data;
using Static_crud.BAL;

namespace Static_crud.Controllers
{
    public class UserController : Controller
    {
        private IConfiguration configuration;

        public UserController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        [CheckAccess]
        #region GetUser
        public IActionResult User()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[GetNewUsers]";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();

            return View(table);
        }
        #endregion

        [CheckAccess]
        #region Delete
        public IActionResult UserDelete(int UserID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[DeleteNewUser]";
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = ex.Message;
            }
            return RedirectToAction("User");
        }
        #endregion

        [CheckAccess]
        #region Add or Edit
        public IActionResult Add_User(int? UserID)
        {
            UserModel modelUser = new UserModel();


            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[PR_User_SelectByPK]";
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = (object)UserID ?? DBNull.Value;

            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows && UserID != null)
            {
                reader.Read();
                modelUser.UserID = Convert.ToInt32(reader["UserID"]);
                modelUser.UserName = reader["UserName"].ToString();
                modelUser.Email = reader["Email"].ToString();
                modelUser.Password = reader["Password"].ToString();
                modelUser.MobileNo = reader["MobileNo"].ToString();
                modelUser.Address = reader["Address"].ToString();
                modelUser.IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]); 
            }
            connection.Close();

            return View(modelUser);
        }
        #endregion

        [CheckAccess]
        #region Save
        [HttpPost]
        public IActionResult Save(UserModel modelUser)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (modelUser.UserID == null || modelUser.UserID == 0)
            {
                command.CommandText = "[dbo].[InsertNewUser]";
            }
            else
            {
                command.CommandText = "[dbo].[UpdateNewUser]";
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelUser.UserID;
            }

            command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = modelUser.UserName;
            command.Parameters.Add("@Email", SqlDbType.VarChar).Value = modelUser.Email;
            command.Parameters.Add("@Password", SqlDbType.VarChar).Value = modelUser.Password;
            command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = modelUser.MobileNo;
            command.Parameters.Add("@Address", SqlDbType.VarChar).Value = modelUser.Address;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = modelUser.IsActive;

            if (command.ExecuteNonQuery() > 0)
            {
                TempData["UserInsertMsg"] = modelUser.UserID == null ? "Record Inserted Successfully" : "Record Updated Successfully";
            }
            connection.Close();

            return RedirectToAction("User");
        }
        #endregion

        #region Register
        public IActionResult UserRegister(UserModel userRegisterModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "[dbo].[PR_User_Register]";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userRegisterModel.UserName;
                    sqlCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value = userRegisterModel.Email;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userRegisterModel.Password;
                    sqlCommand.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userRegisterModel.MobileNo;
                    sqlCommand.Parameters.Add("@Address", SqlDbType.VarChar).Value = userRegisterModel.Address;
                    sqlCommand.ExecuteNonQuery();
                    return RedirectToAction("Login", "User");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Register");
            }

            return RedirectToAction("Register");
        }
        #endregion

        #region Login
        public IActionResult UserLogin(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Login";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Invaild UserName or Password";
                    }

                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return RedirectToAction("Login");
        }
        #endregion

        #region Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session");
            return RedirectToAction("Login", "User");
        }
        #endregion

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
    }
}
