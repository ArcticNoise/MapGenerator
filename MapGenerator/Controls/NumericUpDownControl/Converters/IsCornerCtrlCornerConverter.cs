using System;
using System.ComponentModel;
using System.Linq;

namespace MapGenerator.Controls.NumericUpDownControl.Converters
{
    public class IsCornerCtrlCornerConverter : TypeConverter
    {
        public IsCornerCtrlCornerConverter() { }

        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            return sourceType == Type.GetType("System.String");
        }

        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            return destinationType == Type.GetType("System.String");
        }

        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, System.Globalization.CultureInfo cultureInfo, object source)
        {
            if (source == null)
            {
                throw new System.ArgumentNullException();
            }

            var strArr = ((string)source).Split(',');

            if (strArr.Count() != 4)
                throw new ArgumentException();

            var cornerStates = new bool[4];

            for (var i = 0; i < strArr.Count(); i++)
            {
                if (!bool.TryParse(strArr[i], out cornerStates[i]))
                {
                    throw new System.ArgumentException();
                }
            }
            return new IsCornerCtrlCorner(cornerStates[0], cornerStates[1], cornerStates[2], cornerStates[3]);
        }
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, System.Globalization.CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException();
            }

            if (!(value is IsCornerCtrlCorner))
            {
                throw new System.ArgumentException();
            }

            var ccc = (IsCornerCtrlCorner)(value);

            return $"{ccc.TopLeft},{ccc.TopRight},{ccc.BottomRight},{ccc.BottomLeft}";
        }
    }
}