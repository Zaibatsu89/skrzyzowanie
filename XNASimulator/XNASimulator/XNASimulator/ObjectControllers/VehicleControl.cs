﻿using System;
using System.Collections;
using System.Collections.Generic;
using KruispuntGroep4.Globals;
using KruispuntGroep4.Simulator.Communication;
using KruispuntGroep4.Simulator.Globals;
using KruispuntGroep4.Simulator.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KruispuntGroep4.Simulator.ObjectControllers
{
    class VehicleControl
    {
		private CommunicationForm communicationForm;
        private Lists lists;
        private GraphicsDevice graphics;
		private Random random;
        private int nrOfTries;

        public VehicleControl(GraphicsDevice graphics, Lists lists, CommunicationForm communicationForm)
        {
			this.communicationForm = communicationForm;
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
			for (int i = lists.Vehicles.Count - 1; i >= 0; i--)
            {
				Vehicle vehicle = lists.Vehicles[i];

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
                if (!graphics.Viewport.Bounds.Contains(new Point((int)vehicle.position.X,(int)vehicle.position.Y)))
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
                    if (!vehicle.sprite.Equals(Textures.Pedestrian))
                    {
                        if (!nextTile.laneID.Equals(vehicle.destinationLaneID) && //If it's not his destination lane
                            !nextTile.laneID.Equals(vehicle.spawntile.laneID) && //...and also not his starting lane
                            !nextTile.laneID.Equals(PathsEnum.OuterPathLane.ToString())) //..and not the outer path lane
                        {
                            //Check which pathing lane the vehicle is in
                            if (currentTile.laneID.Equals(PathsEnum.OuterPathLane.ToString()))
                            {
                                //if he runs into the inner lane while on the outer lane
                                if (nextTile.laneID.Equals(PathsEnum.InnerPathLane.ToString())) 
                                {
                                    if (!vehicle.enterInnerLane)
                                    {
                                        //Continue on outer lane by turning right (from spawn)
                                        nextTile = vehicle.turnVehicleTile(TurnEnum.Right, currentTile);
                                    }
                                    else
                                    {   //on entering inner lane turn left
                                        nextTile = vehicle.turnVehicleTile(TurnEnum.Left, nextTile);
                                    }
                                }
                                //if he otherwise cannot continue straight in the outer lane
                                else if (!nextTile.laneID.Equals(PathsEnum.OuterPathLane.ToString())) 
                                {
                                    //Continue on outer lane by turning left
                                    nextTile = vehicle.turnVehicleTile(TurnEnum.Left, currentTile);
                                }
                            }
                            //Vehicle is on the inner path lane
                            else if (currentTile.laneID.Equals(PathsEnum.InnerPathLane.ToString()))
                            {
                                if(nextTile.laneID.Equals(string.Empty)) //runs into one of the center tiles with no lanes
                                {
                                    nextTile = vehicle.turnVehicleTile(TurnEnum.Left, currentTile); //Turn left
                                }
                            }

                            /*
                            //Find the correct tile

                            //The 'index' for the adjacent tiles library
                            IEnumerator<KeyValuePair<RotationEnum, Tile>> enumerator = currentTile.adjacentTiles.GetEnumerator();

                            //Take the first adjacent tile
                            nextTile = enumerator.Current.Value;

                            //Pick the correct path
                            nextTile = ChooseCorrectPath(vehicle, nextTile, enumerator);*/
                        }
                        else if (currentTile.laneID.Equals(PathsEnum.InnerPathLane.ToString()) &&
                                 nextTile.laneID.Equals(PathsEnum.OuterPathLane.ToString()))
                        {
                            if (nextTile.laneID.Equals(PathsEnum.OuterPathLane.ToString())) //runs into outer path lane
                            {
                                if (vehicle.innerLaneTurns > 0) //is still he allowed to make turns in the inner lane?
                                {
                                    nextTile = vehicle.turnVehicleTile(TurnEnum.Right, currentTile); //yes, then turn right
                                    vehicle.innerLaneTurns--;
                                }
                            }
                        }
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
                if (enumerator.Current.Value != null && enumerator.Current.Value.laneID.Length > 0)
                {
                    nextTile = enumerator.Current.Value;
                    bool correctPath = false;

                        Lane tileLane;
                        lists.Lanes.TryGetValue(nextTile.laneID, out tileLane);

                        //Check if this is the tile the vehicle should go on
                        if ((nextTile.laneID.Equals(vehicle.destinationLaneID) || //His destination lane is a valid path
                            nextTile.laneID.Equals(vehicle.spawntile.laneID)) //His starting lane is a valid path
                            &&
                            !isOppositeDirection(vehicle.rotation, enumerator.Current.Key)) //He is not allowed to go in the opposite direction
                        {
                            correctPath = true;
                        }
                        else
                        {
                            correctPath = false;
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
                throw new Exception(Strings.ExceptionPath);
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
            if (tile.Texture.Equals(Textures.RedLight) || tile.Texture.Equals(Textures.SidewalkLightRed))
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
                    }
                    else
                    {
                        //no, so wait..
                        if (!vehicle.type.Equals(VehicleTypeEnum.pedestrian)) //not pedestrian
                        {
                            vehicle.stopCar = true;
                        }
                        else //pedestrian
                        {
                            int x = 0;
                            int y = 0;
                            switch(vehicle.rotation)
                            {
                                case RotationEnum.North: y--;
                                    break;
                                case RotationEnum.East: x++;
                                    break;
                                case RotationEnum.South: y++;
                                    break;
                                case RotationEnum.West: x--;
                                    break;
                            }
                            vehicle.stopCar = false;
                            vehicle.occupyingtile += new Vector2(x, y);
                        }
                    }
                }
                else //not occupied
                {
                    vehicle.stopCar = false;

                    //claim it 
                    tile.isOccupied = true;
                    tile.OccupiedID = vehicle.ID;

                    //update vehicle's lane, if the next tile is part of a new lane
                    if (tile.laneID.Length > 0 && !tile.laneID.Equals(vehicle.currentLane.laneID))
                    {
                        //Remove the vehicle from the previous lane
                        Lane lane = vehicle.currentLane;
                        lane.laneVehicles.Remove(vehicle);

                        //Add it to the current lane
                        lists.Lanes.TryGetValue(tile.laneID, out lane); 
                        vehicle.currentLane = lane;
                        lane.laneVehicles.Add(vehicle);
                    }

                    Tile previousTile = lists.Tiles[(int)vehicle.occupyingtile.X, (int)vehicle.occupyingtile.Y];

                    //release previous tile
                    previousTile.isOccupied = false;
                    previousTile.OccupiedID = string.Empty;

                    //Check if it's leaving a detection tile
					if (previousTile.Equals(vehicle.currentLane.detectionClose))
                    {
						if (!vehicle.currentLane.laneID[1].Equals(0) && !vehicle.currentLane.laneID[1].Equals(1) && !vehicle.currentLane.laneID[1].Equals(7))
						{
							// Send a detection message to the host
							communicationForm.WriteDetectionMessage(vehicle.type, LoopEnum.close, (!previousTile.isOccupied).ToString().ToLower(), vehicle.spawntile.laneID, vehicle.destinationLaneID);
						}
						else
						{
							// Send a detection message to the host
							communicationForm.WriteDetectionMessage(vehicle.type, LoopEnum.close, Strings.Null, vehicle.spawntile.laneID, vehicle.destinationLaneID);
						}
                    }
					else if (previousTile.Equals(vehicle.currentLane.detectionFar))
					{
						if (!vehicle.currentLane.laneID[1].Equals(0) && !vehicle.currentLane.laneID[1].Equals(1) && !vehicle.currentLane.laneID[1].Equals(7))
						{
							// Send a detection message to the host
							communicationForm.WriteDetectionMessage(vehicle.type, LoopEnum.far, (!previousTile.isOccupied).ToString().ToLower(), vehicle.spawntile.laneID, vehicle.destinationLaneID);
						}
						else
						{
							// Send a detection message to the host
							communicationForm.WriteDetectionMessage(vehicle.type, LoopEnum.far, Strings.Null, vehicle.spawntile.laneID, vehicle.destinationLaneID);
						}
					}

                    //Check if it's entering a detection tile
                    if (tile.Equals(vehicle.currentLane.detectionClose))
                    {
						if (!vehicle.currentLane.laneID[1].Equals(0) && !vehicle.currentLane.laneID[1].Equals(1) && !vehicle.currentLane.laneID[1].Equals(7))
						{
							// Send a detection message to the host
							communicationForm.WriteDetectionMessage(vehicle.type, LoopEnum.close, (!tile.isOccupied).ToString().ToLower(), vehicle.spawntile.laneID, vehicle.destinationLaneID);
						}
						else
						{
							// Send a detection message to the host
							communicationForm.WriteDetectionMessage(vehicle.type, LoopEnum.close, Strings.Null, vehicle.spawntile.laneID, vehicle.destinationLaneID);
						}
                    }
					else if (tile.Equals(vehicle.currentLane.detectionFar))
					{
						if (!vehicle.currentLane.laneID[1].Equals(0) && !vehicle.currentLane.laneID[1].Equals(1) && !vehicle.currentLane.laneID[1].Equals(7))
						{
							// Send a detection message to the host
							communicationForm.WriteDetectionMessage(vehicle.type, LoopEnum.far, (!tile.isOccupied).ToString().ToLower(), vehicle.spawntile.laneID, vehicle.destinationLaneID);
						}
						else
						{
							// Send a detection message to the host
							communicationForm.WriteDetectionMessage(vehicle.type, LoopEnum.far, Strings.Null, vehicle.spawntile.laneID, vehicle.destinationLaneID);
						}
					}

                    //set the new tile
                    vehicle.occupyingtile = tile.GridCoordinates;
                }
            }
        }
	}
}