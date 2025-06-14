using Rhino.DocObjects.Custom;
using Rhino.FileIO;
using Rhino.Geometry;
using RhinoInsideRhino.Commands;
using RhinoInsideRhino.Converters;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace RhinoInsideRhino.ObjectModel
{


    // You must define a Guid attribute for your user data derived class
    // in order to support serialization. Every custom user data class
    // needs a custom Guid
    [Guid("be1d5f13-ca1a-42e9-868f-5504aad62539")]
    public class CurveHostUserData : UserData, IHostUserData
    {

     public SerializableData Data { get; set; } = new SerializableData();

    public override bool ShouldWrite => true;

    public CurveHostUserData()
    {
            Data = new SerializableData();
    }

    protected override void OnDuplicate(UserData source)
    {

        var src = source as CurveHostUserData;

        if (src != null)
        {
            Data = src.Data.Clone();
        }
    }

    protected override bool Read(BinaryArchiveReader archive)
    {
        try
        {
            var jsonString = System.Text.Encoding.UTF8.GetString(archive.ReadByteArray());

            var options = new JsonSerializerOptions();
            options.Converters.Add(new ColorJsonConverter());

            Data = JsonSerializer.Deserialize<SerializableData>(jsonString, options);

            return true;
        }
        catch
        {
            return false;
        }
    }

    protected override bool Write(BinaryArchiveWriter archive)
    {
        try
        {

            var options = new JsonSerializerOptions();
            options.Converters.Add(new ColorJsonConverter());

            var jsonString = JsonSerializer.Serialize(Data, options);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(jsonString);
            archive.WriteByteArray(byteArray);

            return true;
        }
        catch
        {
            return false;
        }
    }

}

    }

