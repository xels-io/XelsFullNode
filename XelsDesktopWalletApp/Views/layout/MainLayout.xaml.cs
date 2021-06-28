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

namespace XelsDesktopWalletApp.Views.layout
{
    /// <summary>
    /// Interaction logic for MainLayout.xaml
    /// </summary>
    public partial class MainLayout : Window
    {
        public MainLayout()
        {
            InitializeComponent();
        }

       

        private void MenuItem_ClickPage1(object sender, RoutedEventArgs e)
        {
            this.page_Content.Children.Add(new Page1());
        }

        private void MenuItem_ClickPage2(object sender, RoutedEventArgs e)
        {
            this.page_Content.Children.Add(new Page2());
        }

      
    }
}
