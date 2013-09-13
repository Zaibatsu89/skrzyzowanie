using KruispuntGroep6.Simulator.Globals;
using KruispuntGroep6.Simulator.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using KruispuntGroep6.Simulator.Main;
using System.Collections.Generic;
using XNASimulator.Globals;

namespace KruispuntGroep6.Simulator.ObjectControllers
{
    class VehicleControl
    {
        private Lists lists;
        private GraphicsDevice graphics;
		private Random random;

        public VehicleControl(GraphicsDevice graphics, Lists lists)
        {
            this.lists = lists;
            this.graphics = graphics;
			random = new Random();
        }

        public void Update(GameTime gameTime)
        {
            for (int i = lists.Vehicles.Count - 1; i > 0; i--)
            {
                Vehicle vehicle = lists.Vehicles[i];

                //If the vehicle has an ID, meaning its alive
                if (!vehicle.Equals(string.Empty))
                {
                    this.CheckAlive(vehicle);
                    this.CheckNextTile(vehicle);

                    //update rotation of the vehicle 
                    if (!vehicle.stopRedLight && !vehicle.stopCar)
                    {
                        switch (vehicle.rotation)
                        {
                            case RotationEnum.North:
                                vehicle.position -= new Vector2(0, vehicle.speed);
                                vehicle.drawposition -= new Vector2(0, vehicle.speed);
                                vehicle.collission = new Rectangle((int)vehicle.position.X, (int)vehicle.position.Y, vehicle.sprite.Width, vehicle.sprite.Height);
                                break;
                            case RotationEnum.East:
                                vehicle.position += new Vector2(vehicle.speed, 0);
                                vehicle.drawposition += new Vector2(vehicle.speed, 0);
                                vehicle.collission = new Rectangle((int)vehicle.position.X, (int)vehicle.position.Y, vehicle.sprite.Width, vehicle.sprite.Height);
                                break;
                            case RotationEnum.West:
                                vehicle.position -= new Vector2(vehicle.speed, 0);
                                vehicle.drawposition -= new Vector2(vehicle.speed, 0);
                                vehicle.collission = new Rectangle((int)vehicle.position.X, (int)vehicle.position.Y, vehicle.sprite.Width, vehicle.sprite.Height);
                                break;
                            case RotationEnum.South:
                                vehicle.position += new Vector2(0, vehicle.speed);
                                vehicle.drawposition += new Vector2(0, vehicle.speed);
                                vehicle.collission = new Rectangle((int)vehicle.position.X, (int)vehicle.position.Y, vehicle.sprite.Width, vehicle.sprite.Height);
                                break;
                        }
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Vehicle vehicle in lists.Vehicles)
            {
                if (vehicle.alive)
                {
                    spriteBatch.Draw(vehicle.sprite,
                                            vehicle.drawposition,
                                            null,
                                            Color.White,
                                            Rotation.getRotation(vehicle.rotation),
                                            vehicle.origin,
                                            1.0f,
                                            SpriteEffects.None,
                                            0.0f);
                }
            }
        }

        private void CheckAlive(Vehicle vehicle)
        {
            if (vehicle.alive)
            {
                //If the vehicle is no longer on screen
                if (!graphics.PresentationParameters.Bounds.Contains(new Point((int)vehicle.position.X,
                    (int)vehicle.position.Y)) )
                {
                    vehicle.alive = false;

                    //Free up the last tile it was on
                    lists.Tiles[(int)vehicle.occupyingtile.X, (int)vehicle.occupyingtile.Y].OccupiedID = string.Empty;
                    lists.Tiles[(int)vehicle.occupyingtile.X, (int)vehicle.occupyingtile.Y].isOccupied = false;

                    //Remove the vehicle from the list
                    lists.Vehicles.Remove(vehicle);

                    //lists.Vehicles[vehicle.ID[1]] = new Vehicle(string.Empty);

                }
            }
            else
            {
                if (!vehicle.ID.Equals(string.Empty))
                    vehicle.alive = true;
            }
        }

        private void CheckNextTile(Vehicle vehicle)
        {
            Tile currentTile = lists.Tiles[(int)vehicle.occupyingtile.X,(int)vehicle.occupyingtile.Y];

            switch (vehicle.rotation)
            {
                case RotationEnum.North:
                    //check if there is a tile north of the one the vehicle is occupying
                    this.CheckNextTile(currentTile, vehicle, RotationEnum.North);
                    break;
                case RotationEnum.East:
                    this.CheckNextTile(currentTile, vehicle, RotationEnum.East);
                    break;
                case RotationEnum.South:
                    this.CheckNextTile(currentTile, vehicle, RotationEnum.South);
                    break;
                case RotationEnum.West:
                    this.CheckNextTile(currentTile, vehicle, RotationEnum.West);
                    break;
            }
        }

        private void CheckNextTile(Tile currentTile, Vehicle vehicle, RotationEnum direction)
        {
            Tile nextTile;

            if (currentTile.adjacentTiles.ContainsKey(direction.ToString()))
            {
                nextTile = currentTile.adjacentTiles[direction.ToString()];
                if (vehicle.collission.Intersects(nextTile.CollisionRectangle))
                {
                    CheckTileOccupation(vehicle, nextTile);
                }
                else
                {
                    CheckTileOccupation(vehicle, currentTile);
                }
            }
        }

        private void CheckTileOccupation(Vehicle vehicle, Tile tile)
        {
            
            //first check if it's a red light ahead
            if (tile.Texture.Equals(Textures.RedLight))
            {
                vehicle.stopRedLight = true;
            }
            else //if it's not, see if vehicle can occupy it
            {
                vehicle.stopRedLight = false;

                //check if occupied
                if (tile.isOccupied)
                {

                    //is it occupied by this vehicle?
                    if (tile.GridCoordinates.Equals(vehicle.occupyingtile))
                    {
                        //yes, so go..
                        vehicle.stopCar = false;

                        if (tile.Equals(vehicle.currentLane.detectionClose))
                        {

                        }
                        else if (tile.Equals(vehicle.currentLane.detectionFar))
                        {

                        }
                    }
                    else
                    {
                        //no, so wait..
                        vehicle.stopCar = true;
                    }
                }
                else //not occupied
                {
                    vehicle.stopCar = false;

                    //claim it 
                    tile.isOccupied = true;
                    tile.OccupiedID = vehicle.ID;

                    //release previous tile
                    lists.Tiles[(int)vehicle.occupyingtile.X, (int)vehicle.occupyingtile.Y].isOccupied = false;
                    lists.Tiles[(int)vehicle.occupyingtile.X, (int)vehicle.occupyingtile.Y].OccupiedID = string.Empty;

                    //let the detection know
                    if (tile.Equals(vehicle.currentLane.detectionClose))
                    {

                    }
                    else if (tile.Equals(vehicle.currentLane.detectionFar))
                    {

                    }


                    //set the new tile
                    vehicle.occupyingtile = tile.GridCoordinates;
                }
            }
        }
	}
}