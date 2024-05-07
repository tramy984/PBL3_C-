using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata;

namespace PBL3_vstim.Controllers
{
    public class DangnhapController : Controller
    {
        public int ID;
        public IActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Dangnhap(string TDN, string MK)
        {
            TDN = Request.Form["TDN"];
            MK = Request.Form["MK"];
            if (TDN == null || MK == null)
            {
                ViewBag.thongbao("Ban chua nhap du thong tin");
                return View();
            }
            else
            {
                string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=HDTN;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Tạo câu lệnh SQL để kiểm tra xem tên đăng nhập đã tồn tại chưa
                    string query = "SELECT COUNT(*) FROM Taikhoan WHERE Tendangnhap = @Username  AND Matkhau=@mk";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Username", TDN);
                    cmd.Parameters.AddWithValue("@mk", MK);
                    int count = (int)cmd.ExecuteScalar();
                   
                    if (count == 0)
                    {
                        ViewBag.thongbao = "Vui lòng nhập lại thông tin.";
                        return View();
                    }
                    else
                    {
                        query = "SELECT Vaitro,ID,Trangthai FROM TaiKhoan WHERE Tendangnhap = @Username ";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@Username", TDN);
                        SqlDataReader reader = cmd.ExecuteReader();
                        int VT=-1;
                        bool TT = true ;
                        while(reader.Read()) {
                            VT = Int32.Parse(reader["Vaitro"].ToString());
                            ID = Int32.Parse(reader["ID"].ToString());
                            TT = Boolean.Parse(reader["Trangthai"].ToString());
                        }
                        if (TT == false)
                        {
                            ViewBag.thongbao = "Tài khoản đã bị khoá.";
                            return View();
                        }
                        else
                        {
                            if (VT == 1) return Redirect("/Canhan/Trangchucanhan");
                            else if (VT == 2) return Redirect("/Tochuc/Trangchutochuc");
                            else return View();
                        }
                    }
                }
            }

        }
    }
}
