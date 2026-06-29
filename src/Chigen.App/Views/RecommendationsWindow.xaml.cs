using System.Windows;

namespace Chigen.App.Views
{
    public partial class RecommendationsWindow : Window
    {
        public string ResultText => txtRecommendations.Text;

        public RecommendationsWindow(string currentText)
        {
            InitializeComponent();
            txtRecommendations.Text = currentText;
            txtRecommendations.Focus();
            txtRecommendations.SelectAll();
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
