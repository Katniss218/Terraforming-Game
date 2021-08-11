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
            light.range = body.GetMass() / 500000f;
            light.intensity = body.GetMass() / 500000000f;

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
            trail.time = 1500;
            trail.receiveShadows = false;

            Keyframe start = new Keyframe( 0.0f, 3.0f );
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
            trail.time = 500;
            trail.receiveShadows = false;

            Keyframe start = new Keyframe( 0.0f, 3.0f );
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

        private void GenerateSolarSystem()
        {
            Main.bodies = new CelestialBody[Random.Range( 4, 9 ) + 1];
            RenderTexture[] texs = new RenderTexture[Main.bodies.Length];


            CelestialBody sun = SpawnCelestialBody( "Sun" );


            sun.transform.position = new Vector3( 5, 5 );
            sun.SetTemperature( 3000 );
            sun.DepositResource( new InventoryResource() { type = ResourceType.Water, amount = 1000000f }, 0 );
            float scale = sun.GetRadius();
            sun.graphicsTransform.localScale = new Vector3( scale * 2, scale * 2, scale * 2 );
            sun.collider.radius = scale + 4;

            MakeStar( sun );

            Debug.Log( "sun: " + sun.GetRadius() );


            Main.bodies[0] = sun;

            float lastPlanetSma = sun.GetRadius() * Random.Range( 1.5f, 2.5f );

            // planets
            for( int i = 1; i < Main.bodies.Length; i++ )
            {
                Main.bodies[i] = SpawnCelestialBody( "Planet " + i );
                Main.bodies[i].parentBody = sun;
                Main.bodies[i].SetOrbit( sun, lastPlanetSma, 0 );
                Debug.Log( "planet: " + lastPlanetSma );
                lastPlanetSma += Random.Range( 50.0f, 100.0f );


                float size = Random.Range( 500.0f, 1000.0f );

                if( Random.Range( 0, 3 ) == 0 )
                {
                    size *= Random.Range( 10.0f, 15.0f );
                }
                Main.bodies[i].DepositResource( new InventoryResource() { type = ResourceType.Water, amount = size }, 0 );
                scale = Main.bodies[i].GetRadius();
                Main.bodies[i].graphicsTransform.localScale = new Vector3( scale * 2, scale * 2, scale * 2 );
                Main.bodies[i].collider.radius = scale + 4;
                Main.bodies[i].inventory.AddResource( ResourceType.Iron, Random.Range( 5, 15 ) );

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