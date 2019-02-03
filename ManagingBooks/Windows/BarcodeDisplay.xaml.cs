using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using NetBarcode;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for BarcodeDisplay.xaml
    /// </summary>
    public partial class BarcodeDisplay : Window
    {
        public BarcodeDisplay()
        {
            InitializeComponent();
        }

        public BarcodeDisplay(string barcodeString)
        {
            InitializeComponent();
            Barcode barcode = new Barcode(barcodeString, true);
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
            Bitmap bitmap = tc.ConvertFrom(barcode.GetByteArray(ImageFormat.Bmp)) as Bitmap;
            BarcodeShow.Source = BitmapToImageSource(bitmap);
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
