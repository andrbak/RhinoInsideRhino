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
            RhinoApp.WriteLine("Object Changed");
            // Serialize the data to JSON
            var inputsJson = new Dictionary<string, object>
            {
                ["txt_in"] = Compress(Geometry.ToJSON(new Rhino.FileIO.SerializationOptions()))
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
                    ["model"] = Data.ModelId.ToString(),
                    ["type"] = "GH"
                }
            };
            // Construct the HTTP POST request

            RhinoApp.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody).ToString());
            var request = (HttpWebRequest)WebRequest.Create("https://api.prod.configurator-backend.modelup3d.com/compute?outputId=" + Data.outputId);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = "AecTech25Hack";
            request.Headers.Add("Authorization", $"Bearer {Data.token}");
            request.Headers.Add("x-compute-server", "wilfred-r8");
            byte[] byteArray = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody));
            request.ContentLength = byteArray.Length;
            // Send the request to Modelup
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            string output = string.Empty;
            // Read and display the server response
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string txt_out = reader.ReadToEnd();
                    output = txt_out;
                }
            }

            // Decompress the response
            string decompressedOutput = Decompress(output);
            // Deserialize the response
            var outputData = Rhino.Geometry.GeometryBase.FromJSON(decompressedOutput);
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

    }
}
