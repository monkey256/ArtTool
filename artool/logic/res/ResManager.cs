using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace artool.logic
{
    static class ResManager
    {
        private static Dictionary<string, ITexture> textures_;

        static ResManager()
        {
            textures_ = new Dictionary<string, ITexture>();
        }

        private static string _rectifyPath(string path)
        {
            return path.Replace('/', '\\').ToLower();
        }

        private static ITexture _loadFromFile(string path)
        {
            if (!File.Exists(path))
                return null;

            ITexture r = null;

            try
            {
                var source = new BitmapImage(new Uri(path, UriKind.Absolute));
                source.Freeze();
                var tex = new Texture_Impl();
                tex.ImagePath = path;
                tex.Source = source;
                tex.Width = source.PixelWidth;
                tex.Height = source.PixelHeight;
                r = tex;
            }
            catch
            {
                r = null;
            }

            return r;
        }

        public static ITexture LoadTexture(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            ITexture r = null;
            var fp = _rectifyPath(path);
            if (textures_.TryGetValue(fp, out r))
                return r;

            var tex = _loadFromFile(fp);
            if (tex != null)
                textures_[fp] = tex;

            return tex;
        }
    }
}
