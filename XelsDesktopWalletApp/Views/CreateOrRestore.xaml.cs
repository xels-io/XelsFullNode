using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for CreateOrRestore.xaml
    /// </summary>
    public partial class CreateOrRestore : Window
    {
        public CreateOrRestore()
        {
            InitializeComponent();
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            Restore restore = new Restore();
            restore.Show();
            this.Close();
        }
        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            Restore restore = new Restore();
            Create create = new Create();
            create.Show();
            this.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Close();
        }
    }
}
