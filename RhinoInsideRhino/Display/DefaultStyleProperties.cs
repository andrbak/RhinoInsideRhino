using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInsideRhino.Display
{
    public static class DefaultStyleProperties
    {
        public static Dictionary<string, object> SelectedStyle = new Dictionary<string, object>
            {
                { "thickness", 5.0 },
                { "transparency", 0.4 },
                { "color", Color.Red },
                { "OutlineColor", Color.Red }
            };
        public static Dictionary<string, object> BaseStyle = new Dictionary<string, object>
            {
                { "thickness", 5.0 },
                { "transparency", 0.4 },
                { "color", Color.Blue },
                { "OutlineColor", Color.Blue}
            };


    }

}
