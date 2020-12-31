using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CakeShopProject
{
	/// <summary>
	/// Interaction logic for AddReceiptPage.xaml
	/// </summary>
	public partial class AddReceiptPage : Page
	{
		class CakeBillView
        {
			public string CakeName { get; set; }
			public long Cost { get; set; }
			public int Quantity { get; set; }
		}

		BindingList<CakeBillView> myCakeView;
		List<BILLDETAIL> myBillDetail;
		BILL myBill;
		string myBillId;
		int mode;
		long prepaidMoney = 0;
		long totalMoney = 0;
		CakeShopDBEntities db = new CakeShopDBEntities();

		public AddReceiptPage(List<BILLDETAIL> tempBillDetail, BILL tempBill )
		{
			InitializeComponent();
			myBillDetail = tempBillDetail;
			myBill = tempBill;
			mode = 0;
		}

		public AddReceiptPage(string tempBillId)
		{
			InitializeComponent();
			myBillId = tempBillId;
			mode = 1;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			if (mode == 1) 
            {
				myBill = db.BILLs.Where(c => c.BILL_ID == myBillId).FirstOrDefault();
				myBill.PREPAID_MONEY = (long)myBill.PREPAID_MONEY;
				myBillDetail = db.BILLDETAILs.Where(c => c.BILL_ID == myBillId).ToList();
			}
            else
            { /*do nothing*/ }


			//general info
			nameTextBox.Text = myBill.CUSTOMER_NAME;
			phoneTextBox.Text = myBill.PHONE;
			emailTextBox.Text = myBill.EMAIL;
			addressTextBox.Text = myBill.ADDRESS;
			noteTextBox.Text = myBill.NOTE;

			//Table
			myCakeView = new BindingList<CakeBillView>();
			foreach (var item in myBillDetail)
			{
				var cake = db.CAKEs.Select(c => new { c.CAKE_ID, c.CAKE_NAME, c.CAKE_PRICE }).Where(c => c.CAKE_ID == item.CAKE_ID).FirstOrDefault();
				myCakeView.Add(new CakeBillView { CakeName = cake.CAKE_NAME, Cost = (long)cake.CAKE_PRICE, Quantity = (int)item.QUANTITY });
            }
			cakeDataList.ItemsSource = myCakeView;

			//count total
			totalMoney = 0;
			foreach (var item in myCakeView)
			{
				try
				{
					totalMoney += item.Quantity * item.Cost;
				}
				catch
				{
					MessageBox.Show("Số tiền vượt quá giới hạn tính toán", "Lỗi");
					totalMoney = 0;
					break;
				}

			}
			totalTextBlock.Text = $"Tổng: {totalMoney} VNĐ";

            //status check box
            if (myBill.STATUS == 2) // status: 0: deleted, 1: shipping, 2: completed
            {
				shippedCheckBox.IsChecked = true;
			}

			//pay method
			if (myBill.BILLTYPE == 0) //type = 0: online, 1: offline
            {
				payOnlineRadioBtn.IsChecked = true;
                if (myBill.PREPAID_MONEY > 0)
                {
					prepaidTextBox.Text = $"{myBill.PREPAID_MONEY}";
					prepaidTextBox.Visibility = Visibility.Visible;

					var temp = totalMoney - myBill.PREPAID_MONEY;
					resultTextBlock.Text = $"Số tiền còn lại: {temp} VNĐ";
				}
			}
            else if (myBill.BILLTYPE == 1)
			{
				payOfflineRadioBtn.IsChecked = true;

			}
		}

		private void payOnlineRadioBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
			if(payOnlineRadioBtn.IsChecked == true)
            {
				prepaidTextBox.Visibility = Visibility.Visible;
            }
        }

        private void payOfflineRadioBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
			if (payOnlineRadioBtn.IsChecked == true)
			{
				prepaidTextBox.Visibility = Visibility.Collapsed;
				resultTextBlock.Visibility = Visibility.Collapsed;
				prepaidTextBox.Text = "";
				myBill.PREPAID_MONEY = 0;
			}
		}

        private void prepaidTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
			if (prepaidTextBox.Text.Length > 0)
			{
				if (!long.TryParse(prepaidTextBox.Text, out long money))
				{
					resultTextBlock.Visibility = Visibility.Collapsed;
					MessageBox.Show("Số tiền nhập không phải là số hoặc vượt quá giới hạn", "Lỗi");
				}
				else
				{
                    if (money < 0)
                    {
						myBill.PREPAID_MONEY = 0;
						resultTextBlock.Visibility = Visibility.Collapsed;
						MessageBox.Show("Số tiền nhập là số âm", "Lỗi");
					}
                    else
                    {
						long temp = totalMoney - money;
                        if (temp < 0)
                        {
							myBill.PREPAID_MONEY = 0;
							resultTextBlock.Visibility = Visibility.Collapsed;
							MessageBox.Show("Số tiền nhập vượt quá số tiền cần trả", "Lỗi");
						}
                        else
                        {
							myBill.PREPAID_MONEY = money;
							resultTextBlock.Visibility = Visibility.Visible;
							resultTextBlock.Text = $"Số tiền còn lại: {temp} VNĐ";

						}
                    }
					
				}
			}
            else
            {
				myBill.PREPAID_MONEY = 0;
				resultTextBlock.Visibility = Visibility.Collapsed;
			}
		}

        private void backButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void doneBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
