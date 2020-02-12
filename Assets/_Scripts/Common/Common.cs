using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public static class Common
    {
        public enum Altitude
        {
            HIGH, MEDIUM, LOW, GROUNDED
        }

        public static Dictionary<Altitude, Vector3> scaleMap = new Dictionary<Altitude, Vector3>()
        {
            { Altitude.GROUNDED, new Vector3(.35f, .35f, .35f)  },
            { Altitude.LOW, new Vector3(.7f, .7f, .7f)  },
            { Altitude.MEDIUM, new Vector3(.85f, .85f, .85f)  },
            { Altitude.HIGH, new Vector3(1f, 1f, 1f) }
        };

        public static Dictionary<Airplane.ModelID, string> airplaneModelMap = new Dictionary<Airplane.ModelID, string>()
        {
            {
                Airplane.ModelID.RAF_SUPERMARINE_SPITFIRE, "Supermarine Spitfire"
            }
        };

        public static Dictionary<Altitude, Vector3> shadowOffsetsMap = new Dictionary<Altitude, Vector3>()
        {
            { Altitude.GROUNDED, new Vector3(.1f, 0f, -.05f) },
            { Altitude.LOW, new Vector3(.5f, 0f, -.3f) },
            { Altitude.MEDIUM, new Vector3(1.9f, 0f, -1.8f) },
            { Altitude.HIGH, new Vector3(5.5f, 0f, -6.7f) }
        };

        public static Dictionary<Altitude, string> layerMasksMap = new Dictionary<Altitude, string>()
        {
            { Altitude.GROUNDED, "Ground" },
            { Altitude.LOW, "SkyLow" },
            { Altitude.MEDIUM, "SkyMedium" },
            { Altitude.HIGH, "SkyHigh" }
        };
    }
}

