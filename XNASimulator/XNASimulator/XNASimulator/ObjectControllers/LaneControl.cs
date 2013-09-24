using System;
using System.Collections.Generic;
using KruispuntGroep4.Simulator.Globals;
using KruispuntGroep4.Simulator.Objects;
using Microsoft.Xna.Framework;

namespace KruispuntGroep4.Simulator.ObjectControllers
{
    class LaneControl
    {
        private Lists lists;
        private int totalSpawnedVehicles;

        public LaneControl(Lists lists)
        {
            this.lists = lists;
            this.totalSpawnedVehicles = 0;
        }

        public void Update(GameTime gametime)
        {
            foreach (KeyValuePair<string, Lane> lane in lists.Lanes)
            {
                SpawnQueuedVehicles(lane.Value);
            }
        }

        public void ChangeTrafficLight(LightsEnum newValue, string laneID)
        {
            Lane lane;
            string opposite = string.Empty;

            lists.Lanes.TryGetValue(laneID, out lane);

            switch (laneID[0])
            {
                case 'N': opposite = "S";
                    break;
                case 'E': opposite = "W";
                    break;
                case 'S': opposite = "N";
                    break;
                case 'W': opposite = "E";
                    break;               
            }

            switch (laneID[1])
            {
                case '0':
                    switch (newValue)
                    {
                        case LightsEnum.Red: 
                            lane.trafficLight.Texture = Textures.SidewalkLightRed;
                            lists.Lanes.TryGetValue((opposite + '7'), out lane);
                            lane.trafficLight.Texture = Textures.SidewalkLightRed;
                            break;
                        case LightsEnum.Green: 
                            lane.trafficLight.Texture = Textures.SidewalkLightGreen;
                            lists.Lanes.TryGetValue((opposite + '7'), out lane);
                            lane.trafficLight.Texture = Textures.SidewalkLightGreen;
                            break;
                    }
                    break;
                case '7':
                    switch (newValue)
                    {
                        case LightsEnum.Red: 
                            lane.trafficLight.Texture = Textures.SidewalkLightRed;
                            lists.Lanes.TryGetValue((opposite + '0'), out lane);
                            lane.trafficLight.Texture = Textures.SidewalkLightRed;
                            break;
                        case LightsEnum.Green: 
                            lane.trafficLight.Texture = Textures.SidewalkLightGreen;
                            lists.Lanes.TryGetValue((opposite + '0'), out lane);
                            lane.trafficLight.Texture = Textures.SidewalkLightGreen;
                            break;
                    }
                    break;
                default :
                    switch (newValue)
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
                    break;
            }
        }

        public void SpawnVehicle(VehicleTypeEnum vehicleType, string spawnLaneID, string destinationLaneID)
        {         
            Lane spawnLane;
            Vehicle newVehicle;
            string vehicleID;

            //Get the lane the vehicle needs to be spawned in
            lists.Lanes.TryGetValue(spawnLaneID, out spawnLane);

            //Calculate a unique ID
            vehicleID = vehicleType.ToString() + totalSpawnedVehicles;
            totalSpawnedVehicles++;

            //Create an empty vehicle with only an ID
            newVehicle = new Vehicle(vehicleID);

            #region vehicle creation
            switch (vehicleType)
            {
				case VehicleTypeEnum.pedestrian: newVehicle = new Vehicle(Textures.Pedestrian, vehicleID, VehicleTypeEnum.pedestrian, 1.1f);
					break;
				case VehicleTypeEnum.bicycle: newVehicle = new Vehicle(Textures.Bike, vehicleID, VehicleTypeEnum.bicycle, 1.4f);
					break;
                case VehicleTypeEnum.bus: newVehicle = new Vehicle(Textures.Bus, vehicleID, VehicleTypeEnum.bus, 1.7f);
                    break;
                case VehicleTypeEnum.car: newVehicle = new Vehicle(Textures.Car, vehicleID, VehicleTypeEnum.car, 2f);
                    break; 
            }
            #endregion

            newVehicle.destinationLaneID = destinationLaneID;

            #region vehicle route
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
            #endregion

            //If the lane has space...
            if (!spawnLane.spawnTile.isOccupied)
            {
                //Add the vehicle to the Lane and fill it with the Lane's data
                newVehicle = spawnLane.AddVehicle(newVehicle);

                //Add randomness to the location of bicycles and pedestrians
                if (newVehicle.sprite.Equals(Textures.Bike) || newVehicle.sprite.Equals(Textures.Pedestrian))
                {
                    Random random = new Random();
                    Vector2 randomiser = new Vector2(random.Next(-4,4), random.Next(-4,4));
                    newVehicle.drawposition += randomiser;
                }

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

                    vehicle = lane.AddVehicle(vehicle); //Add to lane
                    lists.Vehicles.Add(vehicle); //Add to master list
                }
            }           
        }

    }
}