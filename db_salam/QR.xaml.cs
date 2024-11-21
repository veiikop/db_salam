using QRCoder.Xaml;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace db_salam
{
    /// <summary>
    /// Логика взаимодействия для QR.xaml
    /// </summary>
    public partial class QR : Window
    {
        public QR()
        {
            InitializeComponent();
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(inputBox.Text, QRCodeGenerator.ECCLevel.H);
            XamlQRCode qrCode = new XamlQRCode(qrCodeData);

            string foregroundColor = "#FF69B4"; // цвет qr-кода
            string backgroundColor = "#FFFFFF"; // цвет фона qr-кода

            DrawingImage qrCodeAsXaml = qrCode.GetGraphic(20, foregroundColor, backgroundColor);
            qrImage.Source = qrCodeAsXaml;
        }
    }
}
