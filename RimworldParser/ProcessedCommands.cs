using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RimworldParser
{
    internal class ProcessedCommands
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

        public ProcessedCommands()
        {
            Commands = new List<ProcessedCommand>();
        }

        [JsonProperty("commands")]
        public List<ProcessedCommand> Commands { get; set; }

        [JsonProperty("total")]
        public int Total
        {
            get => Commands.Count;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Settings);
        }
    }

    internal class ProcessedCommand
    {
        [JsonProperty("usage")]
        public string Usage { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("level")]
        public UserLevel UserLevel { get; set; }

        public static ProcessedCommand FromRawCommand(RawCommand rawCommand)
        {
            ProcessedCommand processedCommand = new ProcessedCommand();

            processedCommand.Usage       = rawCommand.Usage;
            processedCommand.Description = rawCommand.Description;
            processedCommand.UserLevel   = rawCommand.UserLevel;

            return processedCommand;
        }
    }
}
