using System;
using System.IO;
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
    public partial class CaptchaWindow : Window
    {
        private string captchaText;

        public CaptchaWindow(MemoryStream captchaImageStream, string captchaText)
        {
            InitializeComponent();
            this.captchaText = captchaText;

            // Загружаем изображение капчи
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = captchaImageStream;
            bitmapImage.EndInit();
            CaptchaImage.Source = bitmapImage;
        }

        private void Button_Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (CaptchaInput.Text.Equals(captchaText, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Капча пройдена!");
                this.DialogResult = true; //успешная проверка капчи
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный текст капчи. Попробуйте снова.");
                CaptchaInput.Clear();
            }
        }
    }
}
