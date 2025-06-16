using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInsideRhino.ObjectModel
{
    public abstract class ParameterObject
    {
        public Action ValueChanged;
        public string Type { get; set; }

        public object Value { get; set; }   

        public string Name { get; set; }    

        public string Id { get; set; }

        public abstract Control GetEtoControl();

        public ParameterObject()
        {
            ValueChanged = () => { };
        }
    }
}
