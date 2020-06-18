using System.Windows;
using System.Windows.Controls.Primitives;
using MapGenerator.Controls.NumericUpDownControl.Converters;
using MapGenerator.Controls.NumericUpDownControl.Enums;

namespace MapGenerator.Controls.NumericUpDownControl
{
    public class ArrowRepeatButton : RepeatButton
    {
        private static readonly DependencyProperty ButtonArrowTypeProperty =
            DependencyProperty.Register("ButtonArrowType", typeof(EButtonArrowType), typeof(ArrowRepeatButton), new FrameworkPropertyMetadata(EButtonArrowType.Down));
        private static readonly DependencyProperty IsCornerCtrlCornerProperty =
            DependencyProperty.Register("IsCornerCtrlCorner", typeof(IsCornerCtrlCorner), typeof(ArrowRepeatButton), new FrameworkPropertyMetadata(new IsCornerCtrlCorner(false, true, true, false)));

        static ArrowRepeatButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ArrowRepeatButton), new FrameworkPropertyMetadata(typeof(ArrowRepeatButton)));
        }
        public EButtonArrowType ButtonArrowType
        {
            get => (EButtonArrowType)GetValue(ButtonArrowTypeProperty);
            set => SetValue(ButtonArrowTypeProperty, value);
        }
        public IsCornerCtrlCorner IsCornerCtrlCorner
        {
            get => (IsCornerCtrlCorner)GetValue(IsCornerCtrlCornerProperty);
            set => SetValue(IsCornerCtrlCornerProperty, value);
        }
    }
}