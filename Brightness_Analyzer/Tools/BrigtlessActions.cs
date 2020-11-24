using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brightness_Analyzer.Tools
{
    class BrigtlessActions
    {

        public static double AreaBrightless(Bitmap bmp)
        {
            double brightlessCounter = 0;
            double mediumBrightless;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    brightlessCounter += bmp.GetPixel(x, y).GetBrightness();
                }
            }
            //Console.WriteLine(brightlessCounter.ToString());
            mediumBrightless = brightlessCounter / (bmp.Width * bmp.Height);
            return mediumBrightless;

        }
    }
}
