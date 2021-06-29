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
    /// Interaction logic for AddToken.xaml
    /// </summary>
    public partial class AddToken : UserControl
    {
        public AddToken()
        {
            InitializeComponent();
            this.rowTokenContractAddress.Height = new GridLength(0);
            this.rowTokenSymbol.Height = new GridLength(0);
            this.rowTokenName.Height = new GridLength(0);
            this.rowTokenDecimal.Height = new GridLength(0);
            this.gridAllPage.Height = 200;
        }
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
