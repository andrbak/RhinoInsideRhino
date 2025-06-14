using System;
using Rhino;
using Rhino.Commands;
using Rhino.Input.Custom;
using Rhino.Input;
using Rhino.DocObjects;
using RhinoInsideRhino.ObjectModel;

namespace RhinoInsideRhino.Commands
{
    public class CreateCurveHostObject : Command
    {
        public CreateCurveHostObject()
        {
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static CreateCurveHostObject Instance { get; private set; }

        public override string EnglishName => "CreateCurveHostObject";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // Setup object getter
            GetObject go = new GetObject();
            go.SetCommandPrompt("Select one or more curves");

            go.GeometryFilter = ObjectType.Curve;
            go.AcceptNothing(false);




            // set up the options
            OptionToggle boolOption = new OptionToggle(false, "No", "Yes");
            go.AddOptionToggle("KeepOriginals", ref boolOption);



            // Get options and objects
            while (true)
            {
                GetResult get_rc = go.GetMultiple(1, 0);
                if (go.CommandResult() != Result.Success)
                    return go.CommandResult();


                if (get_rc == GetResult.Option)
                {
                    continue;
                }



                break;
            }

            if (go.CommandResult() != Result.Success)
                return go.CommandResult();



            bool keepOriginal = boolOption.CurrentValue;

            // Collect selected curves and create curve host objects
            foreach (var obj in go.Objects())
            {
                var curve = obj.Curve();
                if (curve == null) continue;

                CurveHostObject curveHostObjects = new CurveHostObject(curve);
                RhinoDoc.ActiveDoc.Objects.AddRhinoObject(curveHostObjects, curve);

                // Optionally, delete the original Brep object if KeepOriginal is false
                if (!keepOriginal)
                {
                    doc.Objects.Delete(obj, true);
                }
            }

            return Result.Success;
        }
    }
}