using System;
using System.Drawing;


[Serializable]
public class SerializableData
    {


        public Color Color { get; set; } = Color.Blue;
        public int Thickness { get; set; } = 5;

        public SerializableData()
        {
        }
        protected SerializableData(SerializableData other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            Color = other.Color;
            Thickness = other.Thickness;
        }

        public SerializableData Clone()
        {
            return new SerializableData(this);
        }









}

