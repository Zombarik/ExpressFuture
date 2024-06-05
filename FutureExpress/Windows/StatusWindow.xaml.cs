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
    /// Логіка взаємодії для StatusWindow.xaml
    /// </summary>
    public partial class StatusWindow : Window
    {
        public OrderStatu currentItem { get; private set; }
        public StatusWindow(OrderStatu p)
        {
            InitializeComponent();
            currentItem = p;
            this.DataContext = currentItem;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}