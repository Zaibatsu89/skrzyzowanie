using System.Collections.Generic;
using System.ServiceModel;
using KruispuntGroep6.Simulator.Objects;
using System.Runtime.CompilerServices;
using System;
using KruispuntGroep6.Simulator.Main;
using Microsoft.Xna.Framework.Graphics;

namespace KruispuntGroep6.Simulator.Globals
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