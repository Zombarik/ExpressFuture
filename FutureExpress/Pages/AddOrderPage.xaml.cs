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
using FutureExpress.Windows;

namespace FutureExpress.Pages
{
    /// <summary>
    /// Логіка взаємодії для AddOrderPage.xaml
    /// </summary>
    public partial class AddOrderPage : Page
    {
        Order _currentOrder;
        User _currentUser;

        public AddOrderPage()
        {
            InitializeComponent();
            LoadDataAndInit();
        }

        /// <summary>
        /// Завантаження і ініціалізація полей
        /// </summary>
        void LoadDataAndInit()
        {

            _currentOrder = CreateNewOrder();
            // поточний користувач
            _currentUser = Manager.CurrentUser;
            if (_currentUser != null && _currentUser.RoleId == 3)
            {
                TextBlockOrderNumber.Text = $"Замовлення №{_currentOrder.OrderID} на і'мя " +
                    $"{ _currentUser.Clients.SingleOrDefault().GetFio}";
            }
            else
            {
                TextBlockOrderNumber.Text = $"Замовлення №{_currentOrder.OrderID}";
            }
            //TextBlockTotalCost.Text = $"Итого {:C}";
            //DateTimeOrderCreateDate.Value = _currentOrder.OrderCreateDate;
            //OrderDeliveryDate.Text = _currentOrder.OrderDeliveryDate.ToLongDateString();
            TextBlockOrderGetCode.Text = _currentOrder.GetCode.ToString();
            DataContext = _currentOrder;
            ComboPickupPoint.ItemsSource = DataDBEntities.GetContext().PickupPoints.ToList();
            ComboService.ItemsSource = DataDBEntities.GetContext().Services.ToList();
            ComboZone.ItemsSource = DataDBEntities.GetContext().Zones.ToList();
            DataGridRate.ItemsSource = DataDBEntities.GetContext().Rates.OrderBy(p => p.Service.Name).
                ThenBy(p => p.Zone.Name).
                ThenBy(p => p.Weight).
                ThenBy(p => p.Price).ToList();

        }

        static Order CreateNewOrder()
        {
            Order order = new Order();
            if (DataDBEntities.GetContext().Orders.Count() == 0)
            {
                order.OrderID = 1;
            }
            else
            {
                order.OrderID = DataDBEntities.GetContext().Orders.Max(p => p.OrderID) + 1; ;
            }
            order.OrderCreateDate = DateTime.Now;
            order.OrderStatusID = 1;
            order.OrderDeliveryDate = DateTime.Now.AddDays(1);
            order.UserName = Manager.CurrentUser.UserName;
            Random rnd = new Random();
            order.GetCode = rnd.Next(100, 1000);
            return order;
        }

        private void ComboService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRate();
        }

        private void ComboZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRate();
        }

        private void UpDownWeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LoadRate();
        }

        private void LoadRate()
        {
            // отримуємо поточні данні з бд
            var currentData = DataDBEntities.GetContext().Rates.OrderBy(p => p.Service.Name).
                ThenBy(p => p.Zone.Name).
                ThenBy(p => p.Weight).
                ThenBy(p => p.Price).ToList();
            // вибір тільки товарів, по певному діапазону скидок
            if (ComboService.SelectedIndex > -1)
            {
                currentData = currentData.Where(p => p.ServiceId == (ComboService.SelectedItem as Service).ServiceId).ToList();
                int days = (ComboService.SelectedItem as Service).DaysCount;
                DateTime? current = DateTimeOrderCreateDate.Value;
                current = current.Value.AddDays(days);
                _currentOrder.OrderDeliveryDate = Convert.ToDateTime(current);
                DateTimeOrderDeliveryDate.Value = Convert.ToDateTime(current);
                double maxweight = currentData.Max(p => p.Weight);
                UpDownWeight.Maximum = maxweight;
            }
            if (ComboZone.SelectedIndex > -1)
                currentData = currentData.Where(p => p.ZoneId == (ComboZone.SelectedItem as Zone).ZoneID).ToList();


            if ((UpDownWeight.Value > 0) && (ComboService.SelectedIndex > -1) && (ComboZone.SelectedIndex > -1))
            {
                currentData = currentData.Where(p => p.Weight >= UpDownWeight.Value).ToList().GetRange(0, 1);
                if (currentData.Count > 0)
                {
                    _currentOrder.Rate = currentData[0];
                    _currentOrder.RateId = currentData[0].RateId;
                }
                else
                {
                    _currentOrder.RateId = null;
                }

            }
            else
            if (UpDownWeight.Value > 0)
            {
                currentData = currentData.Where(p => p.Weight >= UpDownWeight.Value).ToList();
            }
            else
            {
                _currentOrder.RateId = null;
            }

            DataGridRate.ItemsSource = currentData;
        }

        // відображення номерів рядків в DataGrid
        private void DataGridGoodLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.MainFrame.CanGoBack)
                Manager.MainFrame.GoBack();
        }

        private StringBuilder CheckFields()
        {
            StringBuilder s = new StringBuilder();
            // перевірка полей на вміст
            if (ComboService.SelectedIndex == -1)
                s.AppendLine("Виберіть тип послуг");
            if (ComboZone.SelectedIndex == -1)
                s.AppendLine("Виберіть відстань");
            if (UpDownWeight.Value <= 0)
                s.AppendLine("Вкажіть вагу відправлення");
                      
            if (_currentOrder.RateId == null)
                s.AppendLine("Відсутній тариф");
            if (ComboPickupPoint.SelectedItem == null)
                s.AppendLine("Не обрано пункт видачі");

            return s;
        }



        private void BtnBuyItem_Click(object sender, RoutedEventArgs e)
        {

            StringBuilder _error = CheckFields();
            // якщо помилки є, то виводимо помилки в MessageBox
            // і перериваємо виконання 
            if (_error.Length > 0)
            {
                MessageBox.Show(_error.ToString());
                return;
            }


            MessageBoxResult messageBoxResult = MessageBox.Show($"Оформить купівлю???",
                    "Оформлення", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    // пункт видачі
                    _currentOrder.OrderPickupPointID = Convert.ToInt32(ComboPickupPoint.SelectedValue);
                    // додаємо товар
                    DataDBEntities.GetContext().Orders.Add(_currentOrder);

                    DataDBEntities.GetContext().SaveChanges();  // зберігаємо зміни в БД
                                                                    // показуємо талон замовлення в новому вікні 
                    OrderTicketWindow orderTicketWindow = new OrderTicketWindow(_currentOrder);
                    orderTicketWindow.ShowDialog();
                    Manager.MainFrame.GoBack();  // повертаємося на попередню форму                    
            }
           

        }
    }
}
