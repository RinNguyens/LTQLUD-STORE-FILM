using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DoAnLTUDQL1.Converter
{
    class IsOnPlayListConvert : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // var a = values[0] as User;
            var a = values[0] as UserProfile;
            var e = values[2] as Phim;
            if (a != null)
            {
                if (values[1] != DependencyProperty.UnsetValue)
                {
                    var b = int.Parse(values[1].ToString());
                    DBNewFilmEntities d = new DBNewFilmEntities();
                    var q = d.UserProfile.Where(p => p.ID == a.ID && p.UserID == a.UserID).FirstOrDefault();
                    if (q != null)
                    {
                        var s = d.Profile_Playlist.Where(p => p.UserProfileID == q.UserID && p.ProfileID == q.ID && p.PhimID == b).FirstOrDefault();
                        if (s != null && e == null)
                            return Brushes.Red;
                        else if (s != null)
                            return Brushes.Gold;
                    }
                }
            }
            if (e != null)
                return Brushes.Black;
            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
