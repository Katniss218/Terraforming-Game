using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TerraformingGame.UI
{
    public class SolarSystemPanelEntry : MonoBehaviour
    {
        public CelestialBody body;

        [SerializeField] private RawImage rawImage;
        public Camera renderCam;
        
        public void OnClick()
        {
            SelectionManager.Select( body );
        }

        public void SetRenderTexture( RenderTexture rt )
        {
            this.rawImage.texture = rt;
        }
    }
}