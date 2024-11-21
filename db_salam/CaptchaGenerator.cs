using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

public class CaptchaGenerator
{
    private static Random random = new Random();
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static MemoryStream GenerateCaptchaImage(out string captchaText)
    {
        int length = random.Next(4, 8); // длина текста капчи от 4 до 7
        captchaText = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        // увеличим размер капчи, чтобы избежать обрезания символов
        Bitmap bitmap = new Bitmap(400, 100);
        Graphics g = Graphics.FromImage(bitmap);

        // задаем фон
        g.Clear(Color.FromArgb(255, 255, 255)); // белый фон
        for (int i = 0; i < 500; i++)
        {
            int x = random.Next(0, bitmap.Width);
            int y = random.Next(0, bitmap.Height);
            bitmap.SetPixel(x, y, Color.FromArgb(random.Next(200), random.Next(200), random.Next(200))); // меньше шум
        }

        int startX = 20; // начальная позиция по X
        int startY = 25; // начальная позиция по Y (изменено)

        // Генерируем текст капчи
        for (int i = 0; i < captchaText.Length; i++)
        {
            float angle = random.Next(-15, 15); // угол наклона
            Font font = new Font("Arial", random.Next(30, 40), FontStyle.Bold); // размер шрифта увеличен
            Brush brush = new SolidBrush(Color.FromArgb(random.Next(0, 150), random.Next(0, 150), random.Next(0, 150))); // цвет текста

            // случайное смещение по X для каждого символа
            startX += random.Next(40, 50); // увеличиваем расстояние между символами
            g.TranslateTransform(startX, startY);
            g.RotateTransform(angle);
            g.DrawString(captchaText[i].ToString(), font, brush, new PointF(0, 0));
            g.ResetTransform();
        }

        // добавляем линии для усложнения
        for (int i = 0; i < 5; i++)
        {
            g.DrawLine(new Pen(Color.FromArgb(random.Next(100, 150), random.Next(100, 150), random.Next(100, 150)), 2),
                random.Next(0, bitmap.Width), random.Next(0, bitmap.Height),
                random.Next(0, bitmap.Width), random.Next(0, bitmap.Height));
        }

        MemoryStream ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Png);
        g.Dispose();
        bitmap.Dispose();

        ms.Position = 0;
        return ms;
    }
}