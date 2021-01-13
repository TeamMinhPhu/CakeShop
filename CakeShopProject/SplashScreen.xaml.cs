using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace CakeShopProject
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var value = ConfigurationManager.AppSettings["ShowSplashScreen"];
            var showSplash = bool.Parse(value);

            if (showSplash == false)
            {
                var screen = new MainWindow();
                screen.Show();

                this.Close();
            }
            else
            {
                var folder = AppDomain.CurrentDomain.BaseDirectory;
                var filepath = $"{folder}Resources\\Data\\facts.txt";
                var quotes = File.ReadAllLines(filepath).ToList(); 

                Random rng = new Random();
                int index = rng.Next(0, quotes.Count);
                string quote = quotes[index];
                randomQuote.Text = quote;

                timer = new System.Timers.Timer();
                timer.Elapsed += Timer_Elapsed;
                timer.Interval = 10;
                timer.Start();
            }
        }

        System.Timers.Timer timer;
        int count = 0;
        int target = 500;

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            count++;
            if (count == target)
            {
                timer.Stop();


                Dispatcher.Invoke(() =>
                {
                    var screen = new MainWindow();
                    screen.Show();

                    this.Close();
                });

            }

            Dispatcher.Invoke(() =>
            {
                progress.Value = count;
            });
        }

        private void turnOffButton_Click(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(
            ConfigurationUserLevel.None);
            config.AppSettings.Settings["ShowSplashScreen"].Value = "false";
            config.Save(ConfigurationSaveMode.Minimal);
            var screen = new MainWindow();
            screen.Show();
            timer.Stop();
            this.Close();
        }
    }
}
