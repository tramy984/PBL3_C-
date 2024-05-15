using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_QLHDTN.Models;
using PBL3_QLHDTN.ViewModel;
using System;
using System.Globalization;
using System.Net;

namespace PBL3_QLHDTN.Controllers
{
    public class TochucController : Controller
    {
        QlhdtnContext db = new QlhdtnContext();

        public IActionResult Trangchutochuc()
        {

            return View();
        }
        public IActionResult TaoHD()
        {
            var LV = db.Linhvucs
               .Select(n => n.Linhvuc1).ToList();
            var ketqua = new LinhvucView
            {
                TenLV = LV,
            };
            return View(ketqua);
        }
        public int LayIDLV(string tenLV)
        {

            int idLinhVuc = -1; // Giả sử mặc định không tìm thấy
            using (var context = new QlhdtnContext())
            {
                var linhVuc = context.Linhvucs.FirstOrDefault(lv => lv.Linhvuc1 == tenLV);
                if (linhVuc != null)
                {
                    idLinhVuc = linhVuc.Idlinhvuc;
                }
            }

            return idLinhVuc;
        }
        [HttpPost]
        public IActionResult TaoHD(string TenHD, string QuanHuyen, string PhuongXa, string linhvuc, DateOnly TGBD, DateOnly TGKT)
        {
            var Lv = db.Linhvucs
               .Select(n => n.Linhvuc1).ToList();
            var ketqua = new LinhvucView
            {
                TenLV = Lv,
            };
            TenHD = Request.Form["TenHD"];
            string DiaDiem = Request.Form["DiaDiem"] + " Phường " + PhuongXa + " Quận " + QuanHuyen;
            string LV = linhvuc;
            string Muctieu = Request.Form["ThongDiep"];
            string SL = Int32.Parse(Request.Form["SL"]).ToString();

            int? IDTC = HttpContext.Session.GetInt32("UserID");


            if (string.IsNullOrEmpty(TenHD) || TGBD == default(DateOnly) || TGKT == default(DateOnly) ||
                string.IsNullOrEmpty(DiaDiem) || string.IsNullOrEmpty(LV) ||
                string.IsNullOrEmpty(Muctieu) || string.IsNullOrEmpty(SL))
            {
                ViewBag.thongbao = "Vui lòng nhập đầy đủ thông tin.";
                return View(ketqua);
            }
            else
            {
                var hd = new Models.Hoatdong
                {
                    Idtochuc = IDTC,
                    Sltnvcan = Convert.ToInt32(SL),
                    Trangthai = 0,
                };
                db.Hoatdongs.Add(hd);
                db.SaveChanges();
                HttpContext.Session.SetInt32("IDhoatdong", hd.Idhoatdong);
                var motahd = new MotaHd
                {
                    Idhoatdong = hd.Idhoatdong,
                    Tenhoatdong = TenHD,
                    Linhvuc = LayIDLV(LV),
                    DiaDiem = DiaDiem,
                    MuctieuHd = Muctieu,
                    Thoigianbatdau = TGBD,
                    Thoigiaketthuc = TGKT,
                };
                db.MotaHds.Add(motahd);
                db.SaveChanges();
                return Redirect("Themban");
            }
        }
        public IActionResult Themban()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Themban(string TenHD)
        {
            int? IDhoatdong = HttpContext.Session.GetInt32("IDhoatdong");
            string tenban = Request.Form["Tenban"];
            string Sl = Request.Form["SLBan"];
            string Nhiemvu = Request.Form["Nhiemvu"];
            string Yeucau = Request.Form["Yeucau"];
            if (string.IsNullOrEmpty(tenban) || string.IsNullOrEmpty(Sl))
            {
                ViewBag.thongbao("Vui lòng nhập tên ban và số lượng tình nguyện viên của ban");
                var ketqua = (from yeucau in db.YeucauTnvs
                              where yeucau.Idhoatdong == IDhoatdong
                              select new ViewModel.banHD
                              {
                                  Tenban = yeucau.Tenban,
                                  Sltnvcan = yeucau.Sltnvcan,
                                  Nhiemvu = yeucau.Nhiemvu,
                                  Yeucau = yeucau.Yeucau,
                              }).ToList();

                return View(ketqua);
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
                    Sltnvdk = 0,
                };
                var u = db.YeucauTnvs
                .Where(ycau => ycau.Idhoatdong.Equals(IDhoatdong) && ycau.Tenban.Equals(tenban))
                .Select(ycau => new { IDhoatdong = IDhoatdong, tenban = tenban })
                .FirstOrDefault();
                if (u == null)
                {
                    db.YeucauTnvs.Add(yc);
                    db.SaveChanges();
                    ViewBag.thongbao = "Tạo ban thành công";
                }
                else
                {
                    ViewBag.thongbao = "Ban " + u.tenban + " đã tồn tại";
                }
                var ketqua = (from yeucau in db.YeucauTnvs
                              where yeucau.Idhoatdong == IDhoatdong
                              select new ViewModel.banHD
                              {
                                  Tenban = yeucau.Tenban,
                                  Sltnvcan = yeucau.Sltnvcan,
                                  Nhiemvu = yeucau.Nhiemvu,
                                  Yeucau = yeucau.Yeucau,
                              }).ToList();

                return View(ketqua);
            }
        }

