using System;
using System.IO;
using System.Data.SqlClient;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace db_salam
{
    public partial class AddImageWindow : Window
    {
        public AddImageWindow()
        {
            InitializeComponent();
        }

        // Обработчик для выбора изображения
        private void SelectImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"; // Разрешенные форматы изображений

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                FilePathTextBox.Text = filePath; // Отображаем путь в TextBox

                // Загружаем изображение в Image
                BitmapImage bitmapImage = new BitmapImage(new Uri(filePath));
                image.Source = bitmapImage;
            }
        }

        // Обработчик для загрузки изображения в базу данных
        private void UploadImage(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("Выберите изображение для загрузки.");
                return;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл не найден.");
                return;
            }

            // Конвертируем изображение в байты
            byte[] imageBytes = File.ReadAllBytes(filePath);

            // Сохраняем изображение в базу данных
            try
            {
                string connectionString = @"Server=WIN-N1U0GT2AIVJ\SQL2023;Database=salam;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO ImageTable (ImageData) VALUES (@ImageData)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ImageData", imageBytes);
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Изображение успешно загружено в базу данных.");
                this.Close(); // Закрываем окно после загрузки
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке изображения в базу данных: {ex.Message}");
            }
        }
    }
}