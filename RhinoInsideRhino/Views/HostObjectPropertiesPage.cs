using System.Diagnostics;
using Eto.Forms;
using Eto.Drawing;
using Rhino.DocObjects;
using Rhino.UI;
using System;
using RhinoInsideRhino.ObjectModel;
using System.Collections.Generic;


namespace RhinoInsideRhino.Views
{
    public class HostObjectPropertiesPage : ObjectPropertiesPage
    {
        private HostObjectPropertiesPageControl m_page_control;

        public override string EnglishPageTitle
        {
            get { return "HostObjectProperties"; }
        }

        //Current page content, add conditions to show different content based on UserData
        public override bool ShouldDisplay(ObjectPropertiesPageEventArgs e)
        {
            bool rc = false;

            if (e.ObjectCount > 0)
            {
                foreach (var obj in e.Objects)
                {
                    if (obj is CurveHostObject)
                    {
                        rc = true;
                        break;
                    }
                }
            }
            return rc;
        }
        // Check if object is a HostObject
        public override object PageControl
        {
            get
            {
                return (m_page_control ?? (m_page_control = new HostObjectPropertiesPageControl(SelectedObjects)));
            }
        }

    }

    public class HostObjectPropertiesPageControl : Panel
    {
        private readonly HostObjectPropertiesPage _page;

        public HostObjectPropertiesPageControl(RhinoObject[] selectedObjects)  // Perhaps this takes an argument to display different content)
        {
            //Check if all selected objects are using the same model
            var allSameModel = true;
            var modellist = new Dictionary<string, string>();
            foreach (var obj in selectedObjects)
            {
                var data = obj.Attributes.UserData.Find(typeof(CurveHostUserData)) as CurveHostUserData;
                if (data != null)
                {
                    if (data.Data.ModelId != Guid.Empty)
                    {
                        modellist[data.Data.ModelId.ToString()] = data.Data.token;
                    }
                }
            }
            var layout = new DynamicLayout();
            layout.AddSeparateRow(new Label { Text = "This is a Smart Object" });
            Content = layout;
        }

    }
}

