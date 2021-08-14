using System;
using System.Collections.Generic;
using TerraformingGame.UI;
using UnityEngine;

namespace TerraformingGame
{
    public class SetupSolarSystem : MonoBehaviour
    {
        public RenderTexture solarSystemRT = null;

        [SerializeField] private Material atmosphereMaterial = null;

        [SerializeField] private Material planetMaterial = null;

        [SerializeField] private Mesh planetMesh = null;

        [SerializeField] private Material planetTrailMaterial = null;

        [SerializeField] private Material shipmentTrailMaterial = null;


        private static SetupSolarSystem _instance = null;

        public static SetupSolarSystem instance
        {
            get
            {
                if( _instance == null )
                {
                    _instance = GameObject.FindObjectOfType<SetupSolarSystem>();
                }
                return _instance;
            }
        }

        private void MakeStar( CelestialBody body )
        {
            Light light = body.gameObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = (float)(body.GetMass() / 12500000000000000000000000000.0);
            light.intensity = (float)(body.GetMass() / 1000000000000000000000000000000.0);
            body.gfx.starLight = light;

            body.isStar = true;
        }

        private static CelestialBody SpawnCelestialBody( string name, Orbit orbit )
        {
            GameObject root = new GameObject( name );

            SphereCollider collider = root.AddComponent<SphereCollider>();
            collider.radius = 1f;
            collider.center = Vector3.zero;

            GameObject gfx = new GameObject( "_GFX" );
            gfx.transform.SetParent( root.transform );


            GameObject gfxSurface = new GameObject( "_srf" );
            gfxSurface.transform.SetParent( gfx.transform );

            MeshFilter meshFilter = gfxSurface.AddComponent<MeshFilter>();
            meshFilter.mesh = SetupSolarSystem.instance.planetMesh;
            MeshRenderer meshRenderer = gfxSurface.AddComponent<MeshRenderer>();
            meshRenderer.material = SetupSolarSystem.instance.planetMaterial;


            GameObject gfxAtmo = new GameObject( "_atm" );
            gfxAtmo.transform.SetParent( gfx.transform );

            meshFilter = gfxAtmo.AddComponent<MeshFilter>();
            meshFilter.mesh = SetupSolarSystem.instance.planetMesh;
            MeshRenderer atmoMeshRenderer = gfxAtmo.AddComponent<MeshRenderer>();
            atmoMeshRenderer.material = SetupSolarSystem.instance.atmosphereMaterial;


            TrailRenderer trail = gfx.AddComponent<TrailRenderer>();
            trail.material = SetupSolarSystem.instance.planetTrailMaterial;
            trail.receiveShadows = false;

            CelestialBodyGraphics graphics = root.AddComponent<CelestialBodyGraphics>();
            CelestialBody body = root.AddComponent<CelestialBody>();

            graphics.surface = gfxSurface.transform;
            graphics.atmosphere = gfxAtmo.transform;
            graphics.surfaceRenderer = meshRenderer;
            graphics.atmosphereRenderer = atmoMeshRenderer;
            graphics.trailRenderer = trail;
            body.collider = collider;
            body.SetOrbit( orbit );
            graphics.SetTrail( 0.1875f, new Color( 1.0f, 1.0f, 0.1f ), 15f );
            graphics.SetAtmosphere( 0 );
            graphics.DisableAtmosphere();

            SolarSystemPanel.instance.AddBody( body );

            return body;
        }

        public static void SpawnShipment( CelestialBody origin, CelestialBody destination )
        {
            GameObject root = new GameObject( "shipment" );

            GameObject gfx = new GameObject( "_GFX" );
            gfx.transform.SetParent( root.transform );
            gfx.transform.localScale = new Vector3( 0.125f, 0.125f, 0.125f );

            MeshFilter meshFilter = gfx.AddComponent<MeshFilter>();
            meshFilter.mesh = SetupSolarSystem.instance.planetMesh;

            MeshRenderer meshRenderer = gfx.AddComponent<MeshRenderer>();
            meshRenderer.material = SetupSolarSystem.instance.planetMaterial;

            SphereCollider collider = root.AddComponent<SphereCollider>();
            collider.radius = 1f;
            collider.center = Vector3.zero;

            TrailRenderer trail = gfx.AddComponent<TrailRenderer>();
            trail.material = SetupSolarSystem.instance.shipmentTrailMaterial;
            trail.time = 20;
            trail.receiveShadows = false;

            Keyframe start = new Keyframe( 0.0f, 0.0625f );
            Keyframe end = new Keyframe( 1.0f, 0.0f );

            AnimationCurve newWidthCurve = new AnimationCurve( start, end );

            trail.widthCurve = newWidthCurve;

            Shipment shipment = root.AddComponent<Shipment>();

            shipment.origin = origin;
            shipment.destination = destination;
            shipment.transform.position = origin.transform.position;

            shipment.inventory.AddResource( ResourceType.Iron, 5 );
        }

        public static void DeleteCelestialBody( CelestialBody body )
        {
            Main.bodies.Remove( body );
            SolarSystemPanel.instance.RemoveBody( body );
            if( SelectionManager.GetSelectedBody() == body )
            {
                SelectionManager.Deselect();
            }
            Destroy( body.gameObject );
        }

        private double RadiusToVolume( double radius )
        {
            return (4.0 / 3.0) * Math.PI * (radius*radius*radius);
        }

        private void GenerateSolarSystem()
        {
            int planetCount = UnityEngine.Random.Range( 3, 10 );


            CelestialBody sun = SpawnCelestialBody( "Sun", null );
            sun.DepositResource( new InventoryResource() { type = ResourceType.Water, amount = Main.MASS_SUN / ResourceType.Water.GetDensity() }, 0 );

            MakeStar( sun );

            //sun.SetTemperature( 5777 );
            sun.SetTemperature( UnityEngine.Random.Range( 3000f, 25000f ) );

            Main.bodies.Add( sun );

            double lastPlanetSma = Main.AU * UnityEngine.Random.Range( 0.2f, 0.9f );

            // planets
            for( int i = 1; i < planetCount; i++ )
            {
                CelestialBody planet = SpawnCelestialBody( "Planet " + i, new Orbit( lastPlanetSma, 0, sun ) );
                planet.SetTemperature( 300 );

                lastPlanetSma *= UnityEngine.Random.Range( 1.2f, 1.85f ) + UnityEngine.Random.Range( 0.0f, 0.2f );


                // randomize mass
                double mass = UnityEngine.Random.Range( 0.1f, 5.0f );
                if( UnityEngine.Random.Range( 0, 3 ) == 0 )
                {
                    mass = UnityEngine.Random.Range( 70f, 300f );
                }
                if( UnityEngine.Random.Range( 0, 6 ) == 0 )
                {
                    mass = UnityEngine.Random.Range( 20f, 90f );

                    if( UnityEngine.Random.Range( 0, 5 ) == 0 )
                    {
                        mass = UnityEngine.Random.Range( 500f, 900f );
                    }
                }
                mass *= Main.MASS_EARTH;

                planet.DepositResource( new InventoryResource() { type = ResourceType.Silicates, amount = mass / ResourceType.Silicates.GetDensity() }, 0 );

                double atmoHeight = 500000;

                planet.gfx.EnableAtmosphere();
                planet.gfx.SetAtmosphere( atmoHeight );

                Main.bodies.Add( planet );
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            this.GenerateSolarSystem();
        }
    }
}