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
    /// Логіка взаємодії для PickupPointWindow.xaml
    /// </summary>
    public partial class PickupPointWindow : Window
    {
        public PickupPoint currentItem { get; private set; }


        public PickupPointWindow(PickupPoint p)
        {
            InitializeComponent();

            currentItem = p;

            TbName.Text = p.Address;
            DataContext = currentItem;

        }


        private StringBuilder CheckFields()
        {
            StringBuilder s = new StringBuilder();
            if (TbName.Text == "")
                s.AppendLine("Введіть адрес");
            return s;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder _error = CheckFields();
            // Якщо помилки є, то виводимо помилки в MessageBox
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

