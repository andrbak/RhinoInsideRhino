using Rhino.DocObjects.Custom;
using Rhino;
using Rhino.FileIO;
using Rhino.Geometry;
using RhinoInsideRhino.Commands;
using RhinoInsideRhino.Converters;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Http;

namespace RhinoInsideRhino.Requests
{
    public class ModelUp
    {
        public string bearerToken { get; set; } = "eyJhbGciOiJIUzI1NiJ9.eyJ2NW4iOjEsImlkIjoxMzAsInA3ZCI6IkY2clowdDFvIiwicDlzIjpbImIyYiIsImM1ZSIsInJfYzE1YSJdLCJleHAiOjE3NTIzMjgwMDl9.KxhJiGG4dUd8xETkHihbEq2YBSu9BZRUy_JiQN-RRiU";
        public string ModelInfo(string projectId, string bearerToken)
        {
            RhinoApp.WriteLine("Getting Modelinfo:"+projectId);
            var request = (HttpWebRequest)WebRequest.Create("https://api.prod.configurator-backend.modelup3d.com/configurator?projectId=" + projectId);
            request.Method = "GET";
            request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            string output = string.Empty;
            var res = request.GetResponse();
            
            StreamReader reader = new StreamReader(res.GetResponseStream());
            var r = reader.ReadToEnd();
            output = r;
        //    {
        //        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        //        {
        //            string txt_out = reader.ReadToEnd();
        //            output = txt_out;
        //        }
          //  }
            return output;
        }
        public string ComputeCall(string requestBody, string bearerToken)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.prod.configurator-backend.modelup3d.com/compute?outputId=txt_out");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = "AecTech25Hack";
            request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            request.Headers.Add("x-compute-server", "wilfred-r8");
            byte[] byteArray = Encoding.UTF8.GetBytes(requestBody);
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

            return output;
        }
    }

}