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
using Path = System.IO.Path;

namespace th_teams_assignment_collater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool running = false;

        public MainWindow()
        {
            InitializeComponent();

            CopyFrom.ItemsSource = DirHandler.GetFolderList();
        }

        private void CopyFromSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CopyTo.Text = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Path.GetFileNameWithoutExtension((string)e.AddedItems[0])
            );
        }

        private void CopyFail(string message)
        {

        }

        private async void Collate(object sender, RoutedEventArgs e)
        {
            ProgressText.Text = "Copying files...";
            running = true;
            bool success = await Task.Run(() => DirHandler.DoCollate(CopyFrom.Text, CopyTo.Text, 3, CopyFail));
            running = false;
            if (success)
                ProgressText.Text = "Copy finished.";
            else
                ProgressText.Text = "Files copied, but there are failures.";
        }

        private void FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = running;
        }
    }
}
