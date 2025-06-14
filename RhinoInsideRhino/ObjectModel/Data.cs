using Rhino.Geometry;
using RhinoInsideRhino.ObjectModel;
using System;
using System.Collections.Generic;
using System.Drawing;



[Serializable]
public class Data
    {


    public Dictionary<string, ParameterObject> Parameters { get; set; }

    public List<object> GeneratedGeometries { get; set; }

    Guid ModelId { get; set; } = Guid.Empty;



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

