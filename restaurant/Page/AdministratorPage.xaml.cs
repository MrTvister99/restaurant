using restaurant.Window;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;


using static System.Net.Mime.MediaTypeNames;

namespace restaurant.Page
{

    public partial class AdministratorPage : System.Windows.Controls.Page
    {
        public int i;
        public static ObservableCollection<application> TowarList = new ObservableCollection<application>();
        public ObservableCollection<string> StatusOptions { get; set; }
        public string Name1;
        string server = "DESKTOP-9MU0DUB";
        string database = "restaurant";
        string username = "MrTv";
        string passwordDB = "1";

        
        List<string> values;
        public AdministratorPage()
        {
            InitializeComponent();
            podkl();
            StatusOptions = new ObservableCollection<string>
        {
            "Ожидает",
            "Готовится",
            "Отклонен",
            "Готов",

        };
            comboBoxStatus.ItemsSource = StatusOptions;

        }

        private void Add(object sender, RoutedEventArgs e)
        {
            AddPage addPage = new AddPage();
            addPage.Show();
        }
        private void podkl1()
        {
            TowarList.Clear();
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();
                string sql = "SELECT m.Id_Product, m.NameProduct, m.Price, m.Type, m.Info, o.DataAdd, o.Status, o.Login " +
                              "FROM Orders o " +
                              "JOIN Menu m ON o.Id_Orders = m.Id_Product";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            application app = new application
                            {
                               // Id_Product = reader["Id_Product"].ToString(),
                                ProductName = reader["NameProduct"].ToString(),
                                Type = reader["Type"].ToString(),
                                Price = reader["Price"].ToString(),
                                Info = reader["Info"].ToString(),
                                Login = reader["Login"].ToString(),
                                DateAdd = reader["DataAdd"].ToString(),
                                Status = reader["Status"].ToString()
                            };
                            TowarList.Add(app);
                        }
                    }
                }
            }
            TowarListView.ItemsSource = TowarList;
        }
        private void podkl()
        {
            TowarList.Clear(); // Очищаем список перед заполнением

            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();

                string sql1 = "SELECT * FROM Menu";

                using (SqlCommand command = new SqlCommand(sql1, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            application app = new application
                            {
                                ProductName = reader["NameProduct"].ToString(),
                               
                                Type = reader["Type"].ToString(),
                                Price = reader["Price"].ToString(),
                                Info = reader["Info"].ToString()
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("Image")))
                            {
                                byte[] imageData = (byte[])reader["Image"];
                                using (MemoryStream stream = new MemoryStream(imageData))
                                {
                                    BitmapImage bitmap = new BitmapImage();
                                    bitmap.BeginInit();
                                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                    bitmap.StreamSource = stream;
                                    bitmap.EndInit();
                                    bitmap.Freeze(); // Это необходимо для использования BitmapImage в другом потоке

                                    app.Image = bitmap;
                                }
                            }

                            TowarList.Add(app);
                        }
                    }
                }

            }

            TowarListView.ItemsSource = TowarList;

            var uniqueEquipments = TowarList.Select(t => t.Type).Distinct().ToList();
            uniqueEquipments.Insert(0, "Все");

            comboBoxEquipment.ItemsSource = uniqueEquipments;
            comboBoxEquipment.SelectionChanged += ComboBoxEquipment;
        }

        public void ExportToExcel(object sender, RoutedEventArgs e)
        {
            //// Создание нового Excel-файла
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "DateAdd";
                worksheet.Cell(currentRow, 2).Value = "ProductName";
                worksheet.Cell(currentRow, 3).Value = "Price";
                worksheet.Cell(currentRow, 4).Value = "Type";
                worksheet.Cell(currentRow, 5).Value = "Info";
                worksheet.Cell(currentRow, 6).Value = "Status";
                foreach (var item in TowarList)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.DateAdd;
                    worksheet.Cell(currentRow, 2).Value = item.ProductName;
                    worksheet.Cell(currentRow, 3).Value = item.Price;
                    worksheet.Cell(currentRow, 4).Value = item.Type;
                    worksheet.Cell(currentRow, 5).Value = item.Info;
                    worksheet.Cell(currentRow, 6).Value = item.Status;
                }

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string filePath = System.IO.Path.Combine(desktopPath, "Orders.xlsx");
                workbook.SaveAs(filePath);
                Console.WriteLine($"Данные экспортированы в файл: {filePath}");
            }
        }
        private void EditClick(object sender, RoutedEventArgs e)
        {
            //if (TowarListView.SelectedItem is application selectedApplication)
            //{
            //    // Открываем окно редактирования с выбранным элементом
            //    var window = new WindowEdit(selectedApplication);
            //    bool? result = window.ShowDialog();

            //    if (result == true)
            //    {
            //        EditApplicationInDatabase(selectedApplication);
            //        if (TowarListView.ItemsSource is ObservableCollection<application> collection)
            //        {
            //            collection.Clear();
            //        }
            //        podkl();
            //    }
            //}
            //else
            //{
            //    // Обработка случая, когда выбранный элемент не существует
            //    MessageBox.Show("Выберите элемент для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
        private void DeleteClick(object sender, RoutedEventArgs e)
        {

            if (TowarListView.SelectedItem is application selectedApplication)
            {

                if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Удаляем запись из базы данных и из списка
                    RemoveApplication(selectedApplication);
                }
            }
        }

        private void ComboBoxEquipment(object sender, SelectionChangedEventArgs e)
        {

            string selectedEquipment = comboBoxEquipment.SelectedItem as string;


            if (selectedEquipment == "Все")
            {
                TowarListView.ItemsSource = TowarList;
            }
            else
            {

                var filteredList = TowarList.Where(t => t.Type == selectedEquipment).ToList();

                TowarListView.ItemsSource = filteredList;
            }
        }
        private void RemoveApplication(application app)
        {

            DeleteApplicationFromDatabase(app);


            TowarList.Remove(app);

            // Обновляем источник данных для обновления списка в интерфейсе
            TowarListView.ItemsSource = null;
            TowarListView.ItemsSource = TowarList;
        }
        private void DeleteApplicationFromDatabase(application app)
        {
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();
                string sql = "DELETE FROM Menu WHERE NameProduct  = @NameProduct";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@NameProduct", app.ProductName);
                    command.ExecuteNonQuery();
                }
            }
        }
        private void myTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = myTextBox1.Text;

            var filteredItems = TowarList.Where(item => item.ProductName.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);

            TowarListView.ItemsSource = filteredItems;
        }
     

        private async void ComboBoxStatus(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(500);
            string selectedStatus = comboBoxStatus.Text;


            application selectedApplication = TowarListView.SelectedItem as application;

            if (selectedApplication != null && selectedStatus != null)
            {

                UpdateStatusInDatabase(selectedApplication, selectedStatus);
                selectedApplication.Status = selectedStatus;
                TowarListView.Items.Refresh();
            }
        }
        private void UpdateStatusInDatabase(application selectedApplication, string Selectionstatus)
        {
            string newStatus = Selectionstatus;

            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();

                string sql = "UPDATE Orders SET Status = @Status WHERE DataAdd = @DataAdd";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Status", newStatus);
                    command.Parameters.AddWithValue("@DataAdd", selectedApplication.DateAdd);
                    // command.Parameters.AddWithValue("@NameClient", selectedApplication.UsersFIO);

                    command.ExecuteNonQuery();
                }
            }
        }
        private void OldCheckBox_Checked(object sender, RoutedEventArgs e)
        {

            ApplyFilter(true);
        }

        private void OldCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyFilter(false);
        }

        private void NewCheckBox_Checked(object sender, RoutedEventArgs e)
        {

            ApplyFilter(false);
        }

        private void NewCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyFilter(true);
        }
        private void ApplyFilter(bool isOld)
        {
            if (isOld)
            {
                var sortedList = new ObservableCollection<application>(TowarList.OrderByDescending(a => DateTime.Parse(a.DateAdd)));
                TowarListView.ItemsSource = sortedList;
            }
            else
            {
                var sortedList = new ObservableCollection<application>(TowarList.OrderBy(a => DateTime.Parse(a.DateAdd)));
                TowarListView.ItemsSource = sortedList;
            }
        }
        public class application
        {
            
            public string DateAdd { get; set; }
            public string ProductName { get; set; }
            public string Price { get; set; }
            public string Type { get; set; }
            public string Login { get; set; }
            public BitmapImage Image { get; set; }
            public string Info { get; set; }
            public string Status { get; set; }
            public string Void { get; set; }
        }


        private async void Menu(object sender, RoutedEventArgs e)
        {
            podkl();
            await Task.Delay(TimeSpan.FromSeconds(1));
            ResetGridToXaml();
        }

        private async void Order(object sender, RoutedEventArgs e)
        {
            podkl1();
            await Task.Delay(TimeSpan.FromSeconds(1));
            UpdateGridOrder();
        }
        private void UpdateGridOrder()
        {

            gridView.Columns[0].Header = "Дата заказа";
            gridView.Columns[1].Header = "Название";
            gridView.Columns[2].Header = "Цена";
            gridView.Columns[3].Header = "Тип";
            gridView.Columns[4].Header = "Информация";
            gridView.Columns[5].Header = "Статус";
            gridView.Columns[6].Header = "Логин";

            gridView.Columns[0].DisplayMemberBinding = new Binding("DateAdd");
            gridView.Columns[1].DisplayMemberBinding = new Binding("ProductName");
            gridView.Columns[2].DisplayMemberBinding = new Binding("Price");
            gridView.Columns[3].DisplayMemberBinding = new Binding("Type");
            gridView.Columns[4].DisplayMemberBinding = new Binding("Info");
            gridView.Columns[5].DisplayMemberBinding = new Binding("Status");
            gridView.Columns[6].DisplayMemberBinding = new Binding("Login");
        }


        private void ResetGridToXaml()
        {
            gridView.Columns[0].Header = "Название";
            gridView.Columns[1].Header = "Цена";
            gridView.Columns[2].Header = "Тип";
            gridView.Columns[2].Header = "Изображение";

            gridView.Columns[4].Header = "";
            gridView.Columns[5].Header = "";
            gridView.Columns[5].Header = "";

            gridView.Columns[0].DisplayMemberBinding = new Binding("ProductName");
            gridView.Columns[1].DisplayMemberBinding = new Binding("Price");
            gridView.Columns[2].DisplayMemberBinding = new Binding("Type");
            gridView.Columns[3].DisplayMemberBinding = new Binding("Image");

            gridView.Columns[4].DisplayMemberBinding = new Binding("");
            gridView.Columns[5].DisplayMemberBinding = new Binding("");
            gridView.Columns[6].DisplayMemberBinding = new Binding("");

        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EnterPage());
        }
    }
}
