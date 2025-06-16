using System.Diagnostics;
using Eto.Forms;
using Eto.Drawing;
using Rhino.DocObjects;
using Rhino.UI;
using System;
using RhinoInsideRhino.ObjectModel;
using System.Collections.Generic;
using System.Linq;

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



        public override void UpdatePage(ObjectPropertiesPageEventArgs e)
        {

            m_page_control.Update(e.Objects);
            //m_page_control = new HostObjectPropertiesPageControl(e.Objects);
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

        public void Update(RhinoObject[] selectedObjects)
        {

            //Check if all selected objects are using the same model
            var allSameModel = true;
            var modellist = new Dictionary<string, string>();

            foreach (var obj in selectedObjects)
            {
                var _data = obj.Attributes.UserData.Find(typeof(CurveHostUserData)) as CurveHostUserData;
                if (_data != null)
                {
                    if (_data.Data.ModelId != string.Empty)
                    {
                        modellist[_data.Data.ModelId.ToString()] = _data.Data.Token;
                    }
                }

            }
            var layout = new DynamicLayout();
            if (selectedObjects.Length < 1)
            {
                Content = layout;

            }
            else
            {
                var data = selectedObjects[0].Attributes.UserData.Find(typeof(CurveHostUserData)) as CurveHostUserData;
                layout.AddSeparateRow(new Label { Text = "This is a Smart Object" });
                layout.AddSeparateRow(new Label { Text = "Selected objects:" + selectedObjects.Length.ToString() + selectedObjects[0].Id.ToString() });
                layout.AddSeparateRow(new Label { Text = "ModelId:" + data.Data.ModelId });
                foreach (var parameter in data.Data.Parameters)
                {


                    layout.AddSeparateRow(parameter.Value.GetEtoControl());


                }

                layout.AddSeparateRow();
                Content = layout;
            }

        
    }

        public HostObjectPropertiesPageControl(RhinoObject[] selectedObjects)  // Perhaps this takes an argument to display different content)
        {
            //Check if all selected objects are using the same model
            var allSameModel = true;
            var modellist = new Dictionary<string, string>();

            foreach (var obj in selectedObjects)
            {
                var _data = obj.Attributes.UserData.Find(typeof(CurveHostUserData)) as CurveHostUserData;
                if (_data != null)
                {
                    if (_data.Data.ModelId != string.Empty)
                    {
                        modellist[_data.Data.ModelId.ToString()] = _data.Data.Token;
                    }
                }

            }
            var layout = new DynamicLayout();
            if (selectedObjects.Length < 1)
            {
                Content = layout;

            }
            var data = selectedObjects[0].Attributes.UserData.Find(typeof(CurveHostUserData)) as CurveHostUserData;
            var layout = new DynamicLayout();
            layout.AddSeparateRow(new Label { Text = "This is a Smart Object" });
            layout.AddSeparateRow(new Label { Text = "ModelId:" + data.Data.ModelId });
            foreach (var parameter in data.Data.Parameters)
            {
                Control etoParameter = parameter.Value.GetEtoControl();              
                layout.AddSeparateRow(parameter.Value.GetEtoControl());
            }

                layout.AddSeparateRow();
                Content = layout;
            }

        }

    }
}

