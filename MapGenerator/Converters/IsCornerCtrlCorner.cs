using System;
using System.ComponentModel;

namespace MapGenerator.Converters
{
    /// <summary>
    /// IsCornerCtrlCorner is used to indicate which corners of the arrow button are also on the corner of the container control
    /// in which it is inserted. If for instance the arrow button is placed on right hand side as with a combo box, then both 
    /// right hand sides corners of IsCornerCtrlCorner will be set to true, while both left hand sides will be set to false.
    /// Order is same as with Border.CornerRadius: topleft, topright, bottomright, bottomleft.
    /// Reason for this is because with some themes (example: Aero), button has slightly rounded corners when these are on 
    /// edge of control.
    /// </summary>
    [TypeConverter(typeof(IsCornerCtrlCornerConverter))]
    public struct IsCornerCtrlCorner : IEquatable<IsCornerCtrlCorner>
    {
        private bool topLeft, topRight, bottomRight, bottomLeft;
        public IsCornerCtrlCorner(bool uniformCtrlCorner)
        {
            topLeft = topRight = bottomRight = bottomLeft = uniformCtrlCorner;
        }
        public IsCornerCtrlCorner(bool topLeft, bool topRight, bool bottomRight, bool bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;
        }
        public bool BottomLeft { get => bottomLeft;
            set => bottomLeft = value;
        }
        public bool BottomRight { get => bottomRight;
            set => bottomRight = value;
        }
        public bool TopLeft { get => topLeft;
            set => topLeft = value;
        }
        public bool TopRight { get => topRight;
            set => topRight = value;
        }

        public static bool operator !=(IsCornerCtrlCorner ccc1, IsCornerCtrlCorner ccc2)
        {
            return ccc1.topLeft != ccc2.topLeft || ccc1.topRight != ccc2.topRight ||
                    ccc1.bottomRight != ccc2.bottomRight || ccc1.bottomLeft != ccc2.topLeft;
        }
        public static bool operator ==(IsCornerCtrlCorner ccc1, IsCornerCtrlCorner ccc2)
        {
            return ccc1.topLeft == ccc2.topLeft && ccc1.topRight == ccc2.topRight &&
                ccc1.bottomRight == ccc2.bottomRight && ccc1.bottomLeft == ccc2.topLeft;
        }
        public bool Equals(IsCornerCtrlCorner cornerCtrlCorner)
        {
            return this == cornerCtrlCorner;
        }
        public override bool Equals(object obj)
        {
            if (obj is IsCornerCtrlCorner)
                return this == (IsCornerCtrlCorner)obj;
            else
                return false;
        }
        public override int GetHashCode()
        {
            return (topLeft ? 0x00001000 : 0x00000000) | (topRight ? 0x00000100 : 0x00000000) |
                (bottomRight ? 0x00000010 : 0x00000000) | (bottomLeft ? 0x00000001 : 0x00000000);
        }
        public override string ToString()
        {
            return topLeft.ToString() + "," + topRight.ToString() + "," + bottomRight.ToString() + "," + bottomLeft.ToString();
        }
    }
}