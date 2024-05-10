using Microsoft.AspNetCore.Mvc;
using WebsiteQLHDTN.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language;
using AspNetCore;

namespace WebsiteQLHDTN.Controllers
{
    public class dangkyHDController : Controller
    {

        QLYHDTNContext db=new QLYHDTNContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult layTenBan(int hoatdongID)
        {
            List<SelectListItem> list = db.YeucauTnvs
                .Where(model => model.Idhoatdong == hoatdongID)
                .Select(ban=> new SelectListItem { Text=ban.Tenban, Value=ban.Tenban })
                .ToList();
            ViewBag.ListTenBan = list;
            return View();
        }

        public IActionResult dangkythamgia(Tinhtrang model_quatrinhTG)
        {
            string uudiem = model_quatrinhTG.UuDiem;
            string khuyetdiem = model_quatrinhTG.KhuyetDiem;
            string ban = model_quatrinhTG.BanThamGia;

            if (HttpContext.Session.GetString("uudiem") == null || HttpContext.Session.GetString("khuyetdiem") == null || string.IsNullOrEmpty(ban))
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ dữ liệu";
            }
            else
            {
                var quatrinhTNV = new Tinhtrang
                {
                    UuDiem = uudiem,
                    KhuyetDiem = khuyetdiem,
                    BanThamGia = ban
                };
                db.Tinhtrangs.Add(quatrinhTNV);
                db.SaveChanges();
                ViewBag.Message = "Đăng ký tham gia thành công";
            }
            return View();
        }

    }

    
}
