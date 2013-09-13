using System;

namespace KruispuntGroep6.Simulator.Globals
{
    enum RotationEnum
    { 
        North, 
        East, 
        South, 
        West 
    }

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