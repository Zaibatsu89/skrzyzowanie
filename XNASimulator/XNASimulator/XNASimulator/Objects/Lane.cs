using System.Collections.Generic;
using KruispuntGroep6.Simulator.Globals;
using System;

namespace KruispuntGroep6.Simulator.Objects
{
    class Lane
    {
        public List<Tile> laneTiles { get; set; }
        public List<Vehicle> laneVehicles { get; set; }
        public Queue<Vehicle> vehicleQueue { get; set; }

        public string laneID { get; private set; }
        public Tile trafficLight { get; set; }
        public Tile detectionFar { get; set; }
        public Tile detectionClose { get; set; }
        public Tile sidewalkCrossing { get; set; }
        public Tile spawnTile { get; set; }

        private Random random;

        public Lane(string ID)
        {
            this.laneID = ID;
            this.laneTiles = new List<Tile>();
            this.laneVehicles = new List<Vehicle>();
            this.vehicleQueue = new Queue<Vehicle>();
        }

        public Vehicle AddVehicle(string vehicleID)
        {
            Vehicle vehicle = new Vehicle(Textures.Car, vehicleID);
            vehicle.rotation = this.spawnTile.Rotation;
            vehicle.position = this.spawnTile.Position;
            vehicle.drawposition = this.spawnTile.DrawPosition;

            //occupy tile
            vehicle.spawntile = this.spawnTile;
            vehicle.occupyingtile = this.spawnTile.GridCoordinates;
            this.spawnTile.isOccupied = true;
            this.spawnTile.OccupiedID = vehicle.ID;

            vehicle.currentLane = this;
            this.laneVehicles.Add(vehicle);
            return vehicle;
        }
    }
}