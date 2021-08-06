using System.Collections.Generic;
using UnityEngine;
namespace TerraformingGame
{
    public class CelestialBody : MonoBehaviour
    {
        public CelestialBody parentBody;
        public double sma; // in meters
        public double anomaly;

        public List<CelestialBodyLayer> groundLayers;

        public CelestialBodyLayer atmosphereLayer; // distinct layer due to vast differences in behaviour.

        public CelestialBodyLayer surfaceLayer
        {
            get
            {
                return this.groundLayers[0];
            }
        }

        public void SetOrbit( CelestialBody parentBody, double sma, double anomaly )
        {
            this.parentBody = parentBody;
            this.sma = sma;
            this.anomaly = anomaly;
        }

        public float GetRadius()
        {
            float acc = 0.0f;
            for( int i = this.groundLayers.Count - 1; i >= 0; i-- )
            {
                acc += this.groundLayers[i].GetDepth( acc );
            }
            return acc;
        }

        public float GetMass()
        {
            float acc = 0.0f;
            for( int i = 0; i < this.groundLayers.Count; i++ )
            {
                acc += groundLayers[i].GetMass();
            }
            return acc;
        }

        /// <summary>
        /// Contains mined resources
        /// </summary>
        public Inventory inventory;

        // gravity (calculated from mass and radius)
        // radius (calculated from layers)
        // mass (calculated from layers)
        // magnetosphere (calculated from the molten metals at above some temperature)

        public void MineResource( InventoryResource resource, int layer )
        {
            float amtMined = this.groundLayers[layer].RemoveResource( resource.type, resource.amount );
            this.inventory.AddResource( resource.type, amtMined );

            // if a ground layer has been completely mined out, remove it. The next layer takes its place and gets exposed.
            if( this.groundLayers[layer].resourceCount == 0 )
            {
                this.groundLayers.RemoveAt( layer );
            }
        }

        public void DepositResource( InventoryResource resource, int layer )
        {
            Debug.Log( this.groundLayers.Count );
            float amtDeposited = this.groundLayers[layer].AddResource( resource.type, resource.amount );
            this.inventory.RemoveResource( resource.type, amtDeposited );
        }

        void Awake()
        {
            this.inventory = new Inventory();
            this.groundLayers = new List<CelestialBodyLayer>( 1 );
            this.groundLayers.Add( new CelestialBodyLayer() );
        }

        void Start()
        {

        }

        void Update()
        {
            // update temperature.
            // try to freeze/melt layers, etc.
            // update lifeforms.
            // update resources that are marked as being mined.

            if( this.parentBody != null )
            {
                float period = 2 * Mathf.PI * Mathf.Sqrt( (float)(sma * sma * sma * parentBody.GetMass()) );

                float speed = 1 / period * 1000000;

                anomaly += speed * Time.deltaTime * Time.timeScale;

                float x = Mathf.Cos( Mathf.Deg2Rad * (float)anomaly );
                float y = Mathf.Sin( Mathf.Deg2Rad * (float)anomaly );
                x *= (float)sma;
                y *= (float)sma;
                x += parentBody.transform.position.x;
                y += parentBody.transform.position.y;

                this.transform.position = new Vector3( x, y );
            }
            // move the object on screen.
        }


        /// Layer flow rules
        /// 
        // for every layer
        // calculate its average density
        // for every molten resource
        // move x amount of each molten resource.
        // - if the resource's density is lower than the layer, move it a layer up

        // deposits bulk resource in a proper layer, creates a layer if necessary, recalculates planet properties, like radius, mass, etc
        // when a layer's bulk resource

        // Liquid material between adjacent liquid layers try to arrange itself by density, creating and removing layers as necessary.
        // Gas gets transferred directly to atmosphere.
        // the solid stuff stays where it was.
        // liquids can only flow through liquid part of the adjacent layer. The speed of flow depends on how much (%) of the target layer is liquified
        // so if a planet is hot, it can mix and flow, if it's cold it stays.


        // I essentially try to generalize core/mantle/crust.

        // the flow rules exactly the same as for the "1-px-wide texture" method


        // a layer is defined by having a significant concentration of a material or a different state of material
        // when does a layer become its own thing?
        // A layer that's liquid can become its own layer if the resource is at least 10% of the layer.
        // so a layer that contains 15% liquid water and 85% liquid silicates will split into layer of water and layer of silicates
    }
}