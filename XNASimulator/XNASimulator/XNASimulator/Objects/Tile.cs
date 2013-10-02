using System.Collections.Generic;
using KruispuntGroep4.Simulator.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KruispuntGroep4.Simulator.Objects
{
    /// <summary>
    /// A Tile object of the tile-based crossroad
    /// and the information it contains
    /// </summary>
    class Tile
    {
        public Texture2D Texture { get; set; }

        //Positions and Dimensions
        public RotationEnum Rotation { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public Vector2 Size { get; private set; }
        public Vector2 Origin { get; private set; }
        public Vector2 Position {get; set; }
        public Vector2 DrawPosition { get; set; }
        public Rectangle CollisionRectangle { get; set; }
        public Vector2 GridCoordinates { get; set; }
        public Dictionary<RotationEnum, Tile> adjacentTiles {get; set; }

        public string OccupiedID { get; set; }
        public string laneID { get; set; } //The lane this tile is a part of, if any

        //Bools
        public bool isOccupied = false;
        public bool isGreen = false;
        public bool isWalkway = false;
        public bool isSpawn = false;

        /// <summary>
        /// A single Tile
        /// </summary>
        /// <param name="texture">The Texture of the tile</param>
        /// <param name="rotation">The rotation of the texture</param>
        /// <param name="gridposition">The position of the tile on the crossroad grid</param>
        public Tile(Texture2D texture, RotationEnum rotation, Vector2 gridposition)
        {
            this.Height = texture.Height;
            this.Width = texture.Width;
            this.GridCoordinates = gridposition;

            this.Size = new Vector2(Width, Height);
            this.Origin = new Vector2(Width / 2, Height / 2);

            this.Texture = texture;
            this.Rotation = rotation;

            this.OccupiedID = string.Empty;
            this.laneID = string.Empty;
            this.adjacentTiles = new Dictionary<RotationEnum, Tile>();
        }
    }
}