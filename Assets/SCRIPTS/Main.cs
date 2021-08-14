using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TerraformingGame
{
    public class Main : MonoBehaviour
    {
        private static bool isPaused = false;
        private static float pausedTimeScale;

        public static List<CelestialBody> bodies = new List<CelestialBody>();


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
        
        private static CameraController ___cameraController = null;
        public static CameraController cameraController
        {
            get
            {
                if( ___cameraController == null )
                {
                    ___cameraController = FindObjectOfType<CameraController>();
                }
                return ___cameraController;
            }
        }


        public const double G = 0.00000000006674; // valid when the calculations are done in kilograms and meters.
        public const double SIGMA = 0.00000005670374419; // stphan boltzmann constant

        private const double REAL_TO_WORLD_POSITION_FACTOR = (1.0 / AU) * 10.0; // 10 units = 1 AU
        private const double REAL_TO_WORLD_RADIUS_FACTOR = (1.0 / RADIUS_SUN); // 1 unit = 1 solar radius
        private const double REAL_TO_WORLD_TIME_FACTOR = MONTH_TO_SECONDS; // 1 real second = 1 in-game month

        public const double MASS_SUN = 1898000000000000000000000000000.0;
        public const double MASS_JUPITER = 1898000000000000000000000000.0;
        public const double MASS_EARTH = 5972000000000000000000000.0;

        public const double RADIUS_SUN = 696340000.0;
        public const double RADIUS_JUPITER = 69911000.0;
        public const double RADIUS_EARTH = 6371000.0;

        public const double AU = 149597870700.0;

        public const double HOUR_TO_SECONDS = 3600.0;
        public const double DAY_TO_SECONDS = HOUR_TO_SECONDS * 24;
        public const double MONTH_TO_SECONDS = DAY_TO_SECONDS * 30;
        public const double YEAR_TO_SECONDS = DAY_TO_SECONDS * 365;

        // "gameplay" time is how many seconds pass in game per single real life second on default timewarp.
        // radius and position converts from real values to display in viewport because the game doesn't use the same scale for positions and sizes.

        /// <summary>
        /// Converts real orbital distance into gameplay orbital distance.
        /// </summary>
        public static Vector3 ToDisplayPosition( Vector3 realPosition )
        {
            return realPosition * (float)REAL_TO_WORLD_POSITION_FACTOR;
        }

        /// <summary>
        /// Converts real celestial body radius into gameplay celestial body radius.
        /// </summary>
        public static float ToDisplayRadius( double realRadius )
        {
            if( realRadius < RADIUS_JUPITER * 5 )
            {
                return (float)(realRadius * REAL_TO_WORLD_RADIUS_FACTOR * 8);
            }
            return (float)(realRadius * REAL_TO_WORLD_RADIUS_FACTOR);
        }

        /// <summary>
        /// Converts IRL timescale into real game timescale.
        /// </summary>
        public static double IRLToGameTime( double realSeconds )
        {
            return realSeconds * REAL_TO_WORLD_TIME_FACTOR;
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

        void Start()
        {
            Time.timeScale = 1;
        }

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
                if( Time.timeScale > 0.25f )
                {
                    Time.timeScale /= 2;
                }
            }
            if( Input.GetKeyDown( KeyCode.Period ) )
            {
                if( Time.timeScale < 8 )
                {
                    Time.timeScale *= 2;
                }
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
                SetupSolarSystem.SpawnShipment( bodies[1], SelectionManager.GetSelectedBody() );
            }
            if( Input.GetKeyDown( KeyCode.F6 ) )
            {
                SelectionManager.GetSelectedBody().DepositResource( new InventoryResource() { amount = 5000000000000000000000.0, type = ResourceType.Water }, 0 );
            }
            if( Input.GetKeyDown( KeyCode.F7 ) )
            {
                SelectionManager.GetSelectedBody().MineResource( new InventoryResource() { amount = 5000000000000000000000.0, type = ResourceType.Water }, 0 );
            }
        }
    }
}