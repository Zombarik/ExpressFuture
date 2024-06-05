using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
using Excel = Microsoft.Office.Interop.Excel;
using FutureExpress.Windows;
using System.Data.Entity;

namespace FutureExpress.Pages
{
    /// <summary>
    /// Логіка взаємодії для RatePage.xaml
    /// </summary>
    public partial class RatePage : Page
    {
        List<Rate> rates;
        int _itemcount = 0;
        public RatePage()
        {
            InitializeComponent();
            // завантаження данних в combobox + додавання додаткових рядків

        }
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            // відкриття редактування товару
            // передача обраного товару в AddRatePage
            // Manager.MainFrame.Navigate(new AddRatePage((sender as Button).DataContext as Rate));

            try
            {
                // якщо ніодного об'єкта не виділено, виходимо
                if (DataGridRate.SelectedItem == null) return;
                // отримуємо виділений об'єкт
                Rate selected = (sender as Button).DataContext as Rate;

                //MessageBox.Show(selected.Service.Name);
                RateWindow window = new RateWindow(selected);

                if (window.ShowDialog() == true)
                {
                    if (window.currentItem != null)
                    {
                        DataDBEntities.GetContext().Entry(window.currentItem).State = EntityState.Modified;
                        DataDBEntities.GetContext().SaveChanges();
                        LoadData();
                        MessageBox.Show("Запис змінений", "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    DataDBEntities.GetContext().Entry(window.currentItem).Reload();
                    LoadData();
                }
            }
            catch
            {
                MessageBox.Show("Помилка");
            }

        }

        private void PageIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //подія відображення данного Page
            // оновлюємо данні кожен раз коли активуємо цей Page
            if (Visibility == Visibility.Visible)
            {
                DataGridRate.ItemsSource = null;
                //завантаження оновленних данних
                var services = DataDBEntities.GetContext().Services.OrderBy(p => p.Name).ToList();
                services.Insert(0, new Service
                {
                    Name = "Всі типи"
                }
                );
                ComboService.ItemsSource = services;
                ComboService.SelectedIndex = 0;

                var zones = DataDBEntities.GetContext().Zones.OrderBy(p => p.Name).ToList();
                zones.Insert(0, new Zone
                {
                    Name = "Всі типи"
                }
                );
                ComboZone.ItemsSource = zones;
                ComboZone.SelectedIndex = 0;
                DataDBEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                rates = DataDBEntities.GetContext().Rates.OrderBy(p => p.Service.Name).OrderBy(p => p.Zone.Name).ToList();
                DataGridRate.ItemsSource = rates;
                _itemcount = rates.Count;
                TextBlockCount.Text = $" Результат запиту: {_itemcount} записів з {_itemcount}";
            }
        }

        void LoadData()
        {
            DataDBEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
            rates = DataDBEntities.GetContext().Rates.Include(p => p.Service).Include(p => p.Zone).OrderBy(p => p.Service.Name).OrderBy(p => p.Zone.Name).ToList();
            DataGridRate.ItemsSource = rates;
            _itemcount = rates.Count;
            TextBlockCount.Text = $" Результат запиту: {_itemcount} записів з {_itemcount}";
        }
        // Пошук товарів, які містять данний пошуковий рядок
        
