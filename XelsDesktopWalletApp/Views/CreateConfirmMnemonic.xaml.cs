using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
        string[] words;
        int[] randomidx = new int[3];

        public CreateConfirmMnemonic()
        {
            InitializeComponent();
        }
        public CreateConfirmMnemonic(WalletCreation walletcreation)
        {
            InitializeComponent();
            InitializeWalletCreationModel(walletcreation);
            RandomSelect();
        }

        private void InitializeWalletCreationModel(WalletCreation cr)
        {
            _walletcreateconfirm.name = cr.name;
            _walletcreateconfirm.passphrase = cr.passphrase;
            _walletcreateconfirm.password = cr.password;
            _walletcreateconfirm.mnemonic = cr.mnemonic;
        }

        #region field property 
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private string valueone;
        public string Valueone
        {
            get { return valueone; }
            set { valueone = value; OnPropertyChanged("Valueone"); }
        }

        private string valuetwo;
        public string Valuetwo
        {
            get { return valuetwo; }
            set { valuetwo = value; OnPropertyChanged("Valuetwo"); }
        }

        private string valuethree;


        public string Valuethree
        {
            get { return valuethree; }
            set { valuethree = value; OnPropertyChanged("Valuethree"); }
        }
        #endregion


        private void RandomSelect()
        {
            //// Initialize array to check
            ///
            string[] rowwords = _walletcreateconfirm.mnemonic.Split('\"');
            _walletcreateconfirm.mnemonic = rowwords[1];
            words = rowwords[1].Split(' ');

            //// Random number select
            for (int i = 0; i < 3; i++)
            {
                var idx = new Random().Next(words.Length);

                if (!randomidx.Contains(idx))
                {
                    randomidx[i] = idx;
                }
            }
            int fInd = randomidx[0] + 1;
            int sInd = randomidx[1] + 1;
            int tInd = randomidx[2] + 1;
            valueone = "Word number " + fInd;
            valuetwo = "Word number " + sInd;
            valuethree = "Word number " + tInd;

            wordone.Text = valueone;
            wordtwo.Text = valuetwo;
            wordthree.Text = valuethree;

        }


        public void CheckMnemonic()
        {
            string firstword = words[randomidx[0]];
            string secondword = words[randomidx[1]];
            string thirdword = words[randomidx[2]];

            //// Check for validation
            if (_walletcreateconfirm.mnemonic != "" && word1.Text == firstword &&
                word2.Text == secondword && word3.Text == thirdword)
            {
                this.canPassMnemonic = true;
            }
            else
            {
                MessageBox.Show("Secret words do not match!");
            }
        }


        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Create cr = new Create();
            cr.Show();
            this.Close();
        }

        private async void createButton_Click(object sender, RoutedEventArgs e)
        {
            CheckMnemonic();

            if (this.canPassMnemonic == true)
            {

                string postUrl = this.baseURL + "/create";

                HttpResponseMessage response = await client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(this._walletcreateconfirm), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Successfully created wallet with Name: " + this._walletcreateconfirm.name);

                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }


            }

        }


    }
}
