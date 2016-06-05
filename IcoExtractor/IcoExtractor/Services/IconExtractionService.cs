using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media;
using IcoExtractor.Interfaces;

namespace IcoExtractor.Services
{
    public class IconExtractionService : IIconExtractionService
    {
        public string IconPath { get; set; }

        public IconExtractionService()
        {
        }

        public IconExtractionService(string path)
        {
            IconPath = path;
        }

        public Icon GetAppIcon()
        {
            if (!String.IsNullOrEmpty(IconPath))
            {
                Icon ico = Icon.ExtractAssociatedIcon(IconPath);
                return ico;
            }
            return null;
        }
    }
}
