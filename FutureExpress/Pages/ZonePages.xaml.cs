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

namespace FutureExpress.Pages
{
    /// <summary>
    /// Логіка взаємодії для ZonePages.xaml
    /// </summary>
    public partial class ZonePages : Page
    {
        List<Zone> zones;
        public ZonePages()
        {
            InitializeComponent();
        }


        void LoadData()
        {
            try
            {
                DtData.ItemsSource = null;
                //завантаження оновлених данних
                DataDBEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                zones = DataDBEntities.GetContext().Zones.OrderBy(p => p.Name).ToList();
                DtData.ItemsSource = zones;
            }
            catch
            {
                MessageBox.Show("Помилка");
            }
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //подія відображення данного Page
            // оновлюємо данні кожен раз коли активується цей Page
            if (Visibility == Visibility.Visible)
            {
                LoadData();
            }
        }

        private void DataGridGoodLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                ZoneWindow window = new ZoneWindow(new Zone());
                if (window.ShowDialog() == true)
                {
                    DataDBEntities.GetContext().Zones.Add(window.currentItem);
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

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // якщо ніодного об'єкта не виділено, виходимо
                if (DtData.SelectedItem == null) return;
                // отримуємо виділений об'єкт
                Zone selected = DtData.SelectedItem as Zone;


                ZoneWindow window = new ZoneWindow(selected);


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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // якщо ніодного об'єкта не виділено, виходимо
                if (DtData.SelectedItem == null) return;
                // отримуємо виділений об'єкт
                MessageBoxResult messageBoxResult = MessageBox.Show($"Видалити запис? ", "Видалення", MessageBoxButton.OKCancel,
MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    Zone deletedItem = DtData.SelectedItem as Zone;





                    if (deletedItem.Rates.Count > 0)
                    {
                        MessageBox.Show("Помилка видалення, є зв'язані записи", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    DataDBEntities.GetContext().Zones.Remove(deletedItem);
                    DataDBEntities.GetContext().SaveChanges();
                    LoadData();
                    MessageBox.Show("Запис видалений", "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка, є зв'язані записи");
            }
            finally
            {
                LoadData();
            }
        }
    }
}
