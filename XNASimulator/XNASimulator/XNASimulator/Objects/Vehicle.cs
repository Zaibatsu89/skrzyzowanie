using KruispuntGroep4.Simulator.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KruispuntGroep4.Simulator.Objects
{
    class Vehicle
    {
		public string ID {get; set;}
		public VehicleTypeEnum type { get; set; }
		public RotationEnum rotation { get; set; }
		public PathsEnum path { get; set; }
        public Vector2 position { get; set; }
        public Vector2 drawposition { get; set; }
		public Texture2D sprite { get; set; }
        public Vector2 origin { get; private set; }
        public bool alive { get; set; }
		public float speed { get; set; }
        public bool stopCar { get; set; }
        public bool stopRedLight { get; set; }
		public Rectangle collission { get; set; }
		public Vector2 occupyingtile { get; set; }
        public string destinationLaneID { get; set; }

		public Lane currentLane { get; set; }
		public Tile spawntile { get; set; }
		
        public Vehicle(Texture2D texture, string ID, VehicleTypeEnum type)
        {
            this.ID = ID;
			this.type = type;
            rotation = RotationEnum.North;
			path = PathsEnum.EastToNorth;
            position = Vector2.Zero;
            drawposition = Vector2.Zero;
            sprite = texture;
            origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            alive = false;
            speed = 3f;
            stopCar = false;
            stopRedLight = false;
            collission = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
            occupyingtile = Vector2.Zero;
            destinationLaneID = string.Empty;
        }

        //An 'empty' vehicle
        public Vehicle(string ID)
        {
            this.ID = ID;
			this.type = VehicleTypeEnum.car;
            rotation = RotationEnum.North;
			path = PathsEnum.EastToNorth;
            position = Vector2.Zero;
            drawposition = Vector2.Zero;
            sprite = Textures.Default;
            origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
			alive = false;
			speed = 1f;
            stopCar = false;
            stopRedLight = false;
            collission = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
			occupyingtile = Vector2.Zero;
            destinationLaneID = string.Empty;
        }
    }
}