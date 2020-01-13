using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DoAnLTUDQL1.Converter
{
    class TenCapDoConvert : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var a = int.Parse(value.ToString());
            if (a == 1)
                return "Basic";
            if (a == 2)
                return "Standard";
            if (a == 3)
                return "Premium";
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
