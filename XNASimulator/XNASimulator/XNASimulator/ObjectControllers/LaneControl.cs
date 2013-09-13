using System.Collections.Generic;
using KruispuntGroep6.Simulator.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using KruispuntGroep6.Simulator.Objects;
using KruispuntGroep6.Simulator.Main;

namespace KruispuntGroep6.Simulator.ObjectControllers
{
    class LaneControl
    {
        private Lists lists;
        private List<string> laneIDs;
        private int totalSpawnedVehicles;

        public LaneControl(Lists lists)
        {
            this.lists = lists;
            this.laneIDs = new List<string>();
            this.totalSpawnedVehicles = 0;

            InitLanes();
        }

        private void InitLanes()
        {
            for (int i = 0; i < MainGame.NrOfLanes; i++)
            {
                laneIDs.Add("N" + i);
                laneIDs.Add("E" + i);
                laneIDs.Add("S" + i);
                laneIDs.Add("W" + i);
            }

            foreach (string ID in laneIDs)
            {
                lists.Lanes.Add(ID, new Lane(ID));
            }
        }

        public void LoadLanes()
        {
            foreach (KeyValuePair<string, Lane> lane in lists.Lanes)
            {
                LoadLane(lane.Value);
            }
        }

        private void LoadLane(Lane lane)
        {   
            //Build all the lanes starting from the location of the stoplights and going outwards
            switch (lane.laneID)
            {
                case "N0": LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer - 1), lane); //Northwest vertical sidewalk
                    break;
                case "N1": LoadLane(new Vector2(MainGame.LaneLengthHor, MainGame.LaneLengthVer - 1), lane); 
                    break;
                case "N2": LoadLane(new Vector2(MainGame.LaneLengthHor + 1, MainGame.LaneLengthVer - 1), lane);
                    break;
                case "N3": LoadLane(new Vector2(MainGame.LaneLengthHor + 2, MainGame.LaneLengthVer - 1), lane);
                    break;
                case "N4": LoadLane(new Vector2(MainGame.LaneLengthHor + 3, MainGame.LaneLengthVer - 1), lane);
                    break;
                case "N5": LoadLane(new Vector2(MainGame.LaneLengthHor + 4, MainGame.LaneLengthVer - 1), lane);
                    break;
                case "N6": LoadLane(new Vector2(MainGame.LaneLengthHor + 5, MainGame.LaneLengthVer - 1), lane);
                    break;
                case "N7": LoadLane(new Vector2(MainGame.LaneLengthHor + 6, MainGame.LaneLengthVer - 1), lane); //Northeast vertical sidewalk
                    break;

