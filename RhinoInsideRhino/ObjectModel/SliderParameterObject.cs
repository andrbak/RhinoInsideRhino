using Eto.Forms;
using System;

namespace RhinoInsideRhino.ObjectModel
{
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
            //int scale = (int)Math.Pow(10, DecimalPlaces);
            int scaledMin = (int)(Min);
            int scaledMax = (int)(Max);

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
            slider.Value = (int)(currentValue);
            valueLabel.Text = currentValue.ToString("F" + DecimalPlaces);

           

            slider.ValueChanged += (sender, e) =>
            {
                double newValue = slider.Value;
                Value = newValue;
                valueLabel.Text = newValue.ToString("F" + DecimalPlaces);
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
