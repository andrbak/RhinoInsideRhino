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
            RhinoApp.WriteLine("Object Changed");
            // Serialize the data to JSON
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
                    ["id"] = Data.ModelId.ToString(),
                    ["type"] = "GH"
                }
            };
            // Construct the HTTP POST request
            RhinoApp.WriteLine(requestBody.ToString());
            RhinoApp.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody));
            var request1 = (HttpWebRequest)WebRequest.Create("https://api.prod.configurator-backend.modelup3d.com/configurator?projectId=F6rZ0t1o");
            request1.Method = "GET";
            //request1.ContentType = "application/json";
            //request1.UserAgent = "AecTech25Hack";
            request1.Headers.Add("Authorization", $"Bearer {Data.token}");
            // Send the request to Modelup

            string output1 = string.Empty;
            // Read and display the server response
            using (WebResponse response = request1.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string txt_out = reader.ReadToEnd();
                    output1 = txt_out;
                }
            }

            RhinoApp.WriteLine(output1);

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
            var decompressedOutputs = JObject.Parse(Decompress(output))["geometry"];
            // Deserialize the response
            var outputData = new List<Rhino.Geometry.GeometryBase>();
            foreach (var decompressedOutput in decompressedOutputs)
            {
                var _geom = (Rhino.Geometry.GeometryBase)Rhino.Geometry.GeometryBase.FromJSON(decompressedOutput.ToString());
                outputData.Add(_geom);
            }
            ;
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