                case "E0": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer - 1), lane); //Northeast horizontal sidewalk
                    break;
                case "E1": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer), lane);
                    break;
                case "E2": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + 1), lane);
                    break;
                case "E3": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + 2), lane);
                    break;
                case "E4": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + 3), lane);
                    break;
                case "E5": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + 4), lane);
                    break;
                case "E6": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + 5), lane);
                    break;
                case "E7": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + 6), lane); //Southeast horizontal sidewalk
                    break;

                case "W0": LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize), lane); //Southwest horizontal sidewalk
                    break;
                case "W1": LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 1), lane);
                    break;
                case "W2": LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 2), lane);
                    break;
                case "W3": LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 3), lane);
                    break;
                case "W4": LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 4), lane);
                    break;
                case "W5": LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 5), lane);
                    break;
                case "W6": LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 6), lane);
                    break;
                case "W7": LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 7), lane); //Northwest horizontal sidewalk
                    break;

                case "S0": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + MainGame.MiddleSize), lane); //Southeast vertical sidewalk
                    break;
                case "S1": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 1, MainGame.LaneLengthVer + MainGame.MiddleSize), lane);
                    break;
                case "S2": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 2, MainGame.LaneLengthVer + MainGame.MiddleSize), lane);
                    break;
                case "S3": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 3, MainGame.LaneLengthVer + MainGame.MiddleSize), lane);
                    break;
                case "S4": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 4, MainGame.LaneLengthVer + MainGame.MiddleSize), lane);
                    break;
                case "S5": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 5, MainGame.LaneLengthVer + MainGame.MiddleSize), lane);
                    break;
                case "S6": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 6, MainGame.LaneLengthVer + MainGame.MiddleSize), lane);
                    break;
                case "S7": LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 7, MainGame.LaneLengthVer + MainGame.MiddleSize), lane); //Southwest vertical sidewalk
                    break;
            }
        }

        private void LoadLane(Vector2 laneStartPosition, Lane lane)
        {
            RotationEnum laneDirection = RotationEnum.North;
            int laneSize = MainGame.LaneLengthHor;

            Tile laneSpawnTile;
            Tile laneStartTile = lists.Tiles[(int)laneStartPosition.X, (int)laneStartPosition.Y];
            lane.laneTiles.Add(laneStartTile);

            //Figure out the lane's direction
            switch (lane.laneID[0])
            {
                case 'N': 
                    laneDirection = RotationEnum.North;
                    laneSize = MainGame.LaneLengthVer;
                    break;
                case 'E': 
                    laneDirection = RotationEnum.East;
                    laneSize = MainGame.LaneLengthHor;
                    break;
                case 'S': 
                    laneDirection = RotationEnum.South;
                    laneSize = MainGame.LaneLengthVer;
                    break;
                case 'W': 
                    laneDirection = RotationEnum.West;
                    laneSize = MainGame.LaneLengthHor;
                    break;
            }

            //Build the lane
            for (int i = 0; i < laneSize - 1; i++)
            {
                //Keep adding tiles in the right direction until you reach the lanelength
                lane.laneTiles.Add(lane.laneTiles[i].adjacentTiles[laneDirection.ToString()]);

                //When the last tile has been added...
                if (lane.laneTiles.Count == laneSize)
                {
                    laneSpawnTile = lane.laneTiles[lane.laneTiles.Count - 1];
                    laneSpawnTile.isSpawn = true;
                    lane.spawnTile = laneSpawnTile;
                }
            }

            //When done building, do tile assignments
            foreach(Tile tile in lane.laneTiles)
            {
                //Make each lane tile know it's part of this new lane
                tile.laneID = lane.laneID;

                if (tile.Texture.Equals(Textures.RedLight))
                {
                    lane.trafficLight = tile;
                }
                else if (tile.Texture.Equals(Textures.CarSortDown) || 
                        tile.Texture.Equals(Textures.CarSortLeft) ||
                        tile.Texture.Equals(Textures.CarSortRight) ||
                        tile.Texture.Equals(Textures.Buslane))
                {
                    //set the detection tiles
                    lane.detectionClose = tile;                   
                    switch (tile.Rotation)
                    {
                        case RotationEnum.North:
                            lane.detectionFar = lists.Tiles[(int)tile.GridCoordinates.X, (int)tile.GridCoordinates.Y + MainGame.DetectionLoopLength];
                            break;
                        case RotationEnum.East:
                            lane.detectionFar = lists.Tiles[(int)tile.GridCoordinates.X - MainGame.DetectionLoopLength, (int)tile.GridCoordinates.Y];
                            break;
                        case RotationEnum.South:
                            lane.detectionFar = lists.Tiles[(int)tile.GridCoordinates.X, (int)tile.GridCoordinates.Y - MainGame.DetectionLoopLength];
                            break;
                        case RotationEnum.West:
                            lane.detectionFar = lists.Tiles[(int)tile.GridCoordinates.X + MainGame.DetectionLoopLength, (int)tile.GridCoordinates.Y];
                            break;
                    }

                }
            }
        }

        public void Update(GameTime gametime)
        {
            foreach (KeyValuePair<string, Lane> lane in lists.Lanes)
            {
                SpawnQueuedVehicles(lane.Value);
            }
        }

        public void Draw(GameTime gametime, SpriteBatch spriteBatch)
        {
        }

        public void SpawnVehicle(string vehicleType, string spawnLaneID, string destinationLaneID)
        {         
            Lane spawnLane;
            Vehicle newVehicle;
            string vehicleID;

            //Get the lane the vehicle needs to be spawned in
            lists.Lanes.TryGetValue(spawnLaneID, out spawnLane);

            //Calculate a unique ID
            vehicleID = vehicleType + totalSpawnedVehicles;
            totalSpawnedVehicles++;

            //Create an empty vehicle with only an ID
            newVehicle = new Vehicle(vehicleID);
     
            //If the lane has space...
            if (!spawnLane.spawnTile.isOccupied)
            {
                //Add the vehicle to the Lane and fill it with the Lane's data
                newVehicle = spawnLane.AddVehicle(vehicleID);

                //Set its destination
                newVehicle.destinationLaneID = destinationLaneID;

                //Add the spawned vehicle to the master list
                lists.Vehicles.Add(newVehicle);
            }
            else
            {
                //If the lane is full, add the empty vehicle object to the lane's queue
                spawnLane.vehicleQueue.Enqueue(newVehicle);
            }
                    
        }

        private void SpawnQueuedVehicles(Lane lane)
        {
                if (lane.vehicleQueue.Count > 0)
                {
                    if (!lane.spawnTile.isOccupied)
                    {
                        Vehicle vehicle = lane.vehicleQueue.Dequeue();
                        lane.AddVehicle(vehicle.ID);
                    }
                }           
        }

    }
}