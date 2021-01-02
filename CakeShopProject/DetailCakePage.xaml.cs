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
    /// Interaction logic for DetailCakePage.xaml
    /// </summary>
    public partial class DetailCakePage : Page
    {
        public delegate void ClosingHandler();
        public event ClosingHandler BackBtnClick;

        class CakeImage
        {
            public string ImageLink { get; set; }
        }


        string _ImageLink = "";
        string myCakeId = "";
        BindingList<CakeImage> myViewImgList;

        CakeShopDBEntities db = new CakeShopDBEntities();


        public DetailCakePage(string tempId)
        {
            InitializeComponent();
            myCakeId = tempId;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var myCake = db.CAKEs.Where(c => c.CAKE_ID == myCakeId).FirstOrDefault();

            if (myCake != null)
            {
                myViewImgList = new BindingList<CakeImage>();

                CakeNameLabel.Content = myCake.CAKE_NAME;
                priceTextBlock.Text = $"Giá: {(long)myCake.CAKE_PRICE} VNĐ";
                descriptionTextBlock.Text = myCake.CAKE_DESCRIPTION;
                var type = db.TYPEs.Where(c => c.TYPE_ID == myCake.TYPE_ID).FirstOrDefault();
                if (type != null)
                {
                    typeTextBlock.Text = $"Loại bánh: {type.TYPE_NAME}";
                }

                ///load img
                var Folder = AppDomain.CurrentDomain.BaseDirectory;

                var ImgList = db.CAKE_IMAGES.Where(c => c.CAKE_ID == myCakeId).OrderBy(c => c.IMAGE_ID).ToList();

                if (ImgList.Count > 0)
                {
                    var mainImg = ImgList[0];
                    _ImageLink = mainImg.IMAGE_LINK;
                    ///display image
                    if (_ImageLink.Length > 0)
                    {
                        _ImageLink = Folder + _ImageLink;
                        var Bitmap = new BitmapImage(new Uri(_ImageLink, UriKind.Absolute));
                        cakeImage.Source = Bitmap;
                    }

                    if (ImgList.Count > 1)
                    {
                        for (int i = 1; i < ImgList.Count; i++)
                        {
                            var link = Folder + ImgList[i].IMAGE_LINK;
                            myViewImgList.Add(new CakeImage { ImageLink = link });
                        }

                        cakeImgListView.ItemsSource = myViewImgList;
                        cakeImgListView.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                MessageBox.Show("Lỗi truy vấn dữ liệu");

                cakeImage.Source = null;
                cakeImgListView.ItemsSource = null;
                UpdateLayout();
                this.NavigationService.GoBack();
                BackBtnClick?.Invoke();
            }
        }

        private void cakeImgListView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var index = cakeImgListView.SelectedIndex;
            if (index >= 0)
            {
                _ImageLink = myViewImgList[index].ImageLink;
                var Bitmap = new BitmapImage(new Uri(_ImageLink, UriKind.Absolute));
                cakeImage.Source = Bitmap;
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            cakeImage.Source = null;
            cakeImgListView.ItemsSource = null;            
            UpdateLayout();
            this.NavigationService.GoBack();
            BackBtnClick?.Invoke();
        }
    }
}
