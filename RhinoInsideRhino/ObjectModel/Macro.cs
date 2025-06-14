using Newtonsoft.Json;
using System;


namespace RhinoInsideRhino.ObjectModel
{
    public class Macro
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("img_path")]
        public string ImagePath { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("model_id")]
        public string ModelId { get; set; } = string.Empty;

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("gh_doc_uuid")]
        public string GhDocUuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        public static Macro FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Macro>(json);
        }
    }
}