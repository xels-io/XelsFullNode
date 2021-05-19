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
using System.Windows.Shapes;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            Send send = new Send();
            send.Show();
            this.Close();
            //MyPopup.IsOpen = true;
        }

        private void receiveButton_Click(object sender, RoutedEventArgs e)
        {
            Receive receive = new Receive();
            receive.Show();
            this.Close();
        }
        private void createButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            //MyPopup.IsOpen = false;
        }

        private void Hyperlink_RequestNavigate(object sender)
        {
            Login lg = new Login();
            lg.Show();
            this.Close();
        }
    }
}
