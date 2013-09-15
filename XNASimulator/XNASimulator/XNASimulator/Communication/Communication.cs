using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KruispuntGroep6.Simulator.ObjectControllers;

namespace XNASimulator.Communication
{
    class Communication
    {
        private LaneControl laneControl;

        public Communication(LaneControl laneControl)
        {
            this.laneControl = laneControl;
        }

        private void ChangeLight()
        {
            laneControl.ChangeTrafficLight("green", "N3");
        }

        private void SpawnVehicle()
        {
            laneControl.SpawnVehicle("car", "N3", "W6");
        }
    }
}
