using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CakeShopProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			MouseDown += Window_MouseDown; //drag window
			MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
			MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

			this.content.Navigate(new CakePage());
		}

		
		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (e.ChangedButton == MouseButton.Left)
					DragMove();
			}
			catch
			{
				// do nothing
			}
		}


		#region "Title Bar Buttons"
		//close
		private void closeProgramButton_Click(object sender, RoutedEventArgs e)
		{
			saveConfig();
			Application.Current.Shutdown();
		}
		//maximize - unmaximize
		private void maximizeProgramButton_Click(object sender, RoutedEventArgs e)
		{
			if (WindowState != WindowState.Maximized)
			{
				WindowState = WindowState.Maximized;
				var Bitmap = new BitmapImage(new Uri("Resources/Icons/unmaximize.png", UriKind.Relative));
				maximizeButtonImage.Source = Bitmap;
			}
			else
			{
				WindowState = WindowState.Normal;
				var Bitmap = new BitmapImage(new Uri("Resources/Icons/maximize.png", UriKind.Relative));
				maximizeButtonImage.Source = Bitmap;
			}
		}
		//minimize
		private void minimizeProgramButton_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}
		#endregion
		private void loadConfig()
		{
			ConfigurationManager.RefreshSection("appSettings");
			var configWidth = ConfigurationManager.AppSettings["Width"];
			this.Width = double.Parse(configWidth);
			var configHeight = ConfigurationManager.AppSettings["Height"];
			this.Height = double.Parse(configHeight);
		}
		private void saveConfig()
		{
			WindowState = WindowState.Normal;
			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			config.AppSettings.Settings["Width"].Value = this.Width.ToString();
			config.AppSettings.Settings["Height"].Value = this.Height.ToString();
			config.Save(ConfigurationSaveMode.Minimal);
		}
		private void ShowMenuHandler()
		{
			menuBar.Visibility = Visibility.Visible;
		}

		#region "Navigate"
		private void infoButton_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.content.Navigate(new InfoPage());
		}
		private void settingsButton_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Splash.Visibility = Visibility.Visible;
			var settings = new SettingScreen();
			settings.ShowDialog();
			Splash.Visibility = Visibility.Collapsed;
		}

		private void cakeList_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.content.Navigate(new CakePage());
		}

		private void addCakeButton_MouseUp(object sender, MouseButtonEventArgs e)
		{
			menuBar.Visibility = Visibility.Collapsed;
			var newAddCakePage = new AddCakePage();
			newAddCakePage.BackBtnClick += ShowMenuHandler;
			this.content.Navigate(newAddCakePage);
		}

		private void statisticsButton_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.content.Navigate(new StatisticsPage());
		}

		private void receiptListButton_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.content.Navigate(new ReceiptPage());
		}

		private void addReceiptButton_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.content.Navigate(new AddReceiptPage());
		}
		#endregion


	}
}
