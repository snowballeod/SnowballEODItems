using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RimworldParser
{
    internal class ProcessedItems
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

        public ProcessedItems()
        {
            Items = new List<ProcessedItem>();
        }

        [JsonProperty("items")]
        public List<ProcessedItem> Items { get; set; }

        [JsonProperty("total")]
        public int Total
        {
            get => Items.Count;
        }

        public void Sort()
        {
            Items.Sort((item1, item2) =>
            {
                if (!string.Equals(item1.Category, item2.Category, StringComparison.OrdinalIgnoreCase))
                    return string.Compare(item1.Category, item2.Category, StringComparison.OrdinalIgnoreCase);

                return string.Compare(item1.Name, item2.Name, StringComparison.OrdinalIgnoreCase);
            });
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Settings);
        }
    }

    internal class ProcessedItem
    {
        private static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        public static ProcessedItem FromRawItem(RawItem rawItem)
        {
            ProcessedItem processedItem = new ProcessedItem();

            processedItem.Name     = TextInfo.ToTitleCase(rawItem.Name);
            processedItem.Price    = rawItem.Price;
            processedItem.Category = rawItem.Category;

            return processedItem;
        }
    }
}
