using Eto.Forms;
using System;

namespace RhinoInsideRhino.ObjectModel
{
    public class SliderParameterObject : ParameterObject
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public int DecimalPlaces { get; set; }
        public bool SnapToTick { get; set; } = false;
        public SliderParameterObject()
        {
            Type = "Slider";
        }

        public override Control GetEtoControl()
        {
            int scale = (int)Math.Pow(10, DecimalPlaces);
            int scaledMin = (int)(Min* scale);
            int scaledMax = (int)(Max* scale);

            var label = new Label
            {
                Text = Name,
                VerticalAlignment = VerticalAlignment.Center
            };

            var slider = new Slider
            {
                MinValue = scaledMin,
                MaxValue = scaledMax,
                Orientation = Orientation.Horizontal,
                Width = 200,
                SnapToTick = SnapToTick,
            };

            var valueLabel = new Label
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            // Initialize Value if null
            if (Value == null)
                Value = Min;

            double currentValue = Convert.ToDouble(Value);
            slider.Value = (int)(currentValue* scale);
            valueLabel.Text = currentValue.ToString("F" + DecimalPlaces);

           

            slider.ValueChanged += (sender, e) =>
            {
                double newValue = slider.Value;
                Value = newValue/ scale; // Convert back to original scale
                valueLabel.Text = (newValue/scale).ToString("F" + DecimalPlaces);
                ValueChanged?.Invoke();
            };

            return new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Items =
        {
                    label,
                     slider,
                    valueLabel
        },
                Spacing = 5
            };
        }

    }
}
