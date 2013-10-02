using System.Collections.Generic;
using KruispuntGroep4.Simulator.Globals;

namespace KruispuntGroep4.Simulator.Objects
{
    /// <summary>
    /// A lane is a collection of tiles with a unique ID
    /// They help vehicles with pathing and are called when
    /// a traffic light tile needs its light switched
    /// </summary>
    class Lane
    {
        public List<Tile> laneTiles { get; set; }
        public List<Vehicle> laneVehicles { get; set; }
        public Queue<Vehicle> vehicleQueue { get; set; }

        public string laneID { get; private set; }
        public PathsEnum pathLaneID { get; private set; }
        public Tile trafficLight { get; set; }
        public Tile detectionFar { get; set; }
        public Tile detectionClose { get; set; }
        public Tile sidewalkCrossing { get; set; }
        public Tile spawnTile { get; set; }

        public bool isPathLane { get; set; }
        public RotationEnum laneDirection { get; set; }

        /// <summary>
        /// A standard entry/exit lane
        /// </summary>
        /// <param name="ID">Unique ID</param>
        public Lane(string ID)
        {
            this.laneID = ID;
            this.isPathLane = false;
            this.laneTiles = new List<Tile>();
            this.laneVehicles = new List<Vehicle>();
            this.vehicleQueue = new Queue<Vehicle>();
        }

        /// <summary>
        /// A central lane used purely for vehicle pathing
        /// </summary>
        /// <param name="ID">Unique ID</param>
        public Lane(PathsEnum ID)
        {
            this.laneID = ID.ToString();
            this.pathLaneID = ID;
            this.isPathLane = true;
            this.laneTiles = new List<Tile>();
            this.laneVehicles = new List<Vehicle>();
            this.vehicleQueue = new Queue<Vehicle>();
        }

        /// <summary>
        /// Adds vehicle to this lane object and 
        /// fills it with the lane's information
        /// Vehicle is returned to continue spawn
        /// process
        /// </summary>
        /// <param name="vehicle">Vehicle being added</param>
        /// <returns>Added vehicle containing lane information</returns>
        public Vehicle AddVehicle(Vehicle vehicle)
        {
            vehicle.rotation = this.laneDirection;
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