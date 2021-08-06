using System.Collections.Generic;
using UnityEngine;

namespace TerraformingGame
{
    public class SetupSolarSystem : MonoBehaviour
    {
        [SerializeField] private Sprite celestialBodySprite = null;
        [SerializeField] private Material trailMaterial = null;

        private CelestialBody SpawnCelestialBody( string name )
        {
            GameObject root = new GameObject( name );

            GameObject gameObject = new GameObject();
            gameObject.transform.SetParent( root.transform );

            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = celestialBodySprite;
            gameObject.transform.localScale = new Vector3( 14, 14 );

            TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
            trail.material = trailMaterial;
            trail.time = 1500;
            trail.receiveShadows = false;

            Keyframe start = new Keyframe( 0.0f, 3.0f );
            Keyframe end = new Keyframe( 1.0f, 0.0f );

            AnimationCurve newWidthCurve = new AnimationCurve( start, end );

            trail.widthCurve = newWidthCurve;

            CelestialBody body = root.AddComponent<CelestialBody>();

            return body;
        }

        private void GenerateSolarSystem()
        {
            CelestialBody sun = SpawnCelestialBody( "Sun" );
            sun.transform.position = new Vector3( 5, 5 );
            sun.DepositResource( new InventoryResource() { type = ResourceType.Water, amount = 1000000f }, 0 );
            float scale = sun.GetRadius();
            sun.transform.localScale = new Vector3( scale, scale );

            Debug.Log( "sun: " + sun.GetRadius() );

            CelestialBody[] planets = new CelestialBody[Random.Range( 3, 9 )];

            float lastPlanetSma = sun.GetRadius();

            for( int i = 0; i < planets.Length; i++ )
            {
                planets[i] = SpawnCelestialBody( "Planet " + i );
                planets[i].parentBody = sun;
                planets[i].SetOrbit( sun, lastPlanetSma, 0 );
                Debug.Log( "planet: " + lastPlanetSma );
                lastPlanetSma += Random.Range( 50.0f, 100.0f );


                float size = Random.Range( 500.0f, 1000.0f );

                if( Random.Range(0, 3) == 0 )
                {
                    size *= Random.Range( 10.0f, 15.0f );
                }
                planets[i].DepositResource( new InventoryResource() { type = ResourceType.Water, amount = size }, 0 );
                scale = planets[i].GetRadius();
                planets[i].transform.localScale = new Vector3( scale, scale );
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            this.GenerateSolarSystem();
        }
    }
}