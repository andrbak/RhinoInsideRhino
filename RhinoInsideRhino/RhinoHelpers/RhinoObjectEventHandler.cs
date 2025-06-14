using Rhino;
using Rhino.DocObjects;
using RhinoInsideRhino.ObjectModel;
using System;
using System.Linq;

namespace RhinoInsideRhino.RhinoHelpers

{
    class RhinoObjectEventHandler
    {


        private bool _isOpeningDocument = false;


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
            //RhinoApp.WriteLine("Registering event handlers.");

            RhinoDoc.AddRhinoObject += OnAddRhinoObject;
            RhinoDoc.ReplaceRhinoObject += OnReplaceRhinoObject;
            RhinoDoc.DeleteRhinoObject += OnDeleteRhinoObject;
            RhinoDoc.UndeleteRhinoObject += OnUndeleteRhinoObject;
            RhinoDoc.SelectObjects += OnSelectRhinoObjects;
            RhinoDoc.DeselectObjects += OnDeselectRhinoObjects;
            RhinoDoc.DeselectAllObjects += OnDeselectAllRhinoObjects;

            RhinoDoc.BeginOpenDocument += OnBeginOpenDocument;
            RhinoDoc.EndOpenDocument += OnEndOpenDocument;
        }

        private void OnBeginOpenDocument(object sender, DocumentOpenEventArgs e)
        {
            _isOpeningDocument = true;
        }

        private void OnEndOpenDocument(object sender, DocumentOpenEventArgs e)
        {
            _isOpeningDocument = false;
        }

        private void UnRegister()
        {
            RhinoDoc.AddRhinoObject -= OnAddRhinoObject;
            RhinoDoc.ReplaceRhinoObject -= OnReplaceRhinoObject;
            RhinoDoc.DeleteRhinoObject -= OnDeleteRhinoObject;
            RhinoDoc.UndeleteRhinoObject -= OnUndeleteRhinoObject;
            RhinoDoc.SelectObjects -= OnSelectRhinoObjects;
            RhinoDoc.DeselectObjects -= OnDeselectRhinoObjects;
            RhinoDoc.DeselectAllObjects -= OnDeselectAllRhinoObjects;

            RhinoDoc.BeginOpenDocument -= OnBeginOpenDocument;
            RhinoDoc.EndOpenDocument -= OnEndOpenDocument;
        }




        public bool TryReplaceWithCustomObject(RhinoObject originalObject)
        {

            if (originalObject is IHostObject || !originalObject.Attributes.HasUserData)
                return false;

            foreach (var data in originalObject.Attributes.UserData)
            {

                if (data is CurveHostUserData userData)
                {
                    var customObj = HostObjectFactory.CreateCustomObject(userData, originalObject.Geometry);

                    if (customObj != null)
                    {
                        var success = RhinoDoc.ActiveDoc.Objects.Replace(new ObjRef(originalObject), customObj as RhinoObject);
                        return success;
                    }
                }
            }

            return false;
        }

        private void OnReplaceRhinoObject(object sender, RhinoReplaceObjectEventArgs e)
        {
            if (e.OldRhinoObject is IHostObject hostObject)
            {
                //RhinoApp.WriteLine("Replaced host object: " + hostObject.GetType().Name + " with Id: " + e.OldRhinoObject.Id + ".");
                //RhinoApp.WriteLine("With object: " + e.NewRhinoObject.GetType().Name + " with Id: " + e.OldRhinoObject.Id + ".");
            }
        }


        private void OnAddRhinoObject(object sender, RhinoObjectEventArgs e)
        {

            var obj = e.TheObject;

            // Try replacing with a custom object (will trigger OnAdd again if successful)
            if (TryReplaceWithCustomObject(obj))
                return;


            if (obj is IHostObject hostObject && !_isOpeningDocument)
            {
                //RhinoApp.WriteLine("Added host object: " + hostObject.GetType().Name + " with Id: " + e.TheObject.Id + ".");

                hostObject.Update();
            }
        }



        private void OnDeleteRhinoObject(object sender, RhinoObjectEventArgs e)
        {

            if (e.TheObject is IHostObject hostObject)
            {
                //RhinoApp.WriteLine("Deleted host object: " + hostObject.GetType().Name + " with Id: " + e.TheObject.Id + ".");
            }
        }

        private void OnUndeleteRhinoObject(object sender, RhinoObjectEventArgs e)
        {
            if (e.TheObject is IHostObject hostObject)
            {
                //RhinoApp.WriteLine("Undeleted host object: " + hostObject.GetType().Name + " with Id: " + e.TheObject.Id + ".");
            }
        }

        private void OnSelectRhinoObjects(object sender, RhinoObjectSelectionEventArgs e)
        {
            var hostObjects = e.RhinoObjects
                               .OfType<IHostObject>()
                               .ToArray();

            //RhinoApp.WriteLine("Selected " + hostObjects.Length + " host object(s), total selected: " + e.RhinoObjects.Length + " object(s).");
        }

        private void OnDeselectRhinoObjects(object sender, RhinoObjectSelectionEventArgs e)
        {
            var hostObjects = e.RhinoObjects
                               .OfType<IHostObject>()
                               .ToArray();

            //RhinoApp.WriteLine("Deselected " + hostObjects.Length + " host object(s), total deselected: " + e.RhinoObjects.Length + " object(s).");
        }

        private void OnDeselectAllRhinoObjects(object sender, RhinoDeselectAllObjectsEventArgs e)
        {
            //RhinoApp.WriteLine("Deselected all objects in document: " + e.Document.RuntimeSerialNumber + ".");
        }
    }
}
