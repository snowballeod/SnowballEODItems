using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RimworldParser
{
    internal class RawStoreTraits
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

        public RawStoreTraits()
        {
            Traits = new List<RawTrait>();
            Races  = new List<Race>();
        }

        [JsonProperty("traits")]
        public List<RawTrait> Traits { get; set; }

        [JsonProperty("races")]
        public List<Race> Races { get; set; }

        public static RawStoreTraits FromJson(string json)
        {
            RawStoreTraits rawStoreTraits = JsonConvert.DeserializeObject<RawStoreTraits>(json, Settings);

            rawStoreTraits.Traits.Sort((item1, item2) =>
            {
                return string.Compare(item1.Name, item2.Name, StringComparison.OrdinalIgnoreCase);
            });

            return rawStoreTraits;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Settings);
        }
    }

    internal class Race
    {
        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("defName")]
        public string DefinedName { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        public RaceData Data { get; set; }
    }

    internal class RaceData
    {
        [JsonProperty("customName")]
        public bool CustomName { get; set; }
    }

    internal class RawTrait
    {
        public RawTrait()
        {
            Stats     = new List<string>();
            Conflicts = new List<string>();
        }

        [JsonProperty("canAdd")]
        public bool CanAdd { get; set; }

        [JsonProperty("canRemove")]
        public bool CanRemove { get; set; }

        [JsonProperty("addPrice")]
        public int AddPrice { get; set; }

        [JsonProperty("removePrice")]
        public int RemovePrice { get; set; }

        [JsonProperty("defName")]
        public string DefName { get; set; }

        [JsonProperty("degree")]
        public int Degree { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        public RawTraitData Data { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("stats")]
        public List<string> Stats { get; set; }

        [JsonProperty("conflicts")]
        public List<string> Conflicts { get; set; }

        [JsonProperty("bypassLimit")]
        public bool BypassLimit { get; set; }
    }

    internal class RawTraitData
    {
        public RawTraitData()
        {
            Conflicts = new List<string>();
            Stats     = new List<string>();
        }

        [JsonProperty("canBypassLimit")]
        public bool CanBypassLimit { get; set; }

        [JsonProperty("conflicts")]
        public List<string> Conflicts { get; set; }

        [JsonProperty("customName")]
        public bool CustomName { get; set; }

        [JsonProperty("stats")]
        public List<string> Stats { get; set; }
    }
}