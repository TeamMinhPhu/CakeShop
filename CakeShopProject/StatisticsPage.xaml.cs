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
using LiveCharts;
using LiveCharts.Wpf;

namespace CakeShopProject
{
	/// <summary>
	/// Interaction logic for StatisticsPage.xaml
	/// </summary>
	public partial class StatisticsPage : Page
	{
		int _current_year;
		bool _is_init = false;
		public delegate void ClosingHandler();
		public event ClosingHandler BackBtnClick;
		public SeriesCollection MonthlyChart { get; set; } = new SeriesCollection();
		public SeriesCollection TypeChart { get; set; } = new SeriesCollection();
		CakeShopDBEntities db = new CakeShopDBEntities();
		public List<string> MonthlyChartYear { get; set; } = new List<string>();
		public string[] MonthLabel { get; set; } = new string[]
		{
			"Tháng 1",
			"Tháng 2",
			"Tháng 3",
			"Tháng 4",
			"Tháng 5",
			"Tháng 6",
			"Tháng 7",
			"Tháng 8",
			"Tháng 9",
			"Tháng 10",
			"Tháng 11",
			"Tháng 12",
		};
		public StatisticsPage()
		{
			InitializeComponent();
		}


		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			YearComboBox();
			

			dateBegin.SelectedDate = new DateTime(2018, 1, 1);
			dateEnd.SelectedDate = DateTime.Now;
			_is_init = true;
			cakeTypeChart();
			this.DataContext = this;
		}

		/// <summary>
		/// combobox for selecting year
		/// </summary>
		private void YearComboBox()
		{
			int year = DateTime.Now.Year;
			_current_year = year;
			int MIN_YEAR = 2018;
			if (year < MIN_YEAR)
			{
				MessageBox.Show("Vui lòng chỉnh lại ngày hệ thống của windows", "Ngày giờ không chính xác");
			}
			for (int i = year; i >= MIN_YEAR; i--)
			{
				MonthlyChartYear.Add(i.ToString());
			}
			YearForMonthlyChart.SelectedIndex = 0;
		}

		/// <summary>
		/// Bar chart
		/// </summary>
		private void SetupMonthlyChart()
		{
			List<long> Revenue = new List<long>();
			int selectedYear = _current_year - YearForMonthlyChart.SelectedIndex;
			var bills = db.BILLs.Where(c => c.COMPLETED_DATE.Value.Year == selectedYear && c.STATUS == 2).Select(c => new{c.BILL_ID, c.COMPLETED_DATE } );
			if (bills.Count() == 0 || bills == null)
			{
				MonthlyChart.Clear(); //erase old data
			}
			else
			{
				for (int i = 1; i <= 12; i++)
				{
					var completedBills = bills.Where(c => c.COMPLETED_DATE.Value.Month == i)
														.Select(c => c.BILL_ID).ToList();
					if (completedBills.Count == 0)
					{
						Revenue.Add(0);
						continue;
					}
					var billdetail = db.BILLDETAILs.Where(c => completedBills.Contains(c.BILL_ID)).ToList();
					long totalPrice = 0;
					foreach (var cake in billdetail)
					{
						try
						{
							totalPrice += (long)cake.PRICE * (long)cake.QUANTITY;
						}
						catch { /*do nothing*/ }

					}
					Revenue.Add(totalPrice);

				}

				var newBar = new ColumnSeries()
				{
					Values = new ChartValues<long>(Revenue),
				};
				MonthlyChart.Clear(); //erase old data
				MonthlyChart.Add(newBar);
			}
		}

		private void YearForMonthlyChart_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetupMonthlyChart();
		}

		private void cakeTypeChart()
		{
			var types = db.TYPEs.ToList();
			var bills = db.BILLs.Where(c => c.STATUS == 2 && c.COMPLETED_DATE >= dateBegin.SelectedDate && c.COMPLETED_DATE <= DateTime.Now).Select(c => c.BILL_ID).ToList();
			var billDetails = db.BILLDETAILs.Where(c => bills.Contains(c.BILL_ID)).ToList();

			if (_is_init)
			{
				TypeChart.Clear(); //erase old data

			}
			foreach (var type in types)
			{
				long total = 0;
				var cakebills = billDetails.Where(c => c.CAKE.TYPE_ID == type.TYPE_ID);
				foreach (var cakebill in cakebills)
				{
					total += (long)cakebill.PRICE * (int)cakebill.QUANTITY;
				}

				var newPie = new PieSeries()
				{
					Title = type.TYPE_NAME,
					Values = new ChartValues<long> { total }
				};

				TypeChart.Add(newPie);
			}
		}

		private void backButton_Click(object sender, RoutedEventArgs e)
		{
			this.NavigationService.GoBack();
			BackBtnClick?.Invoke();
		}

		private void dateBegin_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_is_init == true)
			{
				cakeTypeChart();
			}
		}

		private void dateEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_is_init == true)
			{
				cakeTypeChart();
			}
		}
	}
}
