using System;
using System.Collections;
using System.Collections.Generic;
using KruispuntGroep4.Simulator.Globals;
using KruispuntGroep4.Simulator.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KruispuntGroep4.Simulator.ObjectControllers
{
    class VehicleControl
    {
        private Lists lists;
        private GraphicsDevice graphics;
		private Random random;
        private int nrOfTries;

        public VehicleControl(GraphicsDevice graphics, Lists lists)
        {
            this.lists = lists;
            this.graphics = graphics;
            this.nrOfTries = 0;
			random = new Random();
        }

        public void Update(GameTime gameTime)
        {
            for (int i = lists.Vehicles.Count - 1; i >= 0; i--)
            {
                Vehicle vehicle = lists.Vehicles[i];

                //If the vehicle has an ID, meaning its alive
                if (!vehicle.Equals(string.Empty))
                {
                    this.CheckAlive(vehicle);
                    this.MakeNextMove(vehicle);

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

                    //Remove the vehicle from the master list and the lane
                    lists.Vehicles.Remove(vehicle);
                    vehicle.currentLane.laneVehicles.Remove(vehicle);

                }
            }
            else
            {
                if (!vehicle.ID.Equals(string.Empty))
                    vehicle.alive = true;
            }
        }

        private void MakeNextMove(Vehicle vehicle)
        {
            Tile currentTile = lists.Tiles[(int)vehicle.occupyingtile.X,(int)vehicle.occupyingtile.Y];
            Tile nextTile;

            //If the vehicle can move any further in this direction...
            if (currentTile.adjacentTiles.ContainsKey(vehicle.rotation))
            {
                //Grab the next tile on this vehicle's path
                nextTile = currentTile.adjacentTiles[vehicle.rotation];
               
                if (vehicle.collission.Intersects(nextTile.CollisionRectangle))
                {
                    //Check if this tile is part of his path
                    if (!nextTile.laneIDs.Contains(vehicle.destinationLaneID) && //If it's not his destination lane
                        !nextTile.laneIDs.Contains(vehicle.path.ToString()) && //...and not his directional lane
                        !nextTile.laneIDs.Contains(vehicle.spawntile.laneIDs[0])) //...and also not his starting lane
                    {
                        //Find the correct tile

                        //The 'index' for the adjacent tiles library
                        IEnumerator<KeyValuePair<RotationEnum, Tile>> enumerator = currentTile.adjacentTiles.GetEnumerator();

                        //Take the first adjacent tile
                        nextTile = enumerator.Current.Value;

                        //Pick the correct path
                        nextTile = ChooseCorrectPath(vehicle, nextTile, enumerator);
                    }   

                    CheckTileOccupation(vehicle, nextTile);
                }
                else
                {
                    CheckTileOccupation(vehicle, currentTile);
                }
            }
        }

        private Tile ChooseCorrectPath(Vehicle vehicle, Tile nextTile, IEnumerator<KeyValuePair<RotationEnum, Tile>> enumerator)
        {
            if (!(nrOfTries > 4))
            {
                //First check if this tile exists
                if (enumerator.Current.Value != null && enumerator.Current.Value.laneIDs.Count > 0)
                {
                    nextTile = enumerator.Current.Value;
                    bool correctPath = false;

                    //Go through the lanes it's part of
                    foreach (string laneID in nextTile.laneIDs)
                    {
                        Lane tileLane;
                        lists.Lanes.TryGetValue(laneID, out tileLane);

                        //Check if this is the tile the vehicle should go on
                        if ((laneID.Equals(vehicle.destinationLaneID) || //His destination lane is a valid path
                            laneID.Equals(vehicle.path.ToString()) || //The lane corresponding to his direction is a valid path
                            laneID.Equals(vehicle.spawntile.laneIDs[0])) //His starting lane is a valid path
                            &&
                            !isOppositeDirection(vehicle.rotation, enumerator.Current.Key) //He is not allowed to go in the opposite direction
                            &&
                            tileLane.pathDirections.Contains(enumerator.Current.Key)) //He is only allowed to go into the direction the pathlane is going
                        {
                            correctPath = true;
                            break;
                        }
                        else
                        {
                            correctPath = false;
                        }
                    }

                    if (correctPath)
                    {
                        //Rotate the vehicle in the direction of the adjacent tile
                        vehicle.rotation = enumerator.Current.Key;
                        vehicle.drawposition = nextTile.DrawPosition;
                        vehicle.collission = nextTile.CollisionRectangle;
                        vehicle.position = nextTile.Position;

                        //The given tile was the correct one to take, so return it
                        nrOfTries = 0;
                        return nextTile;
                    }
                    else //The vehicle should look to the next adjacent tile
                    {
                        enumerator.MoveNext();
                        nextTile = enumerator.Current.Value;

                        //Check again
                        this.nrOfTries++;
                        return ChooseCorrectPath(vehicle, nextTile, enumerator);
                    }
                }
                else //The tile doesn't exist, so take the next adjacent one
                {
                    enumerator.MoveNext();
                    this.nrOfTries++;
                    return ChooseCorrectPath(vehicle, nextTile, enumerator);
                }
            }
            else
            {
                nrOfTries = 0;
                throw new Exception("Vehicle cannot find path");
            }
        }

        private bool isOppositeDirection(RotationEnum currentDirection, RotationEnum newDirection)
        {
            bool result = false;

            switch(newDirection)
            {
                case RotationEnum.North:
                    if (currentDirection.Equals(RotationEnum.South))
                        result = true;
                    break;
                case RotationEnum.East:
                    if (currentDirection.Equals(RotationEnum.West))
                        result = true;
                    break;
                case RotationEnum.South:
                    if (currentDirection.Equals(RotationEnum.North))
                        result = true;
                    break;
                case RotationEnum.West:
                    if (currentDirection.Equals(RotationEnum.East))
                        result = true;
                    break;
            }
            
            return result;
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

                    //update vehicle's lane, if the next tile is part of a new lane
                    if (tile.laneIDs.Count > 0 && !tile.laneIDs.Contains(vehicle.currentLane.laneID))
                    {
                        //Remove the vehicle from the previous lane
                        Lane lane = vehicle.currentLane;
                        lane.laneVehicles.Remove(vehicle);

                        int i = 0;

                        //if the new lane is the direction that needs to be followed
                        if (tile.laneIDs.Contains(vehicle.path.ToString()))
                        {
                            //get the correct index
                            i = tile.laneIDs.IndexOf(vehicle.path.ToString());
                        }

                        //Add it to the current lane
                        lists.Lanes.TryGetValue(tile.laneIDs[i], out lane); 
                        vehicle.currentLane = lane;
                        lane.laneVehicles.Add(vehicle);
                    }

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