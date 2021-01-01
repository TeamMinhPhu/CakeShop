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
		public delegate void ClosingHandler();
		public event ClosingHandler BackBtnClick;
		public event ClosingHandler DoneOrCancelBtnClick;

		public delegate void backHandler(BILL tempBill);
		public event backHandler BackBtnAddModeClick;

		class CakeBillView
        {
			public string CakeName { get; set; }
			public long Cost { get; set; }
			public int Quantity { get; set; }
		}

		BindingList<CakeBillView> myCakeView;
		public List<BILLDETAIL> myBillDetail { get; set; }
		public BILL myBill { get; set; }

		string myBillId;
		int mode;
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
					resultTextBlock.Text = $"Số tiền còn lại: {temp}";
				}
			}
            else if (myBill.BILLTYPE == 1)
			{
				payOfflineRadioBtn.IsChecked = true;

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
							resultTextBlock.Text = $"Số tiền còn lại: {temp}";

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
            if (mode == 0)
            {
				saveCurrentStatus();
				BackBtnAddModeClick?.Invoke(myBill);
				UpdateLayout();
				this.NavigationService.GoBack();
			}
            else
            {
				BackBtnClick?.Invoke();
				UpdateLayout();
				this.NavigationService.GoBack();
			}
		}

        private void doneBtn_Click(object sender, RoutedEventArgs e)
        {
			///check name
			if(nameTextBox.Text.Length > 0)
            {
				long phone = 0;
				////check phone number
				if (phoneTextBox.Text.Length > 0)
				{
					if (phoneTextBox.Text.Length > 10)
					{
						MessageBox.Show("Số điện thoại vượt quá 10 số", "Lỗi");
						return;
					}
					else
					{
						if (long.TryParse(phoneTextBox.Text, out phone))
						{
							if (phone < 0 || phone > 9999999999)
							{
								MessageBox.Show("Số điện thoại chứa kí tự không hợp lệ", "Lỗi");
								return;
							}
						}
						else
						{
							MessageBox.Show("Số điện thoại chứa kí tự không hợp lệ", "Lỗi");
							return;
						}
					}

					//////// check pay method
					///
					if(payOnlineRadioBtn.IsChecked == false && payOfflineRadioBtn.IsChecked == false)
                    {
						MessageBox.Show("Chưa chọn phương thức thanh toán", "Cảnh báo");
					}
                    else
                    {
						/////save bill
						///
						if (mode == 0)
                        {
							int status = 0;
							if(shippedCheckBox.IsChecked == true)
                            {
								status = 2;
                            }
                            else
                            {
								status = 1;
                            }

							SaveBill(status);
							SaveBillDetail();
                        }
						else if (mode == 1)
                        {
							int status = 0;
							if (shippedCheckBox.IsChecked == true)
							{
								status = 2;
							}
							else
							{
								status = 1;
							}

							SaveBill(status);
						}
					}
				}
                else
                {
					MessageBox.Show("Chưa nhập số điện thoại", "Cảnh báo");
				}
			}
            else
            {
				MessageBox.Show("Chưa nhập tên khách hàng", "Cảnh báo");
			}

			DoneOrCancelBtnClick?.Invoke();
			UpdateLayout();
			this.NavigationService.GoBack();
		}

		private void SaveBill(int status)
        {

			BILL newBill = new BILL();


			if (status != 0)
            {
				int billType = 0;

				if (payOnlineRadioBtn.IsChecked == true)
				{
					billType = 0;
				}
				else
				{
					billType = 1;
				}
				

				if (mode == 0)
				{
					newBill = new BILL
					{
						BILL_ID = myBill.BILL_ID,
						CUSTOMER_NAME = nameTextBox.Text,
						PHONE = phoneTextBox.Text,
						EMAIL = emailTextBox.Text,
						ADDRESS = addressTextBox.Text,
						NOTE = noteTextBox.Text,
						BILLTYPE = billType,
						PREPAID_MONEY = myBill.PREPAID_MONEY,
						STATUS = status

					};
				}
				else if (mode == 1)
				{
					newBill = db.BILLs.Where(c => c.BILL_ID == myBillId).FirstOrDefault();

					newBill.CUSTOMER_NAME = nameTextBox.Text;
					newBill.PHONE = phoneTextBox.Text;
					newBill.EMAIL = emailTextBox.Text;
					newBill.ADDRESS = addressTextBox.Text;
					newBill.NOTE = noteTextBox.Text;
					newBill.BILLTYPE = billType;
					newBill.PREPAID_MONEY = myBill.PREPAID_MONEY;
					newBill.STATUS = status;
				}

				var now = DateTime.UtcNow;
				if (status != 2)
				{
					newBill.COMPLETED_DATE = null;
				}
				else
				{
					newBill.COMPLETED_DATE = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
				}
			}
            else
            {
				if(mode == 1)
                {
					newBill = db.BILLs.Where(c => c.BILL_ID == myBillId).FirstOrDefault();

					newBill.STATUS = status;
				}
			}

			///////////////////////
			if(mode == 0)
            {
				db.BILLs.Add(newBill);
			}

			try
			{
				db.SaveChanges();
			}
			catch
			{
				MessageBox.Show("Lỗi thêm đơn hàng");
			}
		}

		private void SaveBillDetail()
		{
			foreach(var bill in myBillDetail)
            {
				db.BILLDETAILs.Add(bill);
            }

			try
			{
				db.SaveChanges();
			}
			catch
			{
				MessageBox.Show("Lỗi thêm chi tiết đơn hàng");
			}
		}

		private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
			if (mode == 1) 
            {
				SaveBill(0);
            }
			DoneOrCancelBtnClick?.Invoke();
			UpdateLayout();
			this.NavigationService.GoBack();
		}

		private void saveCurrentStatus()
        {
			int billType = 0;

			if (payOnlineRadioBtn.IsChecked == true)
			{
				billType = 0;
			}
			else
			{
				billType = 1;
			}

			int status = 0;
            if (shippedCheckBox.IsChecked == true)
            {
				status = 2;
            }
            else
            {
				status = 1;
            }

			myBill.CUSTOMER_NAME = nameTextBox.Text;
			myBill.PHONE = phoneTextBox.Text;
			myBill.EMAIL = emailTextBox.Text;
			myBill.ADDRESS = addressTextBox.Text;
			myBill.NOTE = noteTextBox.Text;
			myBill.BILLTYPE = billType;
			myBill.PREPAID_MONEY = myBill.PREPAID_MONEY;
			myBill.STATUS = status;

		}

        private void payOnlineRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
			if (payOnlineRadioBtn.IsChecked == true)
			{
				prepaidTextBox.Visibility = Visibility.Visible;
			}
		}

        private void payOfflineRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
			if (payOnlineRadioBtn.IsChecked == true)
			{
				prepaidTextBox.Visibility = Visibility.Collapsed;
				resultTextBlock.Visibility = Visibility.Collapsed;
				prepaidTextBox.Text = "";
				myBill.PREPAID_MONEY = 0;
			}
		}
    }
}
