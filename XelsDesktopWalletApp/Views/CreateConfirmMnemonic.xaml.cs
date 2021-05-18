using Newtonsoft.Json;
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
using XelsDesktopWalletApp.Models;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for CreateConfirmMnemonic.xaml
    /// </summary>
    public partial class CreateConfirmMnemonic : Window
    {
        static HttpClient client = new HttpClient();
        string baseURL = "http://localhost:37221/api/wallet";
        WalletCreation _walletcreateconfirm = new WalletCreation();
        private bool canPassMnemonic = false;

        public CreateConfirmMnemonic()
        {
            InitializeComponent();
        }
        public CreateConfirmMnemonic(WalletCreation walletcreation)
        {
            InitializeComponent();
            InitializeWalletCreationModel(walletcreation);
        }

        private void InitializeWalletCreationModel(WalletCreation cr)
        {
            _walletcreateconfirm.name = cr.name;
            _walletcreateconfirm.passphrase = cr.passphrase;
            _walletcreateconfirm.password = cr.password;
            _walletcreateconfirm.mnemonic = cr.mnemonic;
        }

        public void CheckMnemonic()
        {
            //// Initialize array to check
            string[] words = _walletcreateconfirm.mnemonic.Split(' '); // words[0] = mnemonic no 1, words[1] = mnemonic no 2, words[2] = mnemonic no 3...


            //// Random number select
            //string[] arr1 = new string[] { "one", "two", "three" };
            //var idx = new Random().Next(arr1.Length);
            //return arr1[idx];


            //// Check for validation
            if (_walletcreateconfirm.mnemonic != "" )
            {
                canPassMnemonic = true;
            }
            else
            {
                MessageBox.Show("Secret words do not match!");
            }
        }


        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            CreateShowMnemonic csm = new CreateShowMnemonic();
            csm.Show();
            this.Close();
        }

        private async void createButton_Click(object sender, RoutedEventArgs e)
        {
            CheckMnemonic();

            if (canPassMnemonic == true)
            {
                string postUrl = baseURL + "/create";

                HttpResponseMessage response = await client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(_walletcreateconfirm), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Successfully created wallet with Name: " + _walletcreateconfirm.name);
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }
            }

        }


    }
}
