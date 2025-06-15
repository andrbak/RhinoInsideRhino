using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInsideRhino.ObjectModel
{
    public abstract class ParameterObject
    {

        public string Type { get; set; }

        public object Value { get; set; }   

        public string Name { get; set; }    

        public string Id { get; set; }

        public abstract Control GetEtoControl();
    }


    public class DropDownOption
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }



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
            var dropDown = new DropDown();

            // Populate the DropDown with option texts
            if (Options != null)
            {
                foreach (var option in Options)
                {
                    dropDown.Items.Add(option.Text);
                }

                // Set selected index based on current Value
                var selectedIndex = Options.FindIndex(o => o.Value == Value.ToString());
                if (selectedIndex >= 0)
                    dropDown.SelectedIndex = selectedIndex;
            }

            // Handle selection changed
            dropDown.SelectedIndexChanged += (sender, e) =>
            {
                if (dropDown.SelectedIndex >= 0 && Options != null && dropDown.SelectedIndex < Options.Count)
                {
                    Value = Options[dropDown.SelectedIndex].Value;
                }
            };

            return dropDown;
        }
    }


    public class SliderParameterObject : ParameterObject
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public int DecimalPlaces { get; set; }
        public SliderParameterObject()
        {
            Type = "Slider";
        }

        public override Control GetEtoControl()
        {
            int scale = (int)Math.Pow(10, DecimalPlaces);
            int scaledMin = (int)(Min * scale);
            int scaledMax = (int)(Max * scale);

            var slider = new Slider
            {
                MinValue = scaledMin,
                MaxValue = scaledMax,
                Orientation = Orientation.Horizontal,
                Width = 200
            };

            var valueLabel = new Label
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            // Initialize Value if null
            if (Value == null)
                Value = Min;

            double currentValue = Convert.ToDouble(Value);
            slider.Value = (int)(currentValue * scale);
            valueLabel.Text = currentValue.ToString("F" + DecimalPlaces);

            slider.ValueChanged += (sender, e) =>
            {
                double newValue = slider.Value / (double)scale;
                Value = newValue;
                valueLabel.Text = newValue.ToString("F" + DecimalPlaces);
            };

            return new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Items =
        {
            slider,
            valueLabel
        },
                Spacing = 5
            };
        }

    }
}
