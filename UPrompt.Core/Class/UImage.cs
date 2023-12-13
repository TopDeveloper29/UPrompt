using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace UPrompt.Core
{
    internal class UImage
    {
        internal static bool IsDark(Color color)
        {
            double perceivedBrightness = (color.R * 0.299 + color.G * 0.587 + color.B * 0.114) / 255;
            return perceivedBrightness <= 0.5;
        }
        internal static bool IsUrl(string path)
        {
            Uri uriResult;
            bool isUrl = Uri.TryCreate(path, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return isUrl;
        }
        internal static void DownloadImage(string path, string outputLocation)
        {
            if (IsUrl(path))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(path, outputLocation);
                }
            }
            else
            {
                File.Copy(path, outputLocation, true);
            }
        }
        internal static string GetImageNameFromLocalPath(string localPath)
        {
            string imageName = Path.GetFileName(localPath);
            return imageName;
        }
        internal static string GetImageNameFromUrl(string url)
        {
            Uri uri = new Uri(url);
            string imageName = Path.GetFileName(uri.LocalPath);
            return imageName;
        }

        internal static void ReverseImageColors(Image image, string outputFilePath)
        {
            if (image != null)
            {
                Bitmap originalBitmap = new Bitmap(image.Width, image.Height);
                using (Graphics graphics = Graphics.FromImage(originalBitmap))
                {
                    using (ImageAttributes attributes = new ImageAttributes())
                    {
                        ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                        {
                    new float[] {-1, 0, 0, 0, 0},
                    new float[] {0, -1, 0, 0, 0},
                    new float[] {0, 0, -1, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {1, 1, 1, 0, 1}
                        });

                        attributes.SetColorMatrix(colorMatrix);
                        graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                            0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                    }
                }
                image.Dispose();
                File.Delete(outputFilePath);
                originalBitmap.Save(outputFilePath);
            }
        }
        internal static void ReverseImageColors(PictureBox[] pictureBoxs)
        {
            foreach (PictureBox pictureBox in pictureBoxs)
            {
                Image image = pictureBox.Image;
                if (image != null)
                {
                    Bitmap originalBitmap = new Bitmap(image.Width, image.Height);
                    using (Graphics graphics = Graphics.FromImage(originalBitmap))
                    {
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                            {
                                new float[] {-1, 0, 0, 0, 0},
                                new float[] {0, -1, 0, 0, 0},
                                new float[] {0, 0, -1, 0, 0},
                                new float[] {0, 0, 0, 1, 0},
                                new float[] {1, 1, 1, 0, 1}
                            });

                            attributes.SetColorMatrix(colorMatrix);
                            graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                                0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                        }
                    }
                    pictureBox.Image = originalBitmap;
                }
            }
        }

    }
}
