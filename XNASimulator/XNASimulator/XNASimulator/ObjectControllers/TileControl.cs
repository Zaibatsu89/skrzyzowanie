using KruispuntGroep4.Simulator.Globals;
using KruispuntGroep4.Simulator.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KruispuntGroep4.Simulator.ObjectControllers
{
    class TileControl
    {
        private Lists lists;
        private int DEBUGvehicleID = 0;

        private int LevelWidth
        {
            get { return lists.Tiles.GetLength(0); }
        } //horizontal tiles
        private int LevelHeight
        {
            get { return lists.Tiles.GetLength(1); }
        } //vertical tiles

        public TileControl(Lists lists)
        {
            this.lists = lists;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // For each tile position
            for (int y = 0; y < LevelHeight; ++y)
            {
                for (int x = 0; x < LevelWidth; ++x)
                {
                    // If there is a visible tile in that position
                    if (!Texture2D.Equals(lists.Tiles[x, y].Texture, null))
                    {
                        // Draw it in screen space.                     
                        spriteBatch.Draw(lists.Tiles[x, y].Texture,
                                        lists.Tiles[x, y].DrawPosition,
                                        null,
                                        Color.White,
                                        Rotation.getRotation(lists.Tiles[x, y].Rotation),
                                        lists.Tiles[x, y].Origin,
                                        1.0f,
                                        SpriteEffects.None,
                                        1.0f);
                    }
                }
            }
        }

        public void CheckMouseCollision(Vector2 mouseposition)
        {
            Rectangle mouseArea = new Rectangle((int)mouseposition.X, (int)mouseposition.Y, 1, 1);

            foreach (Tile tile in lists.Tiles)
            {
                if (tile.CollisionRectangle.Contains(mouseArea))
                {
                    if (tile.isSpawn)
                    {
                        Lane lane;
                        Vehicle newVehicle = new Vehicle(DEBUGvehicleID.ToString());

                        lists.Lanes.TryGetValue(tile.laneIDs[0], out lane);

                        VehicleTypeEnum vehicleType = VehicleTypeEnum.bus;
                        switch (vehicleType)
                        {
                            case VehicleTypeEnum.bus: newVehicle = new Vehicle(Textures.Bus, DEBUGvehicleID.ToString());
                                break;
                            case VehicleTypeEnum.car: newVehicle = new Vehicle(Textures.Car, DEBUGvehicleID.ToString());
                                break;
                            case VehicleTypeEnum.truck: newVehicle = new Vehicle(Textures.Truck, DEBUGvehicleID.ToString());
                                break;
                            case VehicleTypeEnum.bike: newVehicle = new Vehicle(Textures.Bike, DEBUGvehicleID.ToString());
                                break;
                        }

                        newVehicle = lane.AddVehicle(newVehicle);

                        string spawnLaneID = "N1";
                        string destinationLaneID = "E6";

                        switch (spawnLaneID[0].ToString() + destinationLaneID[0].ToString())
                        {
                            case "NE": newVehicle.path = PathsEnum.NorthToEast;
                                break;
                            case "NS": newVehicle.path = PathsEnum.NorthToSouth;
                                break;
                            case "NW": newVehicle.path = PathsEnum.NorthToWest;
                                break;

                            case "ES": newVehicle.path = PathsEnum.EastToSouth;
                                break;
                            case "EW": newVehicle.path = PathsEnum.EastToWest;
                                break;
                            case "EN": newVehicle.path = PathsEnum.EastToNorth;
                                break;

                            case "SW": newVehicle.path = PathsEnum.SouthToWest;
                                break;
                            case "SN": newVehicle.path = PathsEnum.SouthToNorth;
                                break;
                            case "SE": newVehicle.path = PathsEnum.SouthToEast;
                                break;

                            case "WN": newVehicle.path = PathsEnum.WestToNorth;
                                break;
                            case "WE": newVehicle.path = PathsEnum.WestToEast;
                                break;
                            case "WS": newVehicle.path = PathsEnum.WestToSouth;
                                break;
                        }

                        newVehicle.destinationLaneID = destinationLaneID;

                        lists.Vehicles.Add(newVehicle);

                        DEBUGvehicleID++;
                    }
                    else
                    {
                        this.ChangeLights(tile);
                    }
                }
            }
        }

        public void ChangeLights(Tile tile)
        {
            if (tile.isGreen == false && tile.Texture.Equals(Textures.RedLight))
            {
                tile.Texture = Textures.GreenLight;
                tile.isGreen = true;
            }
            else if (tile.isGreen == true && tile.Texture.Equals(Textures.GreenLight))
            {
                tile.Texture = Textures.RedLight;
                tile.isGreen = false;
            }
        }

        public void ChangeLights(string laneID, LightsEnum colour)
        {
            Lane lane;

            lists.Lanes.TryGetValue(laneID, out lane);

            //Discard the sidewalk lanes
            if (!laneID[1].Equals('0') && !laneID[1].Equals('7'))
            {
                switch (colour)
                {
                    case LightsEnum.Blink: lane.trafficLight.Texture = Textures.BlinkLight;
                        break;
                    case LightsEnum.Red: lane.trafficLight.Texture = Textures.RedLight;
                        break;
                    case LightsEnum.Green: lane.trafficLight.Texture = Textures.GreenLight;
                        break;
                    case LightsEnum.Yellow: lane.trafficLight.Texture = Textures.YellowLight;
                        break;
                }
            }
        }
    }
}