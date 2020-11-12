using System;
using System.IO;
using System.Reflection;

namespace RimworldParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                PrintHelp();
                return;
            }

            string commandFile  = ValidateFile(args[0]);
            string itemFile     = ValidateFile(args[1]);
            string eventFile    = ValidateFile(args[2]);
            string traitFile    = ValidateFile(args[3]);
            string outputFolder = Path.Combine(Path.GetDirectoryName(commandFile), "Output");

            ParseCommands(commandFile, outputFolder);
            ParseItems(itemFile, outputFolder);
            ParseEvents(eventFile, outputFolder);
            ParseTraits(traitFile, outputFolder);
        }

        private static void PrintHelp()
        {
            Assembly     assembly = Assembly.GetExecutingAssembly();
            AssemblyName name     = assembly.GetName();

            string exeName = $"{name.Name.ToLower()}.exe";

            Console.WriteLine($"Usage: {exeName} [raw trait file]");
            Console.WriteLine($"   EX: {exeName} C:\\Users\\Me\\Documents\\StoreExt.json");
        }

        private static string ValidateFile(string file)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"File '{file}' doesn't exist");
                Console.WriteLine();

                PrintHelp();

                Environment.Exit(0);
            }

            return file;
        }

        #region Parse Commands

        private static void ParseCommands(string commandFile, string outputFolder)
        {
            using (StreamReader reader = new StreamReader(commandFile))
            {
                RawCommands       rawCommands                = RawCommands.FromJson(reader.ReadToEnd());
                ProcessedCommands processedModeratorCommands = new ProcessedCommands();
                ProcessedCommands processedViewerCommands    = new ProcessedCommands();
                ProcessedCommands processedPawnCommands      = new ProcessedCommands();

                foreach (RawCommand rawCommand in rawCommands.Commands)
                {
                    if (string.IsNullOrWhiteSpace(rawCommand.Name))
                        continue;

                    if (rawCommand.Usage.StartsWith("!rwdata"))
                        continue;

                    ProcessedCommand processedCommand = ProcessedCommand.FromRawCommand(rawCommand);

                    if (rawCommand.UserLevel == UserLevel.Moderator)
                    {
                        processedModeratorCommands.Commands.Add(processedCommand);
                    }
                    else
                    {
                        if (IsPawnCommand(rawCommand.Usage))
                            processedPawnCommands.Commands.Add(processedCommand);
                        else
                            processedViewerCommands.Commands.Add(processedCommand);
                    }

                }

                string outputFileModerator = Path.Combine(outputFolder, "SiteCommandsModerator.json");
                string outputFileViewer    = Path.Combine(outputFolder, "SiteCommandsViewer.json");
                string outputFilePawn      = Path.Combine(outputFolder, "SiteCommandsPawn.json");

                GenerateCommandsOutput(processedModeratorCommands, outputFileModerator);
                GenerateCommandsOutput(processedViewerCommands, outputFileViewer);
                GenerateCommandsOutput(processedPawnCommands, outputFilePawn);
            }
        }

        private static bool IsPawnCommand(string command)
        {
            if (command.StartsWith("!insult"))
                return true;

            if (command.StartsWith("!leave"))
                return true;

            if (command.StartsWith("!levelskill"))
                return true;

            if (command.Contains("mypawn", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private static void GenerateCommandsOutput(ProcessedCommands commands, string outputFile)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine(commands.ToJson());
            }
        }

        #endregion

        #region Parse Items

        private static void ParseItems(string itemFile, string outputFolder)
        {
            using (StreamReader reader = new StreamReader(itemFile))
            {
                RawItems       rawItems       = RawItems.FromJson(reader.ReadToEnd());
                ProcessedItems processedItems = new ProcessedItems();

                foreach (RawItem rawItem in rawItems.Items)
                {
                    if (rawItem.Price < 0)
                        continue;

                    rawItem.Category = RecategorizeItem(rawItem);

                    ProcessedItem processedItem = ProcessedItem.FromRawItem(rawItem);
                    processedItems.Items.Add(processedItem);
                }

                processedItems.Sort();

                string outputFileItems = Path.Combine(outputFolder, "StoreItems.json");

                GenerateItemsOutput(processedItems, outputFileItems);
            }
        }

        private static string RecategorizeItem(RawItem item)
        {
            if (string.Equals(item.Category, "TMC Long Range", StringComparison.OrdinalIgnoreCase))
                return "Ranged weapons";

            if (string.Equals(item.Category, "Persona weapons", StringComparison.OrdinalIgnoreCase))
                return "Melee weapons";

            return item.Category;
        }

        private static void GenerateItemsOutput(ProcessedItems items, string outputFile)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine(items.ToJson());
            }
        }

        #endregion

        #region Parse Events

        private static void ParseEvents(string eventFile, string outputFolder)
        {
            using (StreamReader reader = new StreamReader(eventFile))
            {
                RawEvents       rawEvents       = RawEvents.FromJson(reader.ReadToEnd());
                ProcessedEvents processedEvents = new ProcessedEvents();

                foreach (RawEvent rawEvent in rawEvents.Events)
                {
                    if (int.Parse(rawEvent.Price) < 0)
                        continue;

                    if (IsIgnoredEvent(rawEvent.Name))
                        continue;

                    if (HasVariablePrice(rawEvent.Name))
                        rawEvent.Price = "Varies";

                    ProcessedEvent processedEvent = ProcessedEvent.FromRawEvent(rawEvent);
                    processedEvents.Events.Add(processedEvent);
                }

                string outputFileEvents = Path.Combine(outputFolder, "StoreEvents.json");

                GenerateEventsOutput(processedEvents, outputFileEvents);
            }
        }

        private static bool IsIgnoredEvent(string eventName)
        {
            if (string.Equals(eventName, "backpack", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private static bool HasVariablePrice(string eventName)
        {
            if (string.Equals(eventName, "trait", StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.Equals(eventName, "replacetrait", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private static void GenerateEventsOutput(ProcessedEvents events, string outputFile)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine(events.ToJson());
            }
        }

        #endregion

        #region Parse Traits

        private static void ParseTraits(string traitFile, string outputFolder)
        {
            using (StreamReader reader = new StreamReader(traitFile))
            {
                RawStoreTraits       rawTraits              = RawStoreTraits.FromJson(reader.ReadToEnd());
                ProcessedStoreTraits processedVanillaTraits = new ProcessedStoreTraits();
                ProcessedStoreTraits processedMagicTraits   = new ProcessedStoreTraits();

                foreach (RawTrait rawTrait in rawTraits.Traits)
                {
                    if (!rawTrait.CanAdd)
                        continue;

                    ProcessedTrait processedTrait = ProcessedTrait.FromRawTrait(rawTrait);

                    if (rawTrait.AddPrice == 4000)
                        processedMagicTraits.Traits.Add(processedTrait);
                    else
                        processedVanillaTraits.Traits.Add(processedTrait);
                }

                string outputFileVanilla = Path.Combine(outputFolder, "StoreTraitsVanilla.json");
                string outputFileMagic   = Path.Combine(outputFolder, "StoreTraitsMagic.json");

                GenerateTraitsOutput(processedVanillaTraits, outputFileVanilla);
                GenerateTraitsOutput(processedMagicTraits, outputFileMagic);
            }
        }

        private static void GenerateTraitsOutput(ProcessedStoreTraits traits, string outputFile)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine(traits.ToJson());
            }
        }

        #endregion
    }
}
