using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RimworldParser
{
    internal class ProcessedEvents
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Formatting = Formatting.Indented,
            Converters =
            {
                KarmaTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };

        public ProcessedEvents()
        {
            Events = new List<ProcessedEvent>();
        }

        [JsonProperty("events")]
        public List<ProcessedEvent> Events { get; set; }

        [JsonProperty("total")]
        public int Total
        {
            get => Events.Count;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Settings);
        }
    }

    internal class ProcessedEvent
    {
        private static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("karmatype")]
        public KarmaType KarmaType { get; set; }

        public static ProcessedEvent FromRawEvent(RawEvent rawEvent)
        {
            ProcessedEvent processedEvent = new ProcessedEvent();

            processedEvent.Name      = TextInfo.ToTitleCase(rawEvent.Name);
            processedEvent.Price     = rawEvent.Price;
            processedEvent.KarmaType = rawEvent.KarmaType;

            return processedEvent;
        }
    }
}
