using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Rhino.Display;
using Rhino.Geometry;

namespace RhinoInsideRhino.Display
{
    public static class GeometryPreview
    {
        private static CurvePreviewConduit _curveConduit;
        private static BrepPreviewConduit _brepConduit;
        private static MeshPreviewConduit _meshConduit;

        public static void Show(object geo , Color c, int Thickness, double transparency)
        {
            switch (geo)
            {
                case Curve crv:
                    ShowOrUpdateCurve(crv, c, Thickness);
                    break;
                case Brep brep:
                    ShowOrUpdateBrep(brep, Color.Teal, Thickness , transparency);
                    break;
                case Mesh mesh:
                    ShowOrUpdateMesh(mesh, Color.Orange , transparency);
                    break;

            }

            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }
        public static void Show(object geo, Dictionary<string, object> style)
        {
            // Defaults
            Color color = Color.Red;
            int thickness = 2;
            double transparency = 0.5;
            Color OutlineColor = Color.Blue;

            if (style != null)
            {
                if (style.TryGetValue("color", out object colObj) && colObj is Color col)
                    color = col;

                if (style.TryGetValue("thickness", out object thickObj) && thickObj is double thick)
                    thickness = (int)thick;

                if (style.TryGetValue("transparency", out object transObj) && transObj is double trans)
                    transparency = trans;
                if (style.TryGetValue("OutlineColor", out object OutColObj) && OutColObj is Color OutCol)
                    OutlineColor = OutCol;
            }

            // Use the existing function
            Show(geo, color, thickness, transparency);
        }

        public static void ShowOrUpdateCurve(Curve curve, Color color, int thickness)
        {
            if (_curveConduit == null)
            {
                _curveConduit = new CurvePreviewConduit(curve, color, thickness);
                _curveConduit.Enabled = true;
            }
            else
            {
                _curveConduit.Update(curve, color, thickness);
            }
        }
        public static void ShowOrUpdateCurve(Curve curve, Dictionary<string, object> style)
        {
            //initiating vars
            Color color = Color.Red;
            int thickness = 2;

            if (style != null)
            {
                if (style.TryGetValue("OutlineColor", out object c) && c is Color col)
                    color = col;

                if (style.TryGetValue("thickness", out object t) && t is double thick)
                    thickness = (int)thick;
            }

            ShowOrUpdateCurve(curve, color, thickness);
        }

        public static void ShowOrUpdateBrep(Brep brep, Color color, int thickness, double transparency)
        {
            if (_brepConduit == null)
            {
                _brepConduit = new BrepPreviewConduit(brep, color, thickness , transparency       );
                _brepConduit.Enabled = true;
            }
            else
            {
                _brepConduit.Update(brep, color, thickness, transparency);
            }
        }

        public static void ShowOrUpdateMesh(Mesh mesh, Color color, double transparency)
        {
            if (_meshConduit == null)
            {
                _meshConduit = new MeshPreviewConduit(mesh, color, transparency);
                _meshConduit.Enabled = true;
            }
            else
            {
                _meshConduit.Update(mesh, color, transparency);
            }
        }

        public static void ClearAll()
        {
            _curveConduit?.Disable();
            _curveConduit = null;

            _brepConduit?.Disable();
            _brepConduit = null;

            _meshConduit?.Disable();
            _meshConduit = null;

            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }
    }

    class CurvePreviewConduit : DisplayConduit
    {
        private Curve _curve;
        private Color _color;
        private int _thickness;

        public CurvePreviewConduit(Curve curve, Color color, int thickness)
        {
            _curve = curve;
            _color = color;
            _thickness = thickness;
        }

        public void Update(Curve curve, Color color, int thickness)
        {
            _curve = curve;
            _color = color;
            _thickness = thickness;
        }

        protected override void PreDrawObjects(DrawEventArgs e)
        {
            if (_curve != null)
                e.Display.DrawCurve(_curve, _color, _thickness);
        }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            if (_curve != null)
                e.IncludeBoundingBox(_curve.GetBoundingBox(true));
        }

        public void Disable() => Enabled = false;
    }

    class BrepPreviewConduit : DisplayConduit
    {
        private Brep _brep;
        private Color _color;
        private int _thickness;
        private double _transparency;

        public BrepPreviewConduit(Brep brep, Color color, int thickness, double transparency)
        {
            _brep = brep;
            _color = color;
            _thickness = thickness;
            _transparency = transparency;
        }

        public void Update(Brep brep, Color color, int thickness, double transparency)
        {
            _brep = brep;
            _color = color;
            _thickness = thickness;
            _transparency = transparency;
        }

        protected override void PreDrawObjects(DrawEventArgs e)
        {
            if (_brep != null)
            {
                var material = new DisplayMaterial(_color, _transparency);
                e.Display.DrawBrepShaded(_brep, material);
                e.Display.DrawBrepWires(_brep, _color, _thickness);
            }
        }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            if (_brep != null)
                e.IncludeBoundingBox(_brep.GetBoundingBox(true));
        }

        public void Disable() => Enabled = false;
    }

    class MeshPreviewConduit : DisplayConduit
    {
        private Mesh _mesh;
        private Color _color;
        private double _transparency; 

        public MeshPreviewConduit(Mesh mesh, Color color, double transparency)
        {
            _mesh = mesh;
            _color = color;
            _transparency = transparency; 
        }

        public void Update(Mesh mesh, Color color, double transparency)
        {
            _mesh = mesh;
            _color = color;
            _transparency = transparency; 
        }

        protected override void PreDrawObjects(DrawEventArgs e)
        {
            if (_mesh != null)
            {
                var mat = new DisplayMaterial(_color, 0.5);
                e.Display.DrawMeshShaded(_mesh, mat);
                e.Display.DrawMeshWires(_mesh, _color);
            }
        }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            if (_mesh != null)
                e.IncludeBoundingBox(_mesh.GetBoundingBox(true));
        }

        public void Disable() => Enabled = false;
    }
}
