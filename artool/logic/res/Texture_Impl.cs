using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace artool.logic
{
    class Texture_Impl : ITexture
    {
        private string path_;
        private ImageSource source_;
        private int ref_;

        public Texture_Impl()
        {
            path_ = string.Empty;
            source_ = null;
            ref_ = 1;
        }

        public string ImagePath
        {
            get { return path_; }
            set { path_ = value; }
        }

        public ImageSource Source
        {
            get { return source_; }
            set { source_ = value; }
        }

        public void AddRef()
        {
            ref_++;
        }

        public int GetRef()
        {
            return ref_;
        }

        public void Release()
        {
            ref_--;
        }

        public double Width { get; set; }

        public double Height { get; set; }
    }
}
