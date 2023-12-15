using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace UPrompt.Core
{
    public class UImage
    {
        public static string GetCopyOfImage(string path, bool AutoRevertColor = false)
        {
            string VisualDir = $@"{UCommon.Application_Path}Resources\Visual\";
            string RealImagePath;

            // Create Visual directory if not exist
            if (!Directory.Exists(VisualDir)) { Directory.CreateDirectory(VisualDir); }

            // Donwload if it a url copy if it a local file
            if (IsUrl(path))
            {
                RealImagePath = VisualDir + GetFileNameFromUrl(path);

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(path, RealImagePath);
                }
            }
            else
            {
                RealImagePath = VisualDir + GetFileLocalPath(path);
                File.Copy(path, RealImagePath, true);
            }

            // If application should mange image theme automatically revert color if dark
            if (IsDark(UCommon.Windows.TitleBar.BackColor) && AutoRevertColor)
            {
                Image TempImage = Image.FromFile(RealImagePath);
                ReverseImageColors(TempImage, RealImagePath);
            }
            return RealImagePath.Replace("\\","/");
        }
        internal static string GetFileLocalPath(string localPath)
        {
            string imageName = Path.GetFileName(localPath);
            return imageName;
        }
        internal static string GetFileNameFromUrl(string url)
        {
            Uri uri = new Uri(url);
            string imageName = Path.GetFileName(uri.LocalPath);
            return imageName;
        }
        internal static bool IsDark(Color color)
        {
            double perceivedBrightness = (color.R * 0.299 + color.G * 0.587 + color.B * 0.114) / 255;
            return perceivedBrightness <= 0.5;
        }

        private static bool IsUrl(string path)
        {
            Uri uriResult;
            bool isUrl = Uri.TryCreate(path, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return isUrl;
        }
        private static void ReverseImageColors(Image image, string outputFilePath)
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
