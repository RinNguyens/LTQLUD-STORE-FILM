using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAnLTUDQL1.Model
{
    class MyUserModel : User, IDataErrorInfo, INotifyPropertyChanged
    {
        string result;
        private string _Pwd;
        private string _confirmPwd;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Pasword
        {
            get
            {
                return _Pwd;
            }
            set
            {
                if (_Pwd != value)
                {
                    _Pwd = value;
                    OnPropertyChanged("Pasword");
                }
            }
        }
        public string ConfirmPasword
        {
            get
            {
                return _confirmPwd;
            }
            set
            {
                if (_confirmPwd != value)
                {
                    _confirmPwd = value;
                    OnPropertyChanged("ConfirmPasword");
                }
            }
        }

        private void OnPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public string this[string columnName]
        {
            get
            {
                result = null;
                if (!string.IsNullOrEmpty(Pasword) && Pasword.Length < 6)
                {
                    result = "Enter maximium 6 digit character";
                }
                if (!string.IsNullOrEmpty(ConfirmPasword))
                {
                    if (ConfirmPasword != Pasword)
                        result = "Password and Confirm Password isn't matched!";
                }
                return result;
            }
        }

        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
