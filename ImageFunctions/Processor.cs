using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace ImageFunctions
{
   public class Processor
    {
        private Command _command;
        private Image<Rgba32> _src;
   
        public Processor(Image<Rgba32> src, Command command) {
            _command = command;
            _src = src;
        }
        public Image<Rgba32> Process()
        {
            Image<Rgba32> output = null;
            switch (_command.Name.ToLower())
            {
                case "resize":
                  output=  resize(_src,_command.Width,_command.Height,_command.IsStretch);
                    break;
                case "crop":
                    output = crop(_src, (int)_command.X, (int)_command.Y, _command.Width, (int)_command.Height);
                    break;

                case "round":
                    output = round(_src, _command.Width, (int)_command.Height, _command.RoundRadius);
                    break;
                case "combo":
                    output = _src;
                    foreach(Command c in _command.Commands)
                    {
                        Processor p = new Processor(output,c);
                        output = p.Process();
                    }
                   
                    break;
            }

            return output;
        }

        private  Image<Rgba32> resize(Image<Rgba32> src, int width, int? height = null, bool isStretch = true)
        {
            var divisor = src.Width / width;
            if (height == null)
                height = Convert.ToInt32(Math.Round((decimal)(src.Height / divisor)));

            src.Mutate(x => x.Resize(width, (int)height,isStretch));
            return src;
            //image.Save(output, encoder);
            //output.Position = 0;
            //await blockBlob.UploadFromStreamAsync(output);
        }

        private  Image<Rgba32> waterMark(Image<Rgba64> src, Image<Rgba32> mark, int x, int y)
        {
        
            throw new NotImplementedException();
        }

        private  Image<Rgba32> crop(Image<Rgba32> src, int x, int y, int width, int height, bool isSmart=false)
        {
            var rect = new Rectangle(x, y, width, height);

        
            src.Mutate(o => o.Crop(rect));
            return src;
        }

        private Image<Rgba32> round(Image<Rgba32> src,int width,int height,float roundRadius)
        {
            Image<Rgba32> destRound = src.CloneAndConvertToAvatarWithoutApply(new Size(width, height), (float)roundRadius);
            return destRound;
        }

        #region ConvertToAvatar

      


        // This method can be seen as an inline implementation of an `IImageProcessor`:
        // (The combination of `IImageOperations.Apply()` + this could be replaced with an `IImageProcessor`)
        public static void ApplyRoundedCorners(Image<Rgba32> img, float cornerRadius)
        {
            IPathCollection corners = BuildCorners(img.Width, img.Height, cornerRadius);

            var graphicOptions = new GraphicsOptions(true)
            {
                BlenderMode = PixelBlenderMode.Src // enforces that any part of this shape that has color is punched out of the background
            };
            // mutating in here as we already have a cloned original
            img.Mutate(x => x.Fill(graphicOptions, Rgba32.Transparent, corners));
        }

        public static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            // first create a square
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            // then cut out of the square a circle so we are left with a corner
            IPath cornerToptLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the orgional artound the center of the image
            var center = new Vector2(imageWidth / 2F, imageHeight / 2F);

            float rightPos = imageWidth - cornerToptLeft.Bounds.Width + 1;
            float bottomPos = imageHeight - cornerToptLeft.Bounds.Height + 1;

            // move it across the widthof the image - the width of the shape
            IPath cornerTopRight = cornerToptLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerToptLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerToptLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerToptLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }
        #endregion
    }
}
