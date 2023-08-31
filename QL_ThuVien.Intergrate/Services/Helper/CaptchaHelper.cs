using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_ThuVien.Intergrate.Services.Helper
{
    public static class CaptchaHelper
    {
        public static string captchaText;
        public static int width = 400;
        public static int height = 100;
        private static Random rnd = new Random();

        public static string GenerateCaptchaCode(int charCount)
        {
            Random r = new Random();
            string s = "";
            while (s.Length != charCount)
            {
                int a = r.Next(3);
                int ch;
                switch (a)
                {
                    case 1:
                        ch = r.Next(0, 9);
                        s = s + ch.ToString();
                        break;
                    case 2:
                        ch = r.Next(65, 90);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    case 3:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                }
            }
            return s;
        }
       
        public static Bitmap GenetareCaptchaImage()
        {
            //First declare a bitmap and declare graphic from this bitmap
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bitmap);
            //And create a rectangle to delegete this image graphic 
            Rectangle rect = new Rectangle(0, 0, width, height);
            //And create a brush to make some drawings
            HatchBrush hatchBrush = new HatchBrush(HatchStyle.DottedGrid, Color.Aqua, Color.White);
            g.FillRectangle(hatchBrush, rect);

            //here we make the text configurations
            GraphicsPath graphicPath = new GraphicsPath();
            //add this string to image with the rectangle delegate
            graphicPath.AddString(captchaText, FontFamily.GenericMonospace, (int)FontStyle.Bold, 90, rect, null);
            //And the brush that you will write the text
            hatchBrush = new HatchBrush(HatchStyle.Percent20, Color.Black, Color.GreenYellow);
            g.FillPath(hatchBrush, graphicPath);
            //We are adding the dots to the image
            for (int i = 0; i < (int)(rect.Width * rect.Height / 50F); i++)
            {
                int x = rnd.Next(width);
                int y = rnd.Next(height);
                int w = rnd.Next(10);
                int h = rnd.Next(10);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }
            //Remove all of variables from the memory to save resource
            hatchBrush.Dispose();
            g.Dispose();
            //return the image to the related component
            return bitmap;
        }
        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }
}
