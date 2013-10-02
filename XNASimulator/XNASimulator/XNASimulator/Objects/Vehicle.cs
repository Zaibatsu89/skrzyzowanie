using KruispuntGroep4.Simulator.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KruispuntGroep4.Simulator.Objects
{
    /// <summary>
    /// This class represents a move-able object within the simulator
    /// and the information it contains
    /// </summary>
    class Vehicle
    {
		public string ID {get; set;}
		public VehicleTypeEnum type { get; set; }
		public float speed { get; set; }
		public RotationEnum rotation { get; set; }
        public Vector2 position { get; set; }
        public Vector2 drawposition { get; set; }
		public Texture2D sprite { get; set; }
        public Vector2 origin { get; private set; }
        public bool alive { get; set; }
        public bool stopCar { get; set; }
        public bool stopRedLight { get; set; }
		public Rectangle collission { get; set; }
		public Vector2 occupyingtile { get; set; }
        public string destinationLaneID { get; set; }

		public Lane currentLane { get; set; }
		public Tile spawntile { get; set; }
        public bool enterInnerLane { get; set; }
        public int innerLaneTurns { get; set; }
		
        /// <summary>
        /// Default vehicle with the needed values 
        /// assigned in order to be spawned
        /// </summary>
        /// <param name="texture">Sprite/Texture the vehicle uses</param>
        /// <param name="ID">Unique ID</param>
        /// <param name="type">Type such as bus or car</param>
        /// <param name="speed">Speed at which the vehicle moves</param>
        public Vehicle(Texture2D texture, string ID, VehicleTypeEnum type, float speed)
        {
            this.ID = ID;
			this.type = type;
			this.speed = speed;
			rotation = RotationEnum.North;
            position = Vector2.Zero;
            drawposition = Vector2.Zero;
            sprite = texture;
            origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            alive = false;
            stopCar = false;
            stopRedLight = false;
            collission = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
            occupyingtile = Vector2.Zero;
            destinationLaneID = string.Empty;
            enterInnerLane = false;
            innerLaneTurns = 0;
        }

        /// <summary>
        /// An 'empty' vehicle
        /// </summary>
        /// <param name="ID">Unique ID</param>
        public Vehicle(string ID)
        {
            this.ID = ID;
			this.type = VehicleTypeEnum.car;
			this.speed = 1f;
			rotation = RotationEnum.North;
            position = Vector2.Zero;
            drawposition = Vector2.Zero;
            sprite = Textures.Default;
            origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
			alive = false;
            stopCar = false;
            stopRedLight = false;
            collission = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
			occupyingtile = Vector2.Zero;
            destinationLaneID = string.Empty;
            enterInnerLane = false;
            innerLaneTurns = 0;
        }

        /// <summary>
        /// This method will make the vehicle do a left or right turn
        /// </summary>
        /// <param name="direction">Which direction the vehicle needs to turn</param>
        /// <param name="currentTile">Current location of the vehicle</param>
        /// <returns>The tile the vehicle will end up on</returns>
        public Tile turnVehicleTile(TurnEnum direction, Tile currentTile)
        {
            Tile tile = currentTile;

            switch (direction)
            {
                case TurnEnum.Left:
                    switch (this.rotation)
                    {
                        case RotationEnum.North:
                            currentTile.adjacentTiles.TryGetValue(RotationEnum.West, out tile);
                            this.rotation = RotationEnum.West;
                            break;
                        case RotationEnum.East:
                            currentTile.adjacentTiles.TryGetValue(RotationEnum.North, out tile);
                            this.rotation = RotationEnum.North;
                            break;
                        case RotationEnum.South:
                            currentTile.adjacentTiles.TryGetValue(RotationEnum.East, out tile);
                            this.rotation = RotationEnum.East;
                            break;
                        case RotationEnum.West:
                            currentTile.adjacentTiles.TryGetValue(RotationEnum.South, out tile);
                            this.rotation = RotationEnum.South;
                            break;
                    }
                    break;
                case TurnEnum.Right:
                    switch (this.rotation)
                    {
                        case RotationEnum.North:
                            currentTile.adjacentTiles.TryGetValue(RotationEnum.East, out tile);
                            this.rotation = RotationEnum.East;
                            break;
                        case RotationEnum.East:
                            currentTile.adjacentTiles.TryGetValue(RotationEnum.South, out tile);
                            this.rotation = RotationEnum.South;
                            break;
                        case RotationEnum.South:
                            currentTile.adjacentTiles.TryGetValue(RotationEnum.West, out tile);
                            this.rotation = RotationEnum.West;
                            break;
                        case RotationEnum.West:
                            currentTile.adjacentTiles.TryGetValue(RotationEnum.North, out tile);
                            this.rotation = RotationEnum.North;
                            break;
                    }
                    break;
            }

            this.drawposition = tile.DrawPosition;
            this.position = tile.Position;
            this.collission = tile.CollisionRectangle;
            return tile;
        }
    }
}