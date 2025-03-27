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

namespace Батталов_школа
{
    /// <summary>
    /// Логика взаимодействия для LanguagePage.xaml
    /// </summary>
    public partial class LanguagePage : Page
    {
        int CurrentPage = 0;


        public LanguagePage()
        {
            InitializeComponent();
            LanguageListView.ItemsSource = Батталов_LanguageEntities.GetContext().Client.ToList();
            SwitchPage(0, CurrentPage);



        }

        void SwitchPage(int Direction, int? SelectedPage)
        {
            var AllClients = Батталов_LanguageEntities.GetContext().Client.ToList();
            List<Client> CurrentPageList = new List<Client>();
            int valuepage = 10;
            int CountUsers = AllClients.Count;

            int CountPage = (CountUsers % valuepage == 0) ? CountUsers / valuepage : CountUsers / valuepage + 1;
            bool Update = true;

            switch (LanguagePageCombo.SelectedIndex)
            {

                case 0:
                    valuepage = 10;
                    CountPage = (CountUsers % valuepage == 0) ? CountUsers / valuepage : CountUsers / valuepage + 1;
                    PageListBox.Visibility = Visibility.Visible;
                    RigthButton.Visibility = Visibility.Visible;
                    LeftButton.Visibility = Visibility.Visible;
                    break;
                case 1:
                    valuepage = 50;
                    CountPage = (CountUsers % valuepage == 0) ? CountUsers / valuepage : CountUsers / valuepage + 1;
                    PageListBox.Visibility = Visibility.Visible;
                    RigthButton.Visibility = Visibility.Visible;
                    LeftButton.Visibility = Visibility.Visible;
                    break;
                case 2:
                    valuepage = 200;
                    CountPage = (CountUsers % valuepage == 0) ? CountUsers / valuepage : CountUsers / valuepage + 1;
                    PageListBox.Visibility = Visibility.Visible;
                    RigthButton.Visibility = Visibility.Visible;
                    LeftButton.Visibility = Visibility.Visible;
                    break;
                case 3:
                    CurrentPageList = AllClients;
                    PageListBox.Visibility = Visibility.Hidden;
                    RigthButton.Visibility = Visibility.Hidden;
                    LeftButton.Visibility = Visibility.Hidden;
                    break;


            }

            
            int min;

            if (SelectedPage.HasValue)
            {
                CurrentPage = SelectedPage.Value;
                if (CurrentPage >= 0 && CurrentPage < CountPage)
                {
                    int start = CurrentPage * valuepage;
                    min = Math.Min(start + valuepage, CountUsers);

                    for (int i = start; i < min; i++)
                        CurrentPageList.Add(AllClients[i]);
                }
            }
            else
            {
                if (Direction == 1)
                {
                    if (CurrentPage > 0)
                    {
                        CurrentPage--;
                        int start = CurrentPage * valuepage;
                        min = Math.Min(start + valuepage, CountUsers);
                        for (; start < min; start++)
                            CurrentPageList.Add(AllClients[start]);
                    }
                    else
                        Update = false;
                }
                if (Direction == 2)
                {
                    if (CurrentPage < CountPage - 1)
                    {
                        CurrentPage++;
                        int start = CurrentPage * valuepage;
                        min = Math.Min(start + valuepage, CountUsers);
                        for (; start < min; start++)
                            CurrentPageList.Add(AllClients[start]);
                    }
                    else
                        Update = false;
                }

            }

            if (Update)
            {
                PageListBox.Items.Clear();
                for (int i = 0; i < CountPage; i++)
                    PageListBox.Items.Add(i);

                PageListBox.SelectedIndex = CurrentPage;
                LanguageListView.ItemsSource = CurrentPageList;
                PageCountAllBlock.Text = Convert.ToString((CurrentPage + 1) * valuepage) + "из" + Convert.ToString(CountUsers);
            }
        }


        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SwitchPage(0, Convert.ToInt32(PageListBox.SelectedIndex));
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchPage(1,null);
        }

        private void RigthButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchPage(2, null);
        }

        private void LanguagePageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageListBox != null)
            {
                SwitchPage(0, CurrentPage);
            }
           
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;
            var currentClientServices = Батталов_LanguageEntities.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ClientID == currentClient.ID).ToList();

            if (currentClientServices.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как существуют записи на эту услугу((");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Батталов_LanguageEntities.GetContext().Client.Remove(currentClient);
                        Батталов_LanguageEntities.GetContext().SaveChanges();

                        LanguageListView.ItemsSource = Батталов_LanguageEntities.GetContext().Client.ToList();

                        //UpdateServices();
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

        }
    }
}
;