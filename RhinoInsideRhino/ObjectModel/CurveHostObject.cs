using Rhino.Display;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInsideRhino.ObjectModel
{
    class CurveHostObject : CustomCurveObject, IHostObject
    {

        //protected readonly CurveObjectBehavior _behavior = new CurveObjectBehavior();


        //public Action GeometryChanged => _behavior.GeometryChanged;

        public CurveHostObject() : this(new LineCurve())
        {
        }

        public CurveHostObject(Curve curve) : this(curve, new HostUserData())
        {
        }

        public CurveHostObject(Curve curve, HostUserData userData) : base(curve)
        {
            
        }

        public override string ShortDescription(bool plural)
        {
            return plural ? "Host Curves" : "Host Curve";
        }


        protected override void OnDraw(DrawEventArgs e)
        {

            //Color color = 

            if (IsSelected(false) ==1)
            {
                e.Display.DrawCurve(this.CurveGeometry, System.Drawing.Color.Red, 2);
            }
            else
            {
                e.Display.DrawCurve(this.CurveGeometry, System.Drawing.Color.Blue, 10);
            }

        }
    }
}
