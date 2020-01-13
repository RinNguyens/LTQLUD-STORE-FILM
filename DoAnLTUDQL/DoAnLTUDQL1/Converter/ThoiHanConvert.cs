using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DoAnLTUDQL1.Converter
{
    class ThoiHanConvert : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DBNewFilmEntities d = new DBNewFilmEntities();
            var q = value as User;
            if (q != null && q.isCheckLoai != 2)
            {
                var e = q.UserThanhToan.OrderByDescending(p => p.NgayKT).FirstOrDefault();
                if (e != null)
                {
                    string date = e.NgayKT.Value.Subtract(DateTime.Now).Days.ToString();
                    return "Thời Hạn: Còn " + date + " Ngày";
                }
            }
            return "Thời Hạn: Tới Bến";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
