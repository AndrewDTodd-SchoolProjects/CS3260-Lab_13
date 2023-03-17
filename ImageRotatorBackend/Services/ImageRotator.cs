using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using Microsoft.Maui.Platform;
using System.Reflection.Metadata;

namespace ImageRotatorBackend.Services
{
    public class ImageRotator
    {
        public static System.Drawing.Bitmap RotateImage(FileResult imagePath, double degrees)
        {
#if WINDOWS
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imagePath.FullPath);

            int widthSrd = (bitmap.Width * bitmap.Width);
            int heightSrd = (bitmap.Height * bitmap.Height);

            int hypotenus = (int)Math.Sqrt(widthSrd + heightSrd);
            int lOne = (int)Math.Sqrt((widthSrd / 2));
            int lTwo = (int)Math.Sqrt((heightSrd / 2));

            System.Drawing.Bitmap scaledMap = new(hypotenus, lOne + lTwo);

            scaledMap.MakeTransparent();

            using (Graphics g = Graphics.FromImage(scaledMap))
            {
                Rectangle destRegion = new Rectangle((scaledMap.Width - bitmap.Width) / 2, (scaledMap.Height - bitmap.Height) / 2, bitmap.Width, bitmap.Height);
                Rectangle srcRegion = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

                // Set the rotation point to the center in the matrix
                g.TranslateTransform(scaledMap.Width / 2, scaledMap.Height / 2);
                // Rotate
                g.RotateTransform((float)degrees);
                // Restore rotation point in the matrix
                g.TranslateTransform(-scaledMap.Width / 2, -scaledMap.Height / 2);

                // Draw the image on the bitmap
                g.DrawImage(bitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
            }

            return scaledMap;
#else
            throw new NotImplementedException();
#endif
        }
    }
}
