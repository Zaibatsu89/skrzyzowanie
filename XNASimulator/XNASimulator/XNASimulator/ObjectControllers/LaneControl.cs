using System.Collections.Generic;
using KruispuntGroep4.Simulator.Globals;
using KruispuntGroep4.Simulator.Main;
using KruispuntGroep4.Simulator.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public void Draw(GameTime gametime, SpriteBatch spriteBatch)
        {
        }

        public void ChangeTrafficLight(string newValue, string laneID)
        {
            Lane lane;
            lists.Lanes.TryGetValue(laneID, out lane);


            switch(newValue)
            {
                case "green": lane.trafficLight.Texture = Textures.GreenLight;
                    break;
                case "red": lane.trafficLight.Texture = Textures.RedLight;
                    break;
            }
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