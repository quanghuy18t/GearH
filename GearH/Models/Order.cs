namespace GearH.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order
    {
        [Key]
        public int idOrder { get; set; }

        public int idAccount { get; set; }

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(30)]
        public string name { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string address { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0[3|5|7|8|9]\d{8}$", ErrorMessage = "Vui lòng nhập đúng định dạng số điện thoại")]
        public string phone { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email không được để trống")]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Vui lòng điền đúng định dạng Email")]
        public string email { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime order_date { get; set; }

        [Display(Name = "Trạng thái")]
        public int status { get; set; }

        public int payment { get; set; }

        [Display(Name = "Tổng tiền")]
        public int total { get; set; }

        [StringLength(10)]
        public string code { get; set; }

        public virtual Account Account { get; set; }
    }
}
