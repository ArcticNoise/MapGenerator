using System;
using System.ComponentModel;
using System.Linq;

namespace MapGenerator.Converters
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
                throw new System.ArgumentNullException();

            String[] strArr = ((String)source).Split(',');

            if (strArr.Count() != 4)
                throw new System.ArgumentException();

            bool[] cornerstates = new bool[4];

            for (int i = 0; i < strArr.Count(); i++)
            {
                if (!bool.TryParse(strArr[i], out cornerstates[i]))
                    throw new System.ArgumentException();
            }
            return new IsCornerCtrlCorner(cornerstates[0], cornerstates[1], cornerstates[2], cornerstates[3]);
        }
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, System.Globalization.CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (value == null)
                throw new System.ArgumentNullException();
            if (!(value is IsCornerCtrlCorner))
                throw new System.ArgumentException();

            IsCornerCtrlCorner ccc = (IsCornerCtrlCorner)(value);

            return ccc.TopLeft.ToString() + "," + ccc.TopRight.ToString() + "," + ccc.BottomRight.ToString() + "," + ccc.BottomLeft.ToString();
        }
    }
}