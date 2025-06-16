using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RhinoInsideRhino.ObjectModel;

public static class ParameterParser
{
    public static Dictionary<string, ParameterObject> ParseInputs(string json)
    {
        var result = new Dictionary<string, ParameterObject>();
        var jObject = JObject.Parse(json);

        var inputs = jObject["data"]?["getProject"]?["activeModel"]?["inputs"] as JArray;
        if (inputs == null) return result;

        foreach (var input in inputs)
        {
            string type = input["type"]?.ToString();
            string style = input["style"]?.ToString();
            string name = input["name"]?.ToString();
            string id = input["id"]?.ToString();

            ParameterObject param = null;

            if (type == "NUMBER" && style == "slider")
            {
                param = new SliderParameterObject
                {
                    Name = name,
                    Min = input["min"]?.ToObject<double>() ?? 0,
                    Max = input["max"]?.ToObject<double>() ?? 0,
                    DecimalPlaces = input["decimalPlaces"]?.ToObject<int>() ?? 0,
                    Value = input["numberValue"]?.ToObject<double?>()
                };
            }
            else if (type == "SELECT" && style == "dropdown")
            {
                var dropDown = new DropDownParameterObject
                {
                    Name = name,
                    Options = new List<DropDownOption>()
                };

                var options = input["selectOptions"] as JArray;
                if (options != null)
                {
                    foreach (var option in options)
                    {
                        dropDown.Options.Add(new DropDownOption
                        {
                            Text = option["text"]?.ToString(),
                            Value = Int32.Parse(option["value"]?.ToString())
                        });
                    }
                }

                // SelectValue is an array — assuming single selection for simplicity
                var selected = input["selectValue"] as JArray;
                if (selected != null && selected.Count > 0)
                {
                    dropDown.Value = selected[0].ToObject<int>();
                }

                param = dropDown;
            }

            if (param != null)
            {

                result.Add(id, param);
            }
        }

        return result;
    }
}
