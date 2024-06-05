using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логіка взаємодії для OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        int _itemcount = 0; // 
        Order _selected = null;

        public OrderPage()
        {
            InitializeComponent();
            LoadAndInitData();
        }
        void LoadAndInitData()
        {

           
            
            // приховуємо кнопки корзини
            var statuses = DataDBEntities.GetContext().OrderStatus.OrderBy(p => p.Name).ToList();
            statuses.Insert(0, new OrderStatu
            {
                Name = "Всі типи"
            }
            );
            ComboStatus.ItemsSource = statuses;
            ComboStatus.SelectedIndex = 0;
          
        }


        private void UpdateData()
        {
            // отримуємо поточні данні з бд
            var currentData = DataDBEntities.GetContext().Orders.OrderBy(p => p.OrderID).ToList();
            if (Manager.CurrentUser.RoleId == 3)
                currentData = DataDBEntities.GetContext().Orders.Where(p => p.UserName == Manager.CurrentUser.UserName).OrderBy(p => p.OrderID).ToList();
            // вибір тільки товарів, по певному діапазону скидок
            if (ComboStatus.SelectedIndex > 0)
                currentData = currentData.Where(p => p.OrderStatusID == (ComboStatus.SelectedItem as OrderStatu).Id).ToList();

            // вибір тих товарів, в назві яких є пошуковий рядок
            currentData = currentData.Where(p => p.OrderID.ToString().ToLower().
            Contains(TBoxSearch.Text.ToLower())).ToList();

            // сортування
            if (ComboSort.SelectedIndex >= 0)
            {
                // сортування по зростанню цін
                if (ComboSort.SelectedIndex == 0)
                    currentData = currentData.OrderBy(p => p.OrderCreateDate).ToList();
                // сортування по зменшуванню ціни
                if (ComboSort.SelectedIndex == 1)
                    currentData = currentData.OrderByDescending(p => p.OrderCreateDate).ToList();
            }
            // В якості джерела данних присвоюємо список данних
            ListBoxOrders.ItemsSource = currentData;
            // відображення кількості записів
            TextBlockCount.Text = $" Результат запиту:  {currentData.Count}  записів з {_itemcount}";
        }


        private void ComboStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateData();
        }

        private void ListBoxOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //отримуємо всіх виділених товарів
            _selected = null;
            var selected = ListBoxOrders.SelectedItems.Cast<Order>().ToList();
            if (selected.Count == 0) return;
            _selected = selected[0];

        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //оновлення данних після кожної активації вікна
            if (Visibility == Visibility.Visible)
            {
                LoadOrders();
            }
        }


        void LoadOrders()
        {
            DataDBEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
            if (Manager.CurrentUser.RoleId == 3)
            {

                ListBoxOrders.ItemsSource = DataDBEntities.GetContext().Orders.Where(p => p.UserName == Manager.CurrentUser.UserName).OrderBy(p => p.OrderID).ToList();
                MenuItemAccept.Visibility = Visibility.Collapsed;
                MenuItemCancel.Visibility = Visibility.Collapsed;
                MenuItemDeliver.Visibility = Visibility.Collapsed;
                MenuItemGet.Visibility = Visibility.Collapsed;
                MenuItemInPoint.Visibility = Visibility.Collapsed;
            }

            else
            // завантаження данних в listview сортуємо по назві
            {
                ListBoxOrders.ItemsSource = DataDBEntities.GetContext().Orders.OrderBy(p => p.OrderID).ToList();
                ListBoxOrders.ContextMenu.Visibility = Visibility.Visible;
            }
            _itemcount = ListBoxOrders.Items.Count;
            TextBlockCount.Text = $" Результат запиту:  {_itemcount}  записів з {_itemcount}";
        }
        private void MenuItemCancel_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderStatus(6);
        }

        void ChangeOrderStatus(byte id)
        {

           
            // контекстне меню по натисканню правої кнопки миші
            // якщо товар не обрано, завершуємо роботу
            if (_selected == null)
                return;
            List<string> statuses = new List<string> { "", "Створене", "Прийняте в роботу", "Передано в доставку", "В пункті выдачі", "Отримано", "Відмінено" };
            // додаємо товар в корзину
            MessageBoxResult x = MessageBox.Show($"Ви дійсно змінюєте статус замовлення на {statuses[id]}?", "Відміна", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (x == MessageBoxResult.OK)
            {
                _selected.OrderStatusID = id;
                DataDBEntities.GetContext().SaveChanges();  // Зберігаємо зміну в БД
                LoadOrders();
            }
        }

        private void MenuItemAccept_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderStatus(2);
        }

        private void MenuItemDeliver_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderStatus(3);
        }

        private void MenuItemInPoint_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderStatus(4);
        }

        private void MenuItemGet_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderStatus(5);
        }

        private void MenuItemMoreInfo_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null)
                return;
            OrderTicketWindow orderTicketWindow = new OrderTicketWindow(_selected);
            orderTicketWindow.ShowDialog();

        }
    }
}
