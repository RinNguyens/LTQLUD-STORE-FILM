using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DoAnLTUDQL1.MyValidation
{
    class EmailValidation : ValidationRule
    {
        public string regex { get; set; }
        public string Msg { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var error = new ValidationResult(true, null);
            if (string.IsNullOrEmpty(regex))
                regex = "";
            if (!Regex.IsMatch(value.ToString(), regex))
                error = new ValidationResult(false, Msg);
            return error;
        }


    }
}
