using Eto.Forms;
using System;
using System.Drawing;

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
        public Slider Slider { get; set; }
        private int Scale;
        public override Control GetEtoControl()
        {
            Scale = (int)Math.Pow(10, DecimalPlaces);
            int scaledMin = (int)(Min * Scale);
            int scaledMax = (int)(Max * Scale);

            var label = new Label
            {
                Text = Name,
                VerticalAlignment = VerticalAlignment.Center
            };

            Slider = new Slider
            {
                MinValue = scaledMin,
                MaxValue = scaledMax,
                Orientation = Orientation.Horizontal,
                Width = 200,
                SnapToTick = SnapToTick,
            };

            // var valueLabel = new Label
            // {
            //     VerticalAlignment = VerticalAlignment.Center
            // };

            var valueLabel = new TextBox()
            {
                Width = 50,
                ReadOnly = true,
                BackgroundColor = Eto.Drawing.Colors.Transparent,
                ShowBorder = false,
                Height = 20,

            };

            // Initialize Value if null
            if (Value == null)
                Value = Min;

            double currentValue = Convert.ToDouble(Value);
            Slider.Value = (int)(currentValue * Scale);
            valueLabel.Text = currentValue.ToString("F" + DecimalPlaces);
            Slider.MouseDoubleClick += (sender, e) =>
            {
                // Reset to default value on double click
                valueLabel.ReadOnly = false;
                valueLabel.ShowBorder = true; // Show border to indicate edit mode
                valueLabel.Focus();
                valueLabel.KeyUp += OnCommit;

            };



            Slider.ValueChanged += (sender, e) =>
            {
                double newValue = Slider.Value;
                Value = newValue / Scale; // Convert back to original scale
                valueLabel.Text = (newValue / Scale).ToString("F" + DecimalPlaces);
                ValueChanged?.Invoke();
            };

            return new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Items =
                {
                    label,
                    Slider,
                    valueLabel
                },
                Spacing = 5
            };
        }

        protected void OnCommit(object s, KeyEventArgs e)
        {
            var textBox = s as TextBox;
            if (e.Key == Keys.Escape)
            {
                //ParentSL.OnUserInputClosed(e);
                textBox.ReadOnly = true; // Reset to read-only on escape
                textBox.ShowBorder = false; // Hide border
                // Close(); //this would trigger unfocus. no need to call above line
                return;
            }
            else if (e.Key == Keys.Enter)
            {

                if (double.TryParse(textBox.Text, out double userval))
                {
                    Value = userval; // Update the Value property
                    Slider.Value = (int)(userval * Scale); // out-of-bounds safeguard already built in :)
                                                           //ParentSL.OnUserInputClosed(e);

                }                                          // textBox.SelectedText=string.Empty;
                textBox.Text = userval.ToString("F" + DecimalPlaces); // Update text box with formatted value
                textBox.Selection = new Eto.Forms.Range<int>(0, -1); // Reset selection
                textBox.ReadOnly = true; // Reset to read-only on escape
                textBox.ShowBorder = false; // Hide border
                ValueChanged?.Invoke(); // Trigger value changed event
                //Close();//this would trigger unfocus. no need to call above line
                return;
            }
        }
    }
}
