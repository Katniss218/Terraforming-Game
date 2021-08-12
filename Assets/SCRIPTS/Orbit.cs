using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerraformingGame
{
    public class Orbit
    {
        /// <summary>
        /// The semi-major axis of the orbit (m)
        /// </summary>
        public double sma { get; set; }

        /// <summary>
        /// Describes where the body is (0-360)
        /// </summary>
        public double anomaly { get; set; }

        /// <summary>
        /// The body in the center of this orbit.
        /// </summary>
        public CelestialBody parentBody { get; set; }

        public Orbit( double sma, double anomaly, CelestialBody parentBody )
        {
            this.sma = sma;
            this.anomaly = anomaly;
            this.parentBody = parentBody;
        }

        public double GetOrbitalPeriod()
        {
            return 2 * Math.PI * Math.Sqrt( (sma * sma * sma) / (Main.G * parentBody.GetMass()) );
        }

        public Vector3 GetWorldPosition()
        {
            double degrees = this.anomaly;

            double radians = (Math.PI / 180) * degrees;

            double x = Math.Cos( radians );
            double y = Math.Sin( radians );
            x *= this.sma;
            y *= this.sma;

            return new Vector3( (float)x, (float)y, 0 );
        }
    }
}