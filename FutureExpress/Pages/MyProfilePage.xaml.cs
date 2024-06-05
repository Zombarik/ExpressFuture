using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логіка взаємодії для MyProfilePage.xaml
    /// </summary>
    public partial class MyProfilePage : Page
    {
        public Client currentItem { get; private set; }
        private string _filePath = null;
        // назва поточної главної фотографії
        private string _photoName = null;
        // поточна папка додатку
        private static string _currentDirectory = Directory.GetCurrentDirectory() + @"\Images\";
        bool _isNewClient = false;

        public MyProfilePage()
        {
            InitializeComponent();
          
        }


        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                currentItem = DataDBEntities.GetContext().Clients.FirstOrDefault(u => u.UserName == Manager.CurrentUser.UserName);
                if (currentItem is null)
                {
                    currentItem = new Client();
                    currentItem.UserName = Manager.CurrentUser.UserName;
                    _isNewClient = true;
                }
                this.DataContext = currentItem;
            }
        }

        // завантаження фото 
        private void BtnLoadClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //Діалог відкриття файлу
                OpenFileDialog op = new OpenFileDialog();
                op.Title = "Select a picture";
                op.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
                // діалог верне true, якщо файл був відкритий
                if (op.ShowDialog() == true)
                {
                    // перевірка розміру файлу
                    // по умові файл повинен бути не більше 2Мб.
                    FileInfo fileInfo = new FileInfo(op.FileName);
                    if (fileInfo.Length > (1024 * 1024 * 2))
                    {
                        // розмір файлу менше 2Мб
                        throw new Exception("Размер файла должен быть меньше 2Мб");
                    }
                    ImagePhoto.Source = new BitmapImage(new Uri(op.FileName));
                    _photoName = op.SafeFileName;
                    _filePath = op.FileName;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                _filePath = null;
            }
        }
        //підбір імені файлу
        string ChangePhotoName()
        {
            string x = _currentDirectory + _photoName;
            string photoname = _photoName;
            int i = 0;
            if (File.Exists(x))
            {
                while (File.Exists(x))
                {
                    i++;
                    x = _currentDirectory + i.ToString() + photoname;
                }
                photoname = i.ToString() + photoname;
            }
            return photoname;

        }


        private StringBuilder CheckFields()
        {
            StringBuilder s = new StringBuilder();
            if (string.IsNullOrWhiteSpace(currentItem.UserName))
                s.AppendLine("Задайте ім'я користувача");
            if (string.IsNullOrWhiteSpace(currentItem.Name))
                s.AppendLine("Поле ім'я пусте");

            if (string.IsNullOrWhiteSpace(currentItem.Surname))
                s.AppendLine("Поле призвіще пусте");
            if (string.IsNullOrWhiteSpace(currentItem.Phone))
                s.AppendLine("Поле телефон пусте");
            if (string.IsNullOrWhiteSpace(currentItem.Email))
                s.AppendLine("Поле email пусте");

            if (string.IsNullOrWhiteSpace(_photoName))
                s.AppendLine("фото не обрано пусто");

            if (CheckBoxChangePassword.IsChecked == true)
            {
                User user = Manager.CurrentUser;
                if ((PasswordBoxNewPassword1.Password != PasswordBoxNewPassword2.Password) || (PasswordBoxOldPassword.Password != user.Password))
                {
                    s.AppendLine("Паролі не співпадають");
                }
                else
                {
                    user.Password = PasswordBoxNewPassword1.Password;
                }
            }
            return s;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder _error = CheckFields();
            // якщо помилки є, то виводим помилки в MessageBox
            // и прерываем выполнение 
            if (_error.Length > 0)
            {
                MessageBox.Show(_error.ToString());
                return;
            }

            if (_photoName != null)
            {


                // створюємо нову назву файла картинки,
                // так як в папці може бути файл з тим же іменем
                string photo = ChangePhotoName();
                // шлях куди треба скопіювати файл
                string dest = _currentDirectory + photo;
                File.Copy(_filePath, dest);
                currentItem.Photo = photo;


            }
            try
            {


                if (_isNewClient)
                {
                    currentItem.UserName = Manager.CurrentUser.UserName;
                    DataDBEntities.GetContext().Clients.Add(currentItem);
                }


                DataDBEntities.GetContext().SaveChanges();
                MessageBox.Show("Запис змінено");
                _isNewClient = false;
            }
            catch
            {
                MessageBox.Show("Помилка");
            }

        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        
    }
}
