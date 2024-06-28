using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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

namespace restaurant.Window
{
    public partial class AddPage : System.Windows.Window
    {
        string server = "DESKTOP-9MU0DUB";
        string database = "restaurant";
        string username = "MrTv";
        string passwordDB = "1";
        private string selectedImagePath;
        public AddPage()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtProduct.Text) &&
                !string.IsNullOrWhiteSpace(txtPrice.Text) &&
                !string.IsNullOrWhiteSpace(txtInformation.Text))
            {
                if (string.IsNullOrWhiteSpace(selectedImagePath))
                {
                    MessageBox.Show("Не удалось определить путь к изображению.");
                    return;
                }

                byte[] imageBytes;
                try
                {
                    imageBytes = GetImageBytes(selectedImagePath);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
                {
                    connection.Open();

                    string sql = "INSERT INTO Menu " +
                                 "(Image, NameProduct, Price, Type, Info) " +
                                 "VALUES (@Image, @NameProduct, @Price, @Type, @Info)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@NameProduct", txtProduct.Text);
                        command.Parameters.AddWithValue("@Price", txtPrice.Text);
                        command.Parameters.AddWithValue("@Type", ProductType.Text); // Убедитесь, что это правильно
                        command.Parameters.AddWithValue("@Info", txtInformation.Text);
                        command.Parameters.AddWithValue("@Image", imageBytes);

                        try
                        {
                            command.ExecuteNonQuery();
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Поля не должны быть пустыми");
            }
        }

        private string GetImagePathFromUIElement(UIElement element)
        {
            // Здесь вам нужно реализовать логику получения пути к файлу из элемента UI
            // Например, проверяя свойства элемента, если это возможно
            return null;
        }

        private byte[] GetImageBytes(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                throw new ArgumentException("Путь к изображению не существует.", nameof(imagePath));
            }

            using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                byte[] imageBytes = new byte[fs.Length];
                fs.Read(imageBytes, 0, imageBytes.Length);
                return imageBytes;
            }
        }
        private void OpenImageDialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                selectedImagePath = openFileDialog.FileName; // Сохраняем путь к файлу

                // Создаем BitmapImage и устанавливаем Source для Image
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedImagePath);
                bitmap.DecodePixelWidth = 200; // Устанавливаем ширину изображения в пикселях
                bitmap.EndInit();

                // Устанавливаем Source и размеры для Image
                imgProductImage.Source = bitmap;
                imgProductImage.Width = 200; // Устанавливаем желаемую ширину
                imgProductImage.Height = 200; // Устанавливаем желаемую высоту
            }
        }
    }
}

    