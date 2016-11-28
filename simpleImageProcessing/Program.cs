using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simpleImageProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            ImageProcessor amigaImage = new ImageProcessor("./../../1280px-Amiga500_system.jpg");
           
            amigaImage.GenerateEdges();
            amigaImage.SaveImage("AmigaEdges.jpg", ImageFormat.Jpeg);
        }
    }
}
