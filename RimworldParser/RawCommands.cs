using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RimworldParser
{
    internal enum UserLevel
    {
        Anyone,
        Moderator
    };

    internal class RawCommands
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Formatting = Formatting.Indented,
            Converters =
            {
                UserLevelConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };

        public RawCommands()
        {
            Commands = new List<RawCommand>();
        }

        public List<RawCommand> Commands { get; set; }

        public static RawCommands FromJson(string json)
        {
            RawCommands commands = new RawCommands()
            {
                Commands = JsonConvert.DeserializeObject<List<RawCommand>>(json, Settings)
            };

            commands.Commands.Sort((item1, item2) =>
            {
                return string.Compare(item1.Usage, item2.Usage, StringComparison.OrdinalIgnoreCase);
            });

            return commands;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this.Commands, Settings);
        }
    }

    internal class RawCommand
    {
        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("usage")]
        public string Usage { get; set; }

        [JsonProperty("userLevel")]
        public UserLevel UserLevel { get; set; }

        [JsonProperty("shortcut")]
        public bool Shortcut { get; set; }
    }

    internal class Data
    {
        [JsonProperty("isShortcut")]
        public bool IsShortcut { get; set; }
    }

    internal class UserLevelConverter : JsonConverter
    {
        public static readonly UserLevelConverter Singleton = new UserLevelConverter();

        public override bool CanConvert(Type t)
        {
            return t == typeof(UserLevel) || t == typeof(UserLevel?);
        }

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var value = serializer.Deserialize<string>(reader);

            switch (value)
            {
                case "Anyone":
                    return UserLevel.Anyone;

                case "Moderator":
                    return UserLevel.Moderator;
            }

            throw new Exception("Cannot unmarshal type UserLevel");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (UserLevel)untypedValue;

            switch (value)
            {
                case UserLevel.Anyone:
                    serializer.Serialize(writer, "Anyone");
                    return;

                case UserLevel.Moderator:
                    serializer.Serialize(writer, "Moderator");
                    return;
            }

            throw new Exception("Cannot marshal type UserLevel");
        }
    }
}
