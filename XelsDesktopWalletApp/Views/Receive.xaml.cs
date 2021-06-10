using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XelsDesktopWalletApp.Models;

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for Receive.xaml
    /// </summary>
    public partial class Receive : Window
    {

        static HttpClient client = new HttpClient();
        string baseURL = "http://localhost:37221/api";

        private readonly WalletInfo walletInfo = new WalletInfo();

        private string walletName;
        public string WalletName
        {
            get
            {
                return this.walletName;
            }
            set
            {
                this.walletName = value;
            }
        }

        public Receive()
        {
            InitializeComponent();
            generateQRCode();
        }

        public Receive(string walletname)
        {
            InitializeComponent();
            this.DataContext = this;

            this.walletName = walletname;
            this.walletInfo.walletName = this.walletName;
            generateQRCode();
            LoadCreate();
        }


        public async void LoadCreate()
        {
            string addr = await GetUnusedReceivedAddrAPIAsync(this.baseURL);
            addr = FreshAddress(addr);

            this.textBoxTextToQr.Text = addr;
        }


        private string FreshAddress(string adr)
        {
            adr = adr.Trim(new char[] { '"' });
            return adr;
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            Dashboard db = new Dashboard();
            db.Show();
            this.Close();
        }


        private void generateQRCode()
        {
            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            //QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);
            QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(this.textBoxTextToQr.Text, QRCodeGenerator.ECCLevel.H);
            QRCode qrCode = new QRCode(qRCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            this.image.Source = BitmapToImageSource(qrCodeImage);

            //this.textBoxTextToQr.Text
        }

        private ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ReceiveShowall rsa = new ReceiveShowall(this.walletName);
            rsa.Show();
            this.Close();
        }

        private void textBoxTextToQr_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.textBoxTextToQr.Text);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {

            Dashboard ds = new Dashboard(this.walletName);
            ds.Show();
            this.Close();
        }

        private async Task<string> GetUnusedReceivedAddrAPIAsync(string path)
        {
            string getUrl = path + $"/wallet/unusedaddress?WalletName={this.walletInfo.walletName}&AccountName=account 0";
            var content = "";

            HttpResponseMessage response = await client.GetAsync(getUrl);


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
        private void xelsButton_Click(object sender, RoutedEventArgs e)
        {
            Receive r = new Receive(this.walletName);
            r.Show();
            this.Close();
        }
        private void selsButton_Click(object sender, RoutedEventArgs e)
        {
            //TextBlock tb = (TextBlock)this.spnael.FindName("showall");
            //this.spnael.Children.Remove(tb);
            ReceiveSelsBels rsb = new ReceiveSelsBels(this.walletName);
            rsb.Show();
            this.Close();
        }
        private void belsButton_Click(object sender, RoutedEventArgs e)
        {
            ReceiveSelsBels rsb = new ReceiveSelsBels(this.walletName);
            rsb.Show();
            this.Close();
        }



    }
}
