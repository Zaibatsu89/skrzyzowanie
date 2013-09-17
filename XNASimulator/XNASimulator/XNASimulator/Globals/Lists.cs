using System.Collections.Generic;
using KruispuntGroep4.Simulator.Main;
using KruispuntGroep4.Simulator.Objects;

namespace KruispuntGroep4.Simulator.Globals
{
    class Lists
    {
        public List<Vehicle> Vehicles { get; set; }
        public Dictionary<string, Lane> Lanes { get; set; }
        public Tile[,] Tiles { get; set; }

        public Lists()
        {
            Vehicles = new List<Vehicle>(MainGame.TilesHor * MainGame.TilesVer);
            Lanes = new Dictionary<string, Lane>();
            Tiles = new Tile[0, 0];
        }
    }
}