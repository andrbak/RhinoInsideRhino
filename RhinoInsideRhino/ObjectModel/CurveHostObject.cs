using Rhino;
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
using RhinoInsideRhino.Display;

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
        

        protected override void OnDraw(DrawEventArgs e)
        {


           


   
         
            if (this.CurveGeometry != null)
                //GeometryPreview.ShowOrUpdateCurve(this.CurveGeometry, color, thickness);
                GeometryPreview.Show(this.CurveGeometry, IsSelected(false) == 2 ? DefaultStyleProperties.SelectedStyle : DefaultStyleProperties.BaseStyle);
                
            //// Draw generated geometries
            if (Data.GeneratedGeometries != null)
            {

                GeometryPreview.Show(Data.GeneratedGeometries, IsSelected(false) == 2 ? DefaultStyleProperties.SelectedStyle : DefaultStyleProperties.BaseStyle);


            }
            }

        public void Update()
        {
            //Call Compute here!

            RhinoApp.WriteLine($"Object ({Id}) has been updated!");
        }
    }
}
