using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CakeShopProject
{
	/// <summary>
	/// Interaction logic for SettingsScreen.xaml
	/// </summary>
	public partial class SettingScreen : Window
	{
		bool _show_splashScreen;


		public SettingScreen()
		{
			InitializeComponent();
			loadConfig();
			MouseDown += Window_MouseDown;
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				DragMove();
		}

		private void loadConfig()
		{
			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			var configSplashScreen = config.AppSettings.Settings["ShowSplashScreen"].Value;
			_show_splashScreen = bool.Parse(configSplashScreen);

			if (_show_splashScreen == true)
			{
				splashScreen.IsChecked = true;
			}
			else
			{
				splashScreen.IsChecked = false;
			}
		}

		private void splashScreen_Click(object sender, RoutedEventArgs e)
		{
			if (splashScreen.IsChecked.Value)
			{
				_show_splashScreen = true;
			}
			else
			{
				_show_splashScreen = false;
			}
		}

		private void closeButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void applyButton_Click(object sender, RoutedEventArgs e)
		{
			updateConfig();
			DialogResult = true;
		}
		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void updateConfig()
		{
			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			config.AppSettings.Settings["ShowSplashScreen"].Value = _show_splashScreen.ToString();
			config.Save(ConfigurationSaveMode.Minimal);
		}
	}
}
