using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RimworldParser
{
    internal enum KarmaType
    {
        Good,
        Neutral,
        Bad,
        Doom,
    };

    internal class RawEvents
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

        [JsonProperty("incitems")]
        public List<RawEvent> Events { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        public static RawEvents FromJson(string json)
        {
            RawEvents events = JsonConvert.DeserializeObject<RawEvents>(json, Settings);

            events.Events.Sort((item1, item2) =>
            {
                if (item1.KarmaType != item2.KarmaType)
                    return item1.KarmaType - item2.KarmaType;

                return string.Compare(item1.Name, item2.Name, StringComparison.OrdinalIgnoreCase);
            });

            return events;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Settings);
        }
    }

    internal class RawEvent
    {
        [JsonProperty("abr")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("karmatype")]
        public KarmaType KarmaType { get; set; }
    }

    internal class KarmaTypeConverter : JsonConverter
    {
        public static readonly KarmaTypeConverter Singleton = new KarmaTypeConverter();

        public override bool CanConvert(Type t)
        {
            return t == typeof(KarmaType) || t == typeof(KarmaType?);
        }

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var value = serializer.Deserialize<string>(reader);

            switch (value)
            {
                case "Bad":
                    return KarmaType.Bad;

                case "Doom":
                    return KarmaType.Doom;

                case "Good":
                    return KarmaType.Good;

                case "Neutral":
                    return KarmaType.Neutral;
            }

            throw new Exception("Cannot unmarshal type Karmatype");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (KarmaType)untypedValue;

            switch (value)
            {
                case KarmaType.Bad:
                    serializer.Serialize(writer, "Bad");
                    return;

                case KarmaType.Doom:
                    serializer.Serialize(writer, "Doom");
                    return;

                case KarmaType.Good:
                    serializer.Serialize(writer, "Good");
                    return;

                case KarmaType.Neutral:
                    serializer.Serialize(writer, "Neutral");
                    return;
            }

            throw new Exception("Cannot marshal type Karmatype");
        }
    }
}
