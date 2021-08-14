using System.Collections.Generic;
using UnityEngine;

namespace TerraformingGame.UI
{
    public class SolarSystemPanel : MonoBehaviour
    {
        public Transform content;
        public GameObject entryPrefab;

        private static SolarSystemPanel _instance = null;
        public static SolarSystemPanel instance
        {
            get
            {
                if( _instance == null )
                {
                    _instance = GameObject.FindObjectOfType<SolarSystemPanel>();
                }
                return _instance;
            }
        }

        public List<SolarSystemPanelEntry> entries = new List<SolarSystemPanelEntry>();



        public void AddBody( CelestialBody body )
        {
            RenderTexture rt = new RenderTexture( SetupSolarSystem.instance.solarSystemRT );

            GameObject gameObject = GameObject.Instantiate( entryPrefab, content );

            SolarSystemPanelEntry entry = gameObject.GetComponent<SolarSystemPanelEntry>();
            entry.body = body;

            GameObject camObj = new GameObject( "CAM" );

            SolarSystemPanelCameraController sspcc = camObj.AddComponent<SolarSystemPanelCameraController>();

            sspcc.camera.targetTexture = rt;
            sspcc.body = body;

            entry.SetRenderTexture( rt );

            entry.renderCam = sspcc.GetComponent<Camera>();

            entries.Add( entry );
        }

        public void RemoveBody( CelestialBody body )
        {
            for( int i = 0; i < this.entries.Count; i++ )
            {
                if( this.entries[i].body == body )
                {
                    Destroy( this.entries[i].renderCam.gameObject );
                    Destroy( this.entries[i].gameObject );

                    this.entries.RemoveAt( i );
                    return;
                }
            }
        }
    }
}