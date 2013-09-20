using System.Collections.Generic;
using KruispuntGroep4.Simulator.Globals;
using KruispuntGroep4.Simulator.Main;
using KruispuntGroep4.Simulator.Objects;
using Microsoft.Xna.Framework;

namespace KruispuntGroep4.Main
{
    class LaneBuilder
    {
        private Lists lists;
        private List<string> laneIDs;
        private List<PathsEnum> pathlaneIDs;

        public LaneBuilder(Lists lists)
        {
            this.lists = lists;
            this.laneIDs = new List<string>();
            this.pathlaneIDs = new List<PathsEnum>();

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

            pathlaneIDs.Add(PathsEnum.NorthToEast);
            pathlaneIDs.Add(PathsEnum.NorthToSouth);
            pathlaneIDs.Add(PathsEnum.NorthToWest);

            pathlaneIDs.Add(PathsEnum.EastToSouth);
            pathlaneIDs.Add(PathsEnum.EastToWest);
            pathlaneIDs.Add(PathsEnum.EastToNorth);

            pathlaneIDs.Add(PathsEnum.SouthToWest);
            pathlaneIDs.Add(PathsEnum.SouthToNorth);
            pathlaneIDs.Add(PathsEnum.SouthToEast);

            pathlaneIDs.Add(PathsEnum.WestToNorth);
            pathlaneIDs.Add(PathsEnum.WestToEast);
            pathlaneIDs.Add(PathsEnum.WestToSouth);

            #endregion

            foreach (string ID in laneIDs)
            {
                lists.Lanes.Add(ID, new Lane(ID));
            }
            foreach (PathsEnum ID in pathlaneIDs)
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

            #region create pathing lanes
            switch (lane.pathLaneID)
            {
                case PathsEnum.NorthToEast:                   
                    lists.Lanes.TryGetValue("N1", out startLane);
                    lists.Lanes.TryGetValue("E6", out endLane);

                    //Go East first
                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.South, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.East, lane);

                    //Then South
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.South, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.South, lane);
                    break;
                case PathsEnum.NorthToSouth:
                    lists.Lanes.TryGetValue("N4", out startLane);
                    lists.Lanes.TryGetValue("S6", out endLane);

                    //Go West first
                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.South, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.West, lane);

                    //Then South
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.South, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.South, lane);
                    break;
                case PathsEnum.NorthToWest:
                    lists.Lanes.TryGetValue("N3", out startLane);
                    lists.Lanes.TryGetValue("W6", out endLane);

                    //West
                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.South, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.West, lane);
                    break;

                case PathsEnum.EastToSouth:                 
                    lists.Lanes.TryGetValue("E1", out startLane);
                    lists.Lanes.TryGetValue("S6", out endLane);

                    //South
                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.West, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.South, lane);

                    //West
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.West, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.West, lane);
                    break;
                case PathsEnum.EastToWest:
                    lists.Lanes.TryGetValue("E4", out startLane);
                    lists.Lanes.TryGetValue("W6", out endLane);

                    //North
                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.West, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.North, lane);

                    //West
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.West, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.West, lane);
                    break;
                case PathsEnum.EastToNorth:
                    lists.Lanes.TryGetValue("E3", out startLane);
                    lists.Lanes.TryGetValue("N6", out endLane);

                    //North
                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.West, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.North, lane);
                    break;

                case PathsEnum.SouthToEast:
                    lists.Lanes.TryGetValue("S3", out startLane);
                    lists.Lanes.TryGetValue("E6", out endLane);

                    //East
                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.North, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.East, lane);
                    break;
                case PathsEnum.SouthToWest:
                    lists.Lanes.TryGetValue("S1", out startLane);
                    lists.Lanes.TryGetValue("W6", out endLane);

                    //West
                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.North, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.West, lane);

                    //North
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.North, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.North, lane);
                    break;
                case PathsEnum.SouthToNorth:
                    lists.Lanes.TryGetValue("S4", out startLane);
                    lists.Lanes.TryGetValue("N6", out endLane);

                    //East
                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.North, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.East, lane);

                    //North
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.North, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.North, lane);
                    break;

                case PathsEnum.WestToNorth:
                    lists.Lanes.TryGetValue("W1", out startLane);
                    lists.Lanes.TryGetValue("N6", out endLane);

                    //North
                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.East, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.North, lane);

                    //East
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.East, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.East, lane);
                    break;
                case PathsEnum.WestToEast:
                    lists.Lanes.TryGetValue("W4", out startLane);
                    lists.Lanes.TryGetValue("E6", out endLane);

                    //South
                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.East, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.South, lane);

                    //East
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.East, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.East, lane);
                    break;
                case PathsEnum.WestToSouth:
                    lists.Lanes.TryGetValue("W3", out startLane);
                    lists.Lanes.TryGetValue("S6", out endLane);

                    //South
                    startLane.trafficLight.adjacentTiles.TryGetValue(RotationEnum.East, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.South, lane);
                    break;
            }
            #endregion
        }

        private void LoadPathLane(Tile startTile, Lane endLane, RotationEnum direction, Lane pathLane)
        {
            pathLane.pathDirections.Add(direction);

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
                    foreach (string ID in newTile.laneIDs)
                    {
                        //See if it's the end lane
                        if (ID.Equals(endLane.laneID))
                        {
                            //End building
                            endBuild = true;
                        }
                        else //Not the end lane
                        {
                            //Check if it's a non-pathing lane 
                            if (newTile.laneIDs.Count == 1 && this.laneIDs.Contains(ID))
                            {
                                //Stop building (in this direction)
                                endBuild = true;
                            }
                            else //So it's a pathing lane, keep building
                            {
                                pathLane.laneTiles.Add(newTile);
                            }
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
                    if (lane.laneID[1].Equals('6')) //6 is the exit lane, this is to help pathing
                        lane.pathDirections.Add(RotationEnum.North);
                    laneDirection = RotationEnum.North;
                    laneSize = MainGame.LaneLengthVer;
                    lane.laneDirection = RotationEnum.South;
                    break;
                case 'E':
                    if (lane.laneID[1].Equals('6'))
                        lane.pathDirections.Add(RotationEnum.East);
                    laneDirection = RotationEnum.East;
                    laneSize = MainGame.LaneLengthHor;
                    lane.laneDirection = RotationEnum.West;
                    break;
                case 'S':
                    if (lane.laneID[1].Equals('6'))
                        lane.pathDirections.Add(RotationEnum.South);
                    laneDirection = RotationEnum.South;
                    laneSize = MainGame.LaneLengthVer;
                    lane.laneDirection = RotationEnum.North;
                    break;
                case 'W':
                    if (lane.laneID[1].Equals('6'))
                        lane.pathDirections.Add(RotationEnum.West);
                    laneDirection = RotationEnum.West;
                    laneSize = MainGame.LaneLengthHor;
                    lane.laneDirection = RotationEnum.East;
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
            foreach (Tile tile in lane.laneTiles)
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

    }
}
