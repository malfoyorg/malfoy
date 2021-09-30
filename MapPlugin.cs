﻿namespace Malfoy
{
    public class MapPlugin : PluginBase
    {
        private static string _outputHashPath = "";
        private static string _outputWordPath = "";

        public static void Process(MapOptions options)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var fileEntries = Directory.GetFiles(currentDirectory, options.InputPath);

            if (fileEntries.Length == 0)
            {
                WriteMessage($"Lookup file(s) {options.InputPath} not found.");
                return;
            }

            var mapEntries = new string[] { };

            if (options.MapPath == "")
            {
                WriteMessage("No map path specified. Creating blank worldlist.");
            }
            else
            {
                mapEntries = Directory.GetFiles(currentDirectory, options.MapPath);

                if (mapEntries.Length == 0)
                {
                    WriteMessage($"Map file(s) {options.MapPath} was not found.");
                    return;
                }
            }

            if (options.Hash > 0) WriteMessage($"Validating hash mode {options.Hash}.");

            WriteMessage($"Started at {DateTime.Now.ToShortTimeString()}.");

            //Load map file entries
            var map = new List<string>();

            WriteMessage("Loading map files.");

            foreach (var mapEntry in mapEntries)
            {
                map.AddRange(File.ReadAllLines(mapEntry));
            }

            //Cater for blank scenario ie no map entries
            if (map.Count == 0) map.Add("");

            var size = GetFileEntriesSize(fileEntries);
            var progressTotal = 0L;
            var lineCount = 0;

            foreach (var filePath in fileEntries)
            {
                //Create a version based on the file size, so that the hash and dict are bound together
                var fileInfo = new FileInfo(filePath);
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var filePathName = $"{currentDirectory}\\{fileName}";

                _outputHashPath = $"{filePathName}.map.hash";
                _outputWordPath = $"{filePathName}.map.word";

                //Check that there are no output files
                if (!CheckForFiles(new string[] { _outputHashPath, _outputWordPath }))
                {
                    WriteHighlight($"Skipping {filePathName}.");

                    progressTotal += fileInfo.Length;
                    continue;
                }

                var hashes = new List<string>();
                var words = new List<string>();

                //Loop through and check if each email contains items from the lookup, if so add them
                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var splits = line.Split(':');

                        if (splits.Length == 2)
                        {
                            if (!ValidateEmail(splits[0], out var emailStem)) continue;
                            if (!ValidateHash(splits[1], options.Hash)) continue;

                            //Loop through the map and add hash and word pair
                            foreach (var word in map)
                            {
                                hashes.Add(splits[1]);
                                words.Add(word);
                            }
                        }

                        lineCount++;
                        progressTotal += line.Length;

                        //Update the percentage
                        WriteProgress("Processing files", progressTotal, size);
                    }
                }

                File.AppendAllLines(_outputHashPath, hashes);
                File.AppendAllLines(_outputWordPath, words);
            }

            WriteMessage($"Completed at {DateTime.Now.ToShortTimeString()}.");            
        }
    }
}
