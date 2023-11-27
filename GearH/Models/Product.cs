namespace GearH.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [Key]
        public int idProduct { get; set; }

        [Display(Name = "Tên sản phẩm")]
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(255)]
        public string name { get; set; }

        [Display(Name = "Tên hãng")]
        public int idBrand { get; set; }

        [Display(Name = "Loại danh mục")]
        public int idCategory { get; set; }

        [Display(Name = "Số lượng")]
        [Required(ErrorMessage = "Số lượng sản phẩm không được để trống")]
        [Range(1, 1000, ErrorMessage = "Số lượng nhập vào tối thiểu là 1")]
        public int quantity { get; set; }

        [Display(Name = "Số lượng")]
        public int sold { get; set; }

        [Display(Name = "Giá nhập")]
        [Required(ErrorMessage = "Giá nhập không được để trống")]
        [Range(1000, 1000000000, ErrorMessage = "Giá sản phẩm nhập vào tối thiểu là 1.000VNĐ")]
        public int OriginalPrice { get; set; }

        [Display(Name = "Giá sản phẩm")]
        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(1000, 1000000000, ErrorMessage = "Giá sản phẩm nhập vào tối thiểu là 1.000VNĐ")]
        public int price { get; set; }

        [Display(Name = "Mô tả sản phẩm")]
        [Required(ErrorMessage = "Mô tả sản phẩm không được để trống")]
        public string describe { get; set; }

        [Display(Name = "Ảnh minh họa")]
        [Required(ErrorMessage = "Ảnh minh họa không được để trống")]
        [StringLength(255)]
        public string image { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? create_at { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime? update_at { get; set; }

        public virtual Brand Brand { get; set; }

        public virtual Category Category { get; set; }
    }
}
