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
using Microsoft.Win32;

using WeSplitProject.Classes;

namespace CakeShopProject
{
	/// <summary>
	/// Interaction logic for AddCakePage.xaml
	/// </summary>
	public partial class AddCakePage : Page
	{
		public delegate void ClosingHandler();
		public event ClosingHandler BackBtnClick;

        class CakeImage
        {
            public string ImageLink { get; set; }
        }

        string _ImageLink = "";
        BindingList<CakeImage> myViewImgList;

        public AddCakePage()
		{
			InitializeComponent();
		}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            myViewImgList = new BindingList<CakeImage>();
            cakeImgListView.ItemsSource = myViewImgList;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
			BackBtnClick?.Invoke();
			UpdateLayout();
			this.NavigationService.GoBack();
		}

        private void cakeImage_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                bool loadImageSucceeded = false;
                //Get file link
                string[] ImageFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                var Temp = new BindingList<CakeImage>();
                //check image file and show image
                if (ImageFiles.Length > 0)
                {
                    //Case: an image or a folder
                    if (ImageFiles.Length == 1)
                    {
                        //Case: an image                        
                        if (MyFileManager.IsImageFile(ImageFiles[0]))
                        {
                            myViewImgList.Clear();
                            cakeImgListView.Visibility = Visibility.Hidden;

                            _ImageLink = ImageFiles[0];
                            var Bitmap = new BitmapImage(new Uri(_ImageLink, UriKind.Absolute));
                            cakeImage.Source = Bitmap;
                            cakeImageHint.Visibility = Visibility.Hidden;
                            loadImageSucceeded = false;
                        }//case: a folder
                        else if (MyFileManager.IsDictionaryExisted(ImageFiles[0]))
                        {
                            var myFiles = System.IO.Directory.GetFiles(ImageFiles[0]);
                            foreach (var myFile in myFiles)
                            {
                                if (MyFileManager.IsImageFile(myFile))
                                {
                                    Temp.Add(new CakeImage() { ImageLink = myFile });
                                }
                            }

                            if (Temp.Count > 0)
                            {
                                if (Temp.Count == 1)
                                {
                                    myViewImgList.Clear();
                                    cakeImgListView.Visibility = Visibility.Hidden;

                                    _ImageLink = Temp[0].ImageLink;
                                    var Bitmap = new BitmapImage(new Uri(_ImageLink, UriKind.Absolute));
                                    cakeImage.Source = Bitmap;
                                    cakeImageHint.Visibility = Visibility.Hidden;
                                    loadImageSucceeded = false;
                                }
                                else
                                {
                                    cakeImageHint.Visibility = Visibility.Visible;
                                    cakeImageHint.Text = "Chọn hình đại diện từ danh sách";

                                    _ImageLink = "";
                                    var Bitmap = new BitmapImage(new Uri("Resources/Icons/picture.png", UriKind.Relative));
                                    cakeImage.Source = Bitmap;

                                    loadImageSucceeded = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy file ảnh");
                            }
                        }

                    }
                    else //Case multi files
                    {
                        foreach (var myFile in ImageFiles)
                        {
                            if (MyFileManager.IsImageFile(myFile))
                            {
                                Temp.Add(new CakeImage() { ImageLink = myFile });
                            }
                        }

                        if (Temp.Count > 0)
                        {
                            if(Temp.Count == 1)
                            {
                                myViewImgList.Clear();
                                cakeImgListView.Visibility = Visibility.Hidden;

                                _ImageLink = Temp[0].ImageLink;
                                var Bitmap = new BitmapImage(new Uri(_ImageLink, UriKind.Absolute));
                                cakeImage.Source = Bitmap;
                                cakeImageHint.Visibility = Visibility.Hidden;
                                loadImageSucceeded = false;
                            }
                            else
                            {
                                cakeImageHint.Visibility = Visibility.Visible;
                                cakeImageHint.Text = "Chọn hình đại diện từ danh sách";

                                _ImageLink = "";
                                var Bitmap = new BitmapImage(new Uri("Resources/Icons/picture.png", UriKind.Relative));
                                cakeImage.Source = Bitmap;

                                loadImageSucceeded = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy file ảnh");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy file ảnh");
                }

                if (loadImageSucceeded == true)
                {
                    myViewImgList = Temp;
                    cakeImgListView.ItemsSource = myViewImgList;
                    cakeImgListView.Visibility = Visibility.Visible;
                }
                else { /*do nothing*/ }

            }
        }

        private void cakeImage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                var ImageFile = dlg.FileName;
                if (MyFileManager.IsImageFile(ImageFile))
                {
                    myViewImgList.Clear();
                    cakeImgListView.Visibility = Visibility.Hidden;

                    _ImageLink = ImageFile;
                    var Bitmap = new BitmapImage(new Uri(_ImageLink, UriKind.Absolute));
                    cakeImage.Source = Bitmap;
                    cakeImageHint.Visibility = Visibility.Hidden;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy file ảnh");
                }
            }
        }

        private void cakeImgListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var index = cakeImgListView.SelectedIndex;
            if (index >= 0)
            {
                _ImageLink = myViewImgList[index].ImageLink;
                var Bitmap = new BitmapImage(new Uri(_ImageLink, UriKind.Absolute));
                cakeImage.Source = Bitmap;
                cakeImageHint.Visibility = Visibility.Hidden;
            }
        }

        private void addCakeBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

    }
}
