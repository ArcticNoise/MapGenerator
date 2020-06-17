using System;
using System.Windows;
using System.Windows.Data;

namespace MapGenerator.Converters
{
    [ValueConversion(typeof(IsCornerCtrlCorner), typeof(CornerRadius))]
    public class IsCornerCtrlCornerToRadiusConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Must be a string that can be converted into an int, either directly as a decimal or as an hexadecimal
        /// prefixed with either 0x or 0X. The first 8 bits will give the CornerRadius rounding next to edge of control. The following bits will
        /// give rounding of a corner not adjoining an edge. Example: 0x305: inner rounding: 0x3, outer rounding: 0x5. Inner rounding of a 
        /// corner not adjoining an edge currently not used and set to 0.</param>
        /// <param name="culture"></param>
        /// <returns></returns>

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                throw new System.ArgumentNullException();

            if (!(value is IsCornerCtrlCorner))
                throw new System.ArgumentException();

            string str = (string)parameter;
            IsCornerCtrlCorner ccc = (IsCornerCtrlCorner)value;
            int rounding;

            if (!int.TryParse(str, out rounding))
            {
                if (!(str[0] == '0' && (str[1] == 'x' || str[1] == 'X')))
                    throw new System.ArgumentException();

                if (!Int32.TryParse(str.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out rounding))
                    throw new System.ArgumentException();
            }
            int NotEdgeRounding = rounding >> 8;
            int EdgeRounding = rounding & 0x000000FF;

            return new CornerRadius(ccc.TopLeft ? EdgeRounding : NotEdgeRounding, ccc.TopRight ? EdgeRounding : NotEdgeRounding,
                                    ccc.BottomRight ? EdgeRounding : NotEdgeRounding, ccc.BottomLeft ? EdgeRounding : NotEdgeRounding);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Not Implemented.");
        }
    }
}