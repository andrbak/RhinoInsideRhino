using Rhino;
using Rhino.DocObjects;
using System;
using System.Linq;

namespace RhinoInsideRhino.ObjectModel
{
    class RhinoObjectEventHandler
    {


        public RhinoObjectEventHandler()
        {
            Register();
        }

        ~RhinoObjectEventHandler()
        {
            UnRegister();
        }


        private void Register()
        {
            RhinoDoc.AddRhinoObject += OnAddRhinoObject;
            //RhinoDoc.ReplaceRhinoObject += OnReplaceRhinoObject;
            RhinoDoc.DeleteRhinoObject += OnDeleteRhinoObject;
            RhinoDoc.UndeleteRhinoObject += OnUndeleteRhinoObject;
            RhinoDoc.SelectObjects += OnSelectRhinoObjects;
            RhinoDoc.DeselectObjects += OnDeselectRhinoObjects;
            RhinoDoc.DeselectAllObjects += OnDeselectAllRhinoObjects;
        }

        private void UnRegister()
        {
            RhinoDoc.AddRhinoObject -= OnAddRhinoObject;
            //RhinoDoc.ReplaceRhinoObject += OnReplaceRhinoObject;
            RhinoDoc.DeleteRhinoObject -= OnDeleteRhinoObject;
            RhinoDoc.UndeleteRhinoObject -= OnUndeleteRhinoObject;
            RhinoDoc.SelectObjects -= OnSelectRhinoObjects;
            RhinoDoc.DeselectObjects -= OnDeselectRhinoObjects;
            RhinoDoc.DeselectAllObjects -= OnDeselectAllRhinoObjects;
        }

        private void OnAddRhinoObject(object sender, RhinoObjectEventArgs e)
        {
            if (e.TheObject is IHostObject hostObject)
            {
                RhinoApp.WriteLine("Added host object: " + hostObject.GetType().Name + " with Id: " + e.TheObject.Id + ".");
            }
        }



        private void OnDeleteRhinoObject(object sender, RhinoObjectEventArgs e)
        {

            if (e.TheObject is IHostObject hostObject)
            {
                RhinoApp.WriteLine("Deleted host object: " + hostObject.GetType().Name + " with Id: " + e.TheObject.Id + ".");
            }
        }

        private void OnUndeleteRhinoObject(object sender, RhinoObjectEventArgs e)
        {
            if (e.TheObject is IHostObject hostObject)
            {
                RhinoApp.WriteLine("Undeleted host object: " + hostObject.GetType().Name + " with Id: " + e.TheObject.Id + ".");
            }
        }

        private void OnSelectRhinoObjects(object sender, RhinoObjectSelectionEventArgs e)
        {
            var hostObjects = e.RhinoObjects
                               .OfType<IHostObject>()
                               .ToArray();

            RhinoApp.WriteLine("Selected " + hostObjects.Length + " host object(s), total selected: " + e.RhinoObjects.Length + " object(s).");
        }

        private void OnDeselectRhinoObjects(object sender, RhinoObjectSelectionEventArgs e)
        {
            var hostObjects = e.RhinoObjects
                               .OfType<IHostObject>()
                               .ToArray();

            RhinoApp.WriteLine("Deselected " + hostObjects.Length + " host object(s), total deselected: " + e.RhinoObjects.Length + " object(s).");
        }

        private void OnDeselectAllRhinoObjects(object sender, RhinoDeselectAllObjectsEventArgs e)
        {
            //RhinoApp.WriteLine("Deselected all objects in document: " + e.Document.RuntimeSerialNumber + ".");
        }
    }
}
