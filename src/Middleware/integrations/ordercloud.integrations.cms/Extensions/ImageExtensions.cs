using Humanizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ordercloud.integrations.cms
{
	public static class ImageExtensions
	{
        public static Image CreateThumbnail(this Image image)
        {
            double scaleFactor = 200 / Math.Min(image.Width, image.Height); // bring the smaller dimension to 200 px.
            if (scaleFactor > 1) return image; // don't increase image size
            return image.Resize(scaleFactor);
        }

        public static Image Resize(this Image image, double scaleFactor)
		{
            var (targetWidth, targetHeight) = (((int)(image.Width * scaleFactor), (int)(image.Height * scaleFactor))); // preserve aspect ratio
            var destRect = new Rectangle(0, 0, targetWidth, targetHeight);
            var destImage = new Bitmap(targetWidth, targetHeight);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }




        public static byte[] ToBytes(this Image image, ImageFormat format)
        {
            using (var stream = new MemoryStream())
			{
                image.Save(stream, format);
                return stream.ToArray();
            }
		}
    }
}
