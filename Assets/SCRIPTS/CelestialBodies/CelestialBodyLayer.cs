using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraformingGame
{
    public class CelestialBodyLayer
    {
        private List<CelestialBodyResource> resources;

        public int resourceCount { get { return this.resources.Count; } }

        // special resources (type, amount, state), array
        // bulk resource (type, amount, state)

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


        public float AddResource( ResourceType type, float amount )
        {
            // returns amount added
            float amountAdded = amount;
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

        public float RemoveResource( ResourceType type, float amount )
        {
            // returns amount removed
            float amountRemoved = 0;
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

        public float GetDepth( float depthOfAllLayersBelow )
        {
            // returns depth given the depth of all the layers underneath (sphere model).

            return (Mathf.Pow( depthOfAllLayersBelow * depthOfAllLayersBelow + this.GetVolume(), 1.0f / 3.0f ) - depthOfAllLayersBelow) / 2.0f;
        }

        public float GetVolume()
        {
            float acc = 0.0f;
            for( int i = 0; i < this.resources.Count; i++ )
            {
                acc += this.resources[i].amount;
            }
            return acc;
        }

        public float GetMass()
        {
            float acc = 0.0f;
            for( int i = 0; i < this.resources.Count; i++ )
            {
                acc += this.resources[i].amount * this.resources[i].density;
            }
            return acc;
        }
    }
}