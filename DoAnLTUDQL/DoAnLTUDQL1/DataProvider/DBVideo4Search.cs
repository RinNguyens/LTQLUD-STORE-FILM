using DoAnLTUDQL1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAnLTUDQL1.DataProvider
{
    class DBVideo4Search : INotifyPropertyChanged
    {
        private List<TheLoai> cats;
        private List<Phim> release;
        private List<Phim> lstPhim;

        public List<Phim> LstPhim
        {
            get
            {
                return lstPhim;
            }

            set
            {
                lstPhim = value;
                OnPropertyChanged("LstPhim");
            }
        }

        public List<TheLoai> Cats
        {
            get
            {
                return cats;
            }

            set
            {
                cats = value;
                OnPropertyChanged("Cats");
            }
        }

        public List<Phim> VideoReleased
        {
            get
            {
                return release;
            }

            set
            {
                release = value;
                OnPropertyChanged("VideoReleased");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
