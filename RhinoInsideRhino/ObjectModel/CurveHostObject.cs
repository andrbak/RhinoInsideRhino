using Eto.Forms;
using Rhino;
using Rhino.Display;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using System;
using System.Net;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RhinoInsideRhino.Display;
using RhinoInsideRhino.Requests;


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





            if (this.CurveGeometry != null && RhinoInsideRhinoPlugin.Instance.DisplayOptions.ShowHosts)
            {
                e.Display.DrawCurve(this.CurveGeometry, color, thickness);
                //GeometryPreview.ShowOrUpdateCurve(this.CurveGeometry, color, thickness);
                //GeometryPreview.Show(this.CurveGeometry, IsSelected(false) == 2 ? DefaultStyleProperties.SelectedStyle : DefaultStyleProperties.BaseStyle);
            }


            else
            {
                base.OnDraw(e);
            }


            //// Draw generated geometries
            if (Data.GeneratedGeometries != null && RhinoInsideRhinoPlugin.Instance.DisplayOptions.ShowGeneratedGeometries)
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



                // GeometryPreview.Show(Data.GeneratedGeometries, IsSelected(false) == 2 ? DefaultStyleProperties.SelectedStyle : DefaultStyleProperties.BaseStyle);


            }
        }

        public void Update()
        {
            RhinoApp.WriteLine("Object Changed");


            var inputsJson = new Dictionary<string, object>
            {
                ["txt_in"] = Compress(Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string[]> { ["geometry"] = new List<string> { Geometry.ToJSON(new Rhino.FileIO.SerializationOptions()), }.ToArray() }))
            };
            foreach (KeyValuePair<string, ParameterObject> parameter in Data.Parameters)
            {
                inputsJson[parameter.Key] = parameter.Value.Value;
            }

            var requestBody = new Dictionary<string, object>
            {
                ["inputs"] = inputsJson,
                ["model"] = new Dictionary<string, string>
                {
                    ["id"] = Data.ActiveModelId.ToString(),
                    ["type"] = "GH"
                }
            };
            // Construct the HTTP POST request
            
            RhinoInsideRhino.Requests.ModelUp modelup = new RhinoInsideRhino.Requests.ModelUp();

            string output = modelup.ComputeCall(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody));

            var decompressedOutputs = JObject.Parse(Decompress(output))["geometry"];
            // Deserialize the response
            var outputData = new List<object>();
            foreach (var decompressedOutput in decompressedOutputs)
            {
                var _geom = Rhino.Geometry.GeometryBase.FromJSON(decompressedOutput.ToString());
                outputData.Add(_geom);
            }
            ;

            Data.GeneratedGeometries = outputData;


            if (!Data.DisplayOnly)
            {
                BakeGeneratedGeometry();
            }

        }
        public static string Compress(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            var output = new MemoryStream();
            // leaveOpen=true lets us read the MemoryStream after disposing the GZipStream
            using (var gzip = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true))
            {
                gzip.Write(inputBytes, 0, inputBytes.Length);
            }
            return Convert.ToBase64String(output.ToArray());
        }

        public static string Decompress(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return base64;
            byte[] compressedBytes = Convert.FromBase64String(base64);
            var input = new MemoryStream(compressedBytes);
            var gzip = new GZipStream(input, CompressionMode.Decompress);
            var output = new MemoryStream();
            gzip.CopyTo(output);
            return Encoding.UTF8.GetString(output.ToArray());
        }


        public void AttachMacro(string modelId)
        {

            // Set ModelId in Data
            Data.ModelId = modelId;

            //Call compute here to get input parameters

            //Populate Data.Parameters with input parameters (parse json to ParameterObjects of the correct type)

            //For now just some hard coded values
            Data.Parameters = new Dictionary<string, ParameterObject>
            {
                { "Parameter1", new SliderParameterObject() {Name = "Thickness", Value = 5} }
            };
        }







        public void BakeGeneratedGeometry()
        {


            //Delete existing baked geometries
            foreach (var id in Data.BakedObjectIds)
            {
                if (RhinoDoc.ActiveDoc.Objects.Find(id) is Rhino.DocObjects.RhinoObject obj)
                {
                    RhinoDoc.ActiveDoc.Objects.Delete(obj, true);
                }
            }


            foreach (var geom in Data.GeneratedGeometries)
            {
                if (geom == null)
                    continue;

                if (geom is Curve curve)
                {
                    var id = RhinoDoc.ActiveDoc.Objects.AddCurve(curve);
                    Data.BakedObjectIds.Add(id);

                }
                else if (geom is Brep brep)
                {
                    var id = RhinoDoc.ActiveDoc.Objects.AddBrep(brep);
                    Data.BakedObjectIds.Add(id);
                }
                else if (geom is Mesh mesh)
                {
                    var id = RhinoDoc.ActiveDoc.Objects.AddMesh(mesh);
                    Data.BakedObjectIds.Add(id);
                }

                else if (geom is Point3d pt)
                {
                    var id = RhinoDoc.ActiveDoc.Objects.AddPoint(pt);
                    Data.BakedObjectIds.Add(id);
                }
                else if (geom is Line line)
                {
                    var id = RhinoDoc.ActiveDoc.Objects.AddLine(line);
                    Data.BakedObjectIds.Add(id);
                }
                else if (geom is Polyline polyline)
                {
                    var id = RhinoDoc.ActiveDoc.Objects.AddPolyline(polyline);
                    Data.BakedObjectIds.Add(id);

                    // Add more types as needed
                }


            }

        }


    }
}
