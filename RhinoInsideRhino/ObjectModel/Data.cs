﻿using Rhino.Geometry;
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

    public string outputId { get; set; } = "txt_out";

    public string Token { get; set; } = "eyJhbGciOiJIUzI1NiJ9.eyJ2NW4iOjEsImlkIjoxMzAsInA3ZCI6IkY2clowdDFvIiwicDlzIjpbImIyYiIsImM1ZSIsInJfYzE1YSJdLCJleHAiOjE3NTIzMjgwMDl9.KxhJiGG4dUd8xETkHihbEq2YBSu9BZRUy_JiQN-RRiU";

    public List<object> GeneratedGeometries { get; set; }

    public string ModelId { get; set; } = "857500a6-39cd-42f9-bfa8-e5114afc3eb5";

    public string ActiveModelId { get; set; } = "857500a6-39cd-42f9-bfa8-e5114afc3eb5";



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
            DisplayOnly = other.DisplayOnly;

            BakedObjectIds = other.BakedObjectIds != null
                ? new List<Guid>(other.BakedObjectIds)
                : null;

            Parameters = other.Parameters != null
                ? new Dictionary<string, ParameterObject>(other.Parameters)
                : null;

            outputId = other.outputId;
            token = other.token;

            GeneratedGeometries = other.GeneratedGeometries != null
                ? new List<object>(other.GeneratedGeometries)
                : null;

            ModelId = other.ModelId;
            ActiveModelId = other.ActiveModelId;
        }

    public Data Clone()
    {
        return new Data(this);
    }


}

