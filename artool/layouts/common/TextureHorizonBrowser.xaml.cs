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
    /// TextureHorizonBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class TextureHorizonBrowser : UserControl
    {
        private UserControl selected_item_;

        public event Action<string> SelectedChangedCallback;

        public TextureHorizonBrowser()
        {
            InitializeComponent();
        }

        public void RefreshUI(List<string> fps)
        {
            _selectItem(null);
            panel.Children.Clear();

            foreach (var fp in fps)
            {
                var item = _createFileItem(fp);
                panel.Children.Add(item);
            }
            sv.ScrollToHorizontalOffset(0);

            if (panel.Children.Count > 0)
                _selectItem(panel.Children[0] as UserControl);
        }

        public void SelectPrev()
        {
            if (selected_item_ == null)
            {
                if (panel.Children.Count > 0)
                    _selectItem(panel.Children[0] as UserControl);
                return;
            }

            int idx = panel.Children.IndexOf(selected_item_);
            if (idx > 0)
                _selectItem(panel.Children[idx - 1] as UserControl);
        }

        public void SelectNext()
        {
            if (selected_item_ == null)
            {
                if (panel.Children.Count > 0)
                    _selectItem(panel.Children[0] as UserControl);
                return;
            }

            int idx = panel.Children.IndexOf(selected_item_);
            if (idx < panel.Children.Count - 1)
                _selectItem(panel.Children[idx + 1] as UserControl);
        }

        private TextureFileItem _createFileItem(string fp)
        {
            var item = new TextureFileItem();
            item.IsSelected = false;
            item.ImagePath = fp;
            item.Width = 80;
            item.Height = 80;
            item.Margin = new Thickness(0, 0, 5, 0);
            item.RefreshUI();

            item.MouseLeftButtonDown += (_s, _e) =>
            {
                _e.Handled = true;
                _selectItem(_s as UserControl);
            };
            item.MouseRightButtonDown += (_s, _e) =>
            {
                _e.Handled = true;
                _selectItem(_s as UserControl);
            };
            item.MouseLeftButtonUp += (_s, _e) => _e.Handled = true;
            item.MouseRightButtonUp += (_s, _e) => _e.Handled = true;

            return item;
        }

        private void _selectItem(UserControl item)
        {
            if (selected_item_ != item)
            {
                if (selected_item_ != null)
                    (selected_item_ as TextureFileItem).IsSelected = false;
                if (item != null)
                    (item as TextureFileItem).IsSelected = true;
                selected_item_ = item;

                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() => sv.MakesureChildVisible(selected_item_))
                    );

                var fp = string.Empty;
                if (item != null)
                    fp = (item as TextureFileItem).ImagePath;
                if (SelectedChangedCallback != null)
                    SelectedChangedCallback.Invoke(fp);
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            int n = Math.Abs(e.Delta / 120) * 3;
            for (int i = 0; i < n; i++)
            {
                if (e.Delta > 0)
                    scrollviewer.LineLeft();
                else
                    scrollviewer.LineRight();
            }
            e.Handled = true;
        }
    }
}
