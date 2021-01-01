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
		CakeShopDBEntities db = new CakeShopDBEntities();

		public ReceiptPage()
		{
			InitializeComponent();
		}

		BindingList<ReceiptViewModel> _receipt_view_models = new BindingList<ReceiptViewModel>();

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			var bills = db.BILLs.Select(c => new { c.ADDRESS, c.CUSTOMER_NAME, c.BILL_ID, c.EMAIL, c.COMPLETED_DATE, c.PHONE, c.STATUS, c.PREPAID_MONEY, c.BILLDETAILs, c.BILLTYPE, c.NOTE}).ToList();
			foreach (var viewData in bills)
			{
				ReceiptViewModel viewModel = new ReceiptViewModel();
				viewModel.ID = viewData.BILL_ID;
				viewModel.Name = viewData.CUSTOMER_NAME;
				viewModel.Note = viewData.NOTE;
				viewModel.Payment = viewData.PREPAID_MONEY;
				viewModel.Email = viewData.EMAIL;
				viewModel.Address = viewData.ADDRESS;
				viewModel.Phone = viewData.PHONE;

				if (viewData.STATUS == 0)
				{
					viewModel.Status = "Chưa thanh toán";
				}
				else
				{
					viewModel.Status = "Đã thanh toán";
				}
				_receipt_view_models.Add(viewModel);
			}

			receipt.ItemsSource = _receipt_view_models;
		}
	}
}
