    using System;

    [Flags]
    public enum TileFlags
    {
        Empty = 0,
	
        PassageN = 0b0001,
        PassageE = 0b0010,
        PassageS = 0b0100,
        PassageW = 0b1000,

        PassagesStraight = 0b1111,

        PassageNE = 0b0001_0000,
        PassageSE = 0b0010_0000,
        PassageSW = 0b0100_0000,
        PassageNW = 0b1000_0000,

        PassagesDiagonal = 0b1111_0000
    }
    
    public static class MazeFlagsExtensions
    {
        public static bool Has (this TileFlags flags, TileFlags mask) =>
            (flags & mask) == mask;

        public static bool HasAny (this TileFlags flags, TileFlags mask) =>
            (flags & mask) != 0;

        public static bool HasNot (this TileFlags flags, TileFlags mask) =>
            (flags & mask) != mask;

        public static bool HasExactlyOne (this TileFlags flags) =>
            flags != 0 && (flags & (flags - 1)) == 0;

        public static TileFlags With (this TileFlags flags, TileFlags mask) =>
            flags | mask;

        public static TileFlags Without (this TileFlags flags, TileFlags mask) =>
            flags & ~mask;
        
        public static TileFlags StraightPassages (this TileFlags flags) =>
            flags & TileFlags.PassagesStraight;

        public static TileFlags DiagonalPassages (this TileFlags flags) =>
            flags & TileFlags.PassagesDiagonal;
        
        public static TileFlags RotatedDiagonalPassages (this TileFlags flags, int rotation)
        {
            int bits = (int)(flags & TileFlags.PassagesDiagonal);
            bits = (bits >> rotation) | (bits << (4 - rotation));
            return (TileFlags)bits & TileFlags.PassagesDiagonal;
        }
    }
    
    
