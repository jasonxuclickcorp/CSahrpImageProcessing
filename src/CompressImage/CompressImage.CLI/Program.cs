using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace CompressImage.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            //SageImageByQuality();
            ScaleImageV2();
            //SageImageByQuality();
            Console.ReadLine();
        }

        public static void SageImageByQuality()
        {
            using (var image = Image.FromFile(@"TestImages\v2\01.jpg"))
            {
                var path = Path.Combine("TestImages", Guid.NewGuid().ToString() + ".jpg");
                SaveJpeg(path, image, 50);
            }
        }

        /// <summary> 
        /// Saves an image as a jpeg image, with the given quality 
        /// </summary> 
        /// <param name="path"> Path to which the image would be saved. </param> 
        /// <param name="quality"> An integer from 0 to 100, with 100 being the highest quality. </param> 
        public static void SaveJpeg(string path, Image img, int quality)
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

            // Encoder parameter for image quality 
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
            // JPEG image codec 
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            img.Save(path, jpegCodec, encoderParams);
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }

        public static void ScaleImage()
        {
            using (var image = Image.FromFile(@"TestImages\036.jpg"))
            using (var newImage = ScaleImage(image, 1500, 550))
            {
                if (newImage == null)
                {
                    return;
                }
                newImage.Save(Path.Combine("TestImages", Guid.NewGuid().ToString() + ".jpg"), ImageFormat.Jpeg);
            }
        }

        public static void ScaleImageV2()
        {
            var files = Directory.GetFiles(@"D:\github\CSahrpImageProcessing\src\CompressImage\CompressImage.CLI\bin\Debug\TestImages\v2");
            foreach (var item in files)
            {
                Stopwatch stopwatch = new Stopwatch();
                var name = Path.GetFileNameWithoutExtension(item);
                //stopwatch.Start();
                using (var image = Image.FromFile(item))
                {
                    //stopwatch.Stop();
                    //Console.WriteLine("Read File: " + stopwatch.ElapsedMilliseconds);
                    stopwatch.Start();
                    using (var newImage = ScaleImageV2(image, 1500, 550))
                    {
                        if (newImage == null)
                        {
                            return;
                        }
                        stopwatch.Stop();
                        Console.WriteLine("From:" + image.Width + "*" + image.Height + "To:" + newImage.Width + "*" + newImage.Height);
                        Console.WriteLine("Compress stopwatch: " + stopwatch.ElapsedMilliseconds);
                        //stopwatch.Start();
                        newImage.Save(Path.Combine("TestImages", "v2", name + "-rs" + ".jpg"), ImageFormat.Jpeg);
                        //stopwatch.Stop();
                        //Console.WriteLine("Compress+Save: " + stopwatch.ElapsedMilliseconds);
                    }
                }
            }

        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);
            if (ratio > 1)
            {
                ratio = 1;
            }
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
        public static Image ScaleImageV2(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);
            if (ratio > 1)
            {
                ratio = 1;
            }
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            return resizeImage(image, newWidth, newHeight);
        }
        public static Image resizeImage(Image image, int width, int height)
        {
            var destinationRect = new Rectangle(0, 0, width, height);
            var destinationImage = new Bitmap(width, height);

            destinationImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destinationImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destinationRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return (Image)destinationImage;
        }
    }
}
