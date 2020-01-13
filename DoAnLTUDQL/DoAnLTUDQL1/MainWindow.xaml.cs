using De.TorstenMandelkow.MetroChart;
using DoAnLTUDQL1.DataProvider;
using DoAnLTUDQL1.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DoAnLTUDQL1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isEntered;


        DBNewFilmEntities da = new DBNewFilmEntities();
        [ImportMany(typeof(ChartBase), AllowRecomposition = true)]
        public IEnumerable<Lazy<ChartBase>> Plugins;
        DBNewFilmEntities d = new DBNewFilmEntities();
        Storyboard enterStoryb = new Storyboard();
        bool isChanged = false;
        bool imgsearchunClicked = false;
        private double oldExtentWidth;
        private double oldExtentHeight;

        public MainWindow()
        {
            InitializeComponent();
            var mfp = this.FindResource("mainformprovider") as MainFormProvider;
            mfp.LstPhimName = da.Phim.Select(g => g.Name).ToList();
            mfp.PhimLe = da.Phim.ToList();
            mfp.User = null;
            mfp.PhimHot = da.Phim.OrderByDescending(p => p.Views).Take(10).ToList();
            mfp.TopRating = da.Phim.OrderByDescending(p => p.RatingIMDB).Take(10).ToList();
            mfp.Newest = da.Phim.OrderByDescending(p => p.NgayCapNhat).Take(10).ToList();
            mfp.SelectedPhim = null;
            mfp.TheLoai = da.TheLoai.ToList();
            mfp.LstCapDo = da.CapDo.ToList();
            mfp.PreviousForm = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                mfp.PhimLe.Add(da.Phim.ToList().FirstOrDefault());
                if (i % 2 == 0)
                    mfp.PhimLe.Add(da.Phim.Where(p => p.ID == 2).FirstOrDefault());
            }
            maintabcontrol.SelectedIndex = 12;
        }

        private void grid_MouseEnter(object sender, MouseEventArgs e)
        {
            isEntered = true;
            var g = sender as Grid;
            if (g != formchudeGrid)
            {
                var s = g.FindName("scrollviewer") as ScrollViewer;
                var r = s.FindName("rbright") as RepeatButton;
                if ((s.ContentHorizontalOffset >= s.ScrollableWidth && s.ContentHorizontalOffset != 0) || s.ComputedHorizontalScrollBarVisibility == Visibility.Collapsed)
                    r.Visibility = Visibility.Collapsed;
                else if (s.ComputedHorizontalScrollBarVisibility != Visibility.Collapsed)
                {
                    r.Visibility = Visibility.Visible;
                }
            }
        }

        private void grid_MouseLeave(object sender, MouseEventArgs e)
        {
            isEntered = false;
            var s = sender as Grid;
            if (s != formchudeGrid)
            {
                var r = s.FindName("rbright") as RepeatButton;
                r.Visibility = Visibility.Collapsed;
            }
        }

        private void ChangingTabControl()
        {
            var mfp = this.FindResource("mainformprovider") as MainFormProvider;
            mfp.SelectedPhim = null;
            if (mfp.SelectedProfile == null && maintabcontrol.SelectedIndex != 7)
                mfp.User = null;
            if (maintabcontrol.SelectedIndex != 4)
                mfp.IsFormCategory = false;
            maincrollviewer.IsEnabled = true;
            formchudeScroll.IsEnabled = true;
            formXemPhimScroll.IsEnabled = true;
            gridTrailer.Visibility = Visibility.Collapsed;
            mediaPhim.Pause();
            DoubleAnimation d = new DoubleAnimation();
            d.To = 1;
            d.From = 0;
            d.Duration = TimeSpan.FromSeconds(0.5);
            Storyboard.SetTargetName(d, "maintabcontrol");
            Storyboard.SetTargetProperty(d, new PropertyPath("Opacity"));
            Storyboard s = new Storyboard();
            s.Children.Add(d);

            s.Begin(maintabcontrol, true);
            var a = FindResource("mainformprovider") as MainFormProvider;
            a.PreviousForm.Add(maintabcontrol.SelectedIndex.ToString());
        }


        private void btnTieuDePhimHome_Click(object sender, RoutedEventArgs e)
        {
            var a = FindResource("mainformprovider") as MainFormProvider;
            a.IsFormCategory = true;
            maintabcontrol.SelectedIndex = 4;
            tbChuDe.Text = (sender as Button).Content.ToString();
            List<Phim> lst = (sender as Button).DataContext as List<Phim>;
            tabPhimChuDe5.DataContext = (sender as Button).DataContext;
            ChangingTabControl();
        }

        private void scrollviewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var s = sender as ScrollViewer;
            var r = s.FindName("rbright") as RepeatButton;
            var rl = s.FindName("rbleft") as RepeatButton;
            if (IsLoaded && (s.IsMouseOver || r.IsMouseOver))
            {
                if (s.ContentHorizontalOffset >= s.ScrollableWidth && s.ContentHorizontalOffset != 0)
                    r.Visibility = Visibility.Collapsed;
                else
                {
                    r.Visibility = Visibility.Visible;
                }
                if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
                {
                    Point mousePos = Mouse.GetPosition(formchudeScroll);
                    double offsetx = e.HorizontalOffset + mousePos.X;
                    double offsety = e.VerticalOffset + mousePos.Y;
                    oldExtentWidth = e.ExtentWidth - e.ExtentWidthChange;
                    oldExtentHeight = e.ExtentHeight - e.ExtentHeightChange;
                    double relx = offsetx / oldExtentWidth;
                    double rely = offsety / oldExtentHeight;
                    offsetx = Math.Max(relx * e.ExtentWidth - mousePos.X, 0);
                    offsety = Math.Max(rely * e.ExtentHeight - mousePos.Y, 0);
                    if (Double.IsNaN(offsetx) || Double.IsNaN(offsety))
                        return;
                    s?.ScrollToHorizontalOffset(offsetx);
                    s?.ScrollToVerticalOffset(offsety);
                }
            }
        }

        public Grid gridPhim;
        private void gridDataPhim_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gridPhim = sender as Grid;
            var g = gridPhim.DataContext as Phim;
            var heart = gridPhim.FindName("heart") as Path;
            if (heart.IsMouseOver)
                return;
            var mfp = this.FindResource("mainformprovider") as MainFormProvider;
            enterStoryb.Stop();
            enterStoryb.Pause();
            gridPhim.MouseLeave += MouseLeave_Video;
            if (mfp.IsFormCategory)
            {
                formchudeScroll.ScrollToBottom();
                formchudeScroll.IsEnabled = false;
            }
            else
            {
                if (maintabcontrol.SelectedIndex == 5)
                {
                    formXemPhimScroll.ScrollToBottom();
                    formXemPhimScroll.IsEnabled = false;
                }
                else
                {
                    maincrollviewer.ScrollToBottom();
                    maincrollviewer.IsEnabled = false;
                }
            }
            gridPhim.BringIntoView();
            //gridPhim.UpdateLayout();
            if (this.IsLoaded == true)
            {
                mfp.GridDetailHeight = gridPhim.RenderSize.Height;
                mfp.SelectedPhim = g;
            }
        }


        public void MouseEnter_Video(object sender, MouseEventArgs e)
        {
            var g = sender as Grid;
            var mg = this.FindName("maingrid") as Grid;
            var m = g.FindName("media") as MediaElement;
            var heart = g.FindName("heart") as Path;
            var stdetail = g.FindName("stackDetailMedia") as StackPanel;
            var mfp = this.FindResource("mainformprovider") as MainFormProvider;
            var d = new DoubleAnimation();
            d.To = 1.5;
            if (mfp.IsFormCategory)
                d.To = 1.1;
            d.Duration = TimeSpan.FromMilliseconds(500);
            Storyboard.SetTargetName(d, g.Name);
            Storyboard.SetTargetProperty(d, new PropertyPath("LayoutTransform.ScaleX"));
            var d2 = new DoubleAnimation();
            d2.To = 1.5;
            if (mfp.IsFormCategory)
                d2.To = 1.15;
            d2.Duration = TimeSpan.FromMilliseconds(500);
            Storyboard.SetTargetName(d2, g.Name);
            Storyboard.SetTargetProperty(d2, new PropertyPath("LayoutTransform.ScaleY"));
            enterStoryb = new Storyboard();
            enterStoryb.BeginTime = TimeSpan.FromSeconds(1);
            if (isEntered)
            {
                enterStoryb.BeginTime = TimeSpan.FromSeconds(0.3);
            }
            enterStoryb.Children.Add(d);
            enterStoryb.Children.Add(d2);
            enterStoryb.Completed += (o, a) =>
            {
                stdetail.Visibility = Visibility.Visible;
                m.Visibility = Visibility.Visible;
                m.Play();
            };
            //if (!heart.IsMouseOver)
            enterStoryb.Begin(g, true);
        }
        public void MouseLeave_Video(object sender, MouseEventArgs e)
        {
            var g = sender as Grid;
            var m = g.FindName("media") as MediaElement;
            var stdetail = g.FindName("stackDetailMedia") as StackPanel;
            var d = new DoubleAnimation();
            d.Duration = TimeSpan.FromMilliseconds(500);
            Storyboard.SetTargetName(d, g.Name);
            Storyboard.SetTargetProperty(d, new PropertyPath("LayoutTransform.ScaleX"));
            var d2 = new DoubleAnimation();
            d2.Duration = TimeSpan.FromMilliseconds(500);
            Storyboard.SetTargetName(d2, g.Name);
            Storyboard.SetTargetProperty(d2, new PropertyPath("LayoutTransform.ScaleY"));
            var s = new Storyboard();
            s.Children.Add(d);
            s.Children.Add(d2);
            enterStoryb.Stop();
            enterStoryb.Remove(g);
            s.Completed += (o, a) =>
            {
                stdetail.Visibility = Visibility.Collapsed;
                m.Visibility = Visibility.Collapsed;
                m.Pause();
            };
            s.Begin(g, true);
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            var m = sender as MediaElement;
            m.Position = TimeSpan.FromSeconds(0);
            m.Play();
        }
        private void tbProfileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            isChanged = true;

        }
        private void btnToPlayVideo_Click(object sender, RoutedEventArgs e)
        {
            var mfp = FindResource("mainformprovider") as MainFormProvider;
            gridMediaPhim.Visibility = Visibility.Collapsed;
            mfp.GridDetailHeight = 200;
            if (mfp.User != null)
            {
                var s = sender as Button;
                var phim = s.DataContext as Phim;
                tabXemPhim6.DataContext = phim;
                var lst = d.Profile_Playlist.Include("Phim").Where(p => p.UserProfileID == mfp.SelectedProfile.UserID && p.ProfileID == mfp.SelectedProfile.ID).ToList();

                if (lst.Count > 0)
                {

                    var thel = lst.GroupBy(p => p.Phim.TheLoai).Select(g => new { id = g.Key, count = g.Count() }).OrderByDescending(p => p.count).FirstOrDefault();
                    thel.id.Phim1.Remove(phim);
                    var lstdex = thel.id.Phim1.ToList();//d.Phim.Include("TheLoai1").Where(p => p.Category == thel.id).Take(10).ToList();
                    itemsourceDeXuat.ItemsSource = lstdex;
                }
                else
                {
                    itemsourceDeXuat.ItemsSource = mfp.PhimHot;
                }
                maintabcontrol.SelectedIndex = 5;
                mfp.IsFormCategory = false;
                formXemPhimScroll.ScrollToHome();
                ChangingTabControl();
            }
            else ShowTbMessage("Bạn phải đăng nhập để được xem phim!");
        }
        private void ShowTbMessage(string mes)
        {
            tbMessage.Text = mes;
            gridMessage.Visibility = Visibility.Visible;
            DoubleAnimation d = new DoubleAnimation();
            d.To = 1;
            d.From = 0;
            d.Duration = TimeSpan.FromSeconds(0.5);
            Storyboard.SetTargetName(d, "gridMessage");
            Storyboard.SetTargetProperty(d, new PropertyPath("Opacity"));
            Storyboard s = new Storyboard();
            s.Children.Add(d);
            DoubleAnimation d2 = new DoubleAnimation();
            d2.To = 0;
            d2.From = 1;
            d2.BeginTime = TimeSpan.FromSeconds(3);
            d2.Duration = TimeSpan.FromSeconds(2);
            Storyboard.SetTargetName(d2, "gridMessage");
            Storyboard.SetTargetProperty(d2, new PropertyPath("Opacity"));
            s.Children.Add(d2);
            s.Completed += (o, e) =>
            {
                gridMessage.Visibility = Visibility.Collapsed;
            };
            s.Begin(maintabcontrol, true);
        }
        private void heart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnAddPlaylist_Click(sender, e);
        }

        private void btnAddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var mfp = FindResource("mainformprovider") as MainFormProvider;
            if (mfp.User == null || mfp.SelectedProfile == null)
                ShowTbMessage("Bạn cần đăng nhập để thêm Video vào playlist!");
            else
            {
                Phim phim = (sender as Button)?.DataContext as Phim;
                if (phim == null)
                    phim = (sender as Path).Tag as Phim;
                var q = d.UserProfile.Where(p => p.UserID == mfp.SelectedProfile.UserID && p.ID == mfp.SelectedProfile.ID).FirstOrDefault();
                if (q != null)
                {
                    var a = d.Profile_Playlist.Where(p => p.Phim.ID == phim.ID && p.UserProfileID == q.UserID && p.ProfileID == q.ID).FirstOrDefault();
                    if (a != null)
                    {
                        d.Profile_Playlist.Remove(a);
                        if (d.SaveChanges() > 0)
                        {
                            {
                                ShowTbMessage("Đã xóa khỏi Playlist!");
                                mfp.SelectedProfile = q;
                            }
                        }
                    }
                    else
                    {
                        a = new Profile_Playlist();
                        a.UserProfileID = mfp.SelectedProfile.UserID;
                        a.ProfileID = mfp.SelectedProfile.ID;
                        a.PhimID = phim.ID;
                        d.Profile_Playlist.Add(a);
                        if (d.SaveChanges() > 0)
                        {
                            ShowTbMessage("Đã thêm vào Playlist!");
                            mfp.SelectedProfile = q;
                        }
                    }
                }

            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(tbPassSignUp) || Validation.GetHasError(tbUserSignUp) || Validation.GetHasError(tbConfirmSignUp) || Validation.GetHasError(tbEmailSignUp)
                || Validation.GetHasError(tbPhoneSignUp) || string.IsNullOrEmpty(tbUserSignUp.Text) || string.IsNullOrEmpty(tbPassSignUp.Password) || string.IsNullOrEmpty(tbConfirmSignUp.Password)
                || string.IsNullOrEmpty(tbEmailSignUp.Text) || string.IsNullOrEmpty(tbPhoneSignUp.Text) || cbAcountLevel.SelectedValue == null)
                ShowTbMessage("Thông tin chưa đủ hoặc chưa hợp lệ!");
            else
            {
                var mfp = this.FindResource("mainformprovider") as MainFormProvider;
                int thanhtoan = 1;
                MessageBoxResult msg = MessageBox.Show("Bạn có muốn thanh toán luôn không", "", MessageBoxButton.YesNoCancel);
                if (msg == MessageBoxResult.Yes)
                    thanhtoan = 2;
                else if (msg == MessageBoxResult.Cancel)
                    return;
                var a = new User()
                {
                    //ID = d.User.OrderByDescending(p => p.ID).FirstOrDefault().ID + 1,
                    Name = tbUserSignUp.Text,
                    isCheckLoai = 1,
                    MaCapDo = int.Parse(cbAcountLevel.SelectedValue.ToString()),
                    Password = tbPassSignUp.Password,
                    Email = tbEmailSignUp.Text,
                    Phone = tbPhoneSignUp.Text,
                    ThanhToan = thanhtoan,
                    Hinh = "C:\\ltudql1_17ck3_17\\Icons\\user.png"
                };
                d.User.Add(a);

                var usertt = new UserThanhToan() { UserID = a.ID, CapDo = a.MaCapDo };
                if (thanhtoan == 2)
                {
                    usertt.NgayTT = DateTime.Now;
                    usertt.NgayKT = usertt.NgayTT.AddDays((double)(cbAcountLevel.SelectedItem as CapDo).SoNgay);
                    d.UserThanhToan.Add(usertt);
                    a.ThanhToan = 2;
                }
                if (d.SaveChanges() > 0)
                    ShowTbMessage("Đăng ký thành công");
                mfp.User = d.User.Where(p => p.Name == a.Name).FirstOrDefault();
                maintabcontrol.SelectedIndex = 0;
                ChangingTabControl();

            }
        }

        private void btnDangky_Click(object sender, RoutedEventArgs e)
        {
            var a = this.FindResource("mainformprovider") as MainFormProvider;
            maintabcontrol.SelectedIndex = 3;
            tabSignUp4.DataContext = new MyUserModel();
            ChangingTabControl();
        }

        private void home_Click(object sender, RoutedEventArgs e)
        {
            var a = this.FindResource("mainformprovider") as MainFormProvider;
            a.IsFormCategory = false;
            if (a.User != null && a.SelectedProfile != null)
                maintabcontrol.SelectedIndex = 0;
            else
                maintabcontrol.SelectedIndex = 12;
            ChangingTabControl();
        }

        private void btnSubmitDN_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(tbId) || Validation.GetHasError(tbPass))
                return;
            var q = d.User.Include("Phim").Include("UserThanhToan").Where(p => p.Name == tbId.Text && p.Password == tbPass.Password).FirstOrDefault();
            if (q == null)
                ShowTbMessage("Mật khẩu chưa chính xác");
            else
            {
                var mfp = this.FindResource("mainformprovider") as MainFormProvider;
                if (q.isCheckLoai != 2)
                {
                    q.ThanhToan = q.UserThanhToan.OrderByDescending(p => p.NgayKT).FirstOrDefault().NgayKT < DateTime.Now ? 1 : 2;
                    d.SaveChanges();
                }
                mfp.User = q;
                maintabcontrol.SelectedIndex = 7;
                tabChoseProfile8.DataContext = q.UserProfile;
                ChangingTabControl();
                ShowTbMessage("Đăng nhập thành công");
            }
        }
        private void btnSubmitDN_MouseEnter(object sender, MouseEventArgs e)
        {
            btnSubmitDN.Foreground = Brushes.Black;
            btnSubmitDN.Background = Brushes.Green;
        }

        private void btnSubmitDN_MouseLeave(object sender, MouseEventArgs e)
        {
            btnSubmitDN.Foreground = Brushes.White;

        }
        private void btnForgot_Click(object sender, RoutedEventArgs e)
        {
            maintabcontrol.SelectedIndex = 2;
            ChangingTabControl();
        }

        private void btnExitAddProfile_Click(object sender, RoutedEventArgs e)
        {
            gridAddProfile.Visibility = Visibility.Collapsed;
        }

        private void stProfile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var a = FindResource("mainformprovider") as MainFormProvider;
            a.SelectedProfile = (sender as StackPanel).DataContext as UserProfile;
            maintabcontrol.SelectedIndex = 0;
            ChangingTabControl();
        }

        private void btnAddProfile_Click(object sender, RoutedEventArgs e)
        {
            var mfp = FindResource("mainformprovider") as MainFormProvider;
            var q = da.UserProfile.Where(p => p.UserID == mfp.User.ID).OrderByDescending(p => p.ID).FirstOrDefault();
            var pro = gridAddProfile.DataContext as UserProfile;
            if (string.IsNullOrEmpty(pro.ProfileName))
            {
                ShowTbMessage("Nhập ProfileName");
                return;
            }
            var t = da.UserProfile.Where(p => p.ID == mfp.User.ID && p.ProfileName == pro.ProfileName).FirstOrDefault();
            if (t != null)
            {
                ShowTbMessage("ProfileName đã tồn tại!");
                return;
            }
            da.UserProfile.Add(new UserProfile() { ID = q == null ? 1 : q.ID + 1, UserID = mfp.User.ID, ProfileName = pro.ProfileName, Hinh = pro.Hinh });
            mfp.User = da.User.Where(p => p.ID == mfp.User.ID).FirstOrDefault();
            if (da.SaveChanges() > 0)
            {
                ShowTbMessage("Thêm Profile thành công!");
                gridAddProfile.Visibility = Visibility.Collapsed;
                tabChoseProfile8.DataContext = mfp.User.UserProfile.ToList();
            }
            else ShowTbMessage("Something go wong!");
        }


        private void btnLayLaiPass_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(tbUserForgotPass) || Validation.GetHasError(tbPhoneForgotPass) || Validation.GetHasError(tbEmailForgotPass))
                return;
            var q = d.User.Where(p => p.Name == tbUserForgotPass.Text && p.Email == tbEmailForgotPass.Text && p.Phone == tbPhoneForgotPass.Text).FirstOrDefault();
            if (q != null)
            {
                ShowTbMessage("Your Password Is: " + q.Password);
                maintabcontrol.SelectedIndex = 1;
                ChangingTabControl();
            }
        }
        private void btnSaveProfile_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(tbEmailDetail) || Validation.GetHasError(tbPhoneDetail) || string.IsNullOrEmpty(tbProfileName.Text))
                return;
            var a = FindResource("mainformprovider") as MainFormProvider;
            var q = a.User.UserProfile.Where(p => p.ProfileName == tbProfileName.Text).FirstOrDefault();
            if (q != null && tbProfileName.Text != a.SelectedProfile.ProfileName)
            {
                ShowTbMessage("Profile Name trùng với profile khác cùng tài khoản");
                return;
            }
            a.SelectedProfile.Hinh = imageProfileDetail.Source.ToString();
            a.SelectedProfile.ProfileName = tbProfileName.Text;
            if (d.SaveChanges() > 0)
                ShowTbMessage("Đã lưu");
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            var a = FindResource("mainformprovider") as MainFormProvider;
            var u = d.User.Where(p => p.ID == a.User.ID).FirstOrDefault();
            var pro = a.User.UserProfile.Where(p => p.ProfileName == a.SelectedProfile.ProfileName).FirstOrDefault();
            a.User = null;
            a.SelectedProfile = null;
            a.User = u;
            a.SelectedProfile = pro;
            imageProfileDetail.Source = new BitmapImage(new Uri(a.SelectedProfile.Hinh));
        }
        private void btnToThanhToan_Click(object sender, RoutedEventArgs e)
        {
            gridThanhToan.Visibility = Visibility.Visible;
            DoubleAnimation d = new DoubleAnimation();
            d.To = 1;
            d.From = 0;
            d.Duration = TimeSpan.FromSeconds(0.5);
            Storyboard.SetTargetName(d, "gridThanhToan");
            Storyboard.SetTargetProperty(d, new PropertyPath("Opacity"));
            Storyboard s = new Storyboard();
            s.Children.Add(d);
            s.Begin(gridThanhToan, true);
        }

        private void borderToAddProfile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gridAddProfile.Visibility = Visibility.Visible;
            var q = new UserProfile();
            q.Hinh = "C:\\hahaha\\Icons\\user.png";
            gridAddProfile.DataContext = q;
        }
        private void cbModeUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbModeUser.SelectedItem != null && IsLoaded)
            {

                string s = (cbModeUser.SelectedItem as ComboBoxItem).Content.ToString();
                if (s == "Searching")
                {
                    gridAddUserMode.Visibility = Visibility.Collapsed;
                    btnAddNewUser.Visibility = Visibility.Collapsed;
                    btnManageUserSearch.Visibility = Visibility.Visible;
                    gridUser.DataContext = new User();
                    Validation.SetErrorTemplate(tbUserName, null);
                    Validation.SetErrorTemplate(tbPhone, null);
                    Validation.SetErrorTemplate(tbEmail, null);
                }
                else
                {
                    var template = this.FindResource("tberrortemplate") as ControlTemplate;
                    Validation.SetErrorTemplate(tbUserName, template);
                    Validation.SetErrorTemplate(tbPhone, template);
                    Validation.SetErrorTemplate(tbEmail, template);
                    gridUser.DataContext = new User();
                    gridAddUserMode.Visibility = Visibility.Visible;
                    btnAddNewUser.Visibility = Visibility.Visible;
                    btnManageUserSearch.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void dtgUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((cbModeUser.SelectedItem as ComboBoxItem).Content.ToString() == "Adding" && ckbFill.IsChecked == true)
            {
                var q = dtgUser.SelectedItem as User;
                if (q != null)
                {
                    gridUser.DataContext = q;
                }
            }
        }
        private void CloseTextBox(TextBox t)
        {
            DoubleAnimation d = new DoubleAnimation();
            d.Duration = TimeSpan.FromSeconds(0.2);
            Storyboard.SetTargetName(d, "tbsearch");
            Storyboard.SetTargetProperty(d, new PropertyPath("LayoutTransform.ScaleX"));
            Storyboard st = new Storyboard();
            st.Children.Add(d);
            st.Completed += (o, a) => { t.Visibility = Visibility.Collapsed; };
            st.Begin(t, true);

            imgsearchunClicked = false;
        }
        private void tbsearch_LostFocus(object sender, RoutedEventArgs e)
        {
            var t = sender as TextBox;
            CloseTextBox(t);
        }
        private void btnAddNewUser_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(tbUserName) || string.IsNullOrEmpty(tbPassword.Text) || Validation.GetHasError(tbEmail) || Validation.GetHasError(tbPhone) || cbCapDo.SelectedValue == null
                || cbTT.SelectedValue == null || cbLoaiTaiKhoan.SelectedValue == null)
            {
                ShowTbMessage("Chưa đủ thông tin");
                return;
            }
            try
            {
                var db = FindResource("DBUser4Search") as DBUser4Search;
                var q = gridUser.DataContext as User;
                if (q != null && d.User.Where(p => p.Name == q.Name).FirstOrDefault() == null)
                {
                    d.User.Add(q);
                    if (d.SaveChanges() > 0)
                        ShowTbMessage("Đã Thêm");
                    db.Users = d.User.ToList();
                    q = db.Users.Where(p => p.Name == q.Name).FirstOrDefault();
                    dtgUser.SelectedItem = q;
                }
            }
            catch (Exception ex)
            {
                ShowTbMessage(ex.InnerException.ToString());
            }
        }
        private void btnManageUserSearch_Click(object sender, RoutedEventArgs e)
        {
            IQueryable<User> q = d.User;
            if (!string.IsNullOrEmpty(tbUserName.Text))
                q = q.Where(p => p.Name.Contains(tbUserName.Text));
            if (!string.IsNullOrEmpty(tbEmail.Text))
                q = q.Where(p => p.Email.Contains(tbEmail.Text));
            if (!string.IsNullOrEmpty(tbPhone.Text))
                q = q.Where(p => p.Phone.Contains(tbPhone.Text));
            int n;
            if (cbCapDo.SelectedValue != null)
            {
                n = int.Parse(cbCapDo.SelectedValue.ToString());
                q = q.Where(p => p.MaCapDo == n);
            }
            if (cbLoaiTaiKhoan.SelectedValue != null)
            {
                n = int.Parse(cbLoaiTaiKhoan.SelectedValue.ToString());
                q = q.Where(p => p.isCheckLoai == n);
            }
            if (cbTT.SelectedValue != null)
            {
                n = int.Parse(cbLoaiTaiKhoan.SelectedValue.ToString());
                q = q.Where(p => p.ThanhToan == n);
            }
            var db = FindResource("DBUser4Search") as DBUser4Search;
            db.Users = q.ToList();
        }

        private void btnHuyThanhToan_Click(object sender, RoutedEventArgs e)
        {
            gridThanhToan.Visibility = Visibility.Collapsed;
            DoubleAnimation d = new DoubleAnimation();
            d.To = 0;
            d.From = 1;
            d.Duration = TimeSpan.FromSeconds(0.5);
            Storyboard.SetTargetName(d, "gridThanhToan");
            Storyboard.SetTargetProperty(d, new PropertyPath("Opacity"));
            Storyboard s = new Storyboard();
            s.Children.Add(d);
            s.Begin(gridThanhToan, true);
        }
        private void btnThanhToan_Click(object sender, RoutedEventArgs e)
        {
            var cd = cbThanhToan.SelectedItem as CapDo;
            if (cd != null)
            {
                var mfp = FindResource("mainformprovider") as MainFormProvider;
                var user = d.User.Where(p => p.ID == mfp.User.ID).FirstOrDefault();
                if (user != null)
                {
                    user.MaCapDo = cd.ID;
                    user.ThanhToan = 2;
                    var capdo = d.CapDo.Where(p => p.ID == user.MaCapDo).FirstOrDefault();
                    var tt = new UserThanhToan();
                    tt.UserID = user.ID;
                    tt.CapDo = user.MaCapDo;
                    tt.NgayTT = DateTime.Now;
                    tt.NgayKT = tt.NgayTT.AddDays((double)capdo.SoNgay);
                    d.UserThanhToan.Add(tt);
                    if (d.SaveChanges() > 1)
                        ShowTbMessage("Đã thanh toán");
                    mfp.User = null;
                    mfp.User = d.User.Where(p => p.ID == user.ID).FirstOrDefault();
                    dtgLSTT.Items.Refresh();
                    btnHuyThanhToan_Click(sender, e);
                }
            }
        }
        private void btnDetailPhim_Click(object sender, RoutedEventArgs e)
        {
            var mfp = this.FindResource("mainformprovider") as MainFormProvider;
            if (mfp.IsFormCategory)
                formchudeScroll.IsEnabled = true;
            else
            {
                maincrollviewer.IsEnabled = true;
                formXemPhimScroll.IsEnabled = true;
            }
            mfp.SelectedPhim = null;
            //btnDetailPhim.
        }


        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnDetailPhim_Click(sender, e);
        }


        private void gridChude_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var g = sender as Grid;
            var b = sender as Border;
            TheLoai cd = null;
            if (g != null)
            {
                cd = g.Tag as TheLoai;
            }
            else
                cd = b.Tag as TheLoai;
            var a = this.FindResource("mainformprovider") as MainFormProvider;
            a.IsFormCategory = true;
            List<Phim> lst = cd.Phim1.ToList();
            tbChuDe.Text = cd.Name;
            maintabcontrol.SelectedIndex = 4;
            if (lst.Count < 1)
                tbChuDe.Text = "Không có phim nào để hiển thị";
            tabPhimChuDe5.DataContext = lst;
            ChangingTabControl();
        }
        private void tbPass_MouseEnter(object sender, MouseEventArgs e)
        {

            tbPass.Opacity = 0.8;

        }

        private void tbPass_MouseLeave(object sender, MouseEventArgs e)
        {
            tbPass.Opacity = 0.5;
        }

        private void cbDoanhThu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var q = new ObservableCollection<Revenue4Chart>();
            if (cbDoanhThu.SelectedItem == null)
                return;
            var r = (cbDoanhThu.SelectedItem as ComboBoxItem).Content.ToString();
            if (!string.IsNullOrEmpty(r))
            {
                int thang = int.Parse(cbChonTheoThang.SelectedValue.ToString());
                int nam = int.Parse(cbChonTheoNam.SelectedValue.ToString());
                txtChonThang.Visibility = Visibility.Collapsed;
                cbChonTheoThang.Visibility = Visibility.Collapsed;
                switch (r)
                {
                    case "Ngày trong tháng":
                        txtChonThang.Visibility = Visibility.Visible;
                        cbChonTheoThang.Visibility = Visibility.Visible;
                        chartDT.ChartSubTitle = "Thống kê Doanh thu " + thang + " / " + nam;
                        chartseriesDT.Caption = thang + " / " + nam;
                        int n = DateTime.DaysInMonth(nam, thang);
                        for (int i = 1; i <= n; i++)
                        {
                            var item = new Revenue4Chart();
                            item.Name = i.ToString();
                            item.Value = 0;
                            var lst = d.UserThanhToan.Where(p => p.NgayTT.Day == i && p.NgayTT.Month == thang && p.NgayTT.Year == nam).ToList();
                            foreach (var a in lst)
                            {
                                item.Value += (double)a.CapDo1.Phi;
                            }
                            q.Add(item);
                        }
                        break;
                    case "Tháng trong năm":
                        chartDT.ChartSubTitle = "Thống kê Doanh thu theo Tháng trong năm " + nam;
                        chartseriesDT.Caption = "Năm " + nam;
                        for (int i = 1; i < 13; i++)
                        {
                            var item = new Revenue4Chart();
                            item.Name = i.ToString();
                            item.Value = 0;
                            var lst = d.UserThanhToan.Where(p => p.NgayTT.Month == i && p.NgayTT.Year == nam).ToList();
                            foreach (var a in lst)
                            {
                                item.Value += (double)a.CapDo1.Phi;
                            }
                            q.Add(item);
                        }
                        break;
                    case "Quý trong năm":
                        chartDT.ChartSubTitle = "Thống kê Doanh thu theo Quý trong năm " + nam;
                        chartseriesDT.Caption = "Năm " + nam;
                        for (int i = 1; i < 13; i += 3)
                        {
                            var item = new Revenue4Chart();
                            item.Name = i.ToString() + "-" + (i + 2).ToString();
                            item.Value = 0;
                            var lst = d.UserThanhToan.Where(p => p.NgayTT.Month >= i && p.NgayTT.Month <= i + 2 && p.NgayTT.Year == nam).ToList();
                            foreach (var a in lst)
                            {
                                item.Value += (double)a.CapDo1.Phi;
                            }
                            q.Add(item);
                        }
                        break;
                    default:
                        break;
                }
                chartseriesDT.ItemsSource = null;
                chartseriesDT.ItemsSource = q;
            }
        }
        private void ManageMediaElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var s = sender as MediaElement;
            var sli = s.Tag as Slider;
            sli.Maximum = s.NaturalDuration.TimeSpan.TotalSeconds;
            var q = s.Position;
            Thread.Sleep(100);
            var a = s.Position;
            if (s.Position == s.NaturalDuration)
                s.Position = TimeSpan.FromSeconds(0);
            if (q.Milliseconds != a.Milliseconds)
                s.Pause();
            else
                s.Play();
        }
        private void playVideo_Click(object sender, RoutedEventArgs e)
        {
            var q = (sender as Button).DataContext as MediaElement;
            var sli = q.Tag as Slider;
            if (q != null)
            {

                if (q.Position == q.NaturalDuration)
                    q.Position = TimeSpan.FromSeconds(0);
                q.Play();

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += (o, z) =>
                {
                    if (q.Source != null && q.NaturalDuration.HasTimeSpan)
                    {
                        sli.Maximum = q.NaturalDuration.TimeSpan.TotalSeconds;
                        sli.Value = q.Position.TotalSeconds;
                        if (q.Name == "trailer")
                        {
                            var s = q.FindName("tbwidthtrailer") as TextBlock;
                            var t = q.FindName("tbheighttrailer") as TextBlock;
                            var p = q.FindName("progress") as TextBlock;
                            s.Text = q.NaturalVideoWidth.ToString();
                            t.Text = q.NaturalVideoHeight.ToString();
                            p.Text = q.Position.ToString(@"hh\:mm\:ss") + "/" + q.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
                        }
                    }
                };
                timer.Start();
            }
        }

        private void pauseVideo_Click(object sender, RoutedEventArgs e)
        {
            var q = (sender as Button).DataContext as MediaElement;
            if (q != null)
            {
                q.Pause();

            }
        }

        private void stopVideo_Click(object sender, RoutedEventArgs e)
        {
            var q = (sender as Button).DataContext as MediaElement;
            if (q != null)
            {
                q.Stop();
            }
        }
        private void btnExitGridTrailer_Click(object sender, RoutedEventArgs e)
        {
            mediaTrailer.Stop();
            mediatooltip.Stop();
            gridTrailer.Visibility = Visibility.Collapsed;
        }


        private void borderTrailer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnExitGridTrailer_Click(sender, e);
        }
        bool userIsDraggingSlider = false;
        private void btnXemTrailer_Click(object sender, RoutedEventArgs e)
        {
            mediaTrailer.Play();
            //mediatooltip.Play();
            if (mediaPhim.Source != null && gridMediaPhim.Visibility == Visibility.Visible)
                mediaPhim.Pause();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            timer.Tag = mediaTrailer;
            gridTrailer.Visibility = Visibility.Visible;
        }

        private void btnPlayPhim_Click(object sender, RoutedEventArgs e)
        {
            var mfp = FindResource("mainformprovider") as MainFormProvider;
            var tt = mfp.User.UserThanhToan.OrderByDescending(p => p.NgayKT).FirstOrDefault().NgayKT;
            if (mfp.User.isCheckLoai != 2 && ((tt != null && tt < DateTime.Now) || tt == null))
            {
                ShowTbMessage("Hãy thanh toán để được xem phim");
                return;
            }
            gridMediaPhim.Visibility = Visibility.Visible;
            playMediaGridPhim.Visibility = Visibility.Collapsed;
            pauseMediaGrid.Visibility = Visibility.Visible;
            var phim = (sender as Button).DataContext as Phim;
            var a = da.Profile_Playlist.Where(p => p.ProfileID == mfp.SelectedProfile.ID && p.UserProfileID == mfp.SelectedProfile.UserID && p.PhimID == phim.ID).FirstOrDefault();
            DoubleAnimation d = new DoubleAnimation();
            d.From = 0;
            d.Duration = TimeSpan.FromSeconds(0.5);
            //d.To = (maintabcontrol.ActualWidth / mediaPhim.Width) - 0.2;
            d.To = 1;
            Storyboard.SetTargetName(d, "gridMediaPhim");
            Storyboard.SetTargetProperty(d, new PropertyPath("LayoutTransform.ScaleX"));
            DoubleAnimation d2 = new DoubleAnimation();
            d2.From = 0;
            d2.Duration = TimeSpan.FromSeconds(0.5);
            d2.To = 1.3;
            Storyboard.SetTargetName(d2, "gridMediaPhim");
            Storyboard.SetTargetProperty(d2, new PropertyPath("LayoutTransform.ScaleY"));
            Storyboard s = new Storyboard();
            s.Children.Add(d);
            s.Children.Add(d2);
            s.Completed += (o, q) =>
            {
                mediaPhim.Play();
                mediaPhim.Tag = null;
                if (a != null)
                {
                    mediaPhim.Tag = a;
                    mediaPhim.Pause();
                    if (a.ThoiGianXem > TimeSpan.FromSeconds(1) && MessageBox.Show("Lần trước bạn xem tới " + String.Format("{0:hh\\:mm\\:ss}", a.ThoiGianXem) + "\nBạn có muốn tiếp tục tại đó không", "Some Title"
                    , MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        mediaPhim.Position = a.ThoiGianXem.Value;
                    mediaPhim.Play();
                }
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();
                timer.Tag = mediaPhim;
            };
            s.Begin(gridMediaPhim, true);
            formXemPhimScroll.ScrollToTop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var m = (sender as DispatcherTimer).Tag as MediaElement;
            //mediaPhim.Play();
            if ((m.Source != null) && (m.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                if (m == mediaTrailer)
                {
                    sliProgress.Minimum = 0;
                    sliProgress.Maximum = mediaTrailer.NaturalDuration.TimeSpan.TotalSeconds;
                    sliProgress.Value = mediaTrailer.Position.TotalSeconds;
                    lblMaxProgressStatus.Text = " / " + TimeSpan.FromSeconds(sliProgress.Maximum).ToString(@"hh\:mm\:ss");
                    tbPhanGiai.Text = m.NaturalVideoWidth.ToString() + "x" + m.NaturalVideoHeight.ToString();
                }
                else
                {
                    sliProgressPhim.Minimum = 0;
                    sliProgressPhim.Maximum = mediaPhim.NaturalDuration.TimeSpan.TotalSeconds;
                    sliProgressPhim.Value = mediaPhim.Position.TotalSeconds;
                    tbMaxDuration.Text = " / " + TimeSpan.FromSeconds(sliProgressPhim.Maximum).ToString(@"hh\:mm\:ss");
                    tbPhanGiaiPhim.Text = m.NaturalVideoWidth.ToString() + "x" + m.NaturalVideoHeight.ToString();
                    var a = m.Tag as Profile_Playlist;
                    if (a != null)
                    {
                        a = d.Profile_Playlist.Where(p => p.ProfileID == a.ProfileID && p.PhimID == a.PhimID && p.UserProfileID == a.UserProfileID).FirstOrDefault();
                        a.ThoiGianXem = TimeSpan.FromSeconds(mediaPhim.Position.TotalSeconds);
                        if (a.ThoiGianXem.Value == mediaPhim.NaturalDuration)
                            a.ThoiGianXem = TimeSpan.FromSeconds(0);
                        if (a.ThoiGianXem > TimeSpan.FromMinutes(0.5))
                            d.Phim.Where(p => p.ID == a.PhimID).FirstOrDefault().Views += 1;
                        d.SaveChanges();
                    }

                }
            }
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var m = (sender as Slider).Tag as MediaElement;
            if (m == mediaPhim)
            {
                tbCurrent.Text = TimeSpan.FromSeconds(sliProgressPhim.Value).ToString(@"hh\:mm\:ss");
                if (sliProgressPhim.Value == sliProgressPhim.Maximum)
                {
                    pauseMediaGridPhim.Visibility = Visibility.Collapsed;
                    playMediaGridPhim.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
                if (sliProgress.Value == sliProgress.Maximum)
                {
                    pauseMediaGrid.Visibility = Visibility.Collapsed;
                    playMediaPath.Visibility = Visibility.Visible;
                }
            }
        }
        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            var s = sender as Slider;
            if (s == sliProgressPhim)
                mediaPhim.Position = TimeSpan.FromSeconds(s.Value);
            else
                mediaTrailer.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void playMediaPath_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var media = sender as MediaElement;
            if (media == null)
            {
                media = (sender as Grid).Tag as MediaElement;
            }
            if (media.Position == media.NaturalDuration.TimeSpan)
                media.Position = TimeSpan.FromSeconds(0);
            media.Play();
            if (media == mediaPhim)
            {
                pauseMediaGridPhim.Visibility = Visibility.Visible;
                playMediaGridPhim.Visibility = Visibility.Collapsed;
            }
            else
            {
                pauseMediaGrid.Visibility = Visibility.Visible;
                playMediaPath.Visibility = Visibility.Collapsed;
            }
        }

        private void pauseMediaGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var media = sender as MediaElement;
            if (media == null)
            {
                media = (sender as Grid).Tag as MediaElement;
            }
            media.Pause();
            if (media == mediaPhim)
            {
                pauseMediaGridPhim.Visibility = Visibility.Collapsed;
                playMediaGridPhim.Visibility = Visibility.Visible;
            }
            else
            {
                pauseMediaGrid.Visibility = Visibility.Collapsed;
                playMediaPath.Visibility = Visibility.Visible;
            }
        }

        int timeTick = 0;
        DispatcherTimer timer;
        private void gridMediaTrailer_MouseEnter(object sender, MouseEventArgs e)
        {
            var gr = sender as Grid;
            var gridStatus = gr.Tag as Grid;
            gridStatus.Visibility = Visibility.Visible;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (o, q) =>
            {
                timeTick++;
                if (gr.IsMouseOver == false)
                    timer.Stop();
                if (timeTick < 6)
                    gridStatus.Visibility = Visibility.Visible;
                else
                {
                    gridStatus.Visibility = Visibility.Collapsed;
                }
            };
            timer.Start();
        }

        private void gridMediaTrailer_MouseLeave(object sender, MouseEventArgs e)
        {
            var g = (sender as Grid);
            if (g == gridMediaPhim)
                gridstbPhim.Visibility = Visibility.Collapsed;
            else
                gridStatusBar.Visibility = Visibility.Collapsed;
            timeTick = 0;
            timer.Stop();
        }

        private void mediaTrailer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var m = sender as MediaElement;

            if (playMediaPath.Visibility == Visibility.Visible && m == mediaTrailer || (m == mediaPhim && playMediaGridPhim.Visibility == Visibility.Visible))
                playMediaPath_MouseLeftButtonUp(m, e);
            else if (pauseMediaGrid.Visibility == Visibility.Visible && m == mediaTrailer || (m == mediaPhim && pauseMediaGridPhim.Visibility == Visibility.Visible))
                pauseMediaGrid_MouseLeftButtonUp(m, e);
        }

        private void gridMediaTrailer_MouseMove(object sender, MouseEventArgs e)
        {
            timeTick = 0;
        }

        private void sliProgress_MouseMove(object sender, MouseEventArgs e)
        {
            var s = sender as Slider;
            var m = s.Tag as MediaElement;
            var q = e.GetPosition(s);
            if (s == sliProgressPhim)
            {
                tbTooltip.Text = TimeSpan.FromSeconds(s.Maximum / (s.ActualWidth / q.X)).ToString(@"hh\:mm\:ss");
                mediaTooltip.Pause();
                mediaTooltip.Position = TimeSpan.FromSeconds(s.Maximum / (s.ActualWidth / q.X));
                gridTooltip.Visibility = Visibility.Visible;
                gridTooltip.Margin = new Thickness(q.X - gridTooltip.Width / 2, 0, 0, stbPhim.ActualHeight + 5);
            }
            else
            {
                tbtooltip.Text = TimeSpan.FromSeconds(sliProgress.Maximum / (sliProgress.ActualWidth / q.X)).ToString(@"hh\:mm\:ss");
                mediatooltip.Pause();
                mediatooltip.Position = TimeSpan.FromSeconds(sliProgress.Maximum / (sliProgress.ActualWidth / q.X));
                gridtooltip.Visibility = Visibility.Visible;
                gridtooltip.Margin = new Thickness(q.X - gridtooltip.Width / 2, 0, 0, stb.ActualHeight + 5);
            }
        }

        private void sliProgress_MouseLeave(object sender, MouseEventArgs e)
        {
            var s = sender as Slider;
            if (s == sliProgressPhim)
                gridTooltip.Visibility = Visibility.Collapsed;
            else
                gridtooltip.Visibility = Visibility.Collapsed;
        }

        private void sliProgressPhim_MouseEnter(object sender, MouseEventArgs e)
        {
            var sli = sender as Slider;
            DoubleAnimation d = new DoubleAnimation();
            d.To = 1.3;
            Storyboard.SetTargetName(d, sli.Name);
            Storyboard.SetTargetProperty(d, new PropertyPath("RenderTransform.ScaleY"));
            Storyboard s = new Storyboard();
            s.Children.Add(d);
            s.Begin(sli, true);
        }


        private void sliProgress_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var sli = sender as Slider;
            if (sli == sliProgressPhim)
                mediaPhim.Position = TimeSpan.FromSeconds(sli.Value);
            else
                mediaTrailer.Position = TimeSpan.FromSeconds(sliProgress.Value);
            DoubleAnimation d = new DoubleAnimation();
            Storyboard.SetTargetName(d, sli.Name);
            Storyboard.SetTargetProperty(d, new PropertyPath("RenderTransform.ScaleY"));
            Storyboard s = new Storyboard();
            s.Children.Add(d);
            s.Begin(sli, true);
        }

        private void btnDeleteVideo_Click(object sender, RoutedEventArgs e)
        {
            var db = FindResource("DBVideo4Search") as DBVideo4Search;

            //var a = dtgVideo.SelectedItem as Phim;
            var phim = (sender as Button).Tag as Phim;
            if (phim != null)
            {
                var q = d.Phim.Where(p => p.ID == phim.ID).FirstOrDefault();
                if (q != null)
                {
                    MessageBoxResult msg = MessageBox.Show("Xóa các dòng playlist có phim này", "", MessageBoxButton.YesNo);
                    if (msg == MessageBoxResult.Yes)
                    {
                        d.Profile_Playlist.RemoveRange(q.Profile_Playlist);
                        q.User.Clear();
                        q.TheLoai1.Clear();
                        d.Phim.Remove(q);
                        if (d.SaveChanges() > 0)
                            ShowTbMessage("Đã Xóa");
                        int n = db.LstPhim.IndexOf(q);
                        db.LstPhim.Remove(q);
                        dtgVideo.Items.Refresh();
                        dtgVideo.ScrollIntoView(db.LstPhim.ElementAt(n - 1));
                    }
                }
            }
        }

        private void btnUpdateVideo_Click(object sender, RoutedEventArgs e)
        {
            var a = dtgVideo.SelectedItem as Phim;
            var phim = (sender as Button).Tag as Phim;
            if (phim != null)
            {
                var q = d.Phim.Where(p => p.ID == phim.ID).FirstOrDefault();
                if (q != null)
                {
                    if (d.SaveChanges() > 0)
                        ShowTbMessage("Đã Update");
                }
                else if (string.IsNullOrEmpty(phim.Name))
                {
                    ShowTbMessage("Điền vào Name để thêm");
                }
                else
                {
                    phim.ID = d.Phim.OrderByDescending(p => p.ID).FirstOrDefault().ID + 1;
                    phim.Hinh = "";
                    phim.Trailer = "";
                    phim.Category = 1;
                    phim.ThoiGian = "";
                    phim.NoiDung = "";
                    phim.NgayCapNhat = DateTime.Now;
                    d.Phim.Add(phim);
                    if (d.SaveChanges() > 0)
                        ShowTbMessage("Đã update new phim");
                    var db = FindResource("DBVideo4Search") as DBVideo4Search;
                    db.LstPhim.Add(phim);
                    dtgVideo.CancelEdit();
                    dtgVideo.CancelEdit();
                    dtgVideo.Items.Refresh();
                    dtgVideo.ScrollIntoView(phim);
                }
                var mfp = FindResource("mainformprovider") as MainFormProvider;
                mfp.TheLoai = d.TheLoai.ToList();
            }
        }

        private void btnManageVideoSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Phim> q = d.Phim.Include("TheLoai1").ToList();
                int n;
                if (cbCategory.SelectedValue != null)
                {
                    n = int.Parse(cbCategory.SelectedValue.ToString());
                    q = q.Where(p => p.TheLoai1.Any(g => g.ID == n)).ToList();
                }
                if (!string.IsNullOrEmpty(tbNameVideo.Text))
                    q = q.Where(p => p.Name.Contains(tbNameVideo.Text)).ToList();
                if (cbReleased.SelectedValue != null)
                    q = q.Where(p => p.ReleaseDate == cbReleased.SelectedValue.ToString()).ToList();
                if (dpkMin.SelectedDate != null)
                    q = q.Where(p => p.NgayCapNhat >= dpkMin.SelectedDate).ToList();
                if (dpkMax.SelectedDate != null)
                    q = q.Where(p => p.NgayCapNhat <= dpkMax.SelectedDate).ToList();
                if (int.TryParse(tbMinRate.Text, out n))
                    q = q.Where(p => p.RatingIMDB >= n).ToList();
                if (int.TryParse(tbMaxRate.Text, out n))
                    q = q.Where(p => p.RatingIMDB <= n).ToList();
                if (int.TryParse(tbMinViews.Text, out n))
                    q = q.Where(p => p.Views >= n).ToList();
                if (int.TryParse(tbMaxViews.Text, out n))
                    q = q.Where(p => p.Views <= n).ToList();
                var db = FindResource("DBVideo4Search") as DBVideo4Search;
                db.LstPhim = q.ToList();
            }
            catch (Exception ex)
            {
                ShowTbMessage(ex.Message);
            }
        }

        private void slidertrailer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((sender as Slider).Tag as MediaElement).Position = TimeSpan.FromSeconds((sender as Slider).Value);
        }

        private void dtgVideo_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            ShowTbMessage("end");
        }

        private void cbPhimRowDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var c = (sender as ComboBox).SelectedItem as ComboBoxItem;
            var s = c.Content.ToString();
            var t = (sender as ComboBox).Tag as TabControl;
            if (s == "Updating")
                t.SelectedIndex = 0;
            else if (s == "List The Loai")
                t.SelectedIndex = 1;

        }

        private void btnAddNewVideoCat_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var c = b.Tag as ComboBox;
            var phim = c.DataContext as Phim;
            var item = c.SelectedItem as TheLoai;
            var dt = c.Tag as DataGrid;
            if (item != null && phim != null)
            {
                var t = phim.TheLoai1.Where(g => g.ID == item.ID).FirstOrDefault();
                if (t == null)
                {
                    phim.TheLoai1.Add(item);
                    if (d.SaveChanges() > 0)
                        ShowTbMessage("Đã Thêm");
                    dt.Items.Refresh();
                }
                else
                    ShowTbMessage("Đã có");

            }
        }

        private void btnDeleteCatVideo_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var phim = b.DataContext as Phim;
            var dt = b.Tag as DataGrid;
            var item = dt.SelectedItem as TheLoai;
            if (phim != null && item != null)
            {
                phim.TheLoai1.Remove(item);
                if (d.SaveChanges() > 0)
                    ShowTbMessage("Đã xóa");
                //phim.TheLoai1 = phim.TheLoai1;
                dt.Items.Refresh();
            }
        }
        bool mediaFull = false;
        object cont;
        Binding binding;
        private void gridMaximizeMedia_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TimeSpan a = mediaPhim.Position;
            var content = this.Content;
            if (!mediaFull)
            {
                var source = mediaPhim.Source;
                cont = this.Content;
                binding = BindingOperations.GetBinding(mediaPhim, MediaElement.SourceProperty);
                //gridMediaPhim.Children.Remove(mediaPhim);
                //gridMediaPhim.Children.Remove(stbPhim);
                stXemPhim.Children.Remove(gridMediaPhim);
                gridMediaPhim.Width = Double.NaN;
                gridMediaPhim.Height = Double.NaN;
                //this.Background = new SolidColorBrush(Colors.White);
                mediaPhim.Source = source;
                this.Content = gridMediaPhim;
                this.WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                WindowState = WindowState.Maximized;
                mediaPhim.Position = a;
                mediaFull = true;
                mediaPhim.Focus();
                mediaPhim.Play();
            }
            else if (mediaFull)
            {
                this.Content = null;
                this.Content = cont;
                stXemPhim.Children.Insert(0, gridMediaPhim);
                mediaPhim.SetBinding(MediaElement.SourceProperty, binding);
                gridMediaPhim.Width = 1100;
                gridMediaPhim.Height = 550;
                this.WindowState = WindowState.Normal;
                mediaFull = false;
                mediaPhim.Position = a;
            }


        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                var a = FindResource("mainformprovider") as MainFormProvider;
                if (maintabcontrol.SelectedIndex == 8)
                {
                    tbAddImage.Text = new Uri(openFileDialog.FileName).ToString();
                }
                else if (a.SelectedProfile == null)
                    imageAddProfile.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                else
                    imageProfileDetail.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }
        private void MaximizeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;

        }

        private void MaximizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (maintabcontrol.SelectedIndex == 12)
            {
                foreach (TabItem item in tabgioit.Items)
                {
                    item.Width = 450;
                }
                bdr.Height = 420;
            }
            SystemCommands.MaximizeWindow(this);
            Maximize.Visibility = Visibility.Collapsed;
            Restore.Visibility = Visibility.Visible;
        }

        private void MinimizeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void MinimizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void RestoreCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.WindowState == WindowState.Maximized;
        }

        private void RestoreCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (maintabcontrol.SelectedIndex == 12)
            {
                foreach (TabItem item in tabgioit.Items)
                {
                    item.Width = 350;
                }
                bdr.Height = 360;
            }
            SystemCommands.RestoreWindow(this);
            Maximize.Visibility = Visibility.Visible;
            Restore.Visibility = Visibility.Collapsed;
        }

        private void CloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }


        private void btnManageVideo_Click(object sender, RoutedEventArgs e)
        {
            var a = FindResource("DBVideo4Search") as DBVideo4Search;
            a.Cats = d.TheLoai.ToList();
            a.LstPhim = d.Phim.ToList();
            a.VideoReleased = d.Phim.GroupBy(p => p.ReleaseDate).Select(g => g.FirstOrDefault()).ToList();
            maintabcontrol.SelectedIndex = 9;
            ChangingTabControl();
        }

        private void btnManageUser_Click(object sender, RoutedEventArgs e)
        {
            var a = FindResource("DBUser4Search") as DBUser4Search;
            a.Users = d.User.Include("UserProfile").ToList();
            a.CurPage = 1;
            a.CapDo = d.CapDo.ToList();
            a.LoaiTaiKhoan = d.LoaiTaiKhoan.ToList();
            a.TinhTrang = d.TinhTrangThanhToan.ToList();
            gridUser.DataContext = new User();
            maintabcontrol.SelectedIndex = 8;

            ChangingTabControl();
        }

        private void dtgTheLoai_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            var q = new TheLoai();
            dtgTheLoai.SelectedItem = q;
            dtgTheLoai.CancelEdit();
            dtgTheLoai.CancelEdit();
            dtgTheLoai.Items.Refresh();
            dtgTheLoai.SelectedItem = q;
        }

        private void btnManageTheLoai_Click(object sender, RoutedEventArgs e)
        {
            maintabcontrol.SelectedIndex = 11;
            ChangingTabControl();
        }

        private void dtgTheLoai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var q = dtgTheLoai.SelectedItem as TheLoai;
            if (q != null)
            {
                grbDSPhim.Header = "Danh sách phim " + q.Name;
            }
        }

        private void btnUpdateTheLoai_Click(object sender, RoutedEventArgs e)
        {
            var q = (sender as Button).DataContext as TheLoai;
            if (q != null)
            {
                var t = d.TheLoai.Where(p => p.ID == q.ID).FirstOrDefault();
                if (t != null)
                {
                    t.Name = q.Name;
                    if (d.SaveChanges() > 0)
                        ShowTbMessage("Đã update");
                    dtgTheLoai.CancelEdit();
                    dtgTheLoai.CancelEdit();
                    dtgTheLoai.Items.Refresh();
                }
                else
                {
                    q.ID = d.TheLoai.OrderByDescending(p => p.ID).FirstOrDefault().ID + 1;
                    d.TheLoai.Add(q);
                    if (d.SaveChanges() > 0)
                        ShowTbMessage("Đã thêm");
                    var a = FindResource("mainformprovider") as MainFormProvider;
                    a.TheLoai = d.TheLoai.ToList();
                    dtgTheLoai.CancelEdit();
                    dtgTheLoai.CancelEdit();
                    dtgTheLoai.Items.Refresh();
                    dtgTheLoai.Items.MoveCurrentToLast();
                }
            }
        }

        private void btnDeleteTheLoai_Click(object sender, RoutedEventArgs e)
        {
            var q = (sender as Button).DataContext as TheLoai;
            dtgTheLoai.Items.Refresh();
            if (q != null)
            {
                var t = d.TheLoai.Where(p => p.ID == q.ID).FirstOrDefault();
                if (t != null)
                {
                    MessageBoxResult msg = MessageBox.Show("Xóa lun các phim có thể loại này (Chọn No để xóa nhưng không xóa phim)", "", MessageBoxButton.YesNoCancel);
                    if (msg == MessageBoxResult.No)
                    {
                        foreach (var item in t.Phim)
                        {
                            item.Category = d.TheLoai.FirstOrDefault().ID;
                        }
                    }
                    else if (msg == MessageBoxResult.Yes)
                        d.Phim.RemoveRange(t.Phim);
                    else return;
                    d.TheLoai.Remove(t);
                    int n = dtgTheLoai.Items.IndexOf(q);
                    if (d.SaveChanges() > 0)
                    {
                        ShowTbMessage("Đã xóa");
                    }
                    var a = FindResource("mainformprovider") as MainFormProvider;
                    a.TheLoai = d.TheLoai.ToList();
                    dtgTheLoai.Items.Refresh();
                    dtgTheLoai.SelectedIndex = n - 1;
                }
            }
        }

        private void btnAddPhimTheLoai_Click(object sender, RoutedEventArgs e)
        {
            var q = cbPhimName.SelectionBoxItem.ToString();
            if (!string.IsNullOrEmpty(q))
            {
                var tl = dtgTheLoai.SelectedItem as TheLoai;
                var t = tl.Phim1.Where(p => p.Name == q).FirstOrDefault();
                if (t == null)
                {
                    var phim = d.Phim.Where(p => p.Name == q).FirstOrDefault();
                    tl.Phim1.Add(phim);
                    if (d.SaveChanges() > 0)
                        ShowTbMessage("Đã thêm");
                    var a = FindResource("mainformprovider") as MainFormProvider;
                    dtgPhimTheoTL.Items.Refresh();
                    dtgPhimTheoTL.Items.MoveCurrentTo(phim);
                }
                else
                    ShowTbMessage("Đã có");
            }
        }

        private void btnDeletePhimTheLoai_Click(object sender, RoutedEventArgs e)
        {
            var q = (sender as Button).DataContext as Phim;
            if (q != null)
            {
                var tl = dtgTheLoai.SelectedItem as TheLoai;
                var t = d.Phim.Where(p => p.ID == q.ID).FirstOrDefault();
                if (t != null)
                {
                    int n = dtgPhimTheoTL.Items.IndexOf(q);
                    tl.Phim1.Remove(q);
                    if (d.SaveChanges() > 0)
                    {
                        ShowTbMessage("Đã xóa");
                        dtgPhimTheoTL.Items.Remove(q);
                    }
                    dtgPhimTheoTL.Items.Refresh();
                    dtgPhimTheoTL.SelectedIndex = n;
                }
            }
        }

        private void stsearch_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            tbsearch.Text = "";
        }
        private void btndangnhap_Click(object sender, RoutedEventArgs e)
        {
            var mfp = this.FindResource("mainformprovider") as MainFormProvider;
            mfp.User = null;
            MyUserModel m = new MyUserModel();
            m.Pasword = "";
            tabSignIn2.DataContext = m;
            maintabcontrol.SelectedIndex = 1;
            ChangingTabControl();
        }



        private void stsearch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var s = sender as StackPanel;
            if (imgsearchunClicked && (!string.IsNullOrEmpty(tbsearch.Text) && tbsearch.Text != "Tìm kiếm theo tên"))
            {
                tbsearch.Foreground = Brushes.White;
                var a = FindResource("mainformprovider") as MainFormProvider;
                a.IsFormCategory = true;
                tabPhimChuDe5.DataContext = d.Phim.Where(p => p.Name.Contains(tbsearch.Text)).ToList();
                tbChuDe.Text = "Tìm kiếm theo tên";
                maintabcontrol.SelectedIndex = 4;
                ChangingTabControl();
                imgsearchunClicked = false;
            }
            else if (!imgsearchunClicked)
            {
                DoubleAnimation d = new DoubleAnimation();
                d.To = 1.5;
                d.Duration = TimeSpan.FromSeconds(0.3);
                Storyboard.SetTargetName(d, "tbsearch");
                Storyboard.SetTargetProperty(d, new PropertyPath("LayoutTransform.ScaleX"));
                Storyboard st = new Storyboard();
                st.Children.Add(d);
                st.Completed += (o, a) =>
                {
                    tbsearch.Focusable = true;
                    tbsearch.Focus();
                };
                st.Begin(tbsearch, true);
                tbsearch.Visibility = Visibility.Visible;
                imgsearchunClicked = true;
            }
            else
            {
                CloseTextBox(tbsearch);
            }
        }

        private void btnThongKeVideo_Click(object sender, RoutedEventArgs e)
        {
            var a = FindResource("mainformprovider") as MainFormProvider;
            DirectoryCatalog catalog = new DirectoryCatalog("../../../AddIns");
            CompositionContainer cc = new CompositionContainer(catalog);
            cc.ComposeParts(this);
            a.LstChart = Plugins.ToList().Select(g => g.Value).ToList();
            foreach (var item in a.LstChart)
            {
                item.ChartBackground = (Brush)new BrushConverter().ConvertFromString("#FF4E4848");
                item.ChartTitle = "Thống kê Video yêu thích";
                item.ChartSubTitle = "";

                item.Series = new ObservableCollection<ChartSeries>() { new ChartSeries() { ItemsSource= d.Profile_Playlist.Include("Phim").ToList(),DisplayMemberPath="Name",
                                                            ValueMember="Views"} };
                //BindingOperations.SetBinding(item.SeriesSource, item, new Binding("taskItems") { Source = d.Profile_Playlist.Include(p => p.Phim).ToList() }); ;
                item.Name = item.ToString().Split('.').ElementAt(3);
            }
            //Plugins.FirstOrDefault().Value.
            maintabcontrol.SelectedIndex = 10;
            List<Movie4Chart> lst = new List<Movie4Chart>();
            foreach (var item in d.Phim.Where(p => p.Profile_Playlist.Any(q => q != null)).ToList())
            {
                var q = new Movie4Chart();
                q.Name = item.Name;
                q.ReleaseDate = item.ReleaseDate;
                q.RatingIMDB = item.RatingIMDB;
                q.Views = item.Views;
                q.ThoiLuong = TimeSpan.Parse(item.ThoiGian.Replace('h', ':')).TotalMinutes;
                q.SoLuotThich = item.Profile_Playlist.Count;
                q.SoTheLoai = item.TheLoai1.Count;
                lst.Add(q);
            }
            //chart.SeriesSource = lst;
            tabThongKe11.DataContext = lst;
            ChangingTabControl();
        }

        private void cbChart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var q = cbChart.SelectedItem as ChartBase;
            var ch = gridChart.FindName("chart") as ChartBase;
            var lst = ch.Series[0].ItemsSource;

            q.Series.Clear();
            q.Series = ch.Series;
            foreach (var item in q.Series)
            {
                item.ItemsSource = null;
            }
            //q.Series[0].ItemsSource = null;
            gridChart.Children.Clear();
            gridChart.Children.Add(q);
            foreach (var item in q.Series)
            {
                item.ItemsSource = lst;
            }
            //q.Series[0].ItemsSource = lst;
        }

        private void formXemPhimScroll_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Escape))
            {


            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            maintabSignup.SelectedIndex = 0;
            ChangingTabControl();
        }


        private void button_Click2(object sender, RoutedEventArgs e)
        {
            maintabcontrol.SelectedIndex = 3;
            ChangingTabControl();
        }

        private void btnNextSignUp_Click(object sender, RoutedEventArgs e)
        {
            maintabSignup.SelectedIndex = 1;
            ChangingTabControl();
        }

        private void btnThongKeDT_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();
            var temp = d.UserThanhToan.OrderByDescending(p => p.NgayTT.Year).GroupBy(p => p.NgayTT.Year)
    .Select(p => p.FirstOrDefault().NgayTT.Year.ToString()).ToList();
            list.AddRange(temp);
            cbChonTheoNam.ItemsSource = list;
            cbChonTheoNam.SelectedValue = list[0].ToString();
            list = new List<string>();
            for (int i = 1; i <= 12; i++)
            {
                list.Add(i.ToString());
            }
            cbChonTheoThang.ItemsSource = list;
            cbChonTheoThang.SelectedValue = DateTime.Now.Month.ToString();
            maintabcontrol.SelectedIndex = 13;
            cbDoanhThu.SelectedIndex = 0;
            ChangingTabControl();
        }

        

        private void btnupdateUser_Click(object sender, RoutedEventArgs e)
        {
            var a = dtgUser.SelectedItem as User;
            if (a != null)
            {
                var q = d.User.Where(p => p.ID == a.ID).FirstOrDefault();

                if (q != null)
                {
                    if (d.SaveChanges() > 0)
                        ShowTbMessage("Đã Update");
                }
            }
        }

        private void btndeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var db = FindResource("DBUser4Search") as DBUser4Search;

            var a = dtgUser.SelectedItem as User;
            if (a != null)
            {
                var q = d.User.Include("UserThanhToan").Where(p => p.ID == a.ID).FirstOrDefault();
                if (q != null)
                {
                    var msg = MessageBox.Show("Sẽ xóa luôn toàn bộ thông tin thanh toán và profiles của user này! Tiếp tục?", "", MessageBoxButton.YesNoCancel);
                    if (msg == MessageBoxResult.Yes)
                    {
                        //d.UserThanhToan.RemoveRange(q.UserThanhToan);
                        d.Profile_Playlist.RemoveRange(d.Profile_Playlist.Where(p => p.UserProfileID == q.ID));
                        d.UserProfile.RemoveRange(q.UserProfile);
                        q.Phim.Clear();
                        d.User.Remove(q);
                        if (d.SaveChanges() > 0)
                            ShowTbMessage("Đã Xóa");
                        db.Users = d.User.ToList();
                        dtgUser.Items.Refresh();
                    }
                }
            }
        }


        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (maintabcontrol.SelectedIndex == 8)
            {
                var db = FindResource("DBUser4Search") as DBUser4Search;
                db.Users = d.User.ToList();
                dtgUser.SelectedItem = null;

                gridUser.DataContext = new User();
            }
            else if (maintabcontrol.SelectedIndex == 9)
            {
                var db = FindResource("DBVideo4Search") as DBVideo4Search;
                db.LstPhim = d.Phim.ToList();
                dtgVideo.SelectedItem = null;
                cbCategory.SelectedItem = null;
                cbReleased.SelectedItem = null;
                tbNameVideo.Text = "";
                tbMinRate.Text = "";
                tbMinViews.Text = "";
                tbMaxViews.Text = "";
                tbMaxRate.Text = "";
                dpkMin.SelectedDate = null;
                dpkMax.SelectedDate = null;
            }
        }

        private void dtgVideo_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            ShowTbMessage("adding");
            dtgVideo.SelectedItem = new Phim();
        }


        private void btnToChoseProfile_Click(object sender, RoutedEventArgs e)
        {
            maintabcontrol.SelectedIndex = 7;
            ChangingTabControl();
        }


        private void btnXoaProfile_Click(object sender, RoutedEventArgs e)
        {
            var a = FindResource("mainformprovider") as MainFormProvider;
            a.User.UserProfile.Remove(a.SelectedProfile);
            if (d.SaveChanges() > 0)
                ShowTbMessage("Đã xóa profile");
            a.SelectedProfile = null;

            tabChoseProfile8.DataContext = a.User.UserProfile.ToList();
            maintabcontrol.SelectedIndex = 7;
            ChangingTabControl();
        }

        private void btnProfilePlaylist_Click(object sender, RoutedEventArgs e)
        {
            var a = FindResource("mainformprovider") as MainFormProvider;
            a.IsFormCategory = true;
            List<Phim> lst = d.Profile_Playlist.Where(p => p.UserProfile.UserID == a.SelectedProfile.UserID && p.ProfileID == a.SelectedProfile.ID).Select(g => g.Phim).ToList();

            tabPhimChuDe5.DataContext = lst;
            tbChuDe.Text = "Danh sách yêu thích";
            if (lst.Count == 0)
                tbChuDe.Text = "Chưa có phim nào trong danh sách";
            maintabcontrol.SelectedIndex = 4;
            ChangingTabControl();
        }

        private void btnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            var a = this.FindResource("mainformprovider") as MainFormProvider;
            a.User = null;
            a.SelectedProfile = null;
            maintabcontrol.SelectedIndex = 1;
            ChangingTabControl();
            ShowTbMessage("Đã đăng xuất");
        }

        private void btnUserDetailTab_Click(object sender, RoutedEventArgs e)
        {
            maintabcontrol.SelectedIndex = 6;
            ChangingTabControl();
        }


        private void cbSelectShowDetail_Checked(object sender, RoutedEventArgs e)
        {
            dtgVideo.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }

        private void cbSelectShowDetail_Unchecked(object sender, RoutedEventArgs e)
        {
            dtgVideo.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
        }

        private void btnChonLink_Click(object sender, RoutedEventArgs e)
        {
            var q = dtgVideo.SelectedCells[0];
            var row = dtgVideo.ItemContainerGenerator.ContainerFromItem(q.Item) as DataGridRow;
            row.IsSelected = true;
            if (q != null)
            {
                var head = q.Column.Header.ToString();
                if (head == "Hình" || head == "Trailer" || head == "Link Film" || head == "Logo")
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "All Videos Files |*.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                  " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm";
                    if (head == "Hình" || head == "Logo")
                        openFileDialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        var a = FindResource("mainformprovider") as MainFormProvider;
                        var phim = q.Item as Phim;
                        switch (head)
                        {
                            case "Hình":
                                phim.Hinh = openFileDialog.FileName.ToString();
                                break;
                            case "Trailer":
                                phim.Trailer = openFileDialog.FileName.ToString();
                                break;
                            case "Link Film":
                                phim.Linkfilm = openFileDialog.FileName.ToString();
                                break;
                            case "Logo":
                                phim.Logo = openFileDialog.FileName.ToString();
                                break;
                        }
                        var dt = dtgVideo.DataContext;
                        //var select = dtgVideo.SelectedItem;
                        dtgVideo.DataContext = null;
                        dtgVideo.DataContext = dt;
                        row.IsSelected = true;
                        //dtgVideo.SelectedItem = select;
                    }
                }
                else ShowTbMessage("Cột k hợp lệ");
            }
            else ShowTbMessage("Chọn cell cần đổi");
        }

        private void btnAddTK_Click(object sender, RoutedEventArgs e)
        {
            var q = cbTKeTheo.SelectedItem as ComboBoxItem;
            if (q != null)
            {
                var series = new ChartSeries();
                series.ValueMember = q.Content.ToString();
                series.Caption = q.Content.ToString();
                foreach (var item in cbTKDaThem.Items)
                {
                    if ((item as ChartSeries).ValueMember == series.ValueMember)
                    {
                        ShowTbMessage("Đã có");
                        return;
                    }
                }
                series.DisplayMember = "Name";
                series.ItemsSource = null;
                chart.Series.Add(series);
                chart.ChartSubTitle += " " + q.Content.ToString();
                series.ItemsSource = chart.DataContext as List<Movie4Chart>;
                chart.UpdateLayout();
                gridChart.UpdateLayout();
                cbTKDaThem.Items.Add(series);
            }
        }

        private void btnDelteTK_Click(object sender, RoutedEventArgs e)
        {
            var q = cbTKDaThem.SelectedItem as ChartSeries;
            if (q != null)
            {
                q.ItemsSource = null;
                chart.ChartSubTitle = chart.ChartSubTitle.Replace(" " + q.ValueMember, "");
                chart.Series.Remove(q);
                chart.UpdateLayout();
                cbTKDaThem.Items.Remove(q);
                cbTKDaThem.Items.Refresh();
            }
        }

        private void cbTKeTheo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTKeTheo.SelectedItem != null && IsLoaded)
            {
                if (cbModeTK.SelectionBoxItem.ToString() != "Thống kê nhiều cùng lúc")
                {
                    var content = (cbTKeTheo.SelectedItem as ComboBoxItem).Content.ToString();
                    chart.ChartSubTitle = "Thống kê theo " + content;

                    chartseries.ValueMember = content;
                    chartseries.Caption = content;
                    var lst = chartseries.ItemsSource as List<Movie4Chart>;
                    chartseries.ItemsSource = null;
                    chartseries.ItemsSource = lst;
                    gridChart.Children.Clear();
                    gridChart.Children.Add(chart);
                    var q = chart.ChartLegendItems;
                    //var a = new StackedColumnChart();
                    //a.Name = "chart";
                    //a.ChartSubTitle = chart.ChartSubTitle;
                    //a.ChartTitle = chart.ChartTitle;
                    //a.ChartBackground = chart.ChartBackground;
                    //a.Foreground = chart.Foreground;
                    //a.Background = chart.Background;
                    //a.Series = chart.Series;
                    //a.Series[0].ItemsSource = null;
                    //gridChart.Children.Clear();
                    //gridChart.Children.Add(a);
                    //a.Series[0].Items.Refresh();
                    //a.Series[0].ItemsSource = chart.Series[0].ItemsSource;
                }
                else
                {

                }
            }
        }

        private void dtgPhimTheoTL_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            var q = new Phim();
            //q.ID = d.Phim.OrderByDescending(p => p.ID).FirstOrDefault().ID + 1;
            //dtgPhimTheoTL.SelectedItem = q;
            //dtgPhimTheoTL.CancelEdit();
            //dtgPhimTheoTL.CancelEdit();
            //dtgPhimTheoTL.Items.Refresh();
            //dtgPhimTheoTL.SelectedItem = q;
        }
        private void cbModeTK_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var q = cbModeTK.SelectedItem as ComboBoxItem;

            if (q.Content.ToString() == "Thống kê nhiều cùng lúc")
            {
                stModeTKNhieu.Visibility = Visibility.Visible;
                var a = cbTKeTheo.SelectionBoxItem;
                var series = new ChartSeries();
                series.ValueMember = a.ToString();
                series.DisplayMember = "Name";
                series.Caption = a.ToString();
                series.ItemsSource = null;
                chart.Series.Clear();
                chart.Series.Add(series);
                series.ItemsSource = chart.DataContext as List<Movie4Chart>;
                chart.UpdateLayout();
                cbTKDaThem.Items.Add(series);
            }
            else if (q.Content.ToString() == "Thống kê từng loại" && IsLoaded)
            {
                stModeTKNhieu.Visibility = Visibility.Collapsed;
                var t = cbTKeTheo.SelectionBoxItem.ToString();
                var lst = chartseries.ItemsSource as List<Movie4Chart>;
                chartseries.Caption = t;
                chartseries.ValueMember = t;
                chartseries.ItemsSource = null;
                chart.Series.Clear();
                chart.Series.Add(chartseries);
                chart.ChartSubTitle = "Thống kê theo " + t;
                chartseries.ItemsSource = lst;
                chart.UpdateLayout();
                gridChart.UpdateLayout();
                cbTKDaThem.Items.Clear();
            }

        }
        bool ismousedown = false;
        double pos = 0, pos2 = 0, pos3 = 0, pos4 = 0;
        private void gridNavbar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ismousedown = true;
            pos3 = e.GetPosition(maingrid).X;
            pos4 = e.GetPosition(maingrid).Y;
        }

        private void gridNavbar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ismousedown = false;
        }

        private void gridNavbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (ismousedown)
            {
                pos = e.GetPosition(maingrid).X;
                pos2 = e.GetPosition(maingrid).Y;
                this.Left += pos - pos3;
                this.Top += pos2 - pos4;
            }
        }
    }
}
