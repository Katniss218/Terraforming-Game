using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraformingGame
{
    /// <summary>
    /// This is an additional component attached to the celesrial body. You can call methods on it to modify how the celestial body is displayed.
    /// </summary>
    public class CelestialBodyGraphics : MonoBehaviour
    {
        public Transform surface;
        public Transform atmosphere;

        public MeshRenderer surfaceRenderer;
        public MeshRenderer atmosphereRenderer;

        public Light starLight;
        public TrailRenderer trailRenderer;

        // needed to properly update the size if only one of the two changes.
        double radius;
        double atmosphereHeight;

        public void SetTemperature( double newTemperature )
        {
            Color color = TemperatureUtils.GetBlackbodyColor( newTemperature );

            this.surfaceRenderer.material.SetColor( "_EmissionColor", color );
            this.atmosphereRenderer.material.SetColor( "_EmissionColor", color );
            
            Light light = this.GetComponent<Light>();
            if( light != null )
            {
                light.color = color;
            }
        }

        public void SetRadius( double newRadius )
        {
            float displayRadius = Main.ToDisplayRadius( newRadius );
            this.radius = newRadius;

            this.surface.localScale = new Vector3( displayRadius * 2, displayRadius * 2, displayRadius * 2 );

            float displayAtmoHeight = Main.ToDisplayRadius( this.atmosphereHeight );
            float scale = (displayRadius + displayAtmoHeight) / displayRadius;

            this.atmosphere.localScale = new Vector3( displayRadius * 2 * scale, displayRadius * 2 * scale, displayRadius * 2 * scale );
        }

        public void SetTrail( float width, Color color, float lifetime )
        {
            this.trailRenderer.material.SetColor( "_Color", color );
            this.trailRenderer.time = lifetime;

            AnimationCurve newWidthCurve = new AnimationCurve( 
                new Keyframe( 0.0f, width ), 
                new Keyframe( 1.0f, 0.0f ) );

            this.trailRenderer.widthCurve = newWidthCurve;
        }

        public void EnableAtmosphere()
        {
            atmosphere.gameObject.SetActive( true );
        }

        public void DisableAtmosphere()
        {
            atmosphere.gameObject.SetActive( false );
        }

        public void SetAtmosphere( double height ) // in meters
        {
            float displayRadius = Main.ToDisplayRadius( this.radius );
            this.atmosphereHeight = height;

            float displayAtmoHeight = Main.ToDisplayRadius( height );
            float scale = 0;
            if( displayRadius > 0 )
            {
                scale = (displayRadius + displayAtmoHeight) / displayRadius;
            }

            this.atmosphere.localScale = new Vector3( displayRadius * 2 * scale, displayRadius * 2 * scale, displayRadius * 2 * scale );
        }

        void Start()
        {

        }

        void Update()
        {

        }
    }
}