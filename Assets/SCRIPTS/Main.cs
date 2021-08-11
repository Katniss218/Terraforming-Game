using System.Collections.Generic;
using TerraformingGame.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TerraformingGame
{
    public class Main : MonoBehaviour
    {
        private static bool isPaused = false;
        private static float pausedTimeScale;

        public static CelestialBody[] bodies;


        [SerializeField] private Gradient ___blackbody;
        private static Gradient _blackbody;
        public static Gradient blackbody
        {
            get
            {
                if( _blackbody == null )
                {
                    _blackbody = FindObjectOfType<Main>().___blackbody;
                }
                return _blackbody;
            }
        }

        private const float BLACKBODY_LOOKUP_MAX = 29800.0f;

        /// <summary>
        /// Returns a blackbody radiation color given temperature in kelvins
        /// </summary>
        public static Color GetBlackbody( float temp )
        {
            return blackbody.Evaluate( Mathf.Clamp01( temp / BLACKBODY_LOOKUP_MAX ) );
        }

        [SerializeField] private RectTransform _UILayerPanel = null;
        private static RectTransform ___UILayerPanel = null;
        public static RectTransform UILayerPanel
        {
            get
            {
                if( ___UILayerPanel == null )
                {
                    ___UILayerPanel = FindObjectOfType<Main>()._UILayerPanel;
                }
                return ___UILayerPanel;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            Time.timeScale = 1;
        }

        private static void Pause()
        {
            pausedTimeScale = Time.timeScale;
            Time.timeScale = 0;
            isPaused = true;
        }

        private static void Unpause()
        {
            Time.timeScale = pausedTimeScale;
            pausedTimeScale = 0;
            isPaused = false;
        }

        // Update is called once per frame
        void Update()
        {
            if( Input.GetMouseButtonDown( 0 ) )
            {
                if( !EventSystem.current.IsPointerOverGameObject() )
                {
                    RaycastHit hit;
                    if( Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out hit ) )
                    {
                        CelestialBody hitBody = hit.collider.GetComponent<CelestialBody>();

                        if( hitBody != null )
                        {
                            SelectionManager.Select( hitBody );
                        }
                    }
                    else
                    {
                        SelectionManager.Deselect();
                    }
                }
            }
            if( Input.GetKeyDown( KeyCode.Comma ) )
            {
                Time.timeScale /= 2;
            }
            if( Input.GetKeyDown( KeyCode.Period ) )
            {
                Time.timeScale *= 2;
            }

            if( Input.GetKeyDown( KeyCode.Minus ) )
            {
                Camera.main.orthographicSize *= 2;
            }
            if( Input.GetKeyDown( KeyCode.Equals ) )
            {
                Camera.main.orthographicSize /= 2;
            }

            if( Input.GetKey( KeyCode.Alpha1 ) )
            {
                bodies[0].SetTemperature( bodies[0].temperature - 20 );
            }
            if( Input.GetKey( KeyCode.Alpha2 ) )
            {
                bodies[0].SetTemperature( bodies[0].temperature + 20 );
            }

            if( Input.GetKeyDown( KeyCode.KeypadPeriod ) )
            {
                Time.timeScale = 1;
            }
            if( Input.GetKeyDown( KeyCode.Space ) )
            {
                if( isPaused )
                {
                    Unpause();
                }
                else
                {
                    Pause();
                }
            }

            if( Input.GetKeyDown( KeyCode.F5 ) )
            {
                SetupSolarSystem.SpawnShipment( bodies[2], bodies[3] );
            }
        }
    }
}