using Microsoft.Win32;
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
using Word = Microsoft.Office.Interop.Word;
using FutureExpress.Models;

namespace FutureExpress.Windows
{
    /// <summary>
    /// Логіка взаємодії для OrderTicketWindow.xaml
    /// </summary>
    
        public partial class OrderTicketWindow : Window
        {

            Order _currentOrder; // поточне замовлення
            User _currentUser;// поточний користувач
            public OrderTicketWindow(Order order)
            {
                InitializeComponent();
                LoadDataAndInit(order);
            }
            // завантаження й ініціалізація данних
            void LoadDataAndInit(Order order)
            {
                _currentOrder = order;

                _currentUser = Manager.CurrentUser;
                if (_currentUser != null && _currentUser.RoleId==3)
                {
                TextBlockOrderNumber.Text = $"Замовлення №{_currentOrder.OrderID} на ім'я " +
                    $"{ _currentUser.Clients.SingleOrDefault().GetFio} оформлене";
                }
                else
                { 
                    TextBlockOrderNumber.Text = $"Замовлення №{_currentOrder.OrderID} оформлено на ім'я {_currentOrder.User.Clients.Single().GetFio}"; 
                }
                TextBlockTotalCost.Text = $"Ітого {_currentOrder.Rate.Price:C}";
                TextBlockOrderCreateDate.Text = _currentOrder.OrderCreateDate.ToLongDateString();
                TextBlockOrderDeliveryDate.Text = _currentOrder.OrderDeliveryDate.ToLongDateString();
                TextBlockOrderGetCode.Text = _currentOrder.GetCode.ToString();
                TextBlockPickupPoint.Text = _currentOrder.PickupPoint.Address;
            }

            private void BtnOk_Click(object sender, RoutedEventArgs e)
            {
                this.Close();
            }

            private void BtnSavePDF_Click(object sender, RoutedEventArgs e)
            {
                PrintInPdf(_currentOrder);
            }

            void PrintInPdf(Order order)
            {
                try
                {
                    string path = null;
                    // вказуємо файл для збереження
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "PDF (.pdf)|*.pdf"; // Filter files by extension
                                                                // якщо діалог завершився успішно
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        path = saveFileDialog.FileName;
                        Word.Application application = new Word.Application();
                        Word.Document document = application.Documents.Add();
                        Word.Paragraph paragraph = document.Paragraphs.Add();
                        Word.Range range = paragraph.Range;
                        range.Font.Bold = 1;
                        range.Text = $"Номер замовлення: {order.OrderID}";
                        range.InsertParagraphAfter();

                        range = paragraph.Range;
                        range.Font.Bold = 0;
                        range.Text = $"Дата замовлення: {order.OrderCreateDate}\n" +
                            $"Дата отримання замовлення: {order.OrderDeliveryDate}\n" +
                            $"Пункт выдачі: {order.PickupPoint.Address}";
                        range.InsertParagraphAfter();

                        range = paragraph.Range;
                        range.Font.Bold = 1;
                        range.Text = $"Код отримання: {order.GetCode}";
                        range.InsertParagraphAfter();
                        range.Font.Bold = 0;

                       
                        Word.Paragraph generalSumProduct = document.Paragraphs.Add();
                        Word.Range generalRange = generalSumProduct.Range;
                        generalRange.Text = $"\nЗагальна вартість замовлення: {order.Rate.Price:f2} грн.";
                        document.SaveAs2($@"{path}", Word.WdExportFormat.wdExportFormatPDF);
                        MessageBox.Show("Талон збрежено");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            // відображення номерів рядків в DataGrid
            private void DataGridGoodLoadingRow(object sender, DataGridRowEventArgs e)
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
        }
    }
