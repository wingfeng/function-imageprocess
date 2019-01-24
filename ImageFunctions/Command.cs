using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        [JsonProperty("isStretch")]
        public bool IsStretch { get; set; } 
        [JsonProperty("output")]
        public Output Output { get; set; }
        [JsonProperty("commands")]
        public Command[] Commands { get; set; }

    }
    public class Output
    {
        [JsonProperty("format")]
        public string Format { get; set; }
        [JsonProperty("quality")]
        public int Quality { get; set; } = 100;
    }

}
