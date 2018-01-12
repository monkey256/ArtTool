using artool.logic;
using Microsoft.Win32;
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

namespace artool.layouts
{
    /// <summary>
    /// AtlasMain.xaml 的交互逻辑
    /// </summary>
    public partial class AtlasMain : UserControl
    {
        private List<string> fps_;
        private bool ignore_text_changed_;

        private DrawingGroup group_back_;
        private DrawingGroup group_images_;

        public AtlasMain()
        {
            fps_ = null;
            ignore_text_changed_ = false;

            InitializeComponent();

            group_back_ = new DrawingGroup();
            group_images_ = new DrawingGroup();
            group.Children.Add(group_back_);
            group.Children.Add(group_images_);

            ignore_text_changed_ = true;
            _refreshParametersByAuto();
            ignore_text_changed_ = false;
            _refreshResultDisplay();

            this.AllowDrop = true;
            this.Drop += AtlasMain_Drop;
        }

        private void AtlasMain_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var fps = new List<string>();
                var arr = e.Data.GetData(DataFormats.FileDrop) as System.Array;
                for (int i = 0; i < arr.Length; i++)
                {
                    var s = arr.GetValue(i).ToString();
                    if (Util.IsPngFile(s) && ResManager.LoadTexture(s) != null)
                        fps.Add(s);
                }
                
                _refreshByFiles(fps);
            }
        }

        private void btnBrowse_Clicked(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "选择数字图片";
            ofd.Filter = "png文件|*.png";
            ofd.FileName = string.Empty;
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;
            ofd.DefaultExt = "png";
            ofd.Multiselect = true;
            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var fps = ofd.FileNames.ToList();
                _refreshByFiles(fps);
            }
        }

        private void txtParameter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!ignore_text_changed_)
                _refreshResultDisplay();
        }

        private void _refreshByFiles(List<string> fps)
        {
            fps.Sort((a, b) => string.Compare(System.IO.Path.GetFileName(a), System.IO.Path.GetFileName(b)));
            fps_ = fps;
            thb.RefreshUI(fps);

            ignore_text_changed_ = true;
            _refreshParametersByAuto();
            ignore_text_changed_ = false;
            _refreshResultDisplay();
        }

        private void _refreshParametersByAuto()
        {
            if (fps_ == null || fps_.Count == 0)
            {
                txtDistance.Text = "--";
                txtWidth.Text = "--";
                txtHeight.Text = "--";
                return;
            }

            int distance = 1;
            int maxwidth = 0;
            int maxheight = 0;

            foreach (var path in fps_)
            {
                var tex = ResManager.LoadTexture(path);
                if (tex != null)
                {
                    maxwidth = Math.Max(maxwidth, (int)tex.Width);
                    maxheight = Math.Max(maxheight, (int)tex.Height);
                }
            }

            txtDistance.Text = distance.ToString();
            txtWidth.Text = maxwidth.ToString();
            txtHeight.Text = maxheight.ToString();
        }

        private void btnAuto_Clicked(object sender, RoutedEventArgs e)
        {
            ignore_text_changed_ = true;
            _refreshParametersByAuto();
            ignore_text_changed_ = false;
            _refreshResultDisplay();
        }

        private void _refreshResultDisplay()
        {
            if (canvas == null)
                return;

            if (fps_ == null || fps_.Count == 0)
            {
                canvas.Visibility = Visibility.Hidden;
                txtError.Visibility = Visibility.Hidden;
                return;
            }

            int distance, width, height;
            if (!int.TryParse(txtDistance.Text, out distance) ||
                !int.TryParse(txtWidth.Text, out width) ||
                !int.TryParse(txtHeight.Text, out height))
            {
                canvas.Visibility = Visibility.Hidden;
                txtError.Visibility = Visibility.Visible;
                return;
            }

            canvas.Visibility = Visibility.Visible;
            txtError.Visibility = Visibility.Hidden;

            var texs = fps_.Select(a => ResManager.LoadTexture(a)).ToList();
            _refreshResultImage(texs, distance, width, height);
        }

        private void _refreshResultImage(List<ITexture> texs, int distance, int width, int height)
        {
            int all_width = texs.Count * (width + distance);
            var rc = new Rect(0, 0, all_width, height);

            canvas.Width = all_width;
            canvas.Height = height;

            group.ClipGeometry = new RectangleGeometry(rc);

            //背景
            group_back_.Children.Clear();
            var dr = new GeometryDrawing();
            dr.Brush = Brushes.Cyan;
            dr.Geometry = new RectangleGeometry(rc);
            group_back_.Children.Add(dr);

            //图片
            group_images_.Children.Clear();
            for (int i = 0; i < texs.Count; i++)
            {
                var tex = texs[i];
                var w = width + distance;
                var h = height;
                var r = new Rect(i * w, 0, w, h);

                var d = new ImageDrawing();
                d.ImageSource = tex.Source;

                var x = (int)(r.X + w / 2 - tex.Width / 2);
                var y = (int)(r.Y + h / 2 - tex.Height / 2);
                d.Rect = new Rect(x, y, tex.Width, tex.Height);

                group_images_.Children.Add(d);
            }
        }

        private void btnGen_Clicked(object sender, RoutedEventArgs e)
        {
            if (canvas.Visibility == Visibility.Hidden)
            {
                this.ErrBox("请选择有效图片！");
                return;
            }

            var now = DateTime.Now;
            var fp = fps_[0];
            var filepath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(fp),
                string.Format("数字合图_{0:d4}{1:d2}{2:d2}{3:d2}{4:d2}{5:d2}.png", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second)
                );

            if (System.IO.File.Exists(filepath))
            {
                this.ErrBox("请稍后再试！");
                return;
            }

            var bk = group_back_.Children[0];
            (bk as GeometryDrawing).Brush = Brushes.Transparent;
            _printVisualToImageFile(imgcontrol, filepath);
            (bk as GeometryDrawing).Brush = Brushes.Cyan;

            Util.OpenDirFile(filepath);
        }

        private void _printVisualToImageFile(Visual vs, string filepath)
        {
            var rtb = new RenderTargetBitmap((int)imgcontrol.ActualWidth, (int)imgcontrol.ActualHeight, 96, 96, PixelFormats.Default);
            rtb.Render(vs);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var fs = System.IO.File.Open(filepath, System.IO.FileMode.CreateNew))
            {
                encoder.Save(fs);
            }
        }
    }
}
