using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace restaurant.Page
{
    /// <summary>
    /// Логика взаимодействия для EnterPage.xaml
    /// </summary>
    public partial class EnterPage : System.Windows.Controls.Page
    {
        string server = "DESKTOP-9MU0DUB";
        string database = "restaurant";
        string username = "MrTv";
        string passwordDB = "1";
        public string email1;
        public string password1;
        public static string FIO1;
        private string roleClient;
        public static string login;
        public EnterPage()
        {
            InitializeComponent();
        }
        private void btnEnt(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.NavigateToRegistritionPage();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            podkl();
        }
        private void podkl()
        {

            using (SqlConnection connection = new SqlConnection($"Server={server};Database={database};User ID={username};Password={passwordDB}"))
            {
                connection.Open();

                string sql = "SELECT * FROM clients";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        bool found = false;
                        while (reader.Read())
                        {
                            email1 = reader["Login"].ToString();
                            password1 = reader["password"].ToString();
                            password1 = password1.Trim();
                            email1 = email1.Trim();
                            if (password1 == Password.Password.ToString() && email.Text == email1)
                            {
                                found = true;
                                roleClient = reader["User_IDROLE"].ToString();
                            }
                        }
                        if (Password.Password.ToString() != "" && email.Text != "")
                        {
                            if (found)
                            {

                                if (roleClient == "2")
                                {
                                    Registration.loginUser = email.Text;
                                    NavigationService.Navigate(new AdministratorPage());
                                }
                                else
                                {
                                    Registration.loginUser = email.Text;
                                    NavigationService.Navigate(new ClientPage());
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Неправильный логин или пароль");
                            }
                        }

                        else
                        {
                            MessageBox.Show($"Поля не могут быть пустыми");
                        }

                    }
                }
            }
        }
    }
}
