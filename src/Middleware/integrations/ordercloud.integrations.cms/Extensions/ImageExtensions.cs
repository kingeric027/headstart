using Humanizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace ordercloud.integrations.cms.Extensions
{
	public static class ImageExtensions
	{
        public static Image Resize(this Image image, int targetWidth)
        {
            int tagetHeight = image.Height * (targetWidth / image.Width); // preserve aspect ratio
            var destRect = new Rectangle(0, 0, targetWidth, tagetHeight);
            var destImage = new Bitmap(targetWidth, tagetHeight);

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

            return (Image) destImage;
        }
    }
}
