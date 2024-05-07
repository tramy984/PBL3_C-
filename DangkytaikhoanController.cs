using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PBL3_vstim.Models;
using System.ComponentModel.DataAnnotations;
using PBL3_vstim.ViewsModel;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Azure.Messaging;
using System.Diagnostics.Eventing.Reader;

namespace PBL3_vstim.Controllers
{
    public class DangkytaikhoanController : Controller
    {
        HdtnContext db = new HdtnContext();

        public DangkytaikhoanController() {
        }

        [HttpGet]
        public ActionResult Dangkytaikhoan()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangkytaikhoan(DKTK model, string myComboBox)
        {

            string username = model.Tendangnhap;
            string password = model.Matkhau;
            string confirmPassword = model.NLMK;
            string Ten = model.Ten;
            string SDT = model.Sdt;
            string Email = model.Email;
            string ktrSDT = @"^0\d{9}$";
            string Vaitro = null;
            if (myComboBox == "CN")
            {
                Vaitro = "Cá nhân";//Vaitro=1
            }
            else if (myComboBox == "TC")
            {
                Vaitro = "Tổ chức";//Vaitro=2
            }

            if (string.IsNullOrEmpty(username) == true || password.Length < 6 || password.Length > 20 ||
               confirmPassword != password || string.IsNullOrEmpty(Ten) == true ||
               Regex.IsMatch(ktrSDT, SDT) == false) /*{ return View(); }
            else*/
            {

                // Kết nối tới cơ sở dữ liệu (sử dụng connection string của bạn)
                string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=HDTN;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Tạo câu lệnh SQL để kiểm tra xem tên đăng nhập đã tồn tại chưa
                    string query = "SELECT COUNT(*) FROM Taikhoan WHERE Tendangnhap = @Username";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Username", username);
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        ViewBag.thongbao = "Tên đăng nhập đã tồn tại.";
                        return View();
                    }

                  
                    if(Vaitro=="Cá nhân")
                    {
                        query = "INSERT INTO Taikhoan (Tendangnhap, Matkhau,Trangthai,Vaitro) VALUES (@Username, @Password,1,1)";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.ExecuteNonQuery();
                        query = "SELECT ID FROM Taikhoan WHERE Tendangnhap=@TDN";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@TDN", username);
                        int id = (int)cmd.ExecuteScalar();
                        query = "INSERT INTO Canhan(IDcanhan,Ten,Sdt,Email) VALUES (@ID,@Ten,@Sdt,@email)";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@ten", Ten);
                        cmd.Parameters.AddWithValue("@Sdt", SDT);
                        cmd.Parameters.AddWithValue("@email", Email);
                        cmd.ExecuteNonQuery();
                    }
                    if (Vaitro == "Tổ chức")
                    {
                        query = "INSERT INTO Taikhoan (Tendangnhap, Matkhau,Trangthai,Vaitro) VALUES (@Username, @Password,1,2)";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.ExecuteNonQuery();
                        query = "SELECT ID FROM Taikhoan WHERE Tendangnhap=@TDN";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@TDN", username);
                        int id = (int)cmd.ExecuteScalar();
                        query = "INSERT INTO Tochuc(IDtochuc,Ten,Sdt,Email) VALUES (@ID,@Ten,@Sdt,@email)";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@ten", Ten);
                        cmd.Parameters.AddWithValue("@Sdt", SDT);
                        cmd.Parameters.AddWithValue("@email", Email);
                        cmd.ExecuteNonQuery();
                    }
                    if(Vaitro==null)
                    { 

                        return View();
                    }
                    // Xuất thông báo đăng ký thành công
                    ViewBag.Message = "Đăng ký thành công.";
                    return Redirect("/Dangnhap/Dangnhap");
                }
                
            }
            else { return View(); }
        }

    }

}

      
