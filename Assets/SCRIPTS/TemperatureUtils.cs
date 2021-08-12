using UnityEngine;

namespace TerraformingGame
{
    public class TemperatureUtils : MonoBehaviour
    {
        private static Gradient _blackbody = null;
        private static Gradient blackbody
        {
            get
            {
                if( _blackbody == null )
                {
                    _blackbody = GenerateBlackbody();
                }
                return _blackbody;
            }
        }

        private static Gradient GenerateBlackbody()
        {
            // Create the blackbody gradient
            // Max temperature of BLACKBODY_LOOKUP_MAX at time of 1.0f

            GradientColorKey[] keys = new GradientColorKey[8];
            keys[0] = new GradientColorKey( new Color( 1.000f, 0.220f, 0.0f ), 0.033f );
            keys[1] = new GradientColorKey( new Color( 1.000f, 0.494f, 0.0f ), 0.06f );
            keys[2] = new GradientColorKey( new Color( 1.000f, 0.706f, 0.420f ), 0.1f );
            keys[3] = new GradientColorKey( new Color( 1.000f, 0.894f, 0.808f ), 0.167f );
            keys[4] = new GradientColorKey( new Color( 0.914f, 0.929f, 1.0f ), 0.255f );
            keys[5] = new GradientColorKey( new Color( 0.769f, 0.843f, 1.0f ), 0.36f );
            keys[6] = new GradientColorKey( new Color( 0.659f, 0.773f, 1.0f ), 0.671f );
            keys[7] = new GradientColorKey( new Color( 0.624f, 0.749f, 1.0f ), 1.0f );

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[1];
            alphaKeys[0] = new GradientAlphaKey( 1.0f, 0.0f );

            return new Gradient() { colorKeys = keys, alphaKeys = alphaKeys };
        }

        private const double BLACKBODY_LOOKUP_MAX = 29800.0f;

        /// <summary>
        /// Returns a blackbody radiation color given temperature in kelvins
        /// </summary>
        public static Color GetBlackbodyColor( double temperature )
        {
            if( temperature < 798.0 )
            {
                return Color.black;
            }
            return blackbody.Evaluate( Mathf.Clamp01( (float)(temperature / BLACKBODY_LOOKUP_MAX) ) );
        }
    }
}