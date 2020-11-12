using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RimworldParser
{
    internal class RawItems
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Formatting = Formatting.Indented,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };

        public RawItems()
        {
            Items = new List<RawItem>();
        }

        [JsonProperty("items")]
        public List<RawItem> Items { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        public static RawItems FromJson(string json)
        {
            RawItems items = JsonConvert.DeserializeObject<RawItems>(json, Settings);

            items.Items.Sort((item1, item2) =>
            {
                if (!string.Equals(item1.Category, item2.Category, StringComparison.OrdinalIgnoreCase))
                    return string.Compare(item1.Category, item2.Category, StringComparison.OrdinalIgnoreCase);

                return string.Compare(item1.Name, item2.Name, StringComparison.OrdinalIgnoreCase);
            });

            return items;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Settings);
        }
    }

    internal class RawItem
    {
        [JsonProperty("abr")]
        public string Name { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("defname")]
        public string Definition { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }
    }
}
