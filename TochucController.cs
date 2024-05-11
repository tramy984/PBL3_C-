using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PBL3_demo1.Models;
using PBL3_demo1.Viewmodel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PBL3_demo1.Controllers
{
    public class TochucController : Controller
    {
        public int IDhoatdong;
        HdtnContext _db = new HdtnContext();
        public static   int? IDtochuc;
        public IActionResult Trangchutochuc()
        {
            listHD = ListDS();
            return View();
        }
        public IActionResult TaoHD()
        {
            return View();
        }
        [HttpPost]
        public IActionResult TaoHD(string TenHD, string TGBD, string TGKT, string DiaDiem,
            string LinhVuc, string ThongDiep, string SL, string action)
        {

            TenHD = Request.Form["TenHD"];
            TGBD = Request.Form["TGBD"];
            TGKT = Request.Form["TGKT"];
            DiaDiem = Request.Form["DiaDiem"];
            LinhVuc = Request.Form["LinhVuc"];
            ThongDiep = Request.Form["ThongDiep"];

           IDtochuc = HttpContext.Session.GetInt32("UserID");

            if (string.IsNullOrEmpty(TenHD) || string.IsNullOrEmpty(TGBD) || string.IsNullOrEmpty(TGKT) ||
                string.IsNullOrEmpty(DiaDiem) || string.IsNullOrEmpty(LinhVuc) ||
                string.IsNullOrEmpty(ThongDiep)||string.IsNullOrEmpty(SL))
            {
                
                ViewBag.thongbao = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }
            else
            {
                SL = Int32.Parse(Request.Form["SL"]).ToString();
                var hd = new Hoatdong
                {
                    Idtochuc = IDtochuc,
                    Tongsotnvcan =Convert.ToInt32(SL),
                    Trangthai = 0,
                };
                _db.Hoatdongs.Add(hd);
                _db.SaveChanges();
                IDhoatdong = hd.Idhoatdong;
                var motahd = new MotaHd
                {
                    Idhoatdong = hd.Idhoatdong,
                    Tenhoatdong = TenHD,
                    Linhvuc = LinhVuc,
                    Diadiem = DiaDiem,
                    Muctieuhoatdong = ThongDiep,

                };
                _db.MotaHds.Add(motahd);
                return View();
                ViewBag.thongbao = "Tao hoat dong thanh cong";

            }
        }
        public IActionResult Themban() { 
            string tenban= Request.Form["Tenban"];
            string Sl= Request.Form["SLBan"];
            string Nhiemvu= Request.Form["Nhiemvu"];
            string Yeucau=Request.Form["Yeucau"];
            if(string.IsNullOrEmpty(tenban)|| string.IsNullOrEmpty(Sl))
            {
                ViewBag.thongbao("Vui lòng nhập tên ban vầ số lượng tình nguyện viên của ban");
                return Redirect("/Tochuc/TaoHD");
            }
            else
            {
                var yc = new YeucauTnv
                {
                    Idhoatdong = IDhoatdong,
                    Tenban = tenban,
                    Nhiemvu = Nhiemvu,
                    Yeucau = Yeucau,
                    Sltnvcan = Convert.ToInt32(Sl),
                    Sltnvddk=0,
                };
                _db.YeucauTnvs.Add(yc); 
                _db.SaveChanges();

            }
            return Redirect("/Tochuc/TaoHD");

        }
         public static List<HD> listHD = new List<HD>();

        public static List<HD> ListDS()
        {
            HD u = new HD();
            var hd = new Hoatdong { };
            if (hd.Idtochuc == IDtochuc)
            {
                u.Idtochuc = IDtochuc;
                u.Idhoatdong = hd.Idhoatdong;
                var mota = new MotaHd { };
                if (hd.Idhoatdong == mota.Idhoatdong)
                {
                    u.Tenhoatdong = mota.Tenhoatdong;
                    u.Thoigianbatdau = mota.Thoigianbatdau;
                    u.Thoigianketthuc = mota.Thoigianketthuc;
                }


            };
            listHD.Add(u);  
            return listHD;
        }
        public IActionResult DSHD()
        {
           
            return View();
        }
        public IActionResult ChitietHD()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChitietHD(string TenHD) {
            // int hoatdongID = HttpContext.Session.GetInt32("ID_HD");
            int hoatdongID = 16;
                var u = _db.MotaHds.Where(model => model.Idhoatdong.Equals(hoatdongID)).Select(model =>
                    new {
                        Tenhoatdong = model.Tenhoatdong,
                        Thoigianbatdau = model.Thoigianbatdau,
                        Thoigianketthuc = model.Thoigianketthuc,
                        Diadiem = model.Diadiem,
                        Linhvuc = model.Linhvuc,
                        Muctieu = model.Muctieuhoatdong
                    }).FirstOrDefault();
                if (u != null)
                {
                    ViewBag.Tenhoatdong = u.Tenhoatdong.ToString();
                    ViewBag.Thoigianbatdau = u.Thoigianbatdau.ToString();
                    ViewBag.Thoigianketthuc = u.Thoigianketthuc.ToString();
                    ViewBag.Diadiem = u.Diadiem.ToString();
                    ViewBag.Linhvuc = u.Linhvuc.ToString();
                    ViewBag.Muctieu = u.Muctieu.ToString();
                }
                var soluong = _db.Hoatdongs.Where(model => model.Idhoatdong.Equals(hoatdongID)).ToList().Count();
                ViewBag.Tongsotnvcan = soluong;
                var cacBan = _db.YeucauTnvs
                        .Where(model => model.Idhoatdong == hoatdongID)
                        .Select(model => model.Tenban)
                        .ToList();
                ViewBag.ListCacban = cacBan;

                return View();
            }
        
        public IActionResult DSTNV()
        {
            return View();
        }
        public IActionResult Thongtintochuc()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            var u = _db.Tochucs.Where(model => model.Idtochuc.Equals(userID)).Select(model => new { Ten = model.Ten, Email = model.Email, Sdt = model.Sdt, Diachi = model.Diachi }).FirstOrDefault();

            if (u != null)
            {
                ViewBag.Ten = u.Ten.ToString();
                ViewBag.Email = u.Email.ToString();
                ViewBag.Sdt = u.Sdt.ToString();
                ViewBag.Diachi = u.Diachi.ToString();
            }
            var u1 = _db.Motatochucs
                .Where(model => model.Idtochuc.Equals(userID))
                .Select(model => new { Gioithieu = model.Gioithieu, Thanhtuu = model.Thanhtuu, slHD = model.Hoatdongs.Count }).FirstOrDefault();
            if (u1 != null)
            {
                ViewBag.Mota = u1.Gioithieu.ToString();
                ViewBag.Thanhtuu = u1.Thanhtuu.ToString();
                ViewBag.soluongHD = u1.slHD.ToString();

            }
            return View();
        }
    }
}
