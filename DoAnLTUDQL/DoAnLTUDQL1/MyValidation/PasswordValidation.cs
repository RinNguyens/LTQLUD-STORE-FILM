using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DemoLTUDQL2.MyValidation
{
    class PasswordValidation:ValidationRule
    {
        public string regex;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = value as string;
            var e = new ValidationResult(true, null);
            if (string.IsNullOrEmpty(s))
                e = new ValidationResult(false, "Điền vào mật khẩu!");
            else if (s.Length < 6)
                e = new ValidationResult(false, "Mật khẩu yếu! Phải từ 8 ký tự trở lên");
            else if (!Regex.IsMatch(s, regex))
                e = new ValidationResult(false, "Mậu khẩu yếu! Phải gồm ít nhất 1 kí tự hoa và số");
            return e;
        }

    }
}
