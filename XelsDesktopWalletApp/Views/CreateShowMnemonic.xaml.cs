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
using XelsDesktopWalletApp.Models;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for CreateShowMnemonic.xaml
    /// </summary>
    public partial class CreateShowMnemonic : Window
    {

        #region Show Mnemonic Property

        WalletCreation _walletcreate = new WalletCreation();


        //private string _mnemonic;
        //public string _Mnemonic
        //{
        //    get { return _mnemonic; }
        //    set { _mnemonic = value; }
        //}

        #endregion


        public CreateShowMnemonic()
        {
            InitializeComponent();
        }

        
        public CreateShowMnemonic(WalletCreation model)
        {
            InitializeComponent();
            textBoxTextToMnemonic.Text = model.mnemonic;

            InitializeWalletCreationModel(model);
        }

        private void InitializeWalletCreationModel(WalletCreation cr)
        {
            _walletcreate.name = cr.name;
            _walletcreate.passphrase = cr.passphrase;
            _walletcreate.password = cr.password;
            _walletcreate.mnemonic = cr.mnemonic;
        }


        private void copyClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBoxTextToMnemonic.Text);
        }

        private void continueButton_Click(object sender, RoutedEventArgs e)
        {
            CreateConfirmMnemonic ccm = new CreateConfirmMnemonic(_walletcreate);
            ccm.Show();
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Create create = new Create();
            create.Show();
            this.Close();
        }
    }
}
