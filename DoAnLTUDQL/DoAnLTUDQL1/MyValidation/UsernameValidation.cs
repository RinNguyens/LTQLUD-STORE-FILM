using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DoAnLTUDQL1.MyValidation
{
    class UsernameValidation : ValidationRule
    {
        DBNewFilmEntities d = new DBNewFilmEntities();
        public int length { get; set; }
        public string type { get; set; }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string s = value as string;
            var q = d.User.Where(p => p.Name == s).FirstOrDefault();
            var e = new ValidationResult(true, null);
            if (string.IsNullOrEmpty(s))
                e = new ValidationResult(false, "Username không để trống");
            else if (s.Length < length)
                e = new ValidationResult(false, "Username phải từ 6 ký tự trở lên");
            else if (s.Length > 20)
                e = new ValidationResult(false, "Username phải không quá 20 ký tự");
            else if (q == null && type != "signup")
            {
                e = new ValidationResult(false, "Username không tồn tại");
            }
            else if (q != null && type == "signup")
                e = new ValidationResult(false, "Username đã tồn tại");
            return e;
        }
    }
}
