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

namespace CakeShopProject
{
	/// <summary>
	/// Interaction logic for AddReceiptPage.xaml
	/// </summary>
	public partial class AddReceiptPage : Page
	{
		List<CAKE> myCakes;
		string myBillId;
		int mode;
		long prepaidMoney = 0;
		long totalMoney = 0;

		public AddReceiptPage(List<CAKE> tempCake)
		{
			InitializeComponent();
			myCakes = tempCake;
			mode = 0;
		}

		public AddReceiptPage(string tempBillId)
		{
			InitializeComponent();
			myBillId = tempBillId;
			mode = 1;
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
				prepaidMoney = 0;
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
						prepaidMoney = 0;
						resultTextBlock.Visibility = Visibility.Collapsed;
						MessageBox.Show("Số tiền nhập là số âm", "Lỗi");
					}
                    else
                    {
						long temp = totalMoney - money;
                        if (temp < 0)
                        {
							prepaidMoney = 0;
							resultTextBlock.Visibility = Visibility.Collapsed;
							MessageBox.Show("Số tiền nhập vượt quá số tiền cần trả", "Lỗi");
						}
                        else
                        {
							prepaidMoney = money;
							resultTextBlock.Visibility = Visibility.Visible;
							resultTextBlock.Text = $"Số tiền còn lại: {temp} VNĐ";

						}
                    }
					
				}
			}
            else
            {
				prepaidMoney = 0;
				resultTextBlock.Visibility = Visibility.Collapsed;
			}
		}

        private void backButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void doneBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
