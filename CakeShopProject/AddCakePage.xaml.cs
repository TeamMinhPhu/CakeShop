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

using MaterialDesignThemes;
using MaterialDesignColors;
using CakeShopProject.Classes;

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

        int mode;
        CAKE myCake;
        string myCakeId = "";
        string _ImageLink = "";
        BindingList<CakeImage> myViewImgList;
        BindingList<TYPE> myTypes;
        CakeShopDBEntities db = new CakeShopDBEntities();

        /// <summary>
        /// main function
        /// </summary>
        /// ////////////////////////////////////////////////////////
        public AddCakePage()
		{
			InitializeComponent();
            mode = 0;
		}

        public AddCakePage(string tempId)
        {
            InitializeComponent();
            myCakeId = tempId;
            myCakeId = myCakeId.Replace(" ", "");
            mode = 1;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            myViewImgList = new BindingList<CakeImage>();
            cakeImgListView.ItemsSource = myViewImgList;

            int quantity = db.TYPEs.Count();
            if (quantity == 0)
            {
                var newType = new TYPE { TYPE_ID = "TYPE0", TYPE_NAME = "Khác" };
                db.TYPEs.Add(newType);

                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    MessageBox.Show("Lỗi tạo loại bánh");
                }
            }

            myTypes = new BindingList<TYPE>(db.TYPEs.OrderBy(c=>c.TYPE_NAME).ToList());
            typeComboBox.ItemsSource = myTypes;

            //edit mode
            if (mode == 1) 
            {
                myCakeId = myCakeId.Replace(" ", "");

                myCake = db.CAKEs.Where(c => c.CAKE_ID == myCakeId).FirstOrDefault();

                nameTextBox.Text = myCake.CAKE_NAME;
                priceTextBox.Text = $"{(long)myCake.CAKE_PRICE}";

                var type = db.TYPEs.Where(c => c.TYPE_ID == myCake.TYPE_ID).FirstOrDefault();

                typeTextBox.Text = type.TYPE_NAME;
                cakeDescriptionTextBox.Text = myCake.CAKE_DESCRIPTION;

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
                        cakeImageHint.Visibility = Visibility.Hidden;
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
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            cakeImage.Source = null;
            cakeImgListView.ItemsSource = null;
            UpdateLayout();
            this.NavigationService.GoBack();
            BackBtnClick?.Invoke();
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

        private void typeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var index = typeComboBox.SelectedIndex;
            if (index >= 0)
            {
                typeTextBox.Text = myTypes[index].TYPE_NAME;
                typeComboBox.SelectedIndex = -1;
            }
        }

        private void addCakeBtn_Click(object sender, RoutedEventArgs e)
        {
            //check name
            if (nameTextBox.Text.Length <= 0)
            {
                MessageBox.Show("Chưa nhập tên bánh", "Cảnh báo");
            }
            else
            {
                //check price
                if (priceTextBox.Text.Length > 0)
                {
                    //check price
                    if (!long.TryParse(priceTextBox.Text, out long cakePrice))
                    {
                        MessageBox.Show("Số tiền nhập không phải là số hoặc vượt quá giới hạn", "Lỗi");
                    }
                    else
                    {
                        if (mode == 0)
                        {
                            //create ID
                            var temp = db.CAKEs.Count();
                            string newCakeId = $"CAKE{temp}";
                            newCakeId = newCakeId.Replace(" ", "");
                            //Create folder to save image
                            var Folder = AppDomain.CurrentDomain.BaseDirectory;
                            var savedFolderLink = $"Resources\\Images\\{newCakeId}";
                            MyFileManager.CheckDictionary($"{Folder}{savedFolderLink}");

                            SaveNewCake(newCakeId, cakePrice);

                            SaveCakeImage(newCakeId, savedFolderLink);

                            SaveType(newCakeId);
                        }
                        else
                        {
                            myCakeId = myCakeId.Replace(" ", "");

                            //Create folder to save image
                            var Folder = AppDomain.CurrentDomain.BaseDirectory;
                            var savedFolderLink = $"Resources\\Images\\{myCakeId}";
                            MyFileManager.CheckDictionary($"{Folder}{savedFolderLink}");

                            db.CAKE_IMAGES.RemoveRange(db.CAKE_IMAGES.Where(c => c.CAKE_ID == myCakeId).ToList());

                            SaveNewCake(myCakeId, cakePrice);

                            SaveCakeImage(myCakeId, savedFolderLink);

                            SaveType(myCakeId);
                        }

                        cakeImage.Source = null;
                        cakeImgListView.ItemsSource = null;
                        UpdateLayout();
                        this.NavigationService.GoBack();
                        BackBtnClick?.Invoke();
                    }
                }
                else
                {
                    MessageBox.Show("Chưa nhập số tiền", "Cảnh báo");
                }
            }
        }

        private void SaveNewCake(string myCakeId, long price)
        {
            if (mode == 0)
            {
                var now = DateTime.UtcNow;
                var newCake = new CAKE
                {
                    CAKE_ID = myCakeId,
                    CAKE_NAME = nameTextBox.Text,
                    CAKE_PRICE = price,
                    CAKE_DESCRIPTION = cakeDescriptionTextBox.Text,
                    EXIST_STATUS = true,
                    ADDED_DATE = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second)

                };

                db.CAKEs.Add(newCake);
            }
            else
            {
                var cake = db.CAKEs.Where(c => c.CAKE_ID == myCakeId).FirstOrDefault();
                var now = DateTime.UtcNow;

                cake.CAKE_NAME = nameTextBox.Text;
                cake.CAKE_PRICE = price;
                cake.CAKE_DESCRIPTION = cakeDescriptionTextBox.Text;
                cake.ADDED_DATE = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            }
           

            try
            {
                db.SaveChanges();
            }
            catch
            {
                MessageBox.Show("Lỗi thêm bánh");
            }
        }

        private void SaveCakeImage(string myCakeId, string savedFolderLink)
        {
            cakeImage.Source = null;
            cakeImgListView.ItemsSource = null;
            UpdateLayout();

            if (_ImageLink.Length == 0)
            {
                if (myViewImgList.Count > 0)
                {
                    _ImageLink = myViewImgList[0].ImageLink;
                }
            }
            var Folder = AppDomain.CurrentDomain.BaseDirectory;

            //Main cake image link: Resources\\Images\\CAKE{}\\CAKE{}.jpg
            List<string> imgLink = new List<string>();
            List<string> imgCode = new List<string>();

            var cakeImgLink = $"{savedFolderLink}\\{myCakeId}.jpg";
            imgCode.Add(myCakeId);

            //Save main cake Image
            if (_ImageLink.Length > 0)
            {
                //Save main Image
                var myFilePath = $"{Folder}{cakeImgLink}";

                if (_ImageLink != myFilePath)
                {
                    try
                    {
                        //Check if existed image having same name then replace
                        MyFileManager.CheckExistedFile(myFilePath);
                        //Copy image to new folder
                        System.IO.File.Copy(_ImageLink, myFilePath);
                        imgLink.Add(cakeImgLink);
                    }
                    catch 
                    {
                        imgLink.Add("");
                    }

                }
            }
            else
            {
                imgLink.Add("");
            }



            //Save other images
            if (myViewImgList.Count > 0)
            {
                int i = 0;
                foreach (var item in myViewImgList)
                {
                    var newImg = $"{savedFolderLink}\\{myCakeId}_{i}.jpg";
                    imgCode.Add($"{myCakeId}_{i}");

                    //Save Image
                    var myFilePath = $"{Folder}{newImg}";

                    if (item.ImageLink != myFilePath)
                    {
                        try
                        {
                            //Check if existed image having same name then replace
                            MyFileManager.CheckExistedFile(myFilePath);
                            //Copy image to new folder
                            System.IO.File.Copy(item.ImageLink, myFilePath);
                            imgLink.Add(newImg);
                        }
                        catch
                        {
                            imgLink.Add("");
                        }

                    }
                    else
                    {
                        imgLink.Add(newImg);
                    }
                    i++;
                }
            }

            for (int i = 0; i < imgLink.Count; i++)
            {
                var newCakeImg = new CAKE_IMAGES
                {
                    CAKE_ID = myCakeId,
                    IMAGE_ID = imgCode[i].Replace(" ", ""),
                    IMAGE_LINK = imgLink[i]
                };

                db.CAKE_IMAGES.Add(newCakeImg);
            }


            try
            {
                db.SaveChanges();
            }
            catch
            {
                MessageBox.Show("Lỗi thêm hình");
            }
        }

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

        private void SaveType(string myCakeId)
        {
            var types = db.TYPEs.ToList();
            var cake = db.CAKEs.Where(c => c.CAKE_ID == myCakeId).FirstOrDefault();

            if (typeTextBox.Text.Length > 0)
            {
                TYPE type = null;
                foreach (var item in types)
                {
                    if (RemoveSign(item.TYPE_NAME).ToLower() == RemoveSign(typeTextBox.Text).ToLower())
                    {
                        type = item;
                        break;
                    }
                }

                //new type
                if (type == null)
                {
                    //create new type
                    var quantity = db.TYPEs.Count();
                    var typecode = $"TYPE{quantity}";
                    typecode = typecode.Replace(" ", "");

                    var newType = new TYPE { TYPE_ID = typecode, TYPE_NAME = typeTextBox.Text };

                    cake.TYPE_ID = newType.TYPE_ID;

                    db.TYPEs.Add(newType);
                }
                else // old type
                {
                    cake.TYPE_ID = type.TYPE_ID;
                }
            }
            else
            {
                var type0 = "TYPE0";
                cake.TYPE_ID = type0;
            }
            

            try
            {
                db.SaveChanges();
            }
            catch
            {
                MessageBox.Show("Lỗi thêm loại bánh");
            }

        }


    }
}
