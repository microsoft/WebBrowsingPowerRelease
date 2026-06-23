using System.Windows;
using System.Windows.Input;
using EdgeBrowsingPower.Services;
using WinForms = System.Windows.Forms;
using WpfCursors = System.Windows.Input.Cursors;
using WpfMessageBox = System.Windows.MessageBox;

namespace EdgeBrowsingPower;

public partial class MainWindow : Window
{
    private readonly ZipResourceService zipResourceService = new();

    public MainWindow()
    {
        InitializeComponent();
        MsaAccountTextBox.Focus();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        ClearStatus();

        string msaAccountId = MsaAccountTextBox.Text.Trim();
        string password = MsaPasswordBox.Password;

        if (!ValidateInputs(msaAccountId, password))
        {
            return;
        }

        string? destinationFolder = SelectDestinationFolder();
        if (destinationFolder is null)
        {
            SetStatus("Folder selection canceled.", isError: false);
            return;
        }

        await RunCreationAsync(destinationFolder, msaAccountId, password);
    }

    private bool ValidateInputs(string msaAccountId, string password)
    {
        if (string.IsNullOrWhiteSpace(msaAccountId))
        {
            SetStatus("Enter an MSA account ID before creating files.");
            MsaAccountTextBox.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            SetStatus("Enter a password before creating files.");
            MsaPasswordBox.Focus();
            return false;
        }

        return true;
    }

    private static string? SelectDestinationFolder()
    {
        using WinForms.FolderBrowserDialog dialog = new()
        {
            Description = "Choose where EdgeBrowsingPower should extract the bundled files.",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true
        };

        return dialog.ShowDialog() == WinForms.DialogResult.OK
            ? dialog.SelectedPath
            : null;
    }

    private async Task RunCreationAsync(string destinationFolder, string msaAccountId, string password)
    {
        SetControlsEnabled(false);
        Mouse.OverrideCursor = WpfCursors.Wait;
        SetStatus("Creating files...", isError: false);

        try
        {
            await Task.Run(() => zipResourceService.Create(destinationFolder, msaAccountId, password));

            MsaPasswordBox.Clear();
            SetStatus("Creation completed successfully.", isError: false);
            WpfMessageBox.Show(
                "Creation completed successfully.\n\nRun RunJobABLTraining.cmd first, then run RunJobABLTrained.cmd.\n\nIf you have any problem, check Browsing_BatteryLife_v2.0.15_and_YouTubeToS_v1.0.2.pdf.",
                "EdgeBrowsingPower",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (EdgeBrowsingPowerException ex)
        {
            SetStatus(ex.Message);
            WpfMessageBox.Show(ex.Message, "EdgeBrowsingPower", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception)
        {
            const string message = "Creation failed. The operation did not complete.";
            SetStatus(message);
            WpfMessageBox.Show(message, "EdgeBrowsingPower", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
            SetControlsEnabled(true);
        }
    }

    private void SetControlsEnabled(bool isEnabled)
    {
        MsaAccountTextBox.IsEnabled = isEnabled;
        MsaPasswordBox.IsEnabled = isEnabled;
        CreateButton.IsEnabled = isEnabled;
        ExitButton.IsEnabled = isEnabled;
    }

    private void ClearStatus()
    {
        StatusTextBlock.Text = string.Empty;
    }

    private void SetStatus(string message, bool isError = true)
    {
        StatusTextBlock.Foreground = isError
            ? System.Windows.Media.Brushes.DarkRed
            : System.Windows.Media.Brushes.DarkGreen;
        StatusTextBlock.Text = message;
    }
}