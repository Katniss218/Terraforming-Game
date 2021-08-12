using System.Collections.Generic;
using UnityEngine;
namespace TerraformingGame
{
    public class CelestialBody : MonoBehaviour
    {
        /// <summary>
        /// Returns the parent body this body is orbiting around. Null for the root body.
        /// </summary>
        public CelestialBody parentBody
        {
            get
            {
                return this.orbit?.parentBody;
            }
        }

        /// <summary>
        /// Contains the orbit of this body.
        /// </summary>
        public Orbit orbit { get; private set; }

        public List<CelestialBodyLayer> groundLayers;

        public CelestialBodyLayer atmosphereLayer; // distinct layer due to vast differences in behaviour.

        public Transform graphicsTransform = null;

        public new SphereCollider collider = null;

        public CelestialBodyLayer surfaceLayer
        {
            get
            {
                return this.groundLayers[0];
            }
        }

        public void SetOrbit( CelestialBody parentBody, double sma, double anomaly )
        {
            this.orbit = new Orbit( sma, anomaly, parentBody );
        }

        /// <summary>
        /// Returns the radius of this body (m)
        /// </summary>
        public double GetRadius()
        {
            double acc = 0.0f;
            for( int i = this.groundLayers.Count - 1; i >= 0; i-- )
            {
                acc += this.groundLayers[i].GetDepth( acc );
            }
            return acc;
        }

        /// <summary>
        /// Returns the mass of this body (kg)
        /// </summary>
        public double GetMass()
        {
            double acc = 0.0f;
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

        /// <summary>
        /// Contains the temperature of this body (K)
        /// </summary>
        public float temperature { get; private set; }

        /// <summary>
        /// Sets the temperature (K)
        /// </summary>
        public void SetTemperature( float temperature )
        {
            if( temperature < 0 )
            {
                Debug.LogWarning( "Tried setting temperature to below absolute 0" );
                return;
            }

            this.temperature = temperature;

            MeshRenderer meshRenderer = this.graphicsTransform.gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material.SetColor( "_EmissionColor", TemperatureUtils.GetBlackbodyColor( this.temperature ) );

            Light light = this.GetComponent<Light>();
            if( light != null )
            {
                light.color = TemperatureUtils.GetBlackbodyColor( this.temperature );
            }
        }

        /// <summary>
        /// Returns the force of gravity at a given distance in respect to this body (N)
        /// </summary>
        public double GetGravity( double r )
        {
            return Main.G * this.GetMass() / (r * r);
        }

        // magnetosphere (calculated from the molten metals at above some temperature)

        public void MineResource( InventoryResource resource, int layer )
        {
            double amtMined = this.groundLayers[layer].RemoveResource( resource.type, resource.amount );
            this.inventory.AddResource( resource.type, amtMined );

            // if a ground layer has been completely mined out, remove it. The next layer takes its place and gets exposed.
            if( this.groundLayers[layer].resourceCount == 0 )
            {
                this.groundLayers.RemoveAt( layer );
            }
        }

        public void DepositResource( InventoryResource resource, int layer )
        {
            double amtDeposited = this.groundLayers[layer].AddResource( resource.type, resource.amount );
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
                // orbital period is is seconds per one revolution.
                // angle speed is in degrees per second
                double period = this.orbit.GetOrbitalPeriod();
                double angleSpeed = 1.0 / period * 360.0;

                this.orbit.anomaly += Main.ToDisplayTime( angleSpeed ) * Time.deltaTime * Time.timeScale;

                this.transform.position = Main.ToDisplayPosition( orbit.GetWorldPosition() );
                this.transform.position += this.parentBody.transform.position;
            }
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

        //for a layer to be molten, over 75% of its bulk must be in a liquid state.
    }
}