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
using XelsDesktopWalletApp.Models.CommonModels;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for CreateConfirmMnemonic.xaml
    /// </summary>
    public partial class CreateConfirmMnemonic : Window
    {
        //static HttpClient client = new HttpClient();
        string baseURL = URLConfiguration.BaseURL;//"http://localhost:37221/api/wallet";
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
            this._walletcreateconfirm.name = cr.name;
            this._walletcreateconfirm.passphrase = cr.passphrase;
            this._walletcreateconfirm.password = cr.password;
            this._walletcreateconfirm.mnemonic = cr.mnemonic;
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
            string[] rowwords = this._walletcreateconfirm.mnemonic.Split('\"');
            this._walletcreateconfirm.mnemonic = rowwords[1];
            this.words = rowwords[1].Split(' ');

            //// Random number select
            for (int i = 0; i < 3; i++)
            {
                var idx = new Random().Next(this.words.Length);

                if (!this.randomidx.Contains(idx))
                {
                    this.randomidx[i] = idx;
                }
            }
            int fInd = this.randomidx[0] + 1;
            int sInd = this.randomidx[1] + 1;
            int tInd = this.randomidx[2] + 1;
            this.valueone = "Word number " + fInd;
            this.valuetwo = "Word number " + sInd;
            this.valuethree = "Word number " + tInd;
            
            this.wordone.Text = this.valueone;
            this.wordtwo.Text = this.valuetwo;
            this.wordthree.Text = this.valuethree;

        }


        public void CheckMnemonic()
        {
            string firstword = this.words[this.randomidx[0]];
            string secondword = this.words[this.randomidx[1]];
            string thirdword = this.words[this.randomidx[2]];

            //// Check for validation
            if (this._walletcreateconfirm.mnemonic != "" && this.word1.Text == firstword &&
                this.word2.Text == secondword && this.word3.Text == thirdword)
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

                HttpResponseMessage response = await URLConfiguration.Client.PostAsync(postUrl, new StringContent(JsonConvert.SerializeObject(this._walletcreateconfirm), Encoding.UTF8, "application/json"));

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
