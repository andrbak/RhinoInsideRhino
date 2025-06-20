using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;


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
                    var jsonFiles = Directory.GetFiles(subdir, "*config.json", SearchOption.TopDirectoryOnly);
                    if (jsonFiles.Length == 0)
                        continue;
                    var jsonSecret = Directory.GetFiles(subdir, ".secrets.json", SearchOption.TopDirectoryOnly);
                    Dictionary<string,string> jsonSecretContent;
                    var jsonContentDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(jsonFiles[0]));
                    if (jsonSecret.Length == 0)
                    {
                        var rootSecret = Directory.GetFiles(parentFolder, ".secrets.json", SearchOption.TopDirectoryOnly);
                        if (rootSecret.Length == 0)
                        {
                            continue; // No secrets file found, skip this macro
                        }
                        
                        jsonSecretContent = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(rootSecret[0]))[jsonContentDict["gh_doc_uuid"]];
                    }
                    else
                    {
                        jsonSecretContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(jsonSecret[0]));
                    }
                    // If multiple JSON files, choose first one. Adjust logic if needed.
                    jsonContentDict["token"] = jsonSecretContent.ContainsKey(jsonContentDict["token_key"]) ? jsonSecretContent[jsonContentDict["token_key"]] : jsonContentDict["token"];
                    jsonContentDict.Remove("token_key");
                    jsonContentDict["model_id"] = jsonSecretContent.ContainsKey(jsonContentDict["model_id_key"]) ? jsonSecretContent[jsonContentDict["model_id_key"]] : jsonContentDict["model_id"];
                    jsonContentDict.Remove("model_id_key");
                    var jsonContent = JsonConvert.SerializeObject(jsonContentDict, Formatting.Indented);
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