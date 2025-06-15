using System.Collections.Generic;
using System.IO;
using System;


namespace RhinoInsideRhino.ObjectModel
{
    public static class MacroLoader
    {
        public static List<Macro> LoadMacrosFromSubfolders(string parentFolder)
        {
            var macros = new List<Macro>();

            if (!Directory.Exists(parentFolder))
                return macros;

            var subdirectories = Directory.GetDirectories(parentFolder);

            foreach (var subdir in subdirectories)
            {
                try
                {
                    var jsonFiles = Directory.GetFiles(subdir, "*.json", SearchOption.TopDirectoryOnly);
                    if (jsonFiles.Length == 0)
                        continue;

                    // If multiple JSON files, choose first one. Adjust logic if needed.
                    string jsonContent = File.ReadAllText(jsonFiles[0]);
                    var macro = Macro.FromJson(jsonContent);

                    macro.ImagePath = Path.Combine(subdir, macro.ImagePath);

                    macros.Add(macro);
                }
                catch (Exception ex)
                {
                    // Log or handle error (e.g., malformed JSON, IO issues)
                    Console.WriteLine("Error loading macro from " + subdir + ": " + ex.Message);
                }
            }

            return macros;
        }
    }
}