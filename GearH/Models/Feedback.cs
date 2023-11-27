﻿namespace GearH.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Feedback")]
    public partial class Feedback
    {
        [Key]
        public int idFeedback { get; set; }

        public int idAccount { get; set; }

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(30)]
        public string name { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email không được để trống")]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Vui lòng điền đúng định dạng Email")]
        [StringLength(100)]
        public string email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0[3|5|7|8|9]\d{8}$", ErrorMessage = "Vui lòng nhập đúng định dạng số điện thoại")]
        [StringLength(20)]
        public string phone { get; set; }

        [Display(Name = "Ghi chú")]
        [Required(ErrorMessage = "Ghi chú không được để trống")]
        public string note { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime create_at { get; set; }

        public virtual Account Account { get; set; }
    }
}
