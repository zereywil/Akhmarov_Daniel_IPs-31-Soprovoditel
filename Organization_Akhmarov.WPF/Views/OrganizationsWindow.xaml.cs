using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Organization_Akhmarov.Data.Models;
using Organization_Akhmarov.Data.Services;

namespace Organization_Akhmarov.WPF{
    public partial class OrganizationsWindow : Window{
        private OrganizationService _orgService;
        private Organization? _selectedOrganization;

        public OrganizationsWindow()
        {
            InitializeComponent();
            _orgService = new OrganizationService();
            LoadOrganizations();
        }

        private void LoadOrganizations(){
            LvOrganizations.ItemsSource = _orgService.GetAll();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e){
            var dialog = new OrganizationDialog();
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.OrganizationName))
            {
                _orgService.Add(new Organization(dialog.OrganizationName));
                LoadOrganizations();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e){
            if (_selectedOrganization == null){
                MessageBox.Show("Выберите организацию", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new OrganizationDialog(_selectedOrganization.Name);
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.OrganizationName)){
                _selectedOrganization.Name = dialog.OrganizationName;
                _orgService.Update(_selectedOrganization);
                LoadOrganizations();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e){
            if (_selectedOrganization == null){
                MessageBox.Show("Выберите организацию", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new ConfirmDeleteDialog(_selectedOrganization.Name);
            if (dialog.ShowDialog() == true){
                _orgService.Delete(_selectedOrganization.Id);
                _selectedOrganization = null;
                LoadOrganizations();
            }
        }

        private void LvOrganizations_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e){
            var item = ItemsControl.ContainerFromElement(LvOrganizations, (DependencyObject)e.OriginalSource)
                       as ListViewItem;
            if (item != null)
                item.IsSelected = true;
        }

        private void LvOrganizations_ContextMenuOpening(object sender, ContextMenuEventArgs e){
            bool hasSelection = _selectedOrganization != null;

            foreach (var obj in LvOrganizations.ContextMenu.Items){
                if (obj is MenuItem mi)
                {
                    string header = mi.Header?.ToString();
                    if (header == "Изменить" || header == "Удалить")
                        mi.IsEnabled = hasSelection;
                }
            }
        }


        private void BtnSelect_Click(object sender, RoutedEventArgs e){
            OpenMainWorkWindow();
        }

        private void LvOrganizations_MouseDoubleClick(object sender, MouseButtonEventArgs e){
            OpenMainWorkWindow();
        }

        private void OpenMainWorkWindow(){
            if (_selectedOrganization == null){
                MessageBox.Show("Выберите организацию", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var mainWindow = new MainWorkWindow(_selectedOrganization);
            mainWindow.Show();
            this.Close();
        }

        private void LvOrganizations_SelectionChanged(object sender, SelectionChangedEventArgs e){
            _selectedOrganization = LvOrganizations.SelectedItem as Organization;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e){
            Application.Current.Shutdown();
        }
    }
}