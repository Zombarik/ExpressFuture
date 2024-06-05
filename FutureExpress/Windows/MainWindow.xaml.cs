using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FutureExpress.Models;
using FutureExpress.Pages;

namespace FutureExpress
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User _currentUser; //текущий пользователь в системе
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            _currentUser = Manager.CurrentUser;
            if (_currentUser != null && _currentUser.RoleId == 3)
            {
                TextBlockUserName.Text = $"Ви ввійшли як: {_currentUser.Clients.Single().GetFio}";
            }
            if (_currentUser != null && _currentUser.RoleId < 3)
            {
                TextBlockUserName.Text = $"Ви ввійшли як: {_currentUser.Role.RoleName}";
            }
            MainFrame.Navigate(new OrderPage());
            Manager.MainFrame = MainFrame;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult x = MessageBox.Show("Ви дійсно хочете вийти?",
"Вийти", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (x == MessageBoxResult.Cancel)
                e.Cancel = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Owner.Show();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PackIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new MyProfilePage());
        }

        private void BtnBackClick(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void PackIcon_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void MainFrame_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
          
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                BtnBack.Visibility = Visibility.Visible;
                PackLogout.Visibility = Visibility.Collapsed;
                PackAccount.Visibility = Visibility.Collapsed;
                AddNewOrder.Visibility = Visibility.Collapsed;
                TextBlockUserName.Visibility = Visibility.Collapsed;
                if (Manager.CurrentUser is null)
                    return;
                
                    BtnRate.Visibility = Visibility.Collapsed;
                    BtnService.Visibility = Visibility.Collapsed;
                    BtnZone.Visibility = Visibility.Collapsed;
                    BtnPickupPoints.Visibility = Visibility.Collapsed;
                    BtnStatuses.Visibility = Visibility.Collapsed;
                    BtnOrder.Visibility = Visibility.Collapsed;


                //if (Manager.CurrentUser.Role == true)
                //    BtnEditGoods.Visibility = Visibility.Collapsed;
                //else
                //{
                //    BtnMyAccount.Visibility = Visibility.Collapsed;
                //    BtnMyOrders.Visibility = Visibility.Collapsed;
                //}

            }
            else
            {
                BtnBack.Visibility = Visibility.Collapsed;
                PackLogout.Visibility = Visibility.Visible;
                PackAccount.Visibility = Visibility.Visible;
                TextBlockUserName.Visibility = Visibility.Visible;
                AddNewOrder.Visibility = Visibility.Visible;
                BtnOrder.Visibility = Visibility.Collapsed;

                if (Manager.CurrentUser.RoleId == 1)
                {
                    BtnRate.Visibility = Visibility.Visible;
                    BtnService.Visibility = Visibility.Visible;
                    BtnZone.Visibility = Visibility.Visible;
                    BtnPickupPoints.Visibility = Visibility.Visible;
                    BtnStatuses.Visibility = Visibility.Visible;
                    BtnOrder.Visibility = Visibility.Visible;
                }

                if (Manager.CurrentUser.RoleId <= 2)
                {
                    PackAccount.Visibility = Visibility.Collapsed;
                    AddNewOrder.Visibility = Visibility.Collapsed;
                    BtnOrder.Visibility = Visibility.Visible;
                }
                    if (Manager.CurrentUser.RoleId >= 2 )
                {
                   
                    BtnRate.Visibility = Visibility.Collapsed;
                    BtnService.Visibility = Visibility.Collapsed;
                    BtnZone.Visibility = Visibility.Collapsed;
                    BtnPickupPoints.Visibility = Visibility.Collapsed;
                    BtnStatuses.Visibility = Visibility.Collapsed;
                }

                //if (Manager.CurrentUser is null)
                //    return;
                //BtnOrder.Visibility = Visibility.Visible;
                //BtnBuyes.Visibility = Visibility.Visible;
                //if (Manager.CurrentUser.Role == true)
                //    BtnEditGoods.Visibility = Visibility.Visible;
                //else
                //{
                //    BtnMyAccount.Visibility = Visibility.Visible;
                //    BtnMyOrders.Visibility = Visibility.Visible;
                //}

            }
        }

        private void BtnService_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ServicePage());
        }

        private void BtnZone_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ZonePages());
        }

        private void BtnPickupPoints_Click(object sender, RoutedEventArgs e)
        {

            MainFrame.Navigate(new PickUpPointPage());
        }

        private void BtnStatuses_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrderStatusPage());
        }

        private void BtnRate_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RatePage());
        }

        private void AddNewOrder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new AddOrderPage());
        }

        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersPage());
        }
    }
}
