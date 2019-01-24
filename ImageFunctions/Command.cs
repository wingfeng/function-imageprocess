using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ImageFunctions
{
    public class Command
    {
        [JsonProperty("command")]
        public string Name { get; set; }
        [JsonProperty("x")]
        public int? X { get; set; }
        [JsonProperty("y")]
        public int? Y { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int? Height { get; set; }
        [JsonProperty("radius")]
        public float RoundRadius { get; set; }
        /// <summary>
        /// Watermark postion
        /// 0:top left;
        /// 1:top right;
        /// 2:bottom left;
        /// 3:bottom right;
        /// 4:central
        /// </summary>
        [JsonProperty("pos")]
        public int Pos { get; set; }
        [JsonProperty("watermark_url")]
        public string WaterMarkPath { get; set; }
        [JsonProperty("opacity")]
        public float Opacity { get; set; }
        [JsonProperty("isStretch")]
        public bool IsStretch { get; set; } 
        [JsonProperty("output")]
        public Output Output { get; set; }
        [JsonProperty("commands")]
        public Command[] Commands { get; set; }

        internal Stream GetWatermarkStream()
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(WaterMarkPath);
//            stream.Position = 0;
            return stream;
        }

    }
    public class Output
    {
        [JsonProperty("format")]
        public string Format { get; set; }
        [JsonProperty("quality")]
        public int Quality { get; set; } = 100;
    }

}
