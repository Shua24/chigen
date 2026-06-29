using System.Windows;

namespace Chigen.App.Views
{
    public partial class ConclusionWindow : Window
    {
        public string ResultText => txtConclusion.Text;

        public ConclusionWindow(string currentText)
        {
            InitializeComponent();
            txtConclusion.Text = currentText;
            txtConclusion.Focus();
            txtConclusion.SelectAll();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
