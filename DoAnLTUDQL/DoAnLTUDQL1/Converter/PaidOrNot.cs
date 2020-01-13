using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DoAnLTUDQL1.Converter
{
    class PaidOrNot : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int n = int.Parse(value.ToString());
            if (n == 1)
                return "Chưa thanh toán";
            if (n == 2)
                return "Đã thanh toán";
            return "Admin nên được miễn nè";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
