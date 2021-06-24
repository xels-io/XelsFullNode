﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xels.Bitcoin.Features.Wallet.Interfaces;
using XelsDesktopWalletApp.Models;
using XelsDesktopWalletApp.Models.CommonModels;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for Restore.xaml
    /// </summary>
    public partial class Restore : Window
    {

        //private readonly IWalletManager _walletManager;
        static HttpClient client = new HttpClient();
        string baseURL = URLConfiguration.BaseURL;  // "http://localhost:37221/api/wallet";



        public Restore()
        {
            InitializeComponent();
        }

        //public Restore(IWalletManager walletManager)
        //{
        //    this._walletManager = walletManager;

        //    InitializeComponent();
        //}


        public bool isValid()
        {
            if (name.Text == string.Empty)
            {
                MessageBox.Show("Name is required!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (mnemonic.Text == string.Empty)
            {
                MessageBox.Show("Field is required!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (creationDate.Text == string.Empty)
            {
                MessageBox.Show("Date is required!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (password.Text == string.Empty)
            {
                MessageBox.Show("Password is required!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (passphrase.Text == string.Empty)
            {
                MessageBox.Show("Passphrase is required!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }



        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            CreateOrRestore cr = new CreateOrRestore();
            cr.Show();
            this.Close();
        }

        private async void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (isValid())
            {
                string postUrl = baseURL + "/Wallet/recover";

                WalletRecovery recovery = new WalletRecovery();
                recovery.name = name.Text;
                //recovery.creationDate = creationDate.SelectedDate.Value;
                recovery.creationDate = creationDate.Text;
                recovery.mnemonic = mnemonic.Text;
                recovery.passphrase = passphrase.Text;
                recovery.password = password.Text;

                //this._walletManager.RecoverWallet(recovery.password, recovery.name, recovery.mnemonic,
                //        recovery.creationDate, passphrase: recovery.passphrase);


                HttpResponseMessage response = await client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(recovery), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Successfully saved data with Name: " + recovery.name);
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }
            }

        }



    }
}
