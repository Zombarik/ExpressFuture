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
using System.Windows.Shapes;
using FutureExpress.Models;

namespace FutureExpress.Windows
{
    /// <summary>
    /// Логіка взаємодії для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<User> users = DataDBEntities.GetContext().Users.ToList();
                User user = users.FirstOrDefault(p => p.Password == PbPassword.Password && p.UserName == TbUsername.Text);
                if (user != null)
                {
                    Manager.CurrentUser = user;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Owner = this;
                    this.Hide();
                    mainWindow.Show();

                }

                else
                {
                    MessageBox.Show("Не вірний логін або пароль");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // на экране отображается форма с двумя кнопками
            MessageBoxResult x = MessageBox.Show("Ви дійсно хочете закрити додаток ? ",
            "Вийти", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (x == MessageBoxResult.Cancel)
                e.Cancel = true;
        }

        private void BtnRegs_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                RegsWindow regsWindow = new RegsWindow();
            
                if (regsWindow.ShowDialog() == true)
                {
                    MessageBox.Show("Ok");
                }
            }
            catch
            {
                MessageBox.Show("Помилка");
            }
        }
    }
}
