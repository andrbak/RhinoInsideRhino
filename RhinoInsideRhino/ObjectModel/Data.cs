using Rhino.Geometry;
using RhinoInsideRhino.ObjectModel;
using System;
using System.Collections.Generic;
using System.Drawing;



[Serializable]
public class Data
    {


    public Dictionary<string, ParameterObject> Parameters { get; set; }

    public string outputId { get; set; } = "txt_out";

    public string token { get; set; } = "eyJhbGciOiJIUzI1NiJ9.eyJ2NW4iOjEsImlkIjoxMzAsInA3ZCI6IkY2clowdDFvIiwicDlzIjpbImIyYiIsImM1ZSIsInJfYzE1YSJdLCJleHAiOjE3NTIzMjgwMDl9.KxhJiGG4dUd8xETkHihbEq2YBSu9BZRUy_JiQN-RRiU";

    public List<object> GeneratedGeometries { get; set; }

    public string ModelId { get; set; } = "857500a6-39cd-42f9-bfa8-e5114afc3eb5";



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

