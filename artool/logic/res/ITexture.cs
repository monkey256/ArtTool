using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace artool.logic
{
    public interface ITexture
    {
        void AddRef();

        void Release();

        int GetRef();

        string ImagePath { get; }

        ImageSource Source { get; }

        double Width { get; }

        double Height { get; }
    }
}
