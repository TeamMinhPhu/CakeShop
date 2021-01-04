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
using CakeShopProject.Classes;

namespace CakeShopProject
{
	/// <summary>
	/// Interaction logic for ReceiptPage.xaml
	/// </summary>
	public partial class ReceiptPage : Page
	{
		public delegate void ReceiptEditHandler(string receiptId);
		public event ReceiptEditHandler ReceiptClicked;

		public delegate void ClosingHandler();
		public event ClosingHandler BackBtnClick;

		int _current_page = 1;
		int _total_items;
		int _itemPerPage = 10;
		int _total_pages;
		string _search = "";

		CakeShopDBEntities db = new CakeShopDBEntities();

		public ReceiptPage()
		{
			InitializeComponent();
		}

		List<ReceiptViewModel> viewModels = new List<ReceiptViewModel>();

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{

			viewModels = GetViewModel();
			receipt.ItemsSource = viewModels;
			itemPerPage.SelectedIndex = 1;
			//receipt.Items.Refresh();
		}

		private List<ReceiptViewModel> GetViewModel()
		{
			List<ReceiptViewModel> result = new List<ReceiptViewModel>();

			var query = GetSearchedData();
			_total_items = query.Count;
			_total_pages = Paging.GetTotalPages(_total_items, _itemPerPage);
			query = query.OrderByDescending(c => c.COMPLETED_DATE).Skip((_current_page - 1) * _itemPerPage).Take(_itemPerPage).ToList();

			foreach (var viewData in query)
			{
				ReceiptViewModel viewModel = new ReceiptViewModel();

				var ListPrice = db.BILLDETAILs.Where(c => c.BILL_ID == viewData.BILL_ID).Select(c => new { c.PRICE, c.QUANTITY}).ToList();
				if(ListPrice == null)
                {
					viewModel.Payment = 0;
                }
                else
                {
					long totalPrice = 0;
					foreach(var cake in ListPrice)
                    {
                        try
                        {
							totalPrice += (long)cake.PRICE * (int)cake.QUANTITY;
                        }
                        catch { /*do nothing*/ }
                    }
					viewModel.Payment = totalPrice;
				}
				
				viewModel.ID = viewData.BILL_ID;
				viewModel.Name = viewData.CUSTOMER_NAME;
				viewModel.Note = viewData.NOTE;
				viewModel.Email = viewData.EMAIL;
				viewModel.Address = viewData.ADDRESS;
				viewModel.Phone = viewData.PHONE;

				if (viewData.BILLTYPE == 0)
				{
					viewModel.PaymentType = "Online";
				}
				else if (viewData.BILLTYPE == 1)
				{
					viewModel.PaymentType = "Trực tiếp";
				}

				if (viewData.STATUS == 1)
				{
					viewModel.Status = "Chưa thanh toán";
				}
				else if (viewData.STATUS == 2)
				{
					viewModel.Status = "Đã thanh toán";
				}
				else if (viewData.STATUS == 0)
                {
					viewModel.Status = "Đã hủy";
				}
				result.Add(viewModel);
			}
			return result;
		}
		private void backButton_Click(object sender, RoutedEventArgs e)
		{
			this.NavigationService.GoBack();
			BackBtnClick?.Invoke();
		}

		private List<BILL> GetSearchedData()
		{
			List<BILL> result = new List<BILL>();
			db.Dispose(); //prevent memory leak..
			db = new CakeShopDBEntities();

			///search
			_search = RemoveSign(_search);
			result = db.BILLs.ToList().Where(c => RemoveSign(c.CUSTOMER_NAME).Contains(_search) || RemoveSign(c.EMAIL).Contains(_search) || RemoveSign(c.PHONE).Contains(_search)).ToList();
			return result;
		}

		#region "Paging"
		private void previousButton_Click(object sender, RoutedEventArgs e)
		{
			if (_current_page > 1)
			{
				_current_page--;
			}
			else
			{
				_current_page = _total_pages;
			}
			paging.SelectedIndex = _current_page - 1;
			//this.NavigationService.Navigate(new AboutUsPage());
		}


		private void nextButton_Click(object sender, RoutedEventArgs e)
		{
			if (_current_page < _total_pages)
			{
				_current_page++;
			}
			else
			{
				_current_page = 1;
			}
			paging.SelectedIndex = _current_page - 1;
		}

		private void paging_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_current_page = paging.SelectedIndex + 1;
			UpdateView();
		}

		private void UpdateView()
		{
			viewModels.Clear();
			viewModels = GetViewModel();
			receipt.ItemsSource = viewModels;
			UpdatePage();
		}

		public void RenewDataModel()
		{
			_current_page = 1;
			UpdateView();
		}

		private void UpdatePage()
		{
			if (_current_page < 1)
			{
				_current_page = 1;
			}
			_total_pages = Paging.GetTotalPages(_total_items, _itemPerPage);
			viewTotalPages.Text = "/ " + _total_pages.ToString();
			paging.ItemsSource = PagingViewModel.UpdatePage(_total_pages);
			paging.SelectedIndex = _current_page - 1;
		}

		private void itemPerPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch (itemPerPage.SelectedIndex)
			{
				case 0:
					_itemPerPage = 5;
					break;
				case 1:
					_itemPerPage = 10;
					break;
				case 2:
					_itemPerPage = 25;
					break;
				case 3:
					_itemPerPage = 50;
					break;
				default:
					_itemPerPage = 10;
					break;
			}
			_current_page = paging.SelectedIndex + 1;
			UpdateView();
		}
		#endregion


		#region "search UI interaction"
		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			keywordPlaceholderTextBlock.Visibility = Visibility.Hidden;
		}

		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (keywordTextBox.Text.Length == 0)
			{
				keywordPlaceholderTextBlock.Visibility = Visibility.Visible;
			}
		}

		private void keywordTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			//if (e.Key == Key.Return)
			//{
			_search = keywordTextBox.Text;
			RenewDataModel();
			//}
		}

		private void searchButton_Click(object sender, RoutedEventArgs e)
		{
			_search = keywordTextBox.Text;
			RenewDataModel();
		}

		///Vietnamese char
		private static readonly string[] VietNamChar = new string[]
		{
			"aAeEoOuUiIdDyY",
			"áàạảãâấầậẩẫăắằặẳẵ",
			"ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
			"éèẹẻẽêếềệểễ",
			"ÉÈẸẺẼÊẾỀỆỂỄ",
			"óòọỏõôốồộổỗơớờợởỡ",
			"ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
			"úùụủũưứừựửữ",
			"ÚÙỤỦŨƯỨỪỰỬỮ",
			"íìịỉĩ",
			"ÍÌỊỈĨ",
			"đ",
			"Đ",
			"ýỳỵỷỹ",
			"ÝỲỴỶỸ"
		};
		/// <summary>
		/// convert to unsigned string
		/// </summary>
		/// <param name="str"></param>
		/// <returns>unsigned string</returns>
		public static string RemoveSign(string str)
		{
			str = str.Normalize(NormalizationForm.FormC);
			//replace unicode char      
			for (int i = 1; i < VietNamChar.Length; i++)
			{
				for (int j = 0; j < VietNamChar[i].Length; j++)
				{
					str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
				}
			}
			return str;
		}
        #endregion

        private void receipt_MouseUp(object sender, MouseButtonEventArgs e)
        {
			var index = receipt.SelectedIndex;
			if (index >= 0)
			{
				var myID = viewModels[index].ID;
				ReceiptClicked?.Invoke(myID);
			}
		}
    }
}
