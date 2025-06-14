using Eto.Drawing;
using Eto.Forms;
using Newtonsoft.Json;
using Rhino.Geometry;
using Rhino.Render.Fields;
using Rhino.UI;
using RhinoInsideRhino.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;


namespace RhinoInsideRhino.Views
{


    /// <summary>
    /// Required class GUID, used as the panel Id
    /// </summary>
    [System.Runtime.InteropServices.Guid("a63d281d-5ab6-4197-9abc-fc92c35a461e")]
    public class MainPanel : Panel, IPanel
    {
        readonly uint m_document_sn = 0;

        /// <summary>
        /// Provide easy access to the SampleCsEtoPanel.GUID
        /// </summary>
        public static System.Guid PanelId => typeof(MainPanel).GUID;



        private Macro selectedMacro;


        private string selectedFolder;

        private ListBox listBox;






        /// <summary>
        /// Required public constructor with NO parameters
        /// </summary>
        /// 






        public MainPanel(uint documentSerialNumber)
        {

            // Sample data
            //var items = new List<Macro>
            //{
            //    new Macro { Name = "Apple", Description = "A juicy red fruit." },
            //    new Macro { Name = "Banana", Description = "A long yellow fruit." },
            //    new Macro { Name = "Cherry", Description = "A small red fruit." }
            //};



            var selectFolderButton = new Button { Text = "Select Folder" };
            selectFolderButton.Click += (sender, e) =>
            {
                var dlg = new SelectFolderDialog();
                if (dlg.ShowDialog(this) == DialogResult.Ok)
                {
                    selectedFolder = dlg.Directory;
                    MessageBox.Show(this, "Selected folder: " + selectedFolder, "Folder Selected");

                    // Reload macros from the new folder and update the list
                    var macros = MacroLoader.LoadMacrosFromSubfolders(selectedFolder);
                    listBox.DataStore = macros;
                }
            };

            listBox = new ListBox
            {
                DataStore = new List<Macro>(), // Start empty
                ItemTextBinding = Binding.Delegate<Macro, string>(m => m.Name)
            };

            // Selected label
            var selectedLabel = new Label { Text = "" };

            listBox.SelectedIndexChanged += (s, e) =>
            {
                selectedMacro = listBox.SelectedValue as Macro;
                selectedLabel.Text = selectedMacro != null
                    ? "Selected: " + selectedMacro.Name
                    : "Nothing selected";
            };


            //Button
            Button applyButton = new Button { Text = "Apply To Selection" };
            applyButton.Click += (sender, e) => OnApplyButton();


            // Layout
            var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

            layout.AddSeparateRow(selectFolderButton);
            layout.AddSeparateRow(new Label { Text = "Select Macro" }, null);
            layout.AddRow(listBox);
            layout.AddRow(selectedLabel);
            layout.AddRow(applyButton);
            layout.Add(null);
            Content = layout;



           























            //m_document_sn = documentSerialNumber;


            ////Example code
            //Title = GetType().Name;         
            ////ClientSize = new Size(600, 300);

            //var titleLabel =  new Label { Text = "Select an item" };


            //// Sample data
            //var items = new List<Macro>
            //{
            //    new Macro { Name = "Apple", Description = "A juicy red fruit.", Thumbnail = Bitmap.FromResource("RhinoInsideRhino.png") },
            //    new Macro { Name = "Banana", Description = "A long yellow fruit.", Thumbnail = Bitmap.FromResource("RhinoInsideRhino.png") },
            //    new Macro { Name = "Cherry", Description = "A small red fruit.", Thumbnail = Bitmap.FromResource("RhinoInsideRhino.png") }
            //};

            //var listView = new GridView
            //{
            //    DataStore = items,
            //    RowHeight = 60,
            //    AllowMultipleSelection = false
            //};

            //// Image + Name cell
            //listView.Columns.Add(new GridColumn
            //{
            //    HeaderText = "Item",
            //    Editable = false,
            //    DataCell = new ImageTextCell
            //    {
            //        ImageBinding = Binding.Property<Macro, Image>(r => r.Thumbnail),
            //        TextBinding = Binding.Property<Macro, string>(r => r.Name)
            //    },
            //    Width = 200
            //});

            //// Description cell
            //listView.Columns.Add(new GridColumn
            //{
            //    HeaderText = "Description",
            //    Editable = false,
            //    DataCell = new TextBoxCell
            //    {
            //        Binding = Binding.Property<Macro, string>(r => r.Description)
            //    },
            //    Width = 350
            //});

            //// Label to display selected item
            //var selectedLabel = new Label { Text = "Select an item" };
            //listView.SelectedRowsChanged += (s, e) =>
            //{
            //    var selectedIndex = listView.SelectedRow;
            //    if (selectedIndex >= 0)
            //        selectedLabel.Text = "Selected: " + items[selectedIndex].Name;
            //    else
            //        selectedLabel.Text = "Nothing selected";
            //};


            //DynamicLayout layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
            //layout.AddSeparateRow(titleLabel, null);
            ////layout.AddSeparateRow(listView, null);
            ////layout.AddSeparateRow(selectedLabel, null);
            //layout.Add(null);


            //Content = layout;


        }

