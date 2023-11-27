namespace GearH.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Coupon")]
    public partial class Coupon
    {
        [Key]
        public int idCoupon { get; set; }

        [Display(Name = "Giảm giá")]
        [Required(ErrorMessage = "Giảm giá không được để trống")]
        [Range(1, 100, ErrorMessage = "Giảm giá trong khoảng từ 1% đến 100%")]
        public int discount { get; set; }

        [Display(Name = "Mã giảm giá")]
        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        [StringLength(100)]
        public string code { get; set; }

        [Display(Name = "Số lượng")]
        [Required(ErrorMessage = "Số lượng không được để trống")]
        public int quantity { get; set; }

        [Display(Name = "Điều kiện")]
        [Required(ErrorMessage = "Điều kiện không được để trống")]
        public string condition { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [DataType(DataType.Date)]
        public DateTime date_start { get; set; }

        [Display(Name = "Ngày kết thúc")]
        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        [DataType(DataType.Date)]
        public DateTime date_end { get; set; }
    }
}
