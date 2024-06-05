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
    /// Логіка взаємодії для ServiceWindow.xaml
    /// </summary>
    public partial class ServiceWindow : Window
    {
        public Service currentItem { get; private set; }



        public ServiceWindow(Service p)
        {
            InitializeComponent();

            currentItem = p;

            TbName.Text = p.Name;
            TbDescription.Text = p.Description;
            DataContext = currentItem;

        }


        private StringBuilder CheckFields()
        {
            StringBuilder s = new StringBuilder();
            if (UpDownPay.Value == null)
                s.AppendLine("Введіть кількість днів");
            if (TbName.Text == "")
                s.AppendLine("Введіть назву");
            if (TbDescription.Text == "")
                s.AppendLine("Введіть опис");
            return s;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder _error = CheckFields();
            // якщо помилка є, то виводимо помилку в MessageBox
            // і перериваємо виконання 
            if (_error.Length > 0)
            {
                MessageBox.Show(_error.ToString());
                return;
            }

            this.DialogResult = true;
        }
    }
}


