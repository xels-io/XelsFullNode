using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class Create : Window
    {

        //static HttpClient client = new HttpClient();
        string baseURL = URLConfiguration.BaseURL; //"http://localhost:37221/api/wallet";
        string _mnemonic;
        bool canProceedPass = false;

        public Create()
        {
            InitializeComponent();
            LoadCreate();
        }

        public async void LoadCreate()
        {
            this._mnemonic = await GetAPIAsync(this.baseURL);
        }

        public bool isValid()
        {
            if (name.Text == string.Empty)
            {
                MessageBox.Show("Name is required!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                name.Focus();
                return false;
            }

            if (name.Text.Length < 1 || name.Text.Length > 24)
            {
                MessageBox.Show("Name should be 1 to 24 characters long");
                name.Focus();
                return false;
            }

            // Name: /^[a-zA-Z0-9]*$/
            if (!Regex.IsMatch(name.Text, @"^[a-zA-Z0-9]*$"))
            {
                MessageBox.Show("Please enter a valid wallet name. [a-Z] and [0-9] are the only characters allowed.");
                name.Focus();
                return false;
            }

            if (password.Password == "")
            {
                MessageBox.Show("Password field is required!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                password.Focus();
                return false;
            }

            if ( repassword.Password == "")
            {
                MessageBox.Show("Confirm password field is required!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                repassword.Focus();
                return false;
            }

            // Password:  /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!#$%&'()*+,-./:;<=>?@[\]^_`{|}~])[A-Za-z\d!#$%&'()*+,-./:;<=>?@[\]^_`{|}~]{8,}$/
            if (!Regex.IsMatch(password.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!#$%&'()*+,-./:;<=>?@[\]^_`{|}~])[A-Za-z\d!#$%&'()*+,-./:;<=>?@[\]^_`{|}~]{8,}$"))
            {
                MessageBox.Show("A password must contain at least one uppercase letter, one lowercase letter, one number and one special character.");

                password.Focus();
                return false;
            }

            if (password.Password.Length < 8 )
            {
                MessageBox.Show("A password should be at least 8 characters long");
                password.Focus();
                return false;
            }

            return true;
        }

        public void CheckPassInput()
        {
            if (password.Password == repassword.Password)
            {
                canProceedPass = true;
            }
            else
            {
                MessageBox.Show("The two passwords must match!");
            }
        }

        private void Content_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!name.Text.Equals(String.Empty) && password.Password.Equals(repassword.Password))
                createButton.IsEnabled = true;
            else
                createButton.IsEnabled = false;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            CreateOrRestore cr = new CreateOrRestore();
            cr.Show();
            this.Close();
        }


        private async Task<string> GetAPIAsync(string path)
        {
            string getUrl = path + "/mnemonic?language=English&wordCount=12";
            var content ="";

            HttpResponseMessage response = await URLConfiguration.Client.GetAsync(getUrl);
            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
            return content;
        }


        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            if (isValid())
            {
                CheckPassInput();

                if (canProceedPass == true) {

                    WalletCreation creation = new WalletCreation();
                    creation.name = name.Text;
                    creation.password = password.Password;
                    creation.passphrase = passphrase.Text;
                    creation.mnemonic = _mnemonic;

                    CreateShowMnemonic csm = new CreateShowMnemonic(creation);
                    csm.Show();
                    this.Close();
                }

            }

        }



    }
}
