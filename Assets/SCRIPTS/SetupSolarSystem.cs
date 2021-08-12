using System;
using System.Collections.Generic;
using TerraformingGame.UI;
using UnityEngine;

namespace TerraformingGame
{
    public class SetupSolarSystem : MonoBehaviour
    {
        [SerializeField] private RenderTexture solarSystemRT = null;

        [SerializeField] private Material planetMaterial = null;

        [SerializeField] private Material starMaterial = null;

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
            light.range = (float)(body.GetMass() / 25000000000000000000000000000.0);
            light.intensity = (float)(body.GetMass() / 500000000000000000000000000000.0);

            MeshRenderer meshRenderer = body.graphicsTransform.gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = starMaterial;
        }

        private static CelestialBody SpawnCelestialBody( string name )
        {
            GameObject root = new GameObject( name );

            GameObject gfx = new GameObject( "_GFX" );
            gfx.transform.SetParent( root.transform );

            MeshFilter meshFilter = gfx.AddComponent<MeshFilter>();
            meshFilter.mesh = SetupSolarSystem.instance.planetMesh;

            MeshRenderer meshRenderer = gfx.AddComponent<MeshRenderer>();
            meshRenderer.material = SetupSolarSystem.instance.planetMaterial;

            SphereCollider collider = root.AddComponent<SphereCollider>();
            collider.radius = 1f;
            collider.center = Vector3.zero;

            TrailRenderer trail = gfx.AddComponent<TrailRenderer>();
            trail.material = SetupSolarSystem.instance.planetTrailMaterial;
            trail.time = 15;
            trail.receiveShadows = false;

            Keyframe start = new Keyframe( 0.0f, 0.5f );
            Keyframe end = new Keyframe( 1.0f, 0.0f );

            AnimationCurve newWidthCurve = new AnimationCurve( start, end );

            trail.widthCurve = newWidthCurve;

            CelestialBody body = root.AddComponent<CelestialBody>();

            body.graphicsTransform = gfx.transform;
            body.collider = collider;

            return body;
        }

        public static void SpawnShipment( CelestialBody origin, CelestialBody destination )
        {
            GameObject root = new GameObject( "shipment" );

            GameObject gfx = new GameObject( "_GFX" );
            gfx.transform.SetParent( root.transform );

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

            Keyframe start = new Keyframe( 0.0f, 0.25f );
            Keyframe end = new Keyframe( 1.0f, 0.0f );

            AnimationCurve newWidthCurve = new AnimationCurve( start, end );

            trail.widthCurve = newWidthCurve;

            Shipment shipment = root.AddComponent<Shipment>();

            //shipment.graphicsTransform = gfx.transform;
            //shipment.collider = collider;
            shipment.origin = origin;
            shipment.destination = destination;
            shipment.transform.position = origin.transform.position;

            shipment.inventory.AddResource( ResourceType.Iron, 5 );
        }

        private double RadiusToVolume( double radius )
        {
            return (4.0 / 3.0) * Math.PI * (radius*radius*radius);
        }

        private void GenerateSolarSystem()
        {
            Main.bodies = new CelestialBody[UnityEngine.Random.Range( 4, 9 ) + 1];
           // Main.bodies = new CelestialBody[2];
            RenderTexture[] texs = new RenderTexture[Main.bodies.Length];


            CelestialBody sun = SpawnCelestialBody( "Sun" );

            sun.SetTemperature( 5000 );
           // sun.DepositResource( new InventoryResource() { type = ResourceType.Water, amount = 1000000.0 }, 0 );
            sun.DepositResource( new InventoryResource() { type = ResourceType.Water, amount = Main.MASS_SUN / ResourceType.Water.GetDensity() }, 0 );

            float radius = Main.ToDisplayRadius( sun.GetRadius() );
            sun.graphicsTransform.localScale = new Vector3( radius * 2, radius * 2, radius * 2 );
            sun.collider.radius = radius + 0.5f;

            MakeStar( sun );

            Main.bodies[0] = sun;

            double lastPlanetSma = Main.AU * UnityEngine.Random.Range( 0.3f, 0.7f );

            // planets
            for( int i = 1; i < Main.bodies.Length; i++ )
            {
                Main.bodies[i] = SpawnCelestialBody( "Planet " + i );
                Main.bodies[i].SetOrbit( sun, lastPlanetSma, 0 );
                Debug.Log( "planet: " + lastPlanetSma );
                lastPlanetSma *= UnityEngine.Random.Range( 1.2f, 1.85f );


                double size = UnityEngine.Random.Range( 0.1f, 5.0f );

                if( UnityEngine.Random.Range( 0, 3 ) == 0 )
                {
                    size = UnityEngine.Random.Range( 70f, 300f );
                }
                size *= Main.MASS_EARTH;

                Main.bodies[i].DepositResource( new InventoryResource() { type = ResourceType.Water, amount = size / ResourceType.Water.GetDensity() }, 0 );

                radius = Main.ToDisplayRadius( Main.bodies[i].GetRadius() );
                Main.bodies[i].graphicsTransform.localScale = new Vector3( radius * 2, radius * 2, radius * 2 );
                Main.bodies[i].collider.radius = radius + 0.5f;
            }
            /*Main.bodies[1] = SpawnCelestialBody( "Earth" );
            Main.bodies[1].SetOrbit( sun, 149597870700.0, 0 );
            Debug.Log( "Earth" );
            Main.bodies[1].DepositResource( new InventoryResource() { type = ResourceType.Water, amount = 5972000000000000000000000.0 / ResourceType.Water.GetDensity() }, 0 );

            radius = Main.ToDisplayRadius( Main.bodies[1].GetRadius() );
            Main.bodies[1].graphicsTransform.localScale = new Vector3( radius * 2, radius * 2, radius * 2 );
            Main.bodies[1].collider.radius = radius + 0.5f;*/

            for( int i = 0; i < Main.bodies.Length; i++ )
            {
                texs[i] = new RenderTexture( solarSystemRT );
                SolarSystemPanel.instance.AddBody( Main.bodies[i], texs[i] );
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            this.GenerateSolarSystem();
        }
    }
}