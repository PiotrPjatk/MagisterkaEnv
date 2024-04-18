
    using Unity.Collections;
    using Unity.Mathematics;
    using UnityEngine;
    
    public struct Tile
    {
        [NativeDisableParallelForRestriction]
        NativeArray<TileFlags> cells;
        
        int2 size;
        public int Length => cells.Length;
        
        public Tile (int2 size)
        {
            this.size = size;
            cells = new NativeArray<TileFlags>(size.x * size.y, Allocator.Persistent);
        }
        
        public int SizeEW => size.x;

        public int SizeNS => size.y;

        public int StepN => size.x;

        public int StepE => 1;

        public int StepS => -size.x;

        public int StepW => -1;
        
        
        public TileFlags this[int index]
        {
            get => cells[index];
            set => cells[index] = value;
        }
        
        public TileFlags Set (int index, TileFlags mask) =>
            cells[index] = cells[index].With(mask);

        public TileFlags Unset (int index, TileFlags mask) =>
            cells[index] = cells[index].Without(mask);

        public int2 IndexToCoordinates (int index)
        {
            int2 coordinates;
            coordinates.y = index / size.x;
            coordinates.x = index - size.x * coordinates.y;
            return coordinates;
        }

        public Vector3 CoordinatesToWorldPosition (int2 coordinates, float y = 0f) =>
            new Vector3(
                2f * coordinates.x + 1f - size.x,
                y,
                2f * coordinates.y + 1f - size.y
            );

        public Vector3 IndexToWorldPosition (int index, float y = 0f) =>
            CoordinatesToWorldPosition(IndexToCoordinates(index), y);
        
        
        
        public void Dispose ()
        {
            if (cells.IsCreated)
            {
                cells.Dispose();
            }
        }
    }
