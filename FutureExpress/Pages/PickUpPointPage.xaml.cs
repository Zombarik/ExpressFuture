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
    /// Логіка взаємодії для PickUpPointPage.xaml
    /// </summary>
    public partial class PickUpPointPage : Page
    {
        List<PickupPoint> points;
        public PickUpPointPage()
        {
            InitializeComponent();
        }


        void LoadData()
        {
            try
            {
                DtData.ItemsSource = null;
                //завантаження оновленних данних
                DataDBEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                points = DataDBEntities.GetContext().PickupPoints.OrderBy(p => p.Address).ToList();
                DtData.ItemsSource = points;
            }
            catch
            {
                MessageBox.Show("Помилка");
            }
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //подія відображення данного Page
            // оновлюєм данні кожен раз коли активується цей Page
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


                PickupPointWindow window = new PickupPointWindow(new PickupPoint());
                if (window.ShowDialog() == true)
                {
                    DataDBEntities.GetContext().PickupPoints.Add(window.currentItem);
                    DataDBEntities.GetContext().SaveChanges();
                    LoadData();
                    MessageBox.Show("Запис додано", "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
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
                // якщо ніодного об'єкту не виділено, виходимо
                if (DtData.SelectedItem == null) return;
                // отримуємо виділений об'єкт
                PickupPoint selected = DtData.SelectedItem as PickupPoint;

                PickupPointWindow window = new PickupPointWindow(selected);


                if (window.ShowDialog() == true)
                {
                    if (window.currentItem != null)
                    {
                        DataDBEntities.GetContext().Entry(window.currentItem).State = EntityState.Modified;
                        DataDBEntities.GetContext().SaveChanges();
                        LoadData();
                        MessageBox.Show("Запис змінено", "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
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
                // якщо ніодного об'єкту не виділено, виходимо
                if (DtData.SelectedItem == null) return;
                // отримуємо виділений об'єкт
                MessageBoxResult messageBoxResult = MessageBox.Show($"Видалити запис? ", "Видалення", MessageBoxButton.OKCancel,
MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    PickupPoint deletedItem = DtData.SelectedItem as PickupPoint;





                    if (deletedItem.Orders.Count > 0)
                    {
                        MessageBox.Show("Помилка видалення, є зв'язані записи", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    DataDBEntities.GetContext().PickupPoints.Remove(deletedItem);
                    DataDBEntities.GetContext().SaveChanges();
                    LoadData();
                    MessageBox.Show("Запис видалено", "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
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
