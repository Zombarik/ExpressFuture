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

namespace FutureExpress.Windows
{
    /// <summary>
    /// Логіка взаємодії для RegsWindow.xaml
    /// </summary>
    public partial class RegsWindow : Window
    {
        public Client currentItem { get; private set; }
        private string _filePath = null;
        // назва поточної головної фотографії
        private string _photoName = null;
        // поточна папка програми
        private static string _currentDirectory = Directory.GetCurrentDirectory() + @"\Images\";
        bool IsUserNameFree = true;
        public RegsWindow()
        {
            InitializeComponent();
            currentItem = new Client();
            DataContext = currentItem;
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
                // діалог поверне true, якщо файл був відкритий
                if (op.ShowDialog() == true)
                {
                    // перевірка розміру файлу
                    // по умові файл повинен бути не більше 2Мб.
                    FileInfo fileInfo = new FileInfo(op.FileName);
                    if (fileInfo.Length > (1024 * 1024 * 2))
                    {
                        // розмір файлу менше 2Мб.
                        throw new Exception("Розмір файлу повинен бути менше 2Мб");
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

            if (!IsUserNameFree)
                s.AppendLine("Імя користувача повинно бути унікальним, виберіть друге ім'я");
            if (string.IsNullOrWhiteSpace(currentItem.Name))
                s.AppendLine("Поле ім'я пусте");
            
            if (string.IsNullOrWhiteSpace(currentItem.Surname))
                s.AppendLine("Поле призвіще пусте");
            if (string.IsNullOrWhiteSpace(currentItem.Phone))
                s.AppendLine("Поле телефон пусте");
            if (string.IsNullOrWhiteSpace(currentItem.Email))
                s.AppendLine("Поле email пусте");


            if (string.IsNullOrWhiteSpace(currentItem.PassportSeries))
                s.AppendLine("Вкажіть серію паспорта");
            if (string.IsNullOrWhiteSpace(currentItem.PassportNum))
                s.AppendLine("Вкажітб номер паспорта");
            if (string.IsNullOrWhiteSpace(_photoName))
                s.AppendLine("фото не обрано пусте");

            if (string.IsNullOrWhiteSpace(PasswordBoxNewPassword1.Password))
                s.AppendLine("Придумайте пароль");
            if ((PasswordBoxNewPassword1.Password != PasswordBoxNewPassword2.Password) )
                {
                    s.AppendLine("Паролі не співпадають");
                }
               
            return s;
        }

        private void TextBoxUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<User> users = DataDBEntities.GetContext().Users.ToList();
            User user = users.FirstOrDefault(p =>  p.UserName.ToLower() == TextBoxUserName.Text.ToLower());
            if (user != null)
            {
                IsUserNameFree = false;
                TextBoxUserName.Foreground = Brushes.Red;
            }
            else
            {
                IsUserNameFree = true;
                TextBoxUserName.Foreground = Brushes.Green;
            }
          }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder _error = CheckFields();
            // Якщо помилки є, то виводимо помилки в MessageBox
            // і перериваємл виконання 
            if (_error.Length > 0)
            {
                MessageBox.Show(_error.ToString());
                return;
            }

            if (_photoName != null)
            {


                // формуємо нову назву файлу картинки,
                // так як в папці може бути файл з тим же іменем
                string photo = ChangePhotoName();
                // шлях куди треба скопіювати файл
                string dest = _currentDirectory + photo;
                File.Copy(_filePath, dest);
                currentItem.Photo = photo;


            }
            try
            {
                User user = new User();
                user.Password = PasswordBoxNewPassword1.Password;
                user.UserName = TextBoxUserName.Text;
                user.RoleId = 3;

                DataDBEntities.GetContext().Users.Add(user);
                DataDBEntities.GetContext().SaveChanges();


                DataDBEntities.GetContext().Clients.Add(currentItem);
                DataDBEntities.GetContext().SaveChanges();
                MessageBox.Show("Реєстрація пройшла успішно");
                this.DialogResult = true;
                
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
