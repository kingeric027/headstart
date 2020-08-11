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
        public static Image CreateSquareThumbnail(this Image srcImage, int targetDimension)
        {
            var aspectRatio = srcImage.Width / srcImage.Height;
            if (aspectRatio > 2 || aspectRatio < 0.5)
			{
                return srcImage.CreateSquareWhiteSpaced(targetDimension);
			} else
			{
                return srcImage.CreateSquareCropped(targetDimension);
            }
        }

        private static Image CreateSquareCropped(this Image srcImage, int targetDimension)
		{
            var scaleFactor = targetDimension / (double)Math.Min(srcImage.Width, srcImage.Height);
            if (scaleFactor > 1) return null; // Don't increase image size
            var srcDimension = (int)(targetDimension / scaleFactor);
            var rectX = (srcImage.Width - srcDimension) / 2;
            var rectY = (srcImage.Height - srcDimension) / 2;
            var destRect = new Rectangle(0, 0, targetDimension, targetDimension);
            var srcRect = new Rectangle(rectX, rectY, srcDimension, srcDimension);

            return srcImage.CreateSquare(targetDimension, srcRect, destRect);
        }

        private static Image CreateSquareWhiteSpaced(this Image srcImage, int targetDimension)
        {
            var scaleFactor = targetDimension / (double)Math.Max(srcImage.Width, srcImage.Height);
            if (scaleFactor > 1) return null; // Don't increase image size
            var rectWidth = (int)(srcImage.Width * scaleFactor);
            var rectHeight = (int)(srcImage.Height * scaleFactor);
            var rectX = (targetDimension - rectWidth) / 2;
            var rectY = (targetDimension - rectHeight) / 2;
            var destRect = new Rectangle(rectX, rectY, rectWidth, rectHeight);
            var srcRect = new Rectangle(0, 0, srcImage.Width, srcImage.Height);

            return srcImage.CreateSquare(targetDimension, srcRect, destRect);
        }

        private static Image CreateSquare(this Image srcImage, int targetDimension, Rectangle srcRect, Rectangle destRect)
		{
            var destImage = new Bitmap(targetDimension, targetDimension);
            destImage.SetResolution(srcImage.HorizontalResolution, srcImage.VerticalResolution);

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
                    graphics.DrawImage(srcImage, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, wrapMode);
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