        // Пошук товарів конкретного виробника
        private void ComboTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }
        /// <summary>
        /// Метод для фільтрації і сортування данних
        /// </summary>
        private void UpdateData()
        {
            // отримуємо поточні данні з бд
            var currentRates = DataDBEntities.GetContext().Rates.OrderBy(p => p.Service.Name).OrderBy(p => p.Zone.Name).ToList();
            // вибір тільки тих товарів, які належать данному виробнику
            if (ComboService.SelectedIndex > 0)
                currentRates = currentRates.Where(p => p.ServiceId == (ComboService.SelectedItem as Service).ServiceId).ToList();
            if (ComboZone.SelectedIndex > 0)
                currentRates = currentRates.Where(p => p.ZoneId == (ComboZone.SelectedItem as Zone).ZoneID).ToList();
            // вибір тих товарів, в назві яких є пошуковий рядок
         //   currentRates = currentRates.Where(p => p.RateName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            // сортування
            if (ComboSort.SelectedIndex >= 0)
            {
                // сортування по зростанню ціни
                if (ComboSort.SelectedIndex == 0)
                    currentRates = currentRates.OrderBy(p => p.Price).ToList();
                // сортування по спаданню ціни
                if (ComboSort.SelectedIndex == 1)
                    currentRates = currentRates.OrderByDescending(p => p.Price).ToList();
            }
            // В якості джерела данних привласнюємо список данных
            rates = currentRates;
            DataGridRate.ItemsSource = currentRates;
            // відображення кількостей записів
            TextBlockCount.Text = $" Результат запитів: {currentRates.Count} записів з {_itemcount}";
        }
        // сортування товарів 
        private void ComboSortSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }
        private void BtnAddClick(object sender, RoutedEventArgs e)
        {
            // відкриття  AddRatePage для додавання нового запису
            //Manager.MainFrame.Navigate(new AddRatePage(null));

            try
            {

                RateWindow window = new RateWindow(new Rate());
                if (window.ShowDialog() == true)
                {
                    DataDBEntities.GetContext().Rates.Add(window.currentItem);
                    DataDBEntities.GetContext().SaveChanges();
                    LoadData();
                    MessageBox.Show("Запис доданий", "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch
            {
                MessageBox.Show("Помилка");
            }
        }

        private void BtnDeleteClick(object sender, RoutedEventArgs e)
        {
            // Видалення обраного товару з таблиці
            //отримуємо всі виділені товари
            var selectedRates = DataGridRate.SelectedItems.Cast<Rate>().ToList();
            // вивід повідомлення з питанням
            MessageBoxResult messageBoxResult = MessageBox.Show($"Видалити {selectedRates.Count()} записів???",
                "Видалення", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            //якщо користувач натиснув ОК намагаємося видалити запис
            if (messageBoxResult == MessageBoxResult.OK)
            {
                try
                {
                    // берем з списку видаляємих товарів один елемент
                    Rate x = selectedRates[0];
                    // перевірка, чи є у товара в таблиці про продажі зв'язані записи
                    // якщо так, то викидуємо виключення і видалення переривається
                    if (x.Orders.Count > 0)
                        throw new Exception("Є запис в продажах");
                   
                    // видалення товару
                    DataDBEntities.GetContext().Rates.Remove(x);
                    //збереження змін
                    DataDBEntities.GetContext().SaveChanges();
                    MessageBox.Show("Записи видалені");
                    rates.Clear();
                    rates = DataDBEntities.GetContext().Rates.OrderBy(p => p.Service.Name).OrderBy(p => p.Zone.Name).ToList();
                    DataGridRate.ItemsSource = null;
                    DataGridRate.ItemsSource = rates;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Помилка видалення", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void BtnSellClick(object sender, RoutedEventArgs e)
        {
            // відкриття сторінки про продажі SellRatesPage
            // передача в нього обранного товару
           // Manager.MainFrame.Navigate(new SellRatesPage((sender as Button).DataContext as Rate));
        }
        // відображення номерів рядків в DataGrid
        private void DataGridRateLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        private void BtnEditDev_Click(object sender, RoutedEventArgs e)
        {
           // Manager.MainFrame.Navigate(new DevelopersPage());
        }

        private void PrintExcel()
        {
            //string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\" + "Rates" + ".xltx";
            //Excel.Application xlApp = new Excel.Application();
            //Excel.Worksheet xlSheet = new Excel.Worksheet();
            //try
            //{
            //    xlApp.Workbooks.Open(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            //                              Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            //                              Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            //                              Type.Missing, Type.Missing);
            //    xlApp.Interactive = false;
            //    xlApp.EnableEvents = false;
            //    Excel.Range xlSheetRange;
            //    xlSheet = (Excel.Worksheet)xlApp.Sheets[1];
            //    xlSheet.Name = "Список товаров";
            //    int row = 2;
            //    int i = 0;


            //    foreach (Rate good in rates)
            //    {
            //        xlSheet.Cells[row, 1] = (i + 1).ToString();
            //        string s;
            //        // DateTime y = Convert.ToDateTime(dtOrders.Rows[i].Cells[1].Value);
            //        xlSheet.Cells[row, 2] = good.RateId.ToString();
            //        s = "";


            //        xlSheet.Cells[row, 3] = good.RateName.ToString();
            //        xlSheet.Cells[row, 4] = good.Price.ToString();
            //        s = "";
            //        if (good.Weight != null) s = good.Weight.ToString();
            //        xlSheet.Cells[row, 5] = s;
            //        s = "";
            //        if (good.Width != null) s = good.Width.ToString();
            //        xlSheet.Cells[row, 6] = s;
            //        s = "";
            //        if (good.Length != null) s = good.Length.ToString();
            //        xlSheet.Cells[row, 7] = s;
            //        s = "";
            //        if (good.Heigth != null) s = good.Heigth.ToString();
            //        xlSheet.Cells[row, 8] = s;

            //        xlSheet.Cells[row, 9] = good.Developer.DeveloperName;
            //        xlSheet.Cells[row, 10] = good.GetStatus;
            //        row++;
            //        Excel.Range r = xlSheet.get_Range("A" + row.ToString(), "J" + row.ToString());
            //        r.Insert(Excel.XlInsertShiftDirection.xlShiftDown);
            //        i++;
            //    }




            //    row--;
            //    xlSheetRange = xlSheet.get_Range("A2:J" + (row + 1).ToString(), Type.Missing);
            //    xlSheetRange.Borders.LineStyle = true;
            //    //xlSheet.Cells[row + 1, 9] = "=SUM(I2:I" + row.ToString() + ")";

            //    //xlSheet.Cells[row + 1, 8] = "ИТОГО:";
            //    row++;

            //    xlSheetRange = xlSheet.UsedRange;
            //    xlSheetRange.Columns.AutoFit();
            //    xlSheetRange.Rows.AutoFit();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
            //finally
            //{
            //    xlApp.Visible = true;
            //    xlApp.Interactive = true;
            //    xlApp.ScreenUpdating = true;
            //    xlApp.UserControl = true;
            //}
        }


        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            PrintExcel();
        }

        private void ComboZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void ComboService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }
    }
}