        private void OnApplyButton()
        {
            if (selectedMacro != null)
            {

                //Get selection
                var selectedObjects= Rhino.RhinoDoc.ActiveDoc.Objects.GetSelectedObjects(false, false);


                string message = "Applying macro: " + selectedMacro.Name + " to:";


                foreach (var obj in selectedObjects)
                {

                    if (obj.Geometry is Curve curve)
                    {

                        CurveHostObject curveHostObjects = new CurveHostObject(curve);

                        curveHostObjects.Data.ModelId = selectedMacro.ModelId;


                        //TODO: compute script and get parameters

                    }







                    message += "\n";
                    message += obj.Id;
                }

                Dialogs.ShowMessage(message, Title);

                



            }
                




                









            else
                Dialogs.ShowMessage("No macro selected.", Title);
        }

        public string Title { get; }

        /// <summary>
        /// Example of proper way to display a message box
        /// </summary>
        protected void OnAttachButton()
        {
            // Use the Rhino common message box and NOT the Eto MessageBox,
            // the Eto version expects a top level Eto Window as the owner for
            // the MessageBox and will cause problems when running on the Mac.
            // Since this panel is a child of some Rhino container it does not
            // have a top level Eto Window.
            Dialogs.ShowMessage("Hello Rhino!", Title);
        }

        /// <summary>
        /// Sample of how to display a child Eto dialog
        /// </summary>
        protected void OnChildButton()
        {
            //SampleCsEtoHelloWorld dialog = new SampleCsEtoHelloWorld();
            //dialog.ShowModal(this);
        }

        #region IPanel methods
        public void PanelShown(uint documentSerialNumber, ShowPanelReason reason)
        {
            // Called when the panel tab is made visible, in Mac Rhino this will happen
            // for a document panel when a new document becomes active, the previous
            // documents panel will get hidden and the new current panel will get shown.
            Rhino.RhinoApp.WriteLine($"Panel shown for document {documentSerialNumber}, this serial number {m_document_sn} should be the same");
        }

        public void PanelHidden(uint documentSerialNumber, ShowPanelReason reason)
        {
            // Called when the panel tab is hidden, in Mac Rhino this will happen
            // for a document panel when a new document becomes active, the previous
            // documents panel will get hidden and the new current panel will get shown.
            Rhino.RhinoApp.WriteLine($"Panel hidden for document {documentSerialNumber}, this serial number {m_document_sn} should be the same");
        }

        public void PanelClosing(uint documentSerialNumber, bool onCloseDocument)
        {
            // Called when the document or panel container is closed/destroyed
            Rhino.RhinoApp.WriteLine($"Panel closing for document {documentSerialNumber}, this serial number {m_document_sn} should be the same");
        }


        #endregion IPanel methods

    }
}