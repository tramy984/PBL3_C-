namespace PBL3_QLHDTN.ViewModel
{
    public class Hoatdong
    {
        public int Idhoatdong { get; set; }

        public int Idtochuc { get; set; }

        public int? Sltnvcan { get; set; }

        public string? Trangthai { get; set; }

        public string? Tenhoatdong { get; set; }

        public DateOnly? Thoigianbatdau { get; set; }

        public DateOnly? Thoigiaketthuc { get; set; }

        public DateOnly? Tgbdchinhsua { get; set; }

        public DateOnly? Tgktchinhsua { get; set; }

        public int? Linhvuc { get; set; }
        public string TenLV {  get; set; }

        public DateOnly? Thoigianhuy { get; set; }

        public string? Lydohuy { get; set; }

        public string? DiaDiem { get; set; }

        public string? MuctieuHd { get; set; }

        public static List<string> lv { get; set; }
    }
}
