using System.Windows;

namespace Organization_Akhmarov.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new OrganizationsWindow();
            window.Show();
        }
    }
}