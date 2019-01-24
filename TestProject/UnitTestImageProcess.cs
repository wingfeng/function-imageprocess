using ImageFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Text.RegularExpressions;

namespace TestProject
{
    [TestClass]
    public class UnitTestImageProcess
    {
        private static IImageEncoder GetEncoder(string extension, int quality = 100)
        {

            IImageEncoder encoder = null;

            extension = extension.Replace(".", "");

            var isSupported = Regex.IsMatch(extension, "gif|png|jpe?g", RegexOptions.IgnoreCase);

            if (isSupported)
            {
                switch (extension)
                {
                    case "png":

                        encoder = new PngEncoder();
                        if (quality != 100)
                            ((PngEncoder)encoder).CompressionLevel = quality; //��Qulity����Ĭ��ֵʱ�޸�png��ѹ���ȣ�
                        break;
                    case "jpg":
                        encoder = new JpegEncoder() { Quality = quality };
                        break;
                    case "jpeg":
                        encoder = new JpegEncoder() { Quality = quality };
                        break;
                    case "gif":
                        encoder = new GifEncoder();
                        break;
                    default:
                        break;
                }
            }

            return encoder;
        }

        private const string ImageSrcPath = @"D:\github\function-image-upload-resize\TestProject\8.jpg";
        private const string OutputPath = @"D:\output\";
        [TestMethod]
        public void TestResize()
        {
            var command = new Command
            {
                Name = "resize",
                Width = 400,
                IsStretch = false,
            };
            Image<Rgba32> image = Image.Load(ImageSrcPath);
            var processor = new Processor(image, command);
            image = processor.Process();
            processOutput(command, image, "resize_no_stretch.jpg");

        }
        [TestMethod]
        public void TestResize2()
        {
            var command = new Command
            {
                Name = "resize",
                Width = 400,
                Height = 100,
                IsStretch = true,
            };
            Image<Rgba32> image = Image.Load(ImageSrcPath);
            var processor = new Processor(image, command);
            image = processor.Process();
            processOutput(command, image, "resize_stretch.jpg");

        }
        [TestMethod]
        public void TestCrop400X100()
        {
            var command = new Command
            {
                Name = "crop",
                X = 100,
                Y = 100,
                Width = 400,
                Height = 100,

            };
            Image<Rgba32> image = Image.Load(ImageSrcPath);
            var processor = new Processor(image, command);
            image = processor.Process();
            processOutput(command, image, "crop_400x100.jpg");

        }
        [TestMethod]
        public void TestRound()
        {
            for (int i = 0; i < 10; i++)
            {
                var command = new Command
                {
                    Name = "round",
                    Width = 400,
                    Height = 200,
                    RoundRadius = 5,
                    Output = new Output()
                    {
                        Format = "gif"
                    }
                };
                Image<Rgba32> image = Image.Load(ImageSrcPath);
                var processor = new Processor(image, command);
                image = processor.Process();
                processOutput(command, image, "round_400x200_50.gif");
            }
        }
        [TestMethod]
        public void TestWaterMark()
        {

            var command = new Command
            {
                Name = "combo",
                Commands = new Command[]
               {
                   new Command()
                   {
                        Name = "watermark",
                    WaterMarkPath= @"D:\github\function-image-upload-resize\TestProject\watermark.png",
                    Pos = 0,
                    Opacity = 10f
                   },
                   new Command()
                   {
                        Name = "watermark",
                    WaterMarkPath= @"D:\github\function-image-upload-resize\TestProject\watermark.png",
                    Pos = 1,
                    Opacity = 20f
                   },
                    new Command()
                   {
                        Name = "watermark",
                    WaterMarkPath= @"D:\github\function-image-upload-resize\TestProject\watermark.png",
                    Pos = 2,
                    Opacity = 50f
                   },
                   new Command()
                   {
                        Name = "watermark",
                    WaterMarkPath= @"https://wingshare.blob.core.chinacloudapi.cn/vip/imagesharp-logo.png",
                    Pos = 3,
                    Opacity = 80f
                   },
                   new Command()
                   {
                        Name = "watermark",
                    WaterMarkPath= @"https://wingshare.blob.core.chinacloudapi.cn/vip/imagesharp-logo.png",
                    Pos = 4,
                    Opacity = 100f
                   }
               }

            };
            Image<Rgba32> image = Image.Load(ImageSrcPath);
            var processor = new Processor(image, command);
            image = processor.Process();
            processOutput(command, image, "waterMark.jpg");

        }
        [TestMethod]
        public void TestCombo()
        {
            for (int i = 0; i < 10; i++)
            {
                var command = new Command
                {
                    Name = "combo",
                    Commands = new Command[]
                    {
                        new Command
                            {
                                Name = "crop",
                                X=100,
                                Y=100,
                                Width = 400,
                                Height = 100,

                            },
                        new Command
                            {
                                Name = "round",
                                Width = 400,
                                Height = 100,
                                RoundRadius = 15,
                                Output = new Output()
                                {
                                    Format = "gif"
                                }
                            }
                    },
                    Output = new Output()
                    {
                        Format = "gif"
                    }
                };
                Image<Rgba32> image = Image.Load(ImageSrcPath);
                var processor = new Processor(image, command);
                image = processor.Process();
                processOutput(command, image, "combo_crop_round.gif");
            }
        }
        private void processOutput(Command com, Image<Rgba32> src, string name)
        {
            string outputName = OutputPath + name;
            var extension = Path.GetExtension(outputName);
            var encoder = GetEncoder(extension);
            if (com.Output != null)
            {
                var quality = com.Output.Quality;
                if (string.IsNullOrWhiteSpace(com.Output.Format))
                {
                    outputName = outputName.Replace(extension, $".{com.Output.Format}");
                    encoder = GetEncoder(com.Output.Format, quality);
                }
                else
                    encoder = GetEncoder(extension, quality);
            }
            src.Save(outputName, encoder);
        }
    }
}
