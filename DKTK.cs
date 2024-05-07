using PBL3_vstim.Models;
using System.ComponentModel.DataAnnotations;

namespace PBL3_vstim.ViewsModel
{
    public class DKTK
    {

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string Tendangnhap { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [MaxLength(20, ErrorMessage = "Mật khẩu phải có tối đa 20 ký tự.")]
        public string Matkhau { get; set; }

        [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        [Compare("Matkhau", ErrorMessage = "Mật khẩu nhập lại không trùng khớp.")]
        public string NLMK {  get; set; }
        
        [Required(ErrorMessage = "*")]
        public string Vaitro { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string Ten { get; set; }
        
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Sdt { get; set; }
        

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

    }
}
