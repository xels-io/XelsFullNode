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
    /// Interaction logic for CreateContract.xaml
    /// </summary>
    public partial class CreateContract : UserControl
    {
        public CreateContract()
        {
            InitializeComponent();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;

        }
    }
}
