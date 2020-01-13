using De.TorstenMandelkow.MetroChart;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAnLTUDQL1.DataProvider
{
    class MainFormProvider : INotifyPropertyChanged
    {
        List<TheLoai> theloai;
        List<Phim> phimhot;
        List<Phim> phimle;
        List<Phim> toprating;
        List<Phim> newest;
        List<Phim> phimdexuat;
        List<CapDo> capdo;
        List<string> lstPhimName;
        List<ChartBase> lstChart;
        User user;
        UserProfile selectedProfile;
        Phim selectedPhim = new Phim();
        bool isFormCategory = false;
        double gridDetailHeight;
        List<string> previousForm;

        //List<CapDo> capdo;

        public List<TheLoai> TheLoai
        {
            get
            {
                return theloai;
            }

            set
            {
                theloai = value;
                OnPropertyChanged("TheLoai");
            }
        }

        public List<Phim> PhimHot
        {
            get
            {
                return phimhot;
            }

            set
            {
                phimhot = value;
                OnPropertyChanged("PhimHot");
            }
        }

        public List<Phim> PhimLe
        {
            get
            {
                return phimle;
            }

            set
            {
                phimle = value;
                OnPropertyChanged("PhimLe");
            }
        }

        public User User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
                OnPropertyChanged("User");
            }
        }

        public Phim SelectedPhim
        {
            get
            {
                return selectedPhim;
            }

            set
            {
                selectedPhim = value;
                OnPropertyChanged("SelectedPhim");
            }
        }

        public bool IsFormCategory
        {
            get
            {
                return isFormCategory;
            }

            set
            {
                isFormCategory = value;
                OnPropertyChanged("IsFormCategory");
            }
        }

        public double GridDetailHeight
        {
            get
            {
                return gridDetailHeight;
            }

            set
            {
                gridDetailHeight = value;
                OnPropertyChanged("GridDetailHeight");
            }
        }

        public List<string> PreviousForm
        {
            get
            {
                return previousForm;
            }

            set
            {
                previousForm = value;
                OnPropertyChanged("PreviousForm");
            }
        }

        public List<Phim> TopRating
        {
            get
            {
                return toprating;
            }

            set
            {
                toprating = value;
                OnPropertyChanged("TopRating");
            }
        }

        public List<Phim> Newest
        {
            get
            {
                return newest;
            }

            set
            {
                newest = value;
                OnPropertyChanged("Newest");
            }
        }

        public List<Phim> PhimDeXuat
        {
            get
            {
                return phimdexuat;
            }

            set
            {
                phimdexuat = value;
                OnPropertyChanged("PhimDeXuat");
            }
        }

        public List<CapDo> LstCapDo
        {
            get
            {
                return capdo;
            }

            set
            {
                capdo = value;
                OnPropertyChanged("LstCapDo");
            }
        }

        public UserProfile SelectedProfile
        {
            get
            {
                return selectedProfile;
            }

            set
            {
                selectedProfile = value;
                OnPropertyChanged("SelectedProfile");
            }
        }

        public List<ChartBase> LstChart
        {
            get
            {
                return lstChart;
            }

            set
            {
                lstChart = value;
                OnPropertyChanged("LstChart");
            }
        }

        public List<string> LstPhimName
        {
            get
            {
                return lstPhimName;
            }

            set
            {
                lstPhimName = value;
                OnPropertyChanged("LstPhimName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
