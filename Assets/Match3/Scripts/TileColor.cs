namespace Match3
{
    [System.Serializable]
    public struct TileColor
    {
        public int value;

        public readonly static TileColor none = new((int)TileColorSamples.None);
        public readonly static TileColor blue = new((int)TileColorSamples.Blue);
        public readonly static TileColor red = new((int)TileColorSamples.Red);
        public readonly static TileColor green = new((int)TileColorSamples.Green);
        public readonly static TileColor yellow = new((int)TileColorSamples.Yellow);
        public readonly static TileColor orange = new((int)TileColorSamples.Orange);
        public readonly static TileColor purple = new((int)TileColorSamples.Purple);

        public TileColor(int value)
        {
            this.value = value;
        }

        public override bool Equals(object other)
        {
            if (!(other is TileColor))
            {
                return false;
            }

            return Equals((TileColor)other);
        }
        public bool Equals(TileColor other)
        {
            return value == other.value && value == other.value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public static bool operator ==(TileColor lhs, TileColor rhs) => lhs.value == rhs.value;
        public static bool operator !=(TileColor lhs, TileColor rhs) => lhs.value != rhs.value;

        public override string ToString()
        {
            switch (value)
            {
                case (int)TileColorSamples.Blue: return "Blue";
                case (int)TileColorSamples.Red: return "Red";
                case (int)TileColorSamples.Green: return "Green";
                case (int)TileColorSamples.Yellow: return "Yellow";
                case (int)TileColorSamples.Orange: return "Orange";
                case (int)TileColorSamples.Purple: return "Purple";
            }
            return $"Color({value})";
        }
    }

    public enum TileColorSamples
    {
        None = -1,
        Blue = 0,
        Red = 1,
        Green = 2,
        Yellow = 3,
        Orange = 4,
        Purple = 5,
    }
}
