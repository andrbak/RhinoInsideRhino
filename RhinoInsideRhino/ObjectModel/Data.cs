using Rhino.Geometry;
using RhinoInsideRhino.ObjectModel;
using System;
using System.Collections.Generic;
using System.Drawing;



[Serializable]
public class Data
    {



    public bool DisplayOnly { get; set; } = true;

    public List<Guid> BakedObjectIds { get; set; } = new List<Guid>();

    public Dictionary<string, ParameterObject> Parameters { get; set; }

    public string outputId { get; set; } = string.Empty;

    public string token { get; set; } = string.Empty;

    public List<object> GeneratedGeometries { get; set; }

    public string ModelId { get; set; } = "";



    public Color Color { get; set; } = Color.Blue;
        public int Thickness { get; set; } = 5;

        public Data()
        {
        }
        protected Data(Data other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            Color = other.Color;
            Thickness = other.Thickness;
        }

        public Data Clone()
        {
            return new Data(this);
        }


}

