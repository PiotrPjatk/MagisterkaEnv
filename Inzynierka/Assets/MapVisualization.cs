using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
    public class MapVisualization : ScriptableObject
    {
        [SerializeField] private MapTile end,
            straight,
            cornerClosed,
            cornerOpen,
            tJunctionClosed,
            xJunctionClosed,
            tJunctionOpenNE,
            tJunctionOpenSE,
            tJunctionOpen,
            xJunctionOpenNE,
            xJunctionOpenNE_SE,
            xJunctionOpenNE_SW,
            xJunctionClosedNE,
            xJunctionOpen;
        

        public void Visualize (Tile tile)
        {
            for (int i = 0; i < tile.Length; i++)
            {
                var prefabWithRotation = GetPrefab(tile[i]);
                var instance = prefabWithRotation.Item1.GetInstance();
                instance.transform.SetPositionAndRotation(
                    tile.IndexToWorldPosition(i), rotations[prefabWithRotation.Item2]
                );
            }
        }
        
        (MapTile, int) GetPrefab (TileFlags flags) => flags switch
        {
            TileFlags.PassageN => (end, 0),
            TileFlags.PassageE => (end, 1),
            TileFlags.PassageS => (end, 2),
            TileFlags.PassageW => (end, 3),

            TileFlags.PassageN | TileFlags.PassageS => (straight, 0),
            TileFlags.PassageE | TileFlags.PassageW => (straight, 1),

            TileFlags.PassageN | TileFlags.PassageE => GetCorner(flags, 0),
            TileFlags.PassageE | TileFlags.PassageS => GetCorner(flags, 1),
            TileFlags.PassageS | TileFlags.PassageW => GetCorner(flags, 2),
            TileFlags.PassageW | TileFlags.PassageN => GetCorner(flags, 3),


            TileFlags.PassagesStraight & ~TileFlags.PassageW => GetTJunction(flags, 0),
            TileFlags.PassagesStraight & ~TileFlags.PassageN => GetTJunction(flags, 1),
            TileFlags.PassagesStraight & ~TileFlags.PassageE => GetTJunction(flags, 2),
            TileFlags.PassagesStraight & ~TileFlags.PassageS => GetTJunction(flags, 3),


            _ => GetXJunction(flags)
        };
        
        
        (MapTile, int) GetTJunction (TileFlags flags, int rotation) => (
            flags.RotatedDiagonalPassages(rotation) switch
            {
                TileFlags.Empty => tJunctionClosed,
                TileFlags.PassageNE => tJunctionOpenNE,
                TileFlags.PassageSE => tJunctionOpenSE,
                _ => tJunctionOpen
            },
            rotation
        );

        (MapTile, int) GetCorner (TileFlags flags, int rotation) => (
            flags.HasAny(TileFlags.PassagesDiagonal) ? cornerOpen : cornerClosed, rotation
        );
        (MapTile, int) GetXJunction (TileFlags flags) =>
            flags.DiagonalPassages() switch
            {
                TileFlags.Empty => (xJunctionClosed, 0),

                TileFlags.PassageNE => (xJunctionOpenNE, 0),
                TileFlags.PassageSE => (xJunctionOpenNE, 1),
                TileFlags.PassageSW => (xJunctionOpenNE, 2),
                TileFlags.PassageNW => (xJunctionOpenNE, 3),

                TileFlags.PassageNE | TileFlags.PassageSE => (xJunctionOpenNE_SE, 0),
                TileFlags.PassageSE | TileFlags.PassageSW => (xJunctionOpenNE_SE, 1),
                TileFlags.PassageSW | TileFlags.PassageNW => (xJunctionOpenNE_SE, 2),
                TileFlags.PassageNW | TileFlags.PassageNE => (xJunctionOpenNE_SE, 3),

                TileFlags.PassageNE | TileFlags.PassageSW => (xJunctionOpenNE_SW, 0),
                TileFlags.PassageSE | TileFlags.PassageNW => (xJunctionOpenNE_SW, 1),

                TileFlags.PassagesDiagonal & ~TileFlags.PassageNE => (xJunctionClosedNE, 0),
                TileFlags.PassagesDiagonal & ~TileFlags.PassageSE => (xJunctionClosedNE, 1),
                TileFlags.PassagesDiagonal & ~TileFlags.PassageSW => (xJunctionClosedNE, 2),
                TileFlags.PassagesDiagonal & ~TileFlags.PassageNW => (xJunctionClosedNE, 3),

                _ => (xJunctionOpen, 0),
            };
        
        
        static Quaternion[] rotations =
        {
            Quaternion.identity,
            Quaternion.Euler(0f, 90f, 0f),
            Quaternion.Euler(0f, 180f, 0f),
            Quaternion.Euler(0f, 270f, 0f)
        };
    }
