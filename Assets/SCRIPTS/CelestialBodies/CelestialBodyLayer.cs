using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerraformingGame
{
    public class CelestialBodyLayer
    {
        private List<CelestialBodyResource> resources;

        public int resourceCount { get { return this.resources.Count; } }

        // temperature in kelvins
        private float temperature;
        private float averageDensity;

        // each layer has its own temperature
        // when temperature changes, the resources in the layer can change state
        // for now, don't simulate pressure.

        // layers are ordered by density of the bulk resources

        public CelestialBodyLayer()
        {
            this.resources = new List<CelestialBodyResource>();
        }

        public CelestialBodyResource[] GetResources()
        {
            return this.resources.ToArray();
        }


        public double AddResource( ResourceType type, double amount )
        {
            // returns amount added
            double amountAdded = amount;
            for( int i = 0; i < this.resources.Count; i++ )
            {
                if( this.resources[i].type != type )
                {
                    continue;
                }

                this.resources[i].amount += amountAdded;
                return amountAdded;
            }

            this.resources.Add( new CelestialBodyResource() { type = type, amount = amount } );

            return amountAdded;
        }

        public double RemoveResource( ResourceType type, double amount )
        {
            // returns amount removed
            double amountRemoved = 0;
            for( int i = 0; i < this.resources.Count; i++ )
            {
                if( this.resources[i].type != type )
                {
                    continue;
                }

                // don't remove too much.
                if( this.resources[i].amount < amount )
                {
                    amountRemoved = this.resources[i].amount;
                }
                else
                {
                    amountRemoved = amount;
                }

                this.resources[i].amount -= amountRemoved;
                if( this.resources[i].amount <= 0 )
                {
                    this.resources.RemoveAt( i );
                }
                break;
            }

            return amountRemoved;
        }

        public double GetDepth( double depthOfAllLayersBelow )
        {
            // returns depth given the depth of all the layers underneath (sphere model).

            return (Math.Pow( depthOfAllLayersBelow * depthOfAllLayersBelow + this.GetVolume(), 1.0 / 3.0 ) - depthOfAllLayersBelow) / 2.0; // divide by 2.0 to get radius from diameter
        }

        public double GetVolume()
        {
            double acc = 0.0f;
            for( int i = 0; i < this.resources.Count; i++ )
            {
                acc += this.resources[i].amount;
            }
            return acc;
        }

        public double GetMass()
        {
            double acc = 0.0f;
            for( int i = 0; i < this.resources.Count; i++ )
            {
                acc += this.resources[i].amount * this.resources[i].density;
            }
            return acc;
        }
    }
}