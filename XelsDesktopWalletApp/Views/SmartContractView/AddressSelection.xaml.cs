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

namespace XelsDesktopWalletApp.Views.SmartContractView
{
    /// <summary>
    /// Interaction logic for AddressSelection.xaml
    /// </summary>
    public partial class AddressSelection : Page
    {
        public AddressSelection()
        {
            InitializeComponent();
        }
        private void useAddressBtn_Click(object sender, RoutedEventArgs e)
        {
            var smtdash = new SmartContractDashboard();
            this.Content = smtdash;
        }
        //private void dashboardBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    Dashboard mw = new Dashboard();
        //    this.Hide();
        //    mw.ShowDialog();
        //    this.Close();
        //}
    }
}
