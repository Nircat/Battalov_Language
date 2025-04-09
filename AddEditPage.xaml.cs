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
using System.Windows.Media.Imaging;

namespace Батталов_школа
{
    public partial class AddEditPage : Page
    {
        private Client _currentClient = new Client();

        public AddEditPage(Client SelectedClient)
        {
            InitializeComponent();

            if (SelectedClient != null)
            {
                _currentClient = SelectedClient;
            }

            DataContext = _currentClient;

            if (_currentClient.GenderCode == "1")
                Man.IsChecked = true;
            else if (_currentClient.GenderCode == "2")
                Woman.IsChecked = true;

            if (_currentClient.ID == 0)
            {
                WrapID.Visibility = Visibility.Hidden;
                IDBlock.Visibility = Visibility.Hidden;
                IDBox.Visibility = Visibility.Hidden;
            }

            // Загрузка изображения (либо фото клиента, либо заглушка)
            string projectDirectory = GetProjectRootDirectory();
            string defaultImagePath = System.IO.Path.Combine(projectDirectory, "Клиенты", "picture.png");

            if (string.IsNullOrEmpty(_currentClient.PhotoPath) || !File.Exists(Path.Combine(projectDirectory, _currentClient.PhotoPath)))
            {
                LogoImage.Source = new BitmapImage(new Uri(defaultImagePath));
            }
            else
            {
                string fullPhotoPath = Path.Combine(projectDirectory, _currentClient.PhotoPath);
                LogoImage.Source = new BitmapImage(new Uri(fullPhotoPath));
            }
            if (string.IsNullOrEmpty(_currentClient.PhotoPath))
            {
                _currentClient.PhotoPath = System.IO.Path.Combine("Клиенты", "picture.png");
            }

        }

        private void ChangePicture_Click(object sender, RoutedEventArgs e)
        {
            string projectDirectory = GetProjectRootDirectory();
            string clientsFolderPath = System.IO.Path.Combine(projectDirectory, "Клиенты");

            if (!Directory.Exists(clientsFolderPath))
            {
                Directory.CreateDirectory(clientsFolderPath);
            }

            OpenFileDialog myOpenFileDialog = new OpenFileDialog
            {
                InitialDirectory = clientsFolderPath
            };

            if (myOpenFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = myOpenFileDialog.FileName;

                _currentClient.PhotoPath = System.IO.Path.Combine("Клиенты", System.IO.Path.GetFileName(selectedFilePath));

                LogoImage.Source = new BitmapImage(new Uri(selectedFilePath));
            }
        }

        private string GetProjectRootDirectory()
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(exePath)));
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentClient.FirstName))
                errors.AppendLine("Укажите фамилию клиента");
            else if (!Regex.IsMatch(_currentClient.FirstName, @"^[A-Za-zА-Яа-яЁё\- ]+$") || _currentClient.FirstName.Length > 50)
                errors.AppendLine("Фамилия может содержать только буквы, пробелы и дефисы, и не может быть длиннее 50 символов");

            if (string.IsNullOrWhiteSpace(_currentClient.LastName))
                errors.AppendLine("Укажите имя клиента");
            else if (!Regex.IsMatch(_currentClient.LastName, @"^[A-Za-zА-Яа-яЁё\- ]+$") || _currentClient.LastName.Length > 50)
                errors.AppendLine("Имя может содержать только буквы, пробелы и дефисы, и не может быть длиннее 50 символов.");

            if (string.IsNullOrWhiteSpace(_currentClient.Patronymic))
                errors.AppendLine("Укажите отчество клиента");
            else if (!Regex.IsMatch(_currentClient.Patronymic, @"^[A-Za-zА-Яа-яЁё\- ]+$") || _currentClient.Patronymic.Length > 50)
                errors.AppendLine("Отчество может содержать только буквы, пробелы и дефисы, и не может быть длиннее 50 символов.");

            if (string.IsNullOrWhiteSpace(_currentClient.Email))
            {
                errors.AppendLine("Укажите Email!");
            }
            else
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

                if (!Regex.IsMatch(_currentClient.Email, pattern))
                    errors.AppendLine("Укажите правильный Email!");

                const string russianLetters = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
                if (_currentClient.Email.Any(letter => russianLetters.Contains(letter)))
                    errors.AppendLine("Email не может содержать кириллицу");

                int domenCount = 0;
                int dotCount = 0;
                for (int i = 0; i < _currentClient.Email.Length; i++)
                {
                    if (_currentClient.Email[i] == '.')
                    {
                        for (int j = i + 1; j < _currentClient.Email.Length; j++)
                            domenCount++;
                    }
                    if (_currentClient.Email[i] == '.')
                        dotCount++;
                }
                if (domenCount < 2)
                    errors.AppendLine("Email неверный. Домен не может быть менее 2 символов");
                if (dotCount != 1)
                    errors.AppendLine("Email может содержать только одну точку");
            }

            if (BirthDP.SelectedDate == null || BirthDP.SelectedDate.Value.Year < 1900)
            {
                MessageBox.Show("Пожалуйста, укажите корректную дату рождения.");
                return;
            }

            if (string.IsNullOrWhiteSpace(BirthDP.Text))
            {
                errors.AppendLine("Укажите дату рождения клиента");
            }
            else
            {
                _currentClient.Birthday = Convert.ToDateTime(BirthDP.Text);
            }

            if (string.IsNullOrWhiteSpace(_currentClient.Phone))
            {
                errors.AppendLine("Укажите номер телефона!");
            }
            else
            {
                string phonePattern = @"^\+?\d[\d\-\(\)\s]+$";
                string clearPhone = _currentClient.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("+", "").Trim();

                if (!Regex.IsMatch(_currentClient.Phone, phonePattern) || !clearPhone.All(char.IsDigit))
                {
                    errors.AppendLine("Телефон может содержать только цифры, плюс, минус, круглые скобки и пробел!");
                }
                else if (_currentClient.Phone.Length > 20)
                {
                    errors.AppendLine("Укажите в телефоне менее 20 символов у клиента");
                }
            }

            if (Woman.IsChecked == true)
                _currentClient.GenderCode = "2";
            else if (Man.IsChecked == true)
                _currentClient.GenderCode = "1";
            else
                errors.AppendLine("Укажите пол клиента");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (_currentClient.ID == 0)
            {
                Батталов_LanguageEntities.GetContext().Client.Add(_currentClient);
            }

            try
            {
                _currentClient.RegistrationDate = DateTime.Today;
                Батталов_LanguageEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                string errorMessages = "";
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessages += $"- {validationError.PropertyName}: {validationError.ErrorMessage}\n";
                    }
                }

                MessageBox.Show("Ошибка валидации:\n" + errorMessages);
            }
        }
    }
}
