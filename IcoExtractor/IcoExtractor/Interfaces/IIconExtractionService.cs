using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcoExtractor.Interfaces
{
    public interface IIconExtractionService
    {
        string IconPath { get; set; }
        Icon GetAppIcon();
    }
}
