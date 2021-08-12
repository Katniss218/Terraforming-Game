using System;
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

        public double GetAlbedo()
        {
            return 0.15; // TODO - at some point, albedo from surface layer.
        }

        /// <summary>
        /// Returns the force of gravity at a given distance in respect to this body (N)
        /// </summary>
        public double GetGravity( double r )
        {
            return Main.G * this.GetMass() / (r * r);
        }

        public double GetSurfaceArea()
        {
            double radius = this.GetRadius();
            return 4.0 * Math.PI * radius * radius;
        }

        public double GetSolarConstant()
        {
            double temperature = this.orbit.parentBody.temperature;
            double Ks = Main.SIGMA * (temperature * temperature * temperature * temperature);
            double radioSq = (4.0 * Math.PI * this.orbit.parentBody.GetRadius()) / (4.0 * Math.PI * this.orbit.sma);
            radioSq *= radioSq;
            Ks = Ks * radioSq;

            return Ks;
        }

        public double GetEnergyIntercepted()
        {
            double Ks = this.GetSolarConstant();
            double radius = this.GetRadius();
            return Ks * Math.PI * (radius * radius); 
            // return [W] = Ks [W/m^2] * PI * r [m] ^2
        }

        public double GetEnergyAbsorbed( double energyIntercepted )
        {
            return energyIntercepted * (1 - this.GetAlbedo());
        }

        public double GetEnergyEmitted() // also known as luminosity.
        {
            return Main.SIGMA * (this.temperature * this.temperature * this.temperature * this.temperature) * this.GetSurfaceArea();
        }

        /// <summary>
        /// Contains mined resources
        /// </summary>
        public Inventory inventory;

        public bool isStar = false;

        /// <summary>
        /// Contains the temperature of this body (K)
        /// </summary>
        public double temperature { get; private set; }

        public Action onTemperatureChanged { get; set; }

        /// <summary>
        /// Sets the temperature (K)
        /// </summary>
        public void SetTemperature( double temperature )
        {
            if( temperature < 0 )
            {
                Debug.LogWarning( "Tried setting temperature to below absolute 0" );
                return;
            }

            this.temperature = temperature;
            this.onTemperatureChanged?.Invoke();

            for( int i = 0; i < this.graphicsTransform.childCount; i++ )
            {
                Transform child = this.graphicsTransform.GetChild( i );

                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                meshRenderer.material.SetColor( "_EmissionColor", TemperatureUtils.GetBlackbodyColor( this.temperature ) );
            }

            Light light = this.GetComponent<Light>();
            if( light != null )
            {
                light.color = TemperatureUtils.GetBlackbodyColor( this.temperature );
            }
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
            // try to freeze/melt layers, etc.
            // update lifeforms.
            // update resources that are marked as being mined.

            double energyIntercepted = 0.0;

            if( this.parentBody != null )
            {
                // orbital period is is seconds per one revolution.
                // angle speed is in degrees per second
                double period = this.orbit.GetOrbitalPeriod();
                double angleSpeed = 1.0 / period * 360.0;

                this.orbit.anomaly += angleSpeed * Main.ToDisplayTime( Time.deltaTime ) * Time.timeScale;

                this.transform.position = Main.ToDisplayPosition( orbit.GetWorldPosition() );
                this.transform.position += this.parentBody.transform.position;

                // Heating, use the sun's heat energy.
                energyIntercepted = this.GetEnergyIntercepted();
            }

            if( !this.isStar )
            {
                // Calculate heating
                double absorbed = this.GetEnergyAbsorbed( energyIntercepted );
                double emitted = this.GetEnergyEmitted();
                double deltaEnergy = absorbed - emitted; // Joules per second.

                deltaEnergy *= Main.ToDisplayTime( Time.timeScale ) * Time.deltaTime;
                // heat capacity [Joule/Kelvin] = mass * specific heat * change in temperature.
                // heat capacity 1Kelvin [Joule] = mass * specific heat
                double heatCapacity = this.GetMass() * ResourceType.Iron.GetSpecificHeat(); // Joules, this is the amount of heat needed to raise temperature by 1 kelvin.

                double deltaKelvins = 0;
                if( heatCapacity > 1 ) // prevent division by 0, if heat capacity is 0.
                {
                    // deltaEnergy is in Joules per second, heat capacity is in joules
                    deltaKelvins = deltaEnergy / heatCapacity; // calculate delta kelvins from the proportion.
                }


                this.SetTemperature( temperature + deltaKelvins );
            }
        }


        // if the temperature is too high, you can move the planet. How?

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