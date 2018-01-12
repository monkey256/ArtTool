using artool.logic;
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
    /// TextureFileItem.xaml 的交互逻辑
    /// </summary>
    public partial class TextureFileItem : UserControl
    {
        #region 依赖属性
        public static readonly DependencyProperty IsSelectedProperty;
        public static readonly DependencyProperty FileNameProperty;
        static TextureFileItem()
        {
            IsSelectedProperty = DependencyProperty.Register(
                "IsSelected",
                typeof(bool),
                typeof(TextureFileItem),
                new FrameworkPropertyMetadata(false),
                null);
            FileNameProperty = DependencyProperty.Register(
                "FileName",
                typeof(string),
                typeof(TextureFileItem),
                new FrameworkPropertyMetadata(""),
                null);
        }
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }
        #endregion

        private string image_path_;
        public string ImagePath
        {
            get { return image_path_; }
            set
            {
                image_path_ = value;
                FileName = System.IO.Path.GetFileName(image_path_);
            }
        }

        public TextureFileItem()
        {
            InitializeComponent();
        }

        public void RefreshUI()
        {
            var tex = ResManager.LoadTexture(ImagePath);
            if (tex != null)
                img.Source = tex.Source;
            else
                img.Source = null;
        }

        public void OpenDir()
        {
            Util.OpenDirFile(_getPath());
        }

        public void OpenFile()
        {
            System.Diagnostics.Process.Start(_getPath());
        }

        private string _getPath()
        {
            return ImagePath;
        }
    }
}
