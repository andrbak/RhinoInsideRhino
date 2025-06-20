using System;
using System.Collections.Generic;
using Rhino;
using Rhino.UI;
using Rhino.UI.DialogPanels;
using RhinoInsideRhino.Display;
using RhinoInsideRhino.Requests;
using RhinoInsideRhino.RhinoHelpers;
using RhinoInsideRhino.Views;

namespace RhinoInsideRhino
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class RhinoInsideRhinoPlugin : Rhino.PlugIns.PlugIn
    {

        private RhinoObjectEventHandler _rhinoObjectEventHandler;

        public DisplayOptions DisplayOptions { get; } = new DisplayOptions();   

        public RhinoInsideRhinoPlugin()
        {
            Instance = this;


        

            // Register the RhinoObjectEventHandler to handle Rhino object events
            _rhinoObjectEventHandler = new RhinoObjectEventHandler();

            
            

            // Register the Eto main panel
            Panels.RegisterPanel(this, typeof(MainPanel), "RhinoInsideRhino", null);

            RhinoApp.Idle += OnIdle;
        }


        private void OnIdle(object sender, System.EventArgs e)
        {
            RhinoApp.Idle -= OnIdle;
            OpenMainPanel();
        }

        public void OpenMainPanel()
        {
            var panelId = typeof(MainPanel).GUID;
            if (!Panels.IsPanelVisible(panelId))
            {
                Panels.OpenPanel(Panels.PanelDockBar(typeof(LayersPanel)), panelId);
            }
        }



        ///<summary>Gets the only instance of the RhinoInsideRhinoPlugin plug-in.</summary>
        public static RhinoInsideRhinoPlugin Instance { get; private set; }

        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and maintain plug-in wide options in a document.

        [Obsolete]
        protected override void ObjectPropertiesPages(List<ObjectPropertiesPage> pages)
        {
            var page = new Views.HostObjectPropertiesPage();
            pages.Add(page);
        }
    }
}