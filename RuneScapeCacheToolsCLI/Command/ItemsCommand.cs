using System;
using Villermen.RuneScapeCacheTools.CLI.Argument;
using Villermen.RuneScapeCacheTools.Utility;

namespace Villermen.RuneScapeCacheTools.CLI.Command
{
    public class ItemsCommand : BaseCommand
    {
        private bool _skip = false;
        private string _file = "items.json";
        private string? _print = null;
        private bool _force = false;

        public ItemsCommand(ArgumentParser argumentParser) : base(argumentParser)
        {
            this.ArgumentParser.AddCommon(CommonArgument.SourceCache);
            this.ArgumentParser.Add(
                "file=",
                "Save item JSON to the given file (instead of \"items.json\").",
                (value) => { this._file = value; }
            );
            this.ArgumentParser.Add(
                "skip",
                "Skip items that can't be decoded.",
                (value) => { this._skip = true; }
            );
            this.ArgumentParser.Add(
                "force",
                "Force fresh extraction of JSON even when JSON version matches.",
                (value) => { this._force = true; }
            );
            this.ArgumentParser.Add(
                "print=",
                "Prints items matching the given filter. E.g., \"name:kwuarm\" or \"properties.unknown2195\".",
                (value) => this._print=value
            );
        }

        public override int Run()
        {
            var itemDefinitionExtractor = new ItemDefinitionExtractor();

            // Try to extract only when source is specified.
            using var sourceCache = this.ArgumentParser.SourceCache;
            if (sourceCache != null)
            {
                if (itemDefinitionExtractor.JsonMatchesCache(sourceCache, this._file))
                {
                    Console.WriteLine("Skipping extraction because JSON is up to date with cache.");
                }
                else
                {
                    itemDefinitionExtractor.ExtractItemDefinitions(sourceCache, this._file, this._skip);
                }
            }

            if (this._print != null)
            {
                itemDefinitionExtractor.PrintItemDefinitions(this._file, this._print);
            }

            return Program.ExitCodeOk;
        }
    }
}
