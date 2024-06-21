using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public struct Int2
    {
        public int x;
        public int y;

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Int2(Vector2Int vector)
        {
            x = vector.x;
            y = vector.y;
        }

        public override bool Equals(object other)
        {
            if (!(other is Int2))
            {
                return false;
            }

            return Equals((Int2)other);
        }
        public bool Equals(Int2 other)
        {
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", x, y);
        }

        public readonly static Int2 zero = new(0, 0);
        public readonly static Int2 one = new(1, 1);
        public readonly static Int2 right = new(1, 0);
        public readonly static Int2 left = new(-1, 0);
        public readonly static Int2 up = new(0, 1);
        public readonly static Int2 down = new(0, -1);

        public static Int2 operator -(Int2 v) => new(-v.x, -v.y);
        public static Int2 operator +(Int2 a, Int2 b) => new(a.x + b.x, a.y + b.y);
        public static Int2 operator -(Int2 a, Int2 b) => new(a.x - b.x, a.y - b.y);
        public static Int2 operator *(Int2 a, int b) => new(a.x * b, a.y * b);
        public static Int2 operator *(int a, Int2 b) => new(a * b.x, a * b.y);
        public static Int2 operator /(Int2 a, int b) => new(a.x / b, a.y / b);
        public static bool operator ==(Int2 lhs, Int2 rhs) => lhs.x == rhs.x && lhs.y == rhs.y;
        public static bool operator !=(Int2 lhs, Int2 rhs) => !(lhs == rhs);

        public static implicit operator Vector2Int(Int2 v) => new Vector2Int(v.x, v.y);
        public static implicit operator Vector2(Int2 v) => new Vector2(v.x, v.y);
        public static implicit operator Int2(Vector2Int v) => new Int2(v.x, v.y);
    }
}
