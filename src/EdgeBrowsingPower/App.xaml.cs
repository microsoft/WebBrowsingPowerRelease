using System.Windows;
using System.Windows.Threading;
using WpfMessageBox = System.Windows.MessageBox;

namespace EdgeBrowsingPower;

public partial class App : System.Windows.Application
{
    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        WpfMessageBox.Show(
            "EdgeBrowsingPower encountered an unexpected error. The operation did not complete.",
            "EdgeBrowsingPower",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        e.Handled = true;
    }
}