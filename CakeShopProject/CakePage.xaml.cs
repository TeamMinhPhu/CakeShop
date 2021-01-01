using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
	/// Interaction logic for CakePage.xaml
	/// </summary>
	public partial class CakePage : Page
	{
		public delegate void CakeEditHandler(string cakeId);
		public event CakeEditHandler EditBtnClick;
		public event CakeEditHandler AddToCard;

		int _current_page = 1;
		string _search = "";
		int _selected_index;
		int _total_page;
		int _total_items;
		int _itemPerPage;

		CakeShopDBEntities db = new CakeShopDBEntities();
		private System.Timers.Timer _timer = new System.Timers.Timer(300); //time delayed to UpdatePage after resizing window

		BindingList<ViewModel> viewModels = new BindingList<ViewModel>();

		public CakePage()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.SizeChanged += _size_changed;
			_timer.AutoReset = false;
			_timer.Elapsed += _timer_Elapsed;

			_search = "";
			sort.SelectedIndex = 0;
		}
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
		#endregion

		//Detail
		private void trip_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				string ID = viewModels[_selected_index].ID;
				//TRIP tripDetail = db.TRIPs.First(c => c.TRIP_ID == ID);
				//this.NavigationService.Navigate(new DetailPage(ID));
			}
			catch
			{
				//do nothing
			}
		}



		private void ListViewTrips_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				_selected_index = ListViewCakes.SelectedIndex;
			}
			catch
			{
				//do nothing
			}
		}

		#region "filter"
		private void sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_current_page = 1;
			UpdateView();
		}
		#endregion

		//Edit
		private void editButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var index = ListViewCakes.SelectedIndex;
			if (index >= 0)
			{
				ListViewCakes.ItemsSource = null;
				UpdateLayout();

				var myID = viewModels[index].ID;

				EditBtnClick?.Invoke(myID);
			}

		}



		#region "Paging"

		//Delay time to update ItemsPerPage when window is resizing--> no more lags --> :)
		private void _size_changed(object sender, SizeChangedEventArgs e)
		{
			_timer.Stop();
			_timer.Start();
		}

		private void _timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				_current_page = 1;
				UpdateView();
			});
			_timer.Stop();
		}
		private void UpdateView()
		{
			viewModels.Clear();
			viewModels = GetViewModel();
			ListViewCakes.ItemsSource = viewModels;
			UpdatePage();
		}

		private void UpdatePage()
		{
			if (_current_page < 1)
			{
				_current_page = 1;
			}
			_total_page = Paging.GetTotalPages(_total_items, _itemPerPage);
			viewTotalPages.Text = "/ " + _total_page.ToString();
			paging.ItemsSource = PagingViewModel.UpdatePage(_total_page);
			paging.SelectedIndex = _current_page - 1;
		}

		private void previousButton_Click(object sender, RoutedEventArgs e)
		{
			if (_current_page > 1)
			{
				_current_page--;
			}
			else
			{
				_current_page = _total_page;
			}
			paging.SelectedIndex = _current_page - 1;
			//this.NavigationService.Navigate(new AboutUsPage());
		}


		private void nextButton_Click(object sender, RoutedEventArgs e)
		{
			if (_current_page < _total_page)
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

		public void RenewDataModel()
		{
			_current_page = 1;
			UpdateView();
		}

		#endregion

		private void deleteButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var index = ListViewCakes.SelectedIndex;
			if (index >= 0)
			{
				try
				{
					var myID = viewModels[index].ID;
					var cake = db.CAKEs.Where(c => c.CAKE_ID == myID).FirstOrDefault();
					if (cake != null)
					{
						cake.EXIST_STATUS = false;
						db.SaveChanges();

						_current_page = 1;
						paging.SelectedIndex = 0;
						UpdateView();

						MessageBox.Show("Đã xóa bánh");
					}
					else
					{
						MessageBox.Show("Không tìm thấy bánh");
					}
				}
				catch
				{
					MessageBox.Show("Không thể xóa bánh", "Lỗi");
				}
			}
		}

		private BindingList<ViewModel> GetViewModel()
		{
			BindingList<ViewModel> result = new BindingList<ViewModel>();

			var query = GetSearchedData();
			_total_items = query.Count;
			_itemPerPage = Paging.GetItemsPerPage(itemsView.ActualWidth, itemsView.ActualHeight);
			query = query.OrderBy(c => c.CAKE_PRICE).Skip((_current_page - 1) * _itemPerPage).Take(_itemPerPage).ToList();

			foreach (var viewData in query)
			{
				ViewModel viewModel = new ViewModel();
				viewModel.ID = viewData.CAKE_ID;
				viewModel.Name = viewData.CAKE_NAME;

				if (viewData.CAKE_IMAGES.ToList()[0].IMAGE_LINK =="")
				{
					viewModel.CoverImage = "Resources/Images/sora.jpg";
				}
				else
				viewModel.CoverImage = viewData.CAKE_IMAGES.ToList()[0].IMAGE_LINK;
				viewModel.Price = (int)viewData.CAKE_PRICE;
				result.Add(viewModel);

			}
			return result;
		}

		private List<CAKE> GetSearchedData()
		{
			List<CAKE> result = new List<CAKE>();
			db.Dispose(); //prevent memory leak..
			db = new CakeShopDBEntities();

			///search
			result = db.CAKEs.Where(c => c.CAKE_NAME.Contains(_search)).ToList();
			return result;
		}

        private void addToReceiptButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
			var index = ListViewCakes.SelectedIndex;
			if (index >= 0)
			{
				var myID = viewModels[index].ID;
				AddToCard?.Invoke(myID);
			}
		}
    }
}
