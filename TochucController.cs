using Microsoft.AspNetCore.Mvc;
using WebsiteQLHDTN.Models;

namespace WebsiteQLHDTN.Controllers
{
    public class TochucController : Controller
    {
        QLYHDTNContext db = new QLYHDTNContext();
        
        public IActionResult thongtinToChuc() 
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            //int userID=2;
            var u = db.Tochucs.Where(model => model.Idtochuc.Equals(userID)).Select(model => new { Ten = model.Ten, Email = model.Email, Sdt = model.Sdt, Diachi = model.Diachi }).FirstOrDefault();

            if (u != null)
            {
                ViewBag.Ten = u.Ten.ToString();
                ViewBag.Email = u.Email.ToString();
                ViewBag.Sdt = u.Sdt.ToString();
                ViewBag.Diachi = u.Diachi.ToString();                
            }
            var u1 = db.Motatochucs
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
