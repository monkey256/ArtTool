using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace artool
{
    public static class AppData
    {
        public static string Envir { get; set; }
        public static MainWindow MainWindow { get; set; }

        static AppData()
        {
            Envir = Util.WorkPath;
            MainWindow = null;
        }
    }
}
