using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace LightingApplier2D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            vm = new ViewModel();
            vm.Dispatcher = Img.Dispatcher;
            //"C:\\Users\\Adam Kowalczyk\\Desktop\\normalCones.jpg"
            string fileName = @"..\..\normalCones.jpg";
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);
            if(File.Exists(path))
            {
                var bitmap = System.Drawing.Image.FromFile(path);
                var bitmapResized = new System.Drawing.Bitmap(bitmap, new System.Drawing.Size(vm.Width, vm.Height));
                var bmp = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                  bitmapResized.GetHbitmap(),
                  IntPtr.Zero,
                  Int32Rect.Empty,
                  BitmapSizeOptions.FromEmptyOptions());
                vm.NormalMap = new WriteableBitmap(bmp);
            }
            vm.Initialize();
            //vm.UpdateBitmap();
            DataContext = vm;
            

        }
        ViewModel vm;

        bool isDragging = false;
        DragablePoint point = null;

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isDragging) return;
            var pos = e.GetPosition(sender as Image);

            foreach(var p in vm.Points)
            {
                if(p.IsHit(pos.X, pos.Y, 8))
                {
                    isDragging = true;
                    point = p;
                    return;
                }
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(e.LeftButton == MouseButtonState.Pressed || e.LeftButton == MouseButtonState.Pressed)) return;
            if (!isDragging) return;
            var pos = e.GetPosition(sender as Image);
            point.X = (int)pos.X;
            point.Y = (int)pos.Y;

            //vm.UpdateBitmap();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDragging) return;
            isDragging = false;
            point = null;

            //vm.UpdateBitmap();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a normal map";
            op.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";
            if (op.ShowDialog() == true)
            {
                var bitmap = System.Drawing.Image.FromFile(op.FileName);
                var bitmapResized = new System.Drawing.Bitmap(bitmap, new System.Drawing.Size(vm.Width, vm.Height));
                //var bmp = new BitmapImage(new Uri(op.FileName));
                var bmp = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                  bitmapResized.GetHbitmap(),
                  IntPtr.Zero,
                  Int32Rect.Empty,
                  BitmapSizeOptions.FromEmptyOptions());
                vm.NormalMap = new WriteableBitmap(bmp);
                vm.Initialize();
                //vm.UpdateBitmap();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a texture";
            op.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";
            if (op.ShowDialog() == true)
            {
                var bitmap = System.Drawing.Image.FromFile(op.FileName);
                var bitmapResized = new System.Drawing.Bitmap(bitmap, new System.Drawing.Size(vm.Width, vm.Height));
                //var bmp = new BitmapImage(new Uri(op.FileName));
                var bmp = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                  bitmapResized.GetHbitmap(),
                  IntPtr.Zero,
                  Int32Rect.Empty,
                  BitmapSizeOptions.FromEmptyOptions());
                //var bmp = new BitmapImage(new Uri(op.FileName));
                vm.Texture = new WriteableBitmap(bmp);
                vm.Initialize();
                //vm.UpdateBitmap();
            }
        }
    }
}
