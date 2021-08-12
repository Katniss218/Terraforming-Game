using System;
using System.Collections.Generic;
using TerraformingGame.UI;
using UnityEngine;

namespace TerraformingGame
{
    public class SetupSolarSystem : MonoBehaviour
    {
        [SerializeField] private RenderTexture solarSystemRT = null;

        [SerializeField] private Material atmosphereMaterial = null;

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
            light.range = (float)(body.GetMass() / 12500000000000000000000000000.0);
            light.intensity = (float)(body.GetMass() / 500000000000000000000000000000.0);

            body.isStar = true;

            for( int i = 0; i < body.graphicsTransform.childCount; i++ )
            {
                Transform child = body.graphicsTransform.GetChild( i );

                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                meshRenderer.material = starMaterial;
            }
        }

        private static CelestialBody SpawnCelestialBody( string name )
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


            GameObject gfxAtmo = new GameObject( "_srf" );
            gfxAtmo.transform.SetParent( gfx.transform );
            gfxAtmo.transform.localScale = new Vector3( 1.25f, 1.25f, 1.25f );

            meshFilter = gfxAtmo.AddComponent<MeshFilter>();
            meshFilter.mesh = SetupSolarSystem.instance.planetMesh;

            meshRenderer = gfxAtmo.AddComponent<MeshRenderer>();
            meshRenderer.material = SetupSolarSystem.instance.atmosphereMaterial;

            TrailRenderer trail = gfx.AddComponent<TrailRenderer>();
            trail.material = SetupSolarSystem.instance.planetTrailMaterial;
            trail.time = 15;
            trail.receiveShadows = false;

            Keyframe start = new Keyframe( 0.0f, 0.1875f );
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
            gfx.transform.localScale = new Vector3( 0.25f, 0.25f, 0.25f );

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

            Keyframe start = new Keyframe( 0.0f, 0.125f );
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
            Main.bodies = new CelestialBody[UnityEngine.Random.Range( 3, 10 ) + 1];
            RenderTexture[] texs = new RenderTexture[Main.bodies.Length];


            CelestialBody sun = SpawnCelestialBody( "Sun" );

            //sun.SetTemperature( 5777 );
            sun.SetTemperature( UnityEngine.Random.Range( 3000f, 25000f ) );
            sun.DepositResource( new InventoryResource() { type = ResourceType.Water, amount = Main.MASS_SUN / ResourceType.Water.GetDensity() }, 0 );

            float radius = Main.ToDisplayRadius( sun.GetRadius() );
            sun.graphicsTransform.localScale = new Vector3( radius * 2, radius * 2, radius * 2 );
            sun.collider.radius = radius + 0.5f;

            MakeStar( sun );

            Main.bodies[0] = sun;

            double lastPlanetSma = Main.AU * UnityEngine.Random.Range( 0.2f, 0.9f );

            // planets
            for( int i = 1; i < Main.bodies.Length; i++ )
            {
                Main.bodies[i] = SpawnCelestialBody( "Planet " + i );
                Main.bodies[i].SetOrbit( sun, lastPlanetSma, 0 );
                Debug.Log( "planet: " + lastPlanetSma );
                lastPlanetSma *= UnityEngine.Random.Range( 1.2f, 1.85f ) + UnityEngine.Random.Range( 0.0f, 0.2f );


                double size = UnityEngine.Random.Range( 0.1f, 5.0f );
                Main.bodies[i].SetTemperature( 300 );

                if( UnityEngine.Random.Range( 0, 3 ) == 0 )
                {
                    size = UnityEngine.Random.Range( 70f, 300f );
                }
                if( UnityEngine.Random.Range( 0, 6 ) == 0 )
                {
                    size = UnityEngine.Random.Range( 20f, 90f );

                    if( UnityEngine.Random.Range( 0, 5 ) == 0 )
                    {
                        size = UnityEngine.Random.Range( 500f, 900f );
                    }
                }
                size *= Main.MASS_EARTH;

                Main.bodies[i].DepositResource( new InventoryResource() { type = ResourceType.Water, amount = size / ResourceType.Water.GetDensity() }, 0 );

                radius = Main.ToDisplayRadius( Main.bodies[i].GetRadius() );
                Main.bodies[i].graphicsTransform.localScale = new Vector3( radius * 2, radius * 2, radius * 2 );
                Main.bodies[i].collider.radius = radius + 0.5f;
            }

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