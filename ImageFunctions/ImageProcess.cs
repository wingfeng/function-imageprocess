using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace ImageFunctions
{
    public static class ImageProcess
    {
        private static readonly string BLOB_STORAGE_CONNECTION_STRING = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
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
                            ((PngEncoder)encoder).CompressionLevel = quality; //当Qulity不是默认值时修改png的压缩比；
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
        [FunctionName("ImageProcess")]
        public static async Task Run([BlobTrigger("images/{name}", Connection = "AzureWebJobsStorage")]Stream input, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {input.Length} Bytes");
            try
            {
                if (input != null)
                {

                    var extension = Path.GetExtension(name);
                    var encoder = GetEncoder(extension);

                    if (encoder != null)
                    {

                        var thumbContainerName = Environment.GetEnvironmentVariable("OUTPUT_CONTAINER_NAME");
                        var storageAccount = CloudStorageAccount.Parse(BLOB_STORAGE_CONNECTION_STRING);
                        var blobClient = storageAccount.CreateCloudBlobClient();
                        var container = blobClient.GetContainerReference(thumbContainerName);


                        var commands = await loadCommand();
                       
                        using (Image<Rgba32> image = Image.Load(input))
                        {
                            foreach (var com in commands)
                            {
                                using (var output = new MemoryStream())
                                {
                                    var processor = new Processor(image.Clone(), com);

                                    var outImages = processor.Process();
                                    if (com.Output != null)
                                    {
                                        var quality = com.Output.Quality;
                                        if (!string.IsNullOrWhiteSpace(com.Output.Format))
                                        {

                                            encoder = GetEncoder(com.Output.Format, quality);
                                        }
                                        else
                                            encoder = GetEncoder(extension, quality);
                                    }

                                    outImages.Save(output, encoder);
                                    output.Position = 0;
                                    var outputName = combineOutputName(name, com);
                                    var blockBlob = container.GetBlockBlobReference(outputName);
                                    await blockBlob.UploadFromStreamAsync(output);
                                }
                            }
                        }
                    }
                    else
                    {
                        log.LogInformation($"No encoder support for: {name}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
                throw;
            }
        }
        private static string combineOutputName(string srcName,Command command)
        {
            string name = Path.GetFileNameWithoutExtension(srcName);
            string ext = Path.GetExtension(srcName);
            string result = "";
            switch (command.Name)
            {
                case "resize":
                    result = $"{name}_resize_w{command.Width}_h{command.Height}";
                    break;
                case "crop":
                    result = $"{name}_crop_x{command.X}_y{command.Y}_w{command.Width}_h{command.Height}";
                    break;
                case "round":
                    result = $"{name}_round_w{command.Width}_h{command.Height}_r{command.RoundRadius}";
                    break;
                case "combo":
                    result = $"{name}_combo";
                    break;

            }
            if (command.Output!=null && command.Output.Quality != 100)
            {
                result = $"{result}_q{command.Output.Quality}";
            }
            if (!string.IsNullOrWhiteSpace(command.Output?.Format))
            {
                return result+"."+command.Output.Format;
            }
            return result+ext;
        }
        private static async Task<Command[]> loadCommand()
        {
            var storageAccount = CloudStorageAccount.Parse(BLOB_STORAGE_CONNECTION_STRING);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var ContainerName = Environment.GetEnvironmentVariable("SRC_CONTAINER_NAME");
            var container = blobClient.GetContainerReference(ContainerName);
            var blockBlob = container.GetBlockBlobReference("commands.json");
            var commands = JsonConvert.DeserializeObject<Command[]>(await blockBlob.DownloadTextAsync());
            return commands;
        }
   
    }
}
