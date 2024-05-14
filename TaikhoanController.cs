using Microsoft.AspNetCore.Mvc;
using PBL3_QLHDTN.Models;
using PBL3_QLHDTN.ViewModel;

namespace PBL3_QLHDTN.Controllers
{
    public class TaikhoanController : Controller
    {
        public int ID;
        QlhdtnContext db = new QlhdtnContext();
        public IActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Dangnhap(Taikhoan user)
        {
            if (HttpContext.Session.GetString("Tendangnhap") == null)
            {

                var u = db.Taikhoans
                 .Where(model => model.Tendangnhap.Equals(user.Tendangnhap) && model.Matkhau.Equals(user.Matkhau))
                 .Select(model => new { Id = model.Id, Vaitro = model.Vaitro, Trangthai = model.Trangthai })
                 .FirstOrDefault();
                if (u == null)
                {
                    ViewBag.thongbao = "Thông tin không chính xác !";
                    return View(user);
                }
                else
                {
                    if (u.Trangthai == false)
                    {
                        ViewBag.thongbao = "Tài khoản đã bị khoá.";
                        return View(user);
                    }
                    else
                    {
                        if (u.Vaitro == 1)
                        {
                            HttpContext.Session.SetInt32("UserID", u.Id);
                            return Redirect("/Canhan/Trangchucanhan");
                        } else
                        if (u.Vaitro == 2)
                        {
                            HttpContext.Session.SetInt32("UserID", u.Id);
                           return Redirect("/Tochuc/Trangchutochuc");
                        } else
                        if (u.Vaitro == 3)
                        {
                            HttpContext.Session.SetInt32("UserID", u.Id);
                            return Redirect("/Quantrivien/Trangchuquantrivien");
                        } else return View();

                    }                   
                }              
            }
            else return View();
        }
        [HttpGet]
        public ActionResult Dangkytaikhoan()
        {
            return View();
        }
        [HttpPost]

        public IActionResult Dangkytaikhoan(DKTK model, string myComboBox)
        {
            // Kiểm tra xem tên đăng nhập đã tồn tại chưa
            var existingUser = db.Taikhoans.FirstOrDefault(u => u.Tendangnhap == model.Tendangnhap);
            if (existingUser != null)
            {
                ModelState.AddModelError("Tendangnhap", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }

            // Kiểm tra xác nhận mật khẩu
            if (model.Matkhau != model.NLMK)
            {
                ModelState.AddModelError("XacnhanMatkhau", "Xác nhận mật khẩu không khớp.");
                return View(model);
            }

            // Thêm tài khoản mới vào cơ sở dữ liệu

            string Vaitro = null;
            if (myComboBox == "CN")
            {
                Vaitro = "Cá nhân";//Vaitro=1
            }
            else if (myComboBox == "TC")
            {
                Vaitro = "Tổ chức";//Vaitro=2
            }

            // Lưu thông tin số điện thoại và email nếu có
            if (!string.IsNullOrEmpty(model.Sdt) && !string.IsNullOrEmpty(model.Email))
            {
                if (Vaitro == "Cá nhân")
                {
                    var newUser = new Taikhoan
                    {
                        Tendangnhap = model.Tendangnhap,
                        Matkhau = model.Matkhau,
                        Trangthai = true,
                        Vaitro = 1
                    };
                    db.Taikhoans.Add(newUser);
                    db.SaveChanges();
                    var canhan = new Canhan
                    {
                        Idcanhan = newUser.Id,
                        Ten = model.Ten,
                        Sdt = model.Sdt,
                        Email = model.Email,

                    };
                    db.Canhans.Add(canhan);
                }
                if (Vaitro == "Tổ chức")
                {
                    var newUser = new Taikhoan
                    {
                        Tendangnhap = model.Tendangnhap,
                        Matkhau = model.Matkhau,
                        Trangthai = true,
                        Vaitro = 2
                    };
                    db.Taikhoans.Add(newUser);
                    db.SaveChanges();
                    var tochuc = new Tochuc
                    {
                        Idtochuc = newUser.Id,
                        Ten = model.Ten,
                        Sdt = model.Sdt,
                        Email = model.Email
                    };
                    db.Tochucs.Add(tochuc);
                }

            }

            db.SaveChanges();

            ViewBag.thongbao = "Đăng ký tài khoản thành công!";
            return Redirect("/Dangnhap/Dangnhap");
        }
    }
}
