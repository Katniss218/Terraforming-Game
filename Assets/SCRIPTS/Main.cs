using System.Collections.Generic;
using UnityEngine;

namespace TerraformingGame
{
    public class Main : MonoBehaviour
    {
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

        // Update is called once per frame
        void Update()
        {
            if( Input.GetKeyDown( KeyCode.Comma ) )
            {
                Time.timeScale /= 2;
            }
            if( Input.GetKeyDown( KeyCode.Period ) )
            {
                Time.timeScale *= 2;
            }

            if( Input.GetKeyDown( KeyCode.KeypadPeriod ) )
            {
                Time.timeScale = 1;
            }
        }
    }
}