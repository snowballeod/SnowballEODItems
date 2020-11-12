using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RimworldParser
{
    internal class ProcessedStoreTraits
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Formatting = Formatting.Indented,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };

        public ProcessedStoreTraits()
        {
            Traits = new List<ProcessedTrait>();
        }

        [JsonProperty("traits")]
        public List<ProcessedTrait> Traits { get; set; }

        [JsonProperty("total")]
        public int Total
        {
            get => Traits.Count;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Settings);
        }
    }

    internal class ProcessedTrait
    {
        private static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;

        public ProcessedTrait()
        {
            Attributes = new List<string>();
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("attributes")]
        public List<string> Attributes { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        public static ProcessedTrait FromRawTrait(RawTrait rawTrait)
        {
            ProcessedTrait processedTrait = new ProcessedTrait();

            processedTrait.Name        = TextInfo.ToTitleCase(rawTrait.Name);
            processedTrait.Description = rawTrait.Description;
            processedTrait.Attributes  = rawTrait.Stats;
            processedTrait.Price       = rawTrait.AddPrice;

            return processedTrait;
        }
    }
}
