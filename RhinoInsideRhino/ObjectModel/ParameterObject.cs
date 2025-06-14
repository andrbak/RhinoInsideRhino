using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInsideRhino.ObjectModel
{
    public class ParameterObject
    {

        public string Type { get; set; }

        public object Value { get; set; }   

        public string Name { get; set; }    
    }





    public class SliderParameterObject : ParameterObject
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Step { get; set; }
        public SliderParameterObject()
        {
            Type = "Slider";
            Min = 0.0;
            Max = 10.0;
            Step = 1.0;
        }
    }
}
