using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAnLTUDQL1.DataProvider
{
    class DBUser4Search : INotifyPropertyChanged
    {
        List<User> users;
        List<LoaiTaiKhoan> loaiTaiKhoan;
        List<CapDo> capDo;
        List<TinhTrangThanhToan> tinhTrang;
        private int curPage;
        private int totalPage;

        public List<LoaiTaiKhoan> LoaiTaiKhoan
        {
            get
            {
                return loaiTaiKhoan;
            }

            set
            {
                loaiTaiKhoan = value;
                OnPropertyChanged("LoaiTaiKhoan");
            }
        }

        public List<CapDo> CapDo
        {
            get
            {
                return capDo;
            }

            set
            {
                capDo = value;
                OnPropertyChanged("CapDo");
            }
        }

        public int CurPage
        {
            get
            {
                return curPage;
            }

            set
            {
                curPage = value;
                OnPropertyChanged("CurPage");
            }
        }

        public int TotalPage
        {
            get
            {
                return totalPage;
            }

            set
            {
                totalPage = value;
                OnPropertyChanged("TotalPage");
            }
        }

        public List<User> Users
        {
            get
            {
                return users;
            }

            set
            {
                users = value;
                OnPropertyChanged("Users");
            }
        }

        public List<TinhTrangThanhToan> TinhTrang
        {
            get
            {
                return tinhTrang;
            }

            set
            {
                tinhTrang = value;
                OnPropertyChanged("TinhTrang");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
