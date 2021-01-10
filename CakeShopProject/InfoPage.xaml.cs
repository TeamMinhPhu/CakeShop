using CakeShopProject.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
	/// Interaction logic for InfoPage.xaml
	/// </summary>
	public partial class InfoPage : Page
	{
		public delegate void ClosingHandler();
		public event ClosingHandler BackBtnClick;

		class ListViewMember
		{
			public string Info { get; set; }
			public string Source { get; set; }
		}

		BindingList<ListViewMember> myMembers;
		public InfoPage()
		{
			InitializeComponent();
		}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			var Members = new List<Member>(MemberDao.getData());
			myMembers = new BindingList<ListViewMember>();
			foreach (var item in Members)
			{
				myMembers.Add(new ListViewMember { Info = MemberDao.ConvertString(item), Source = item.ImgSource });
			}

			MembersListView.ItemsSource = myMembers;

			var Folder = AppDomain.CurrentDomain.BaseDirectory;
			var path = $"{Folder}Resources/Data/AppInfo.txt";

			MyFileManager.CheckFilePath(path);
			var Data = File.ReadAllText(path);

			AppInfo.Text = Data;
		}

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
			this.NavigationService.GoBack();
			BackBtnClick?.Invoke();
		}
    }
}
