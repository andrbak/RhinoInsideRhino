using Eto.Forms;
using System.Collections.Generic;

namespace RhinoInsideRhino.ObjectModel
{
    public class DropDownParameterObject : ParameterObject
    {
        public List<DropDownOption> Options { get; set; }

        public DropDownParameterObject()
        {
            Type = "DropDown";
            Options = new List<DropDownOption>();
        }

        public override Control GetEtoControl()
        {

            var stackLayout = new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Spacing = 5
            };

            var label = new Label
            {
                Text = Name,
                VerticalAlignment = VerticalAlignment.Center
            };


            var dropDown = new DropDown();

            // Populate the DropDown with option texts
            if (Options != null)
            {
                foreach (var option in Options)
                {
                    dropDown.Items.Add(option.Text);
                }

                // Set selected index based on current Value
                var selectedIndex = Options.FindIndex(o => o.Value == (int) Value);
                if (selectedIndex >= 0)
                    dropDown.SelectedIndex = selectedIndex;
            }

            // Handle selection changed
            dropDown.SelectedIndexChanged += (sender, e) =>
            {
                if (dropDown.SelectedIndex >= 0 && Options != null && dropDown.SelectedIndex < Options.Count)
                {
                    Value = Options[dropDown.SelectedIndex].Value;
                    ValueChanged?.Invoke();
                }
            };

            stackLayout.Items.Add(label);
            stackLayout.Items.Add(dropDown);

            return stackLayout;
        }
    }
}
