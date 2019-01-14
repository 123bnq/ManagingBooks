using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
using System.Windows.Shapes;
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
            ImageConverter ic = new ImageConverter();
            System.Drawing.Image img = (System.Drawing.Image)ic.ConvertFrom(barcode.GetByteArray(ImageFormat.Bmp));
            Bitmap bitmap = new Bitmap(img);
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
