﻿using restaurant.Page;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace restaurant
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            frame.Navigate(new Registration());
        }
        public void NavigateToLoginPage()
        {
            frame.NavigationService.Navigate(new EnterPage());
        }
        public void NavigateToRegistritionPage()
        {
            frame.NavigationService.Navigate(new Registration());
        }
    }
}
