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


            Color generatedGeometryColor = Color.DarkCyan;
            Color baseColor = this.Data.Color;
            Color selectedColor = Rhino.ApplicationSettings.AppearanceSettings.SelectedObjectColor;
            Color color = IsSelected(false) == 2 ? selectedColor : baseColor;
            int thickness = IsSelected(false) == 2 ? 1 : Data.Thickness;

            // Draw primary curve
            if (this.CurveGeometry != null)
                e.Display.DrawCurve(this.CurveGeometry, color, thickness);

            // Draw generated geometries
            if (Data.GeneratedGeometries != null)
            {
                foreach (var geom in Data.GeneratedGeometries)
                {
                    if (geom == null)
                        continue;

                    if (geom is Curve curve)
                    {
                        e.Display.DrawCurve(curve, generatedGeometryColor, thickness);
                    }
                    else if (geom is Brep brep)
                    {
                        e.Display.DrawBrepShaded(brep, new DisplayMaterial(generatedGeometryColor));
                        e.Display.DrawBrepWires(brep, generatedGeometryColor, thickness);
                    }
                    else if (geom is Mesh mesh)
                    {
                        e.Display.DrawMeshShaded(mesh, new DisplayMaterial(generatedGeometryColor));
                        e.Display.DrawMeshWires(mesh, generatedGeometryColor);
                    }
                    else if (geom is Point3d point)
                    {
                        e.Display.DrawPoint(point, PointStyle.Simple, 3, generatedGeometryColor);
                    }
                    else if (geom is Point3d pt)
                    {
                        e.Display.DrawPoint(pt, PointStyle.Simple, 3, generatedGeometryColor);
                    }
                    else if (geom is Line line)
                    {
                        e.Display.DrawLine(line, generatedGeometryColor, thickness);
                    }
                    else if (geom is Polyline polyline)
                    {
                        e.Display.DrawPolyline(polyline, generatedGeometryColor, thickness);
                    }
                    // Add more types as needed
                }
            }
        }

        public void Update()
        {
            //Call Compute here!

            RhinoApp.WriteLine($"Object ({Id}) has been updated!");
        }
    }
}
