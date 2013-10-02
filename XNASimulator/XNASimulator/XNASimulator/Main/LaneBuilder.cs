using System.Collections.Generic;
using KruispuntGroep4.Globals;
using KruispuntGroep4.Simulator.Globals;
using KruispuntGroep4.Simulator.Main;
using KruispuntGroep4.Simulator.Objects;
using Microsoft.Xna.Framework;

namespace KruispuntGroep4.Main
{
    /// <summary>
    /// Creates all Lane objects
    /// </summary>
    class LaneBuilder
    {
        private Lists lists;
        private List<string> laneIDs;
        private List<PathsEnum> pathlaneIDs;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="lists">Lists of all Vehicles, Lanes and Tiles</param>
        public LaneBuilder(Lists lists)
        {
            this.lists = lists;
            this.laneIDs = new List<string>();
            this.pathlaneIDs = new List<PathsEnum>();

            InitLanes();
        }

        /// <summary>
        /// Creates all empty Lanes with unique IDs
        /// </summary>
        private void InitLanes()
        {
            for (int i = 0; i < MainGame.NrOfLanes; i++)
            {
                laneIDs.Add(Strings.DirectionNorth + i);
                laneIDs.Add(Strings.DirectionEast + i);
                laneIDs.Add(Strings.DirectionSouth + i);
                laneIDs.Add(Strings.DirectionWest + i);
            }

            #region pathfinding lanes

            pathlaneIDs.Add(PathsEnum.InnerPathLane);
            pathlaneIDs.Add(PathsEnum.OuterPathLane);

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

        /// <summary>
        /// Fills the lanes with the required information
        /// </summary>
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
            Lane pathLane;

            lists.Lanes.TryGetValue(PathsEnum.OuterPathLane.ToString(), out pathLane); //Outer first since inner needs it
            LoadPathLane(pathLane);

            lists.Lanes.TryGetValue(PathsEnum.InnerPathLane.ToString(), out pathLane);
            LoadPathLane(pathLane);
        }

        /// <summary>
        /// Fills a pathing lane with its tiles
        /// </summary>
        /// <param name="lane">Lane in question</param>
        private void LoadPathLane(Lane lane)
        {
            Tile startTile;
            Lane startLane;
            Lane endLane;

            switch (lane.pathLaneID)
            {
                case PathsEnum.InnerPathLane:
                    lists.Lanes.TryGetValue(Strings.LaneNorthTwo, out startLane);
                    lists.Lanes.TryGetValue(Strings.LaneWestFive, out endLane);

                    //Go South first
                    startLane.sidewalkCrossing.adjacentTiles.TryGetValue(RotationEnum.South, out startTile);
                    startTile.adjacentTiles.TryGetValue(RotationEnum.South, out startTile); //2nd tile south
                    LoadPathLane(startTile, endLane, RotationEnum.South, lane);

                    //Then East
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.East, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.East, lane);

                    //North
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.North, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.North, lane);

                    //West
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.West, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.West, lane);

                    break;
                case PathsEnum.OuterPathLane:
                    lists.Lanes.TryGetValue(Strings.LaneNorthOne, out startLane);
                    lists.Lanes.TryGetValue(Strings.LaneWestSix, out endLane);

                    //Go South first
                    startLane.sidewalkCrossing.adjacentTiles.TryGetValue(RotationEnum.South, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.South, lane);

