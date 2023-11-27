namespace GearH.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Text.RegularExpressions;

    [Table("Account")]
    public partial class Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Account()
        {
            Feedbacks = new HashSet<Feedback>();
            Orders = new HashSet<Order>();
        }

        [Key]
        public int idAccount { get; set; }

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(30)]
        public string name { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email không được để trống")]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Vui lòng điền đúng định dạng Email")]
        [UniqueEmail]
        public string email { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [PasswordValidation]
        public string password { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0[3|5|7|8|9]\d{8}$", ErrorMessage = "Vui lòng nhập đúng định dạng số điện thoại")]
        [UniquePhone]
        public string phone { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string address { get; set; }

        public int idRole { get; set; }

        public DateTime create_at { get; set; }

        public DateTime? update_at { get; set; }

        public virtual Role Role { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Feedback> Feedbacks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string email = value.ToString();
                dbGearHDataContext data = new dbGearHDataContext();
                var existEmail = data.Accounts.FirstOrDefault(u => u.email == email);
                if (existEmail != null)
                {
                    return new ValidationResult("Email đã được sử dụng");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class UniquePhoneAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string phone = value.ToString();
                dbGearHDataContext data = new dbGearHDataContext();
                var existEmail = data.Accounts.FirstOrDefault(u => u.phone == phone);
                if (existEmail != null)
                {
                    return new ValidationResult("Số điện thoại đã được sử dụng");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class PasswordValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                string password = value.ToString();

                if (password.Length < 8)
                {
                    ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.";
                    return false;
                }

                if (!char.IsUpper(password[0]))
                {
                    ErrorMessage = "Mật khẩu phải viết hoa chữ cái đầu.";
                    return false;
                }

                if (!Regex.IsMatch(password, @"[!@#$%^&*()]"))
                {
                    ErrorMessage = "Mật khẩu phải chứa ít nhất một ký tự đặc biệt.";
                    return false;
                }
            }

            return true;
        }
    }
}
