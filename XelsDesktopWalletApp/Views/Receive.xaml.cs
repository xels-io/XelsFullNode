﻿using Newtonsoft.Json;
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

namespace XelsDesktopWalletApp.Views
{
    /// <summary>
    /// Interaction logic for Receive.xaml
    /// </summary>
    public partial class Receive : Window
    {
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
            generateQRCode();
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

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            Dashboard db = new Dashboard();
            db.Show();
            this.Close();
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


    }
}
