using System;

namespace KruispuntGroep4.Simulator.Globals
{
    /// <summary>
    /// All wind directions used in tile and vehicle rotations
    /// </summary>
    enum RotationEnum
    { 
        North, 
        East, 
        South, 
        West 
    }

    /// <summary>
    /// Gets the Rad value of a wind direction
    /// </summary>
    struct Rotation
    {
        public static float getRotation(RotationEnum rotation)
        {
            double angle = 0;

            switch (rotation)
            {
                case RotationEnum.North:
                    angle = 0;
                    break;
                case RotationEnum.East:
                    angle = 90;
                    break;
                case RotationEnum.South:
                    angle = 180;
                    break;
                case RotationEnum.West:
                    angle = 270;
                    break;
            }

            angle = Math.PI * angle / 180.0;
            return (float)angle;
        }
    }
}