using System;
using System.ComponentModel;

namespace MapGenerator.Controls.NumericUpDownControl.Converters
{
    /// <summary>
    /// IsCornerCtrlCorner is used to indicate which corners of the arrow button are also on the corner of the container control
    /// in which it is inserted. If for instance the arrow button is placed on right hand side as with a combo box, then both 
    /// right hand sides corners of IsCornerCtrlCorner will be set to true, while both left hand sides will be set to false.
    /// Order is same as with Border.CornerRadius: TopLeft, TopRight, BottomRight, BottomLeft.
    /// Reason for this is because with some themes (example: Aero), button has slightly rounded corners when these are on 
    /// edge of control.
    /// </summary>
    [TypeConverter(typeof(IsCornerCtrlCornerConverter))]
    public readonly struct IsCornerCtrlCorner : IEquatable<IsCornerCtrlCorner>
    {
        public bool BottomLeft { get; }

        public bool BottomRight { get; }

        public bool TopLeft { get; }

        public bool TopRight { get; }
        
        public IsCornerCtrlCorner(bool uniformCtrlCorner)
        {
            TopLeft = TopRight = BottomRight = BottomLeft = uniformCtrlCorner;
        }

        public IsCornerCtrlCorner(bool topLeft, bool topRight, bool bottomRight, bool bottomLeft)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
        }

        public static bool operator !=(IsCornerCtrlCorner ccc1, IsCornerCtrlCorner ccc2)
        {
            return ccc1.TopLeft != ccc2.TopLeft || ccc1.TopRight != ccc2.TopRight ||
                    ccc1.BottomRight != ccc2.BottomRight || ccc1.BottomLeft != ccc2.TopLeft;
        }
        public static bool operator ==(IsCornerCtrlCorner ccc1, IsCornerCtrlCorner ccc2)
        {
            return ccc1.TopLeft == ccc2.TopLeft && ccc1.TopRight == ccc2.TopRight &&
                ccc1.BottomRight == ccc2.BottomRight && ccc1.BottomLeft == ccc2.TopLeft;
        }
        public bool Equals(IsCornerCtrlCorner cornerCtrlCorner)
        {
            return this == cornerCtrlCorner;
        }
        public override bool Equals(object obj)
        {
            if (obj is IsCornerCtrlCorner corner)
            {
                return this == corner;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return (TopLeft ? 0x00001000 : 0x00000000) | (TopRight ? 0x00000100 : 0x00000000) |
                (BottomRight ? 0x00000010 : 0x00000000) | (BottomLeft ? 0x00000001 : 0x00000000);
        }
        public override string ToString()
        {
            return $"{TopLeft},{TopRight},{BottomRight},{BottomLeft}";
        }
    }
}