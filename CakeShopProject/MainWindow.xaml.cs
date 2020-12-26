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
		bool _menu_state_closed = true;

		public class MenuTab
        {
            public string Content { get; set; }
            public string Icon { get; set; }

        }

        BindingList<MenuTab> menuTabs = new BindingList<MenuTab>() { 
            new MenuTab{Content = "TRANG CHỦ", Icon = "Resources/Icons/home.png"},
            new MenuTab{Content = "ĐƠN HÀNG", Icon = "Resources/Icons/orders.png"},
            new MenuTab{Content = "THỐNG KÊ", Icon = "Resources/Icons/statistics.png"},
            new MenuTab{Content = "GIỚI THIỆU", Icon = "Resources/Icons/about.png"},
        };
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			MouseDown += Window_MouseDown; //drag window
			MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
			MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

			this.menuTab.ItemsSource = menuTabs;
			this.content.Navigate(new HomePage());
		}

		//Menu Open - Collapse
		private void menuButton_Click(object sender, RoutedEventArgs e)
		{
			if (_menu_state_closed)
			{
				Storyboard sb = this.FindResource("openMenu") as Storyboard;
				sb.Begin();
			}
			else
			{
				Storyboard sb = this.FindResource("closeMenu") as Storyboard;
				sb.Begin();
			}
			_menu_state_closed = !_menu_state_closed;
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

		private void selectedTab(object sender, MouseButtonEventArgs e)
		{
			var item = (sender as ListView).SelectedIndex;
			switch (item)
			{
				case 0:
					content.Navigate(new HomePage());
					break;
				case 1:
					content.Navigate(new ReceiptPage());
					break;				
				case 2:
					content.Navigate(new StatisticsPage());
					break;		
				case 3:
					content.Navigate(new InfoPage());
					break;
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
	}
}
