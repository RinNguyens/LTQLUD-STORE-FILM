using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DoAnLTUDQL1.Converter
{
    class GridDetailHeight : IMultiValueConverter
    {
        int i = 0;
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double n = Double.Parse(values[2].ToString());
            double a = Double.Parse(values[1].ToString());
            //return Boolean.Parse(values[0].ToString())==false?n*0.78:n*0.65;
            //if(n<2)
            //    return n
            if (values[0].ToString() == "true")
                return n - a + 20;
            return n - a + 10;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

