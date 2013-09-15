using System.Collections.Generic;
using KruispuntGroep6.Simulator.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using KruispuntGroep6.Simulator.Objects;
using KruispuntGroep6.Simulator.Main;
using XNASimulator.Globals;

namespace KruispuntGroep6.Simulator.ObjectControllers
{
    class LaneControl
    {
        private Lists lists;
        private List<string> laneIDs;
        private List<DirectionEnum> pathlaneIDs;
        private int totalSpawnedVehicles;

        public LaneControl(Lists lists)
        {
            this.lists = lists;
            this.laneIDs = new List<string>();
            this.pathlaneIDs = new List<DirectionEnum>();
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

            #region pathfinding lanes

            pathlaneIDs.Add(DirectionEnum.NorthToEast);
            pathlaneIDs.Add(DirectionEnum.NorthToSouth);
            pathlaneIDs.Add(DirectionEnum.NorthToWest);

            pathlaneIDs.Add(DirectionEnum.EastToSouth);
            pathlaneIDs.Add(DirectionEnum.EastToWest);
            pathlaneIDs.Add(DirectionEnum.EastToNorth);

            pathlaneIDs.Add(DirectionEnum.SouthToWest);
            pathlaneIDs.Add(DirectionEnum.SouthToNorth);
            pathlaneIDs.Add(DirectionEnum.SouthToEast);

            pathlaneIDs.Add(DirectionEnum.WestToNorth);
            pathlaneIDs.Add(DirectionEnum.WestToEast);
            pathlaneIDs.Add(DirectionEnum.WestToSouth);

            #endregion

            foreach (string ID in laneIDs)
            {
                lists.Lanes.Add(ID, new Lane(ID));
            }
            foreach (DirectionEnum ID in pathlaneIDs)
            {
                lists.Lanes.Add(ID.ToString(), new Lane(ID));
            }
        }

        public void LoadLanes()
        {
            //First build all the regular lanes
            foreach (KeyValuePair<string, Lane> lane in lists.Lanes)
            {
                if (!lane.Value.isPathLane)
                {
                    LoadLane(lane.Value);
                }
            }

            //Then all the pathing lanes
            foreach (KeyValuePair<string, Lane> lane in lists.Lanes)
            {
                if (lane.Value.isPathLane)
                {
                    LoadPathLane(lane.Value);
                }
            }
        }

        private void LoadPathLane(Lane lane)
        {
            Tile startTile;
            Lane startLane;
            Lane endLane;

            switch (lane.pathLaneID)
            {
                case DirectionEnum.NorthToEast:
                    break;
                case DirectionEnum.NorthToSouth:
                    break;
                case DirectionEnum.NorthToWest:
                    lists.Lanes.TryGetValue("N3", out startLane);
                    lists.Lanes.TryGetValue("W6", out endLane);

                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.South, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.West, lane);
                    break;

                case DirectionEnum.EastToSouth:
                    break;
                case DirectionEnum.EastToWest:
                    break;
                case DirectionEnum.EastToNorth:
                    break;

                case DirectionEnum.SouthToEast:
                    break;
                case DirectionEnum.SouthToWest:
                    break;
                case DirectionEnum.SouthToNorth:
                    break;

                case DirectionEnum.WestToNorth:
                    break;
                case DirectionEnum.WestToEast:
                    break;
                case DirectionEnum.WestToSouth:
                    break;
            }
        }

        private void LoadPathLane(Tile startTile, Lane endLane, RotationEnum direction, Lane pathLane)
        {
            //Add the first tile
            pathLane.laneTiles.Add(startTile);

            Tile newTile = startTile;
            bool endBuild = false;

            while (!endBuild)
            {
                newTile.adjacentTiles.TryGetValue(direction, out newTile);

                //When you reach an existing lane...
                if (newTile.laneIDs.Count > 0)
                {
                    //Check the lane IDs
                    foreach(string ID in newTile.laneIDs)
                    {
                        //See if it's the end lane
                        if (ID.Equals(endLane.laneID))
                        {
                            //End building
                            endBuild = true;
                        }
                        else //Keep building
                        {
                            pathLane.laneTiles.Add(newTile);
                        }
                    }
                }
                else //Tile has no lane, so add it
                {
                    pathLane.laneTiles.Add(newTile);
                }
            }

            //Let the tiles know they're in this lane
            foreach (Tile tile in pathLane.laneTiles)
            {
                if (!tile.laneIDs.Contains(pathLane.laneID))
                {
                    tile.laneIDs.Add(pathLane.laneID);
                }
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
                lane.laneTiles.Add(lane.laneTiles[i].adjacentTiles[laneDirection]);

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
                tile.laneIDs.Add(lane.laneID);

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
            //Set its destination
            newVehicle.destinationLaneID = destinationLaneID;

            #region vehicle route
            switch (spawnLaneID[0].ToString() + destinationLaneID[0].ToString())
            {
                case "NE": newVehicle.direction = DirectionEnum.NorthToEast;
                    break;
                case "NS": newVehicle.direction = DirectionEnum.NorthToSouth;
                    break;
                case "NW": newVehicle.direction = DirectionEnum.NorthToWest;
                    break;

                case "ES": newVehicle.direction = DirectionEnum.EastToSouth;
                    break;
                case "EW": newVehicle.direction = DirectionEnum.EastToWest;
                    break;
                case "EN": newVehicle.direction = DirectionEnum.EastToNorth;
                    break;

                case "SW": newVehicle.direction = DirectionEnum.SouthToWest;
                    break;
                case "SN": newVehicle.direction = DirectionEnum.SouthToNorth;
                    break;
                case "SE": newVehicle.direction = DirectionEnum.SouthToEast;
                    break;

                case "WN": newVehicle.direction = DirectionEnum.WestToNorth;
                    break;
                case "WE": newVehicle.direction = DirectionEnum.WestToEast;
                    break;
                case "WS": newVehicle.direction = DirectionEnum.WestToSouth;
                    break;
            }
            #endregion

            //If the lane has space...
            if (!spawnLane.spawnTile.isOccupied)
            {
                //Add the vehicle to the Lane and fill it with the Lane's data
                newVehicle = spawnLane.AddVehicle(vehicleID);

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

                        vehicle = lane.AddVehicle(vehicle.ID); //Add to lane
                        lists.Vehicles.Add(vehicle); //Add to master list
                    }
                }           
        }

    }
}