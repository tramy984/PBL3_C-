using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
using WebsiteQLHDTN.Models;

namespace WebsiteQLHDTN.Controllers
{
    
    
    public class thongtinHDController : Controller
    {
        QLYHDTNContext db = new QLYHDTNContext();
        public IActionResult thongtinHD()
        {
            return View();
        }
        public IActionResult HienthithongtinHoatdong()
        {
            int? hoatdongID = HttpContext.Session.GetInt32("ID_HD");
            var u = db.MotaHds.Where(model => model.Idhoatdong.Equals(hoatdongID)).Select(model => 
                new { Tenhoatdong = model.Tenhoatdong, Thoigianbatdau = model.Thoigianbatdau, Thoigianketthuc = model.Thoigianketthuc,
                    Diadiem = model.Diadiem, Linhvuc = model.Linhvuc, Thongdiep = model.Thongdiep, Muctieu = model.Muctieuhoatdong}).FirstOrDefault();
            if (u != null)
            {
                ViewBag.Tenhoatdong = u.Tenhoatdong.ToString();
                ViewBag.Thoigianbatdau = u.Thoigianbatdau.ToString();
                ViewBag.Thoigianketthuc = u.Thoigianketthuc.ToString();
                ViewBag.Diadiem=u.Diadiem.ToString();
                ViewBag.Linhvuc = u.Linhvuc.ToString();
                ViewBag.Thongdiep = u.Thongdiep.ToString();
                ViewBag.Muctieu = u.Muctieu.ToString();
            }
            var soluong = db.Hoatdongs.Where(model => model.Idhoatdong.Equals(hoatdongID)).ToList().Count();
            ViewBag.Tongsotnvcan = soluong;
            var cacBan = db.YeucauTnvs
                    .Where(model => model.Idhoatdong == hoatdongID)
                    .Select(model => model.Tenban)
                    .ToList();
            ViewBag.ListCacban=cacBan;

            return View();
        }
    }
}
