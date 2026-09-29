using System.Windows;


namespace Organization_Akhmarov.WPF{
    public partial class ScladDialog : Window
    {
        public string ScladName => TxtScladName.Text;
        public string ScladAddress => TxtScladAddress.Text;

        public ScladDialog(string initialName = "", string initialAddress = ""){
            InitializeComponent();
            TxtScladName.Text = initialName;
            TxtScladAddress.Text = initialAddress;

            if (!string.IsNullOrEmpty(initialName)){
                Title = "Редактирование склада";
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e){
            if (string.IsNullOrWhiteSpace(TxtScladName.Text)){
                MessageBox.Show("Введите название склада", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}