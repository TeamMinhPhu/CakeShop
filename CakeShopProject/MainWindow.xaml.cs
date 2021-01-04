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
		List<BILLDETAIL> myBillDetail;
		BILL myBill;
		int totalCake = 0;
		CakeShopDBEntities db = new CakeShopDBEntities();

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			MouseDown += Window_MouseDown; //drag window
			MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
			MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

			resetBill();

			OpenCakeList();
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
			OpenCakeList();
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
			openReceiptList();
		}

		private void addReceiptButton_MouseUp(object sender, MouseButtonEventArgs e)
		{
            if (myBillDetail.Count > 0)
            {
				menuBar.Visibility = Visibility.Collapsed;
				var newAddReceipt = new AddReceiptPage(myBillDetail, myBill);
				newAddReceipt.BackBtnAddModeClick += ReceiptBackHandler;
				newAddReceipt.DoneOrCancelBtnClick += resetBill;
				this.content.Navigate(newAddReceipt);
			}
            else
            {
				MessageBox.Show("Chưa thêm bánh vào đơn hàng");
            }
		}
		#endregion

		private void ShowMenuHandler()
		{
			menuBar.Visibility = Visibility.Visible;
			OpenCakeList();
		}

		private void OpenCakeList()
        {
			var newCakePage = new CakePage();
			newCakePage.EditBtnClick += openCakeEditMode;
			newCakePage.AddToCard += AddToReceipt;
			newCakePage.ViewBtnClick += openCakeViewMode;
			this.content.Navigate(newCakePage);
		}

		private void openCakeEditMode(string Id)
        {
			menuBar.Visibility = Visibility.Collapsed;
			var newAddCakePage = new AddCakePage(Id);
			newAddCakePage.BackBtnClick += ShowMenuHandler;
			this.content.Navigate(newAddCakePage);
		}

		private void openCakeViewMode(string Id)
		{
			menuBar.Visibility = Visibility.Collapsed;
			var newViewCakePage = new DetailCakePage(Id);
			newViewCakePage.BackBtnClick += ShowMenuHandler;
			this.content.Navigate(newViewCakePage);
		}

		private void openReceiptEditMode(string Id)
		{
			menuBar.Visibility = Visibility.Collapsed;
			var newAddReceipt = new AddReceiptPage(Id);
			newAddReceipt.BackBtnClick += openReceiptList;
			newAddReceipt.DoneOrCancelBtnClick += openReceiptList;
			this.content.Navigate(newAddReceipt);
		}

		private void openReceiptList()
        {
			menuBar.Visibility = Visibility.Collapsed;
			var receiptListPage = new ReceiptPage();
			receiptListPage.ReceiptClicked += openReceiptEditMode;
			receiptListPage.BackBtnClick += ShowMenuBarOnly;
			this.content.Navigate(receiptListPage);
		}

		private void AddToReceipt(string Id)
		{
			var temp = myBillDetail.Where(c => c.CAKE_ID.Contains(Id)).FirstOrDefault();
            if (temp != null)
            {
				temp.QUANTITY++;
				totalCake++;
				notificationGrid.Visibility = Visibility.Visible;
				notificationTextBlock.Text = $"{totalCake}";
			}
            else
            {
				var price = db.CAKEs.Where(c => c.CAKE_ID == Id).Select(c => c.CAKE_PRICE).FirstOrDefault();
                if (price != null)
                {
					myBillDetail.Add(new BILLDETAIL { BILL_ID = myBill.BILL_ID, CAKE_ID = Id, QUANTITY = 1, PRICE = price });
					totalCake++;
					notificationGrid.Visibility = Visibility.Visible;
					notificationTextBlock.Text = $"{totalCake}";
				}
				
            }

			
		}

		private void resetBill()
        {
			menuBar.Visibility = Visibility.Visible;
			totalCake = 0;
			notificationGrid.Visibility = Visibility.Collapsed;
			var billNumber = db.BILLs.Count();
			var billcode = $"BILL{billNumber}";
			billcode = billcode.Replace(" ", "");

			myBillDetail = new List<BILLDETAIL>();
			myBill = new BILL { BILL_ID = billcode };
		}

		private void ReceiptBackHandler(BILL tempBill)
        {
			menuBar.Visibility = Visibility.Visible;
			myBill = tempBill;
        }

		private void ShowMenuBarOnly()
		{
			menuBar.Visibility = Visibility.Visible;
		}


	}
}
