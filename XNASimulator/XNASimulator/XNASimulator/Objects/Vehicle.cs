using KruispuntGroep4.Simulator.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KruispuntGroep4.Simulator.Objects
{
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

        //An 'empty' vehicle
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