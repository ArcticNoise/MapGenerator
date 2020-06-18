using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MapGenerator.Controls.NumericUpDownControl.Helpers
{
    public enum DecimalSeparatorType
    {
        SystemDefined,
        Point,
        Comma
    }

    public enum NegativeSignType
    {
        SystemDefined,
        Minus
    }

    public enum NegativeSignSide
    {
        SystemDefined,
        Prefix,
        Suffix
    }

    internal interface IFrameTxtBoxCtrl
    {
        TextBox TextBox { get; }
    }

    internal static class Coercer
    {
        public static void Initialize<T>() where T : UserControl
        {
            try
            {
                FrameworkPropertyMetadata borderThicknessMetaData = new FrameworkPropertyMetadata
                {
                    CoerceValueCallback = CoerceBorderThickness
                };
                
                Control.BorderThicknessProperty.OverrideMetadata(typeof(T), borderThicknessMetaData);

                // For Background, do not do in XAML part something like:
                // Background="{Binding Background, ElementName=Root}" in TextBoxCtrl settings.
                // Reason: although this will indeed set the Background values as expected, problems arise when user
                // of control does not explicitly not set a value.
                // In this case, Background of TextBoxCtrl get defaulted to values in UserControl, which is null
                // and not what we want.
                // We want to keep the default values of a standard TextBox, which may differ according to themes.
                // Have to treat similarly as with BorderThickness...

                FrameworkPropertyMetadata backgroundMetaData = new FrameworkPropertyMetadata
                {
                    CoerceValueCallback = CoerceBackground
                };

                Control.BackgroundProperty.OverrideMetadata(typeof(T), backgroundMetaData);

                // ... Same for BorderBrush
                var borderBrushMetaData = new FrameworkPropertyMetadata
                {
                    CoerceValueCallback = CoerceBorderBrush
                };
                Control.BorderBrushProperty.OverrideMetadata(typeof(T), borderBrushMetaData);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private delegate void FuncCoerce(IFrameTxtBoxCtrl frameTxtBox, object value);

        private static void CommonCoerce(DependencyObject d, object value, FuncCoerce funco)
        {
            if (d is IFrameTxtBoxCtrl frameTxtBox)
            {
                funco(frameTxtBox, value);
            }
        }

        private static void FuncCoerceBorderThickness(IFrameTxtBoxCtrl frameTxtBox, object value)
        {
            if (value is Thickness thickness)
            {
                frameTxtBox.TextBox.BorderThickness = thickness;
            }
        }

        private static void FuncCoerceBackground(IFrameTxtBoxCtrl frameTxtBox, object value)
        {
            if (value is Brush brush)
            {
                frameTxtBox.TextBox.Background = brush;
            }
        }

        private static void FuncCoerceBorderBrush(IFrameTxtBoxCtrl frameTxtBox, object value)
        {
            if (value is Brush brush)
            {
                frameTxtBox.TextBox.BorderBrush = brush;
            }
        }

        public static object CoerceBorderThickness(DependencyObject d, object value)
        {
            CommonCoerce(d, value, FuncCoerceBorderThickness);
            return new Thickness(0.0);
        }

        public static object CoerceBackground(DependencyObject d, object value)
        {
            CommonCoerce(d, value, FuncCoerceBackground);
            return value;
        }

        public static object CoerceBorderBrush(DependencyObject d, object value)
        {
            CommonCoerce(d, value, FuncCoerceBorderBrush);
            return value;
        }
    }

    internal static class SystemNumberInfo
    {
        private static readonly NumberFormatInfo nfi;

        static SystemNumberInfo()
        {
            var ci = CultureInfo.CurrentCulture;
            nfi = ci.NumberFormat;
        }

        public static string DecimalSeparator => nfi.NumberDecimalSeparator;

        public static string NegativeSign => nfi.NegativeSign;

        public static bool IsNegativePrefix =>
            // for values, see: http://msdn.microsoft.com/en-us/library/system.globalization.numberformatinfo.numbernegativepattern.aspx
            // Assume if negative number format is (xxx), number is prefixed.
            nfi.NumberNegativePattern < 3;
    }

    public class ThicknessToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException();
            }

            if (!(value is Thickness) || !(parameter is string))
            {
                throw new System.ArgumentException();
            }

            bool.TryParse((string)parameter, out var incrRightThickness);
            var thickness = (Thickness)value;
            return new Thickness(thickness.Left + 1.0, thickness.Top + 1.0, incrRightThickness ? thickness.Right + 18.0 : 18.0, thickness.Bottom);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Not Implemented.");
        }
    }
}