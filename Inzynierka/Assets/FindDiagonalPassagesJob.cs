
    using Unity.Burst;
    using Unity.Jobs;

    [BurstCompile]
    public struct FindDiagonalPassagesJob : IJobFor
    {
        public Tile tile;

        public void Execute (int i)
        {
            TileFlags cell = tile[i];
            if (
                cell.Has(TileFlags.PassageN | TileFlags.PassageE) &&
                tile[i + tile.StepN + tile.StepE].Has(TileFlags.PassageS | TileFlags.PassageW)
            )
            {
                cell = cell.With(TileFlags.PassageNE);
            }
            if (
                cell.Has(TileFlags.PassageN | TileFlags.PassageW) &&
                tile[i + tile.StepN + tile.StepW].Has(TileFlags.PassageS | TileFlags.PassageE)
            )
            {
                cell = cell.With(TileFlags.PassageNW);
            }
            if (
                cell.Has(TileFlags.PassageS | TileFlags.PassageE) &&
                tile[i + tile.StepS + tile.StepE].Has(TileFlags.PassageN | TileFlags.PassageW)
            )
            {
                cell = cell.With(TileFlags.PassageSE);
            }
            if (
                cell.Has(TileFlags.PassageS | TileFlags.PassageW) &&
                tile[i + tile.StepS + tile.StepW].Has(TileFlags.PassageN | TileFlags.PassageE)
            )
            {
                cell = cell.With(TileFlags.PassageSW);
            }
            tile[i] = cell;
        }
    }
