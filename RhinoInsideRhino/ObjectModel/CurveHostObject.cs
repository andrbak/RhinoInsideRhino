using Rhino.Display;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInsideRhino.ObjectModel
{
    class CurveHostObject : CustomCurveObject, IHostObject
    {
        private CurveHostUserData _userData => this.Attributes.UserData.Find(typeof(CurveHostUserData)) as CurveHostUserData;

        public Data Data => _userData.Data;

        public CurveHostObject() : this(new LineCurve())
        {
        }

        public CurveHostObject(Curve curve) : this(curve, new CurveHostUserData())
        {
        }

        public CurveHostObject(Curve curve, CurveHostUserData userData) : base(curve)
        {
            Initialize(userData);
        }
        private void Initialize(CurveHostUserData hostUserData)
        {
            Attributes.UserData.Add(hostUserData);
        }


        public override string ShortDescription(bool plural)
        {
            return plural ? "Host Curves" : "Host Curve";
        }


        protected override void OnDraw(DrawEventArgs e) //TODO: Perhaps replace this with display conduit
        {


            Color baseColor = this.Data.Color;
            Color selectedColor = Rhino.ApplicationSettings.AppearanceSettings.SelectedObjectColor;
            Color color  = IsSelected(false) == 2 ? selectedColor : baseColor;
            int thickness = IsSelected(false) == 2 ? 1 : Data.Thickness;
            e.Display.DrawCurve(this.CurveGeometry, color, thickness);
   

        }

        public void Update()
        {
            //Call Compute here!
        }
    }
}