                    //Then East
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.East, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.East, lane);

                    //North
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.North, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.North, lane);

                    //West
                    lane.laneTiles[lane.laneTiles.Count - 1].adjacentTiles.TryGetValue(RotationEnum.West, out startTile);
                    LoadPathLane(startTile, endLane, RotationEnum.West, lane);
                    break;
            }
        }

        /// <summary>
        /// Loads all tiles in a single direction into a pathing lane
        /// </summary>
        /// <param name="startTile">Starting tile of this direction</param>
        /// <param name="endLane">The lane at which the path must stop</param>
        /// <param name="direction">Direction the tiles are added in</param>
        /// <param name="pathLane">The pathing lane in question</param>
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
                if (newTile.laneID.Length > 0)
                {
                    //Stop building in this direction
                    endBuild = true;
                    
                }
                else //Tile has no lane, so add it
                {
                    pathLane.laneTiles.Add(newTile);
                }
            }

            //Let the tiles know they're in this lane
            foreach (Tile tile in pathLane.laneTiles)
            {
                if (!tile.laneID.Equals(pathLane.laneID))
                {
                    tile.laneID = pathLane.laneID;
                }
            }

        }

        /// <summary>
        /// Loads an entry/exit lane depending on which one it is
        /// </summary>
        /// <param name="lane">Lane in question</param>
        private void LoadLane(Lane lane)
        {
            //Build all the lanes starting from the location of the stoplights and going outwards
            switch (lane.laneID)
            {
                case Strings.LaneNorthZero: LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer - 1), lane); //Northwest vertical sidewalk
                    break;
                case Strings.LaneNorthOne: LoadLane(new Vector2(MainGame.LaneLengthHor, MainGame.LaneLengthVer - 1), lane);
                    break;
                case Strings.LaneNorthTwo: LoadLane(new Vector2(MainGame.LaneLengthHor + 1, MainGame.LaneLengthVer - 1), lane);
                    break;
                case Strings.LaneNorthThree: LoadLane(new Vector2(MainGame.LaneLengthHor + 2, MainGame.LaneLengthVer - 1), lane);
                    break;
                case Strings.LaneNorthFour: LoadLane(new Vector2(MainGame.LaneLengthHor + 3, MainGame.LaneLengthVer - 1), lane);
                    break;
                case Strings.LaneNorthFive: LoadLane(new Vector2(MainGame.LaneLengthHor + 4, MainGame.LaneLengthVer - 1), lane);
                    break;
                case Strings.LaneNorthSix: LoadLane(new Vector2(MainGame.LaneLengthHor + 5, MainGame.LaneLengthVer - 1), lane);
                    break;
                case Strings.LaneNorthSeven: LoadLane(new Vector2(MainGame.LaneLengthHor + 6, MainGame.LaneLengthVer - 1), lane); //Northeast vertical sidewalk
                    break;

                case Strings.LaneEastZero: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer - 1), lane); //Northeast horizontal sidewalk
                    break;
                case Strings.LaneEastOne: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer), lane);
                    break;
                case Strings.LaneEastTwo: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + 1), lane);
                    break;
                case Strings.LaneEastThree: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + 2), lane);
                    break;
                case Strings.LaneEastFour: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + 3), lane);
                    break;
                case Strings.LaneEastFive: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + 4), lane);
                    break;
                case Strings.LaneEastSix: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + 5), lane);
                    break;
                case Strings.LaneEastSeven: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + 6), lane); //Southeast horizontal sidewalk
                    break;

                case Strings.LaneWestZero: LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize), lane); //Southwest horizontal sidewalk
                    break;
                case Strings.LaneWestOne: LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 1), lane);
                    break;
                case Strings.LaneWestTwo: LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 2), lane);
                    break;
                case Strings.LaneWestThree: LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 3), lane);
                    break;
                case Strings.LaneWestFour: LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 4), lane);
                    break;
                case Strings.LaneWestFive: LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 5), lane);
                    break;
                case Strings.LaneWestSix: LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 6), lane);
                    break;
                case Strings.LaneWestSeven: LoadLane(new Vector2(MainGame.LaneLengthHor - 1, MainGame.LaneLengthVer + MainGame.MiddleSize - 7), lane); //Northwest horizontal sidewalk
                    break;

                case Strings.LaneSouthZero: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize, MainGame.LaneLengthVer + MainGame.MiddleSize), lane); //Southeast vertical sidewalk
                    break;
                case Strings.LaneSouthOne: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 1, MainGame.LaneLengthVer + MainGame.MiddleSize), lane);
                    break;
                case Strings.LaneSouthTwo: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 2, MainGame.LaneLengthVer + MainGame.MiddleSize), lane);
                    break;
                case Strings.LaneSouthThree: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 3, MainGame.LaneLengthVer + MainGame.MiddleSize), lane);
                    break;
                case Strings.LaneSouthFour: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 4, MainGame.LaneLengthVer + MainGame.MiddleSize), lane);
                    break;
                case Strings.LaneSouthFive: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 5, MainGame.LaneLengthVer + MainGame.MiddleSize), lane);
                    break;
                case Strings.LaneSouthSix: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 6, MainGame.LaneLengthVer + MainGame.MiddleSize), lane);
                    break;
                case Strings.LaneSouthSeven: LoadLane(new Vector2(MainGame.LaneLengthHor + MainGame.MiddleSize - 7, MainGame.LaneLengthVer + MainGame.MiddleSize), lane); //Southwest vertical sidewalk
                    break;
            }
        }

        /// <summary>
        /// Fills an entry/exit lane with its data
        /// </summary>
        /// <param name="laneStartPosition">Starting point of the lane on the crossroad grid</param>
        /// <param name="lane">Lane in question</param>
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
                    lane.laneDirection = RotationEnum.South;
                    break;
                case 'E':
                    laneDirection = RotationEnum.East;
                    laneSize = MainGame.LaneLengthHor;
                    lane.laneDirection = RotationEnum.West;
                    break;
                case 'S':
                    laneDirection = RotationEnum.South;
                    laneSize = MainGame.LaneLengthVer;
                    lane.laneDirection = RotationEnum.North;
                    break;
                case 'W':
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
                tile.laneID = lane.laneID;

                if (tile.Texture.Equals(Textures.RedLight) || tile.Texture.Equals(Textures.SidewalkLightRed))
                {
                    lane.trafficLight = tile;
                }

                else if (tile.Texture.Equals(Textures.Crossing))
                {
                    lane.sidewalkCrossing = tile;
                }
                else if (tile.Texture.Equals(Textures.CarSortDown) ||
                        tile.Texture.Equals(Textures.CarSortLeft) ||
                        tile.Texture.Equals(Textures.CarSortRight) ||
                        tile.Texture.Equals(Textures.Buslane) ||
                        tile.Texture.Equals(Textures.BikeDetect) ||
                        tile.Texture.Equals(Textures.SidewalkDetect))
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