        public IActionResult DSHD()
        {
            var linhvuc = db.Linhvucs.ToList();
            ViewBag.linhvuc = linhvuc;

            int? IDTC = HttpContext.Session.GetInt32("UserID");
            var ketqua = (from hd in db.Hoatdongs
                          join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                          join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                          join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                          where hd.Idtochuc == IDTC
                          select new ViewModel.Hoatdong
                          {
                              Idhoatdong = hd.Idhoatdong,
                              Tenhoatdong = mt.Tenhoatdong,
                              Thoigianbatdau = mt.Thoigianbatdau,
                              Thoigiaketthuc = mt.Thoigiaketthuc,
                              Tgbdchinhsua = mt.Tgbdchinhsua,
                              Tgktchinhsua=mt.Tgktchinhsua,
                              DiaDiem=mt.DiaDiem,
                              TenLV=lv.Linhvuc1,
                              MuctieuHd=mt.MuctieuHd,
                              Sltnvcan=hd.Sltnvcan,
                              Trangthai = tt.Trangthai,
                          }).ToList();

            return View(ketqua);

        }
        public int LayIDTT(string tenTT)
        {

            int idTT = -1; // Giả sử mặc định không tìm thấy
            using (var context = new QlhdtnContext())
            {
                var tt = context.TrangthaiHds.FirstOrDefault(lv => lv.Trangthai == tenTT);
                if (tt != null)
                {
                    idTT = tt.Idtrangthai;
                }
            }

            return idTT;
        }
        [HttpPost]
        public IActionResult DSHD(string cbbtinhtrang, string cbblinhvuc, string thoigian,string tenhoatdong)
        {
            var linhvuc = db.Linhvucs.ToList();
            ViewBag.linhvuc = linhvuc;
            tenhoatdong= Request.Form["Tenhoatdong"];
            int? IDTC = HttpContext.Session.GetInt32("UserID");
            /*DateOnly thoigianDate;
            DateOnly.TryParseExact(thoigian, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out thoigianDate);*/
            
            if (cbbtinhtrang == null)
            {
                if (cbblinhvuc == null && thoigian == null)
                {
                    if (string.IsNullOrEmpty(tenhoatdong) == false)
                    { 
                        var ketqua = (from hd in db.Hoatdongs
                                      join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                      join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                      join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                      where hd.Idtochuc == IDTC && mt.Tenhoatdong == tenhoatdong
                                      select new ViewModel.Hoatdong
                                      {
                                          Idhoatdong = hd.Idhoatdong,
                                          Tenhoatdong = mt.Tenhoatdong,
                                          Thoigianbatdau = mt.Thoigianbatdau,
                                          Thoigiaketthuc = mt.Thoigiaketthuc,
                                          Tgbdchinhsua = mt.Tgbdchinhsua,
                                          Tgktchinhsua = mt.Tgktchinhsua,
                                          DiaDiem = mt.DiaDiem,
                                          TenLV = lv.Linhvuc1,
                                          MuctieuHd = mt.MuctieuHd,
                                          Sltnvcan = hd.Sltnvcan,
                                          Trangthai = tt.Trangthai,
                                      }).ToList();

                        return View(ketqua);
                    }
                    else
                    {
                        var ketqua = (from hd in db.Hoatdongs
                                      join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                      join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                      join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                      where hd.Idtochuc == IDTC
                                      select new ViewModel.Hoatdong
                                      {
                                          Idhoatdong = hd.Idhoatdong,
                                          Tenhoatdong = mt.Tenhoatdong,
                                          Thoigianbatdau = mt.Thoigianbatdau,
                                          Thoigiaketthuc = mt.Thoigiaketthuc,
                                          Tgbdchinhsua = mt.Tgbdchinhsua,
                                          Tgktchinhsua = mt.Tgktchinhsua,
                                          DiaDiem = mt.DiaDiem,
                                          TenLV = lv.Linhvuc1,
                                          MuctieuHd = mt.MuctieuHd,
                                          Sltnvcan = hd.Sltnvcan,
                                          Trangthai = tt.Trangthai,
                                      }).ToList();

                        return View(ketqua);
                    }
                }
                if (cbblinhvuc != null && thoigian==null)
                {
                    if (string.IsNullOrEmpty(tenhoatdong) == false)
                    {
                        var ketqua = (from hd in db.Hoatdongs
                                      join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                      join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                      join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                      where hd.Idtochuc == IDTC && mt.Linhvuc == Convert.ToInt32(cbblinhvuc)
                                      && mt.Tenhoatdong==tenhoatdong
                                      select new ViewModel.Hoatdong
                                      {
                                          Idhoatdong = hd.Idhoatdong,
                                          Tenhoatdong = mt.Tenhoatdong,
                                          Thoigianbatdau = mt.Thoigianbatdau,
                                          Thoigiaketthuc = mt.Thoigiaketthuc,
                                          Tgbdchinhsua = mt.Tgbdchinhsua,
                                          Tgktchinhsua = mt.Tgktchinhsua,
                                          DiaDiem = mt.DiaDiem,
                                          TenLV = lv.Linhvuc1,
                                          MuctieuHd = mt.MuctieuHd,
                                          Sltnvcan = hd.Sltnvcan,
                                          Trangthai = tt.Trangthai,
                                      }).ToList();

                        return View(ketqua);
                    }
                    else
                    {
                        var ketqua = (from hd in db.Hoatdongs
                                      join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                      join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                      join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                      where hd.Idtochuc == IDTC && mt.Linhvuc == Convert.ToInt32(cbblinhvuc)
                                      select new ViewModel.Hoatdong
                                      {
                                          Idhoatdong = hd.Idhoatdong,
                                          Tenhoatdong = mt.Tenhoatdong,
                                          Thoigianbatdau = mt.Thoigianbatdau,
                                          Thoigiaketthuc = mt.Thoigiaketthuc,
                                          Tgbdchinhsua = mt.Tgbdchinhsua,
                                          Tgktchinhsua = mt.Tgktchinhsua,
                                          DiaDiem = mt.DiaDiem,
                                          TenLV = lv.Linhvuc1,
                                          MuctieuHd = mt.MuctieuHd,
                                          Sltnvcan = hd.Sltnvcan,
                                          Trangthai = tt.Trangthai,
                                      }).ToList();

                        return View(ketqua);
                    }
                }
                if(cbblinhvuc==null && thoigian != null)
                {
                    DateTime startday = DateTime.Parse(thoigian);
                    DateTime endday = startday.AddMonths(1).AddDays(-1);
                    ViewBag.thongbao = "THD"+tenhoatdong;
                    if (string.IsNullOrEmpty(tenhoatdong)==true)
                    {
                      
                        var hoatdong = (from hd in db.Hoatdongs
                                        join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                        join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                        join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                        where hd.Idtochuc == IDTC
                                        select new ViewModel.Hoatdong
                                        {
                                            Idhoatdong = hd.Idhoatdong,
                                            Tenhoatdong = mt.Tenhoatdong,
                                            Thoigianbatdau = mt.Thoigianbatdau,
                                            Thoigiaketthuc = mt.Thoigiaketthuc,
                                            Tgbdchinhsua = mt.Tgbdchinhsua,
                                            Tgktchinhsua = mt.Tgktchinhsua,
                                            DiaDiem = mt.DiaDiem,
                                            TenLV = lv.Linhvuc1,
                                            MuctieuHd = mt.MuctieuHd,
                                            Sltnvcan = hd.Sltnvcan,
                                            Trangthai = tt.Trangthai,
                                        }).ToList();
                        List<ViewModel.Hoatdong> ketqua = new List<ViewModel.Hoatdong>();
                        foreach (var i in hoatdong)
                        {
                            DateOnly? TGBD;
                            if (i.Tgbdchinhsua != null)
                            {
                                TGBD = i.Tgbdchinhsua;
                            }
                            else TGBD = i.Thoigianbatdau;
                            DateOnly? TGKT;
                            if (i.Tgktchinhsua != null)
                            {
                                TGKT = i.Tgktchinhsua;
                            }
                            else TGKT = i.Thoigiaketthuc;
                            DateTime tgbd;
                            DateTime tgkt;
                            DateTime.TryParseExact(TGBD.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgbd);
                            DateTime.TryParseExact(TGKT.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgkt);
                            int ss1 = startday.CompareTo(tgbd);
                            int ss2 = endday.CompareTo(tgbd);
                            int ss3 = startday.CompareTo(tgkt);
                            int ss4 = endday.CompareTo(tgkt);
                            if ((ss1 < 0 && ss2 > 0) || (ss3 < 0 && ss4 > 0))
                            {
                                ketqua.Add(i);
                            }

                        }

                        return View(ketqua);
                    }
                    else
                    {
                        var hoatdong = (from hd in db.Hoatdongs
                                        join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                        join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                        join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                        where hd.Idtochuc == IDTC && mt.Tenhoatdong==tenhoatdong
                                        select new ViewModel.Hoatdong
                                        {
                                            Idhoatdong = hd.Idhoatdong,
                                            Tenhoatdong = mt.Tenhoatdong,
                                            Thoigianbatdau = mt.Thoigianbatdau,
                                            Thoigiaketthuc = mt.Thoigiaketthuc,
                                            Tgbdchinhsua = mt.Tgbdchinhsua,
                                            Tgktchinhsua = mt.Tgktchinhsua,
                                            DiaDiem = mt.DiaDiem,
                                            TenLV = lv.Linhvuc1,
                                            MuctieuHd = mt.MuctieuHd,
                                            Sltnvcan = hd.Sltnvcan,
                                            Trangthai = tt.Trangthai,
                                        }).ToList();
                        List<ViewModel.Hoatdong> ketqua = new List<ViewModel.Hoatdong>();
                        foreach (var i in hoatdong)
                        {
                            DateOnly? TGBD;
                            if (i.Tgbdchinhsua != null)
                            {
                                TGBD = i.Tgbdchinhsua;
                            }
                            else TGBD = i.Thoigianbatdau;
                            DateOnly? TGKT;
                            if (i.Tgktchinhsua != null)
                            {
                                TGKT = i.Tgktchinhsua;
                            }
                            else TGKT = i.Thoigiaketthuc;
                            DateTime tgbd;
                            DateTime tgkt;
                            DateTime.TryParseExact(TGBD.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgbd);
                            DateTime.TryParseExact(TGKT.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgkt);
                            int ss1 = startday.CompareTo(tgbd);
                            int ss2 = endday.CompareTo(tgbd);
                            int ss3 = startday.CompareTo(tgkt);
                            int ss4 = endday.CompareTo(tgkt);
                            if ((ss1 < 0 && ss2 > 0) || (ss3 < 0 && ss4 > 0))
                            {
                                ketqua.Add(i);
                            }

                        }

                        return View(ketqua);
                    }

                }
                if(cbblinhvuc != null && thoigian != null)
                {
                    DateTime startday = DateTime.Parse(thoigian);
                    DateTime endday = startday.AddMonths(1).AddDays(-1);
                    if (string.IsNullOrEmpty(tenhoatdong) == true)
                    {
                        var hoatdong = (from hd in db.Hoatdongs
                                        join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                        join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                        join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                        where hd.Idtochuc == IDTC && mt.Linhvuc == Convert.ToInt32(cbblinhvuc)
                                        select new ViewModel.Hoatdong
                                        {
                                            Idhoatdong = hd.Idhoatdong,
                                            Tenhoatdong = mt.Tenhoatdong,
                                            Thoigianbatdau = mt.Thoigianbatdau,
                                            Thoigiaketthuc = mt.Thoigiaketthuc,
                                            Tgbdchinhsua = mt.Tgbdchinhsua,
                                            Tgktchinhsua = mt.Tgktchinhsua,
                                            DiaDiem = mt.DiaDiem,
                                            TenLV = lv.Linhvuc1,
                                            MuctieuHd = mt.MuctieuHd,
                                            Sltnvcan = hd.Sltnvcan,
                                            Trangthai = tt.Trangthai,
                                        }).ToList();
                        List<ViewModel.Hoatdong> ketqua = new List<ViewModel.Hoatdong>();
                        foreach (var i in hoatdong)
                        {
                            DateOnly? TGBD;
                            if (i.Tgbdchinhsua != null)
                            {
                                TGBD = i.Tgbdchinhsua;
                            }
                            else TGBD = i.Thoigianbatdau;
                            DateOnly? TGKT;
                            if (i.Tgktchinhsua != null)
                            {
                                TGKT = i.Tgktchinhsua;
                            }
                            else TGKT = i.Thoigiaketthuc;
                            DateTime tgbd;
                            DateTime tgkt;
                            DateTime.TryParseExact(TGBD.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgbd);
                            DateTime.TryParseExact(TGKT.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgkt);
                            int ss1 = startday.CompareTo(tgbd);
                            int ss2 = endday.CompareTo(tgbd);
                            int ss3 = startday.CompareTo(tgkt);
                            int ss4 = endday.CompareTo(tgkt);
                            if ((ss1 < 0 && ss2 > 0) || (ss3 < 0 && ss4 > 0))
                            {
                                ketqua.Add(i);
                            }

                        }

                        return View(ketqua);
                    }
                    else
                    {
                        var hoatdong = (from hd in db.Hoatdongs
                                        join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                        join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                        join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                        where hd.Idtochuc == IDTC && mt.Linhvuc == Convert.ToInt32(cbblinhvuc)
                                        && mt.Tenhoatdong==tenhoatdong
                                        select new ViewModel.Hoatdong
                                        {
                                            Idhoatdong = hd.Idhoatdong,
                                            Tenhoatdong = mt.Tenhoatdong,
                                            Thoigianbatdau = mt.Thoigianbatdau,
                                            Thoigiaketthuc = mt.Thoigiaketthuc,
                                            Tgbdchinhsua = mt.Tgbdchinhsua,
                                            Tgktchinhsua = mt.Tgktchinhsua,
                                            DiaDiem = mt.DiaDiem,
                                            TenLV = lv.Linhvuc1,
                                            MuctieuHd = mt.MuctieuHd,
                                            Sltnvcan = hd.Sltnvcan,
                                            Trangthai = tt.Trangthai,
                                        }).ToList();
                        List<ViewModel.Hoatdong> ketqua = new List<ViewModel.Hoatdong>();
                        foreach (var i in hoatdong)
                        {
                            DateOnly? TGBD;
                            if (i.Tgbdchinhsua != null)
                            {
                                TGBD = i.Tgbdchinhsua;
                            }
                            else TGBD = i.Thoigianbatdau;
                            DateOnly? TGKT;
                            if (i.Tgktchinhsua != null)
                            {
                                TGKT = i.Tgktchinhsua;
                            }
                            else TGKT = i.Thoigiaketthuc;
                            DateTime tgbd;
                            DateTime tgkt;
                            DateTime.TryParseExact(TGBD.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgbd);
                            DateTime.TryParseExact(TGKT.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgkt);
                            int ss1 = startday.CompareTo(tgbd);
                            int ss2 = endday.CompareTo(tgbd);
                            int ss3 = startday.CompareTo(tgkt);
                            int ss4 = endday.CompareTo(tgkt);
                            if ((ss1 < 0 && ss2 > 0) || (ss3 < 0 && ss4 > 0))
                            {
                                ketqua.Add(i);
                            }

                        }

                        return View(ketqua);
                    }
                }              
            }
            else
            {
                if (cbblinhvuc == null && thoigian == null)
                {
                    if (string.IsNullOrEmpty(tenhoatdong)==false)
                    {
                        var ketqua = (from hd in db.Hoatdongs
                                      join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                      join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                      join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                      where hd.Idtochuc == IDTC && mt.Tenhoatdong == tenhoatdong
                                      && hd.Trangthai==LayIDTT(cbbtinhtrang)
                                      select new ViewModel.Hoatdong
                                      {
                                          Idhoatdong = hd.Idhoatdong,
                                          Tenhoatdong = mt.Tenhoatdong,
                                          Thoigianbatdau = mt.Thoigianbatdau,
                                          Thoigiaketthuc = mt.Thoigiaketthuc,
                                          Tgbdchinhsua = mt.Tgbdchinhsua,
                                          Tgktchinhsua = mt.Tgktchinhsua,
                                          DiaDiem = mt.DiaDiem,
                                          TenLV = lv.Linhvuc1,
                                          MuctieuHd = mt.MuctieuHd,
                                          Sltnvcan = hd.Sltnvcan,
                                          Trangthai = tt.Trangthai,
                                      }).ToList();

                        return View(ketqua);
                    }
                    else
                    {
                        ViewBag.thongbao=LayIDTT(cbbtinhtrang);
                        var ketqua = (from hd in db.Hoatdongs
                                      join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                      join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                      join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                      where hd.Idtochuc == IDTC
                                      && hd.Trangthai == LayIDTT(cbbtinhtrang)
                                      select new ViewModel.Hoatdong
                                      {
                                          Idhoatdong = hd.Idhoatdong,
                                          Tenhoatdong = mt.Tenhoatdong,
                                          Thoigianbatdau = mt.Thoigianbatdau,
                                          Thoigiaketthuc = mt.Thoigiaketthuc,
                                          Tgbdchinhsua = mt.Tgbdchinhsua,
                                          Tgktchinhsua = mt.Tgktchinhsua,
                                          DiaDiem = mt.DiaDiem,
                                          TenLV = lv.Linhvuc1,
                                          MuctieuHd = mt.MuctieuHd,
                                          Sltnvcan = hd.Sltnvcan,
                                          Trangthai = tt.Trangthai,
                                      }).ToList();

                        return View(ketqua);
                    }
                }
                if (cbblinhvuc != null && thoigian == null)
                {
                    if (string.IsNullOrEmpty(tenhoatdong)==false)
                    {
                        var ketqua = (from hd in db.Hoatdongs
                                      join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                      join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                      join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                      where hd.Idtochuc == IDTC && mt.Linhvuc == Convert.ToInt32(cbblinhvuc)
                                      && mt.Tenhoatdong == tenhoatdong && hd.Trangthai == LayIDTT(cbbtinhtrang)
                                      select new ViewModel.Hoatdong
                                      {
                                          Idhoatdong = hd.Idhoatdong,
                                          Tenhoatdong = mt.Tenhoatdong,
                                          Thoigianbatdau = mt.Thoigianbatdau,
                                          Thoigiaketthuc = mt.Thoigiaketthuc,
                                          Tgbdchinhsua = mt.Tgbdchinhsua,
                                          Tgktchinhsua = mt.Tgktchinhsua,
                                          DiaDiem = mt.DiaDiem,
                                          TenLV = lv.Linhvuc1,
                                          MuctieuHd = mt.MuctieuHd,
                                          Sltnvcan = hd.Sltnvcan,
                                          Trangthai = tt.Trangthai,
                                      }).ToList();

                        return View(ketqua);
                    }
                    else
                    {
                        var ketqua = (from hd in db.Hoatdongs
                                      join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                      join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                      join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                      where hd.Idtochuc == IDTC && mt.Linhvuc == Convert.ToInt32(cbblinhvuc)
                                      && hd.Trangthai == LayIDTT(cbbtinhtrang)
                                      select new ViewModel.Hoatdong
                                      {
                                          Idhoatdong = hd.Idhoatdong,
                                          Tenhoatdong = mt.Tenhoatdong,
                                          Thoigianbatdau = mt.Thoigianbatdau,
                                          Thoigiaketthuc = mt.Thoigiaketthuc,
                                          Tgbdchinhsua = mt.Tgbdchinhsua,
                                          Tgktchinhsua = mt.Tgktchinhsua,
                                          DiaDiem = mt.DiaDiem,
                                          TenLV = lv.Linhvuc1,
                                          MuctieuHd = mt.MuctieuHd,
                                          Sltnvcan = hd.Sltnvcan,
                                          Trangthai = tt.Trangthai,
                                      }).ToList();

                        return View(ketqua);
                    }
                }
                if (cbblinhvuc == null && thoigian != null)
                {
                    DateTime startday = DateTime.Parse(thoigian);
                    DateTime endday = startday.AddMonths(1).AddDays(-1);
                    ViewBag.thongbao = "THD" + tenhoatdong;
                    if (string.IsNullOrEmpty(tenhoatdong) == true)
                    {

                        var hoatdong = (from hd in db.Hoatdongs
                                        join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                        join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                        join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                        where hd.Idtochuc == IDTC && hd.Trangthai == LayIDTT(cbbtinhtrang)
                                        select new ViewModel.Hoatdong
                                        {
                                            Idhoatdong = hd.Idhoatdong,
                                            Tenhoatdong = mt.Tenhoatdong,
                                            Thoigianbatdau = mt.Thoigianbatdau,
                                            Thoigiaketthuc = mt.Thoigiaketthuc,
                                            Tgbdchinhsua = mt.Tgbdchinhsua,
                                            Tgktchinhsua = mt.Tgktchinhsua,
                                            DiaDiem = mt.DiaDiem,
                                            TenLV = lv.Linhvuc1,
                                            MuctieuHd = mt.MuctieuHd,
                                            Sltnvcan = hd.Sltnvcan,
                                            Trangthai = tt.Trangthai,
                                        }).ToList();
                        List<ViewModel.Hoatdong> ketqua = new List<ViewModel.Hoatdong>();
                        foreach (var i in hoatdong)
                        {
                            DateOnly? TGBD;
                            if (i.Tgbdchinhsua != null)
                            {
                                TGBD = i.Tgbdchinhsua;
                            }
                            else TGBD = i.Thoigianbatdau;
                            DateOnly? TGKT;
                            if (i.Tgktchinhsua != null)
                            {
                                TGKT = i.Tgktchinhsua;
                            }
                            else TGKT = i.Thoigiaketthuc;
                            DateTime tgbd;
                            DateTime tgkt;
                            DateTime.TryParseExact(TGBD.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgbd);
                            DateTime.TryParseExact(TGKT.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgkt);
                            int ss1 = startday.CompareTo(tgbd);
                            int ss2 = endday.CompareTo(tgbd);
                            int ss3 = startday.CompareTo(tgkt);
                            int ss4 = endday.CompareTo(tgkt);
                            if ((ss1 < 0 && ss2 > 0) || (ss3 < 0 && ss4 > 0))
                            {
                                ketqua.Add(i);
                            }

                        }

                        return View(ketqua);
                    }
                    else
                    {
                        var hoatdong = (from hd in db.Hoatdongs
                                        join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                        join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                        join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                        where hd.Idtochuc == IDTC && mt.Tenhoatdong == tenhoatdong
                                        && hd.Trangthai == LayIDTT(cbbtinhtrang)
                                        select new ViewModel.Hoatdong
                                        {
                                            Idhoatdong = hd.Idhoatdong,
                                            Tenhoatdong = mt.Tenhoatdong,
                                            Thoigianbatdau = mt.Thoigianbatdau,
                                            Thoigiaketthuc = mt.Thoigiaketthuc,
                                            Tgbdchinhsua = mt.Tgbdchinhsua,
                                            Tgktchinhsua = mt.Tgktchinhsua,
                                            DiaDiem = mt.DiaDiem,
                                            TenLV = lv.Linhvuc1,
                                            MuctieuHd = mt.MuctieuHd,
                                            Sltnvcan = hd.Sltnvcan,
                                            Trangthai = tt.Trangthai,
                                        }).ToList();
                        List<ViewModel.Hoatdong> ketqua = new List<ViewModel.Hoatdong>();
                        foreach (var i in hoatdong)
                        {
                            DateOnly? TGBD;
                            if (i.Tgbdchinhsua != null)
                            {
                                TGBD = i.Tgbdchinhsua;
                            }
                            else TGBD = i.Thoigianbatdau;
                            DateOnly? TGKT;
                            if (i.Tgktchinhsua != null)
                            {
                                TGKT = i.Tgktchinhsua;
                            }
                            else TGKT = i.Thoigiaketthuc;
                            DateTime tgbd;
                            DateTime tgkt;
                            DateTime.TryParseExact(TGBD.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgbd);
                            DateTime.TryParseExact(TGKT.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgkt);
                            int ss1 = startday.CompareTo(tgbd);
                            int ss2 = endday.CompareTo(tgbd);
                            int ss3 = startday.CompareTo(tgkt);
                            int ss4 = endday.CompareTo(tgkt);
                            if ((ss1 < 0 && ss2 > 0) || (ss3 < 0 && ss4 > 0))
                            {
                                ketqua.Add(i);
                            }

                        }

                        return View(ketqua);
                    }

                }
                if (cbblinhvuc != null && thoigian != null)
                {
                    DateTime startday = DateTime.Parse(thoigian);
                    DateTime endday = startday.AddMonths(1).AddDays(-1);
                    if (string.IsNullOrEmpty(tenhoatdong) == true)
                    {
                        var hoatdong = (from hd in db.Hoatdongs
                                        join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                        join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                        join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                        where hd.Idtochuc == IDTC && mt.Linhvuc == Convert.ToInt32(cbblinhvuc)
                                        && hd.Trangthai == LayIDTT(cbbtinhtrang)
                                        select new ViewModel.Hoatdong
                                        {
                                            Idhoatdong = hd.Idhoatdong,
                                            Tenhoatdong = mt.Tenhoatdong,
                                            Thoigianbatdau = mt.Thoigianbatdau,
                                            Thoigiaketthuc = mt.Thoigiaketthuc,
                                            Tgbdchinhsua = mt.Tgbdchinhsua,
                                            Tgktchinhsua = mt.Tgktchinhsua,
                                            DiaDiem = mt.DiaDiem,
                                            TenLV = lv.Linhvuc1,
                                            MuctieuHd = mt.MuctieuHd,
                                            Sltnvcan = hd.Sltnvcan,
                                            Trangthai = tt.Trangthai,
                                        }).ToList();
                        List<ViewModel.Hoatdong> ketqua = new List<ViewModel.Hoatdong>();
                        foreach (var i in hoatdong)
                        {
                            DateOnly? TGBD;
                            if (i.Tgbdchinhsua != null)
                            {
                                TGBD = i.Tgbdchinhsua;
                            }
                            else TGBD = i.Thoigianbatdau;
                            DateOnly? TGKT;
                            if (i.Tgktchinhsua != null)
                            {
                                TGKT = i.Tgktchinhsua;
                            }
                            else TGKT = i.Thoigiaketthuc;
                            DateTime tgbd;
                            DateTime tgkt;
                            DateTime.TryParseExact(TGBD.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgbd);
                            DateTime.TryParseExact(TGKT.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgkt);
                            int ss1 = startday.CompareTo(tgbd);
                            int ss2 = endday.CompareTo(tgbd);
                            int ss3 = startday.CompareTo(tgkt);
                            int ss4 = endday.CompareTo(tgkt);
                            if ((ss1 < 0 && ss2 > 0) || (ss3 < 0 && ss4 > 0))
                            {
                                ketqua.Add(i);
                            }

                        }

                        return View(ketqua);
                    }
                    else
                    {
                        var hoatdong = (from hd in db.Hoatdongs
                                        join mt in db.MotaHds on hd.Idhoatdong equals mt.Idhoatdong
                                        join tt in db.TrangthaiHds on hd.Trangthai equals tt.Idtrangthai
                                        join lv in db.Linhvucs on mt.Linhvuc equals lv.Idlinhvuc
                                        where hd.Idtochuc == IDTC && mt.Linhvuc == Convert.ToInt32(cbblinhvuc)
                                        && mt.Tenhoatdong == tenhoatdong && hd.Trangthai == LayIDTT(cbbtinhtrang)
                                        select new ViewModel.Hoatdong
                                        {
                                            Idhoatdong = hd.Idhoatdong,
                                            Tenhoatdong = mt.Tenhoatdong,
                                            Thoigianbatdau = mt.Thoigianbatdau,
                                            Thoigiaketthuc = mt.Thoigiaketthuc,
                                            Tgbdchinhsua = mt.Tgbdchinhsua,
                                            Tgktchinhsua = mt.Tgktchinhsua,
                                            DiaDiem = mt.DiaDiem,
                                            TenLV = lv.Linhvuc1,
                                            MuctieuHd = mt.MuctieuHd,
                                            Sltnvcan = hd.Sltnvcan,
                                            Trangthai = tt.Trangthai,
                                        }).ToList();
                        List<ViewModel.Hoatdong> ketqua = new List<ViewModel.Hoatdong>();
                        foreach (var i in hoatdong)
                        {
                            DateOnly? TGBD;
                            if (i.Tgbdchinhsua != null)
                            {
                                TGBD = i.Tgbdchinhsua;
                            }
                            else TGBD = i.Thoigianbatdau;
                            DateOnly? TGKT;
                            if (i.Tgktchinhsua != null)
                            {
                                TGKT = i.Tgktchinhsua;
                            }
                            else TGKT = i.Thoigiaketthuc;
                            DateTime tgbd;
                            DateTime tgkt;
                            DateTime.TryParseExact(TGBD.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgbd);
                            DateTime.TryParseExact(TGKT.ToString(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tgkt);
                            int ss1 = startday.CompareTo(tgbd);
                            int ss2 = endday.CompareTo(tgbd);
                            int ss3 = startday.CompareTo(tgkt);
                            int ss4 = endday.CompareTo(tgkt);
                            if ((ss1 < 0 && ss2 > 0) || (ss3 < 0 && ss4 > 0))
                            {
                                ketqua.Add(i);
                            }

                        }

                        return View(ketqua);
                    }
                }
            }
            
            return View();
               
        }
        public static string Gettinhtrang(QuanlyTghd quanlyTghd)
        {
            if (quanlyTghd.Trangthaiduyetdon == true && quanlyTghd.Tinhtranghuy == null && quanlyTghd.Tinhtrangthamgia == null)
            {
                return "Đã được duyệt ";
            }

            if (quanlyTghd.Tinhtranghuy == true && quanlyTghd.Tinhtrangthamgia == null)
            {
                return "Đã hủy ";
            }
            if (quanlyTghd.Trangthaiduyetdon == true && quanlyTghd.Tinhtranghuy == null && quanlyTghd.Tinhtrangthamgia == true)
            {
                return "Đã tham gia";
            }
            return "Đã đăng ký";
        }
    }
}
