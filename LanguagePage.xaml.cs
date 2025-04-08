using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Батталов_школа
{
    public partial class LanguagePage : Page
    {
        int CurrentPage = 0;
        bool isLoaded = false;
        List<Client> ClientsList;
        List<Client> CurrentPageList = new List<Client>();
        int CountUsers = 0;

        public LanguagePage()
        {
            InitializeComponent();
            isLoaded = true;
            LanguageListView.ItemsSource = Батталов_LanguageEntities.GetContext().Client.ToList();
            Update();
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SwitchPage(0, PageListBox.SelectedIndex);
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchPage(1, null);
        }

        private void RigthButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchPage(2, null);
        }

        private void LanguagePageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded)
            {
                CurrentPage = 0;
                Update();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;
            var currentClientServices = Батталов_LanguageEntities.GetContext().ClientService
                .Where(p => p.ClientID == currentClient.ID)
                .ToList();

            if (currentClientServices.Count != 0)
            {
                MessageBox.Show("Невозможно выполнить удаление, так как существуют записи на эту услугу((");
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Батталов_LanguageEntities.GetContext().Client.Remove(currentClient);
                        Батталов_LanguageEntities.GetContext().SaveChanges();
                        Update();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        void Update()
        {
            var currentClients = Батталов_LanguageEntities.GetContext().Client.ToList();


            switch (ComboGender.SelectedIndex)
            {
                case 1:
                    currentClients = currentClients.Where(p => p.GenderCode == "1").ToList();
                    break;
                case 2:
                    currentClients = currentClients.Where(p => p.GenderCode == "2").ToList();
                    break;
            }

            string searchText = BoxSerch.Text.ToLower();
            currentClients = currentClients
                .Where(p =>
                    p.FirstName.ToLower().Contains(searchText) ||
                    p.LastName.ToLower().Contains(searchText) ||
                    p.Patronymic.ToLower().Contains(searchText) ||
                    p.Email.ToLower().Contains(searchText) ||
                    p.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").ToLower().Contains(searchText))
                .ToList();

            switch (ComboSort.SelectedIndex)
            {
                case 1:
                    currentClients = currentClients.OrderBy(p => p.FirstName).ToList();
                    break;
                case 2:
                    currentClients = currentClients
                        .OrderByDescending(p => p.LastLoginDATA.HasValue)
                        .ThenByDescending(p => p.LastLoginDATA)
                        .ToList();
                    break;
                case 3:
                    currentClients = currentClients.OrderByDescending(p => p.CountLogin).ToList();
                    break;
            }

            ClientsList = currentClients;

            SwitchPage(0, 0);
        }

        void SwitchPage(int Direction, int? SelectedPage)
        {
            CurrentPageList.Clear();
            CountUsers = ClientsList.Count;
            var AllClients = ClientsList;

            int valuepage = 10;
            int CountPage;

            switch (LanguagePageCombo.SelectedIndex)
            {
                case 0:
                    valuepage = 10;
                    break;
                case 1:
                    valuepage = 50;
                    break;
                case 2:
                    valuepage = 200;
                    break;
                case 3:
                    CurrentPageList = ClientsList;
                    PageListBox.Visibility = Visibility.Hidden;
                    RigthButton.Visibility = Visibility.Hidden;
                    LeftButton.Visibility = Visibility.Hidden;

                    LanguageListView.ItemsSource = CurrentPageList;
                    LanguageListView.Items.Refresh();
                    PageCountAllBlock.Text = $"{CountUsers} из {AllClients.Count}";
                    return;
            }

            CountPage = (CountUsers + valuepage - 1) / valuepage;

            PageListBox.Visibility = Visibility.Visible;
            RigthButton.Visibility = Visibility.Visible;
            LeftButton.Visibility = Visibility.Visible;

            bool doUpdate = true;
            int min;

            if (SelectedPage.HasValue)
            {
                CurrentPage = SelectedPage.Value;
     
                int start = CurrentPage * valuepage;
                min = Math.Min(start + valuepage, CountUsers);
                for (int i = start; i < min; i++)
                    CurrentPageList.Add(ClientsList[i]);
            }


            else
            {
                if (Direction == 1 && CurrentPage > 0)
                {
                    CurrentPage--;
                }
                else if (Direction == 2 && CurrentPage < CountPage - 1)
                {
                    CurrentPage++;
                }
                else
                {
                    doUpdate = false;
                }

                if (doUpdate)
                {
                    int start = CurrentPage * valuepage;
                    min = Math.Min(start + valuepage, CountUsers);
                    for (int i = start; i < min; i++)
                        CurrentPageList.Add(ClientsList[i]);
                }
            }

            if (doUpdate)
            {
                PageListBox.Items.Clear();
                for (int i = 0; i < CountPage; i++)
                    PageListBox.Items.Add(i);
                PageListBox.SelectedIndex = CurrentPage;

                int start = CurrentPage * valuepage;
                min = Math.Min(start + valuepage, CountUsers);

                PageCountAllBlock.Text = $"{CountUsers} из {AllClients.Count}";
                LanguageListView.ItemsSource = CurrentPageList;
                LanguageListView.Items.Refresh();
            }
        }

        private void ComboGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded)
                Update();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded)
                Update();
        }

        private void BoxSerch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isLoaded)
                Update();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Батталов_LanguageEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                Update();
            }
        }
    }
}
