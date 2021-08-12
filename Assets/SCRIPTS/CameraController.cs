using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraformingGame
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private new Camera camera;

        private Transform followObject = null;

        public bool isFollowing { get { return this.followObject != null; } }

        private static Vector3 OFFSET = new Vector3( 0.0f, 0.0f, -100.0f );


        public void MakeFollow( Transform obj )
        {
            this.followObject = obj;
        }

        public void StopFollowing()
        {
            this.followObject = null;
            this.transform.position = OFFSET;
        }

        public void ZoomIn( float factor = 2.0f )
        {
            camera.orthographicSize /= factor;
        }

        public void ZoomOut( float factor = 2.0f )
        {
            camera.orthographicSize *= factor;
        }


        void Start()
        {
            this.camera = this.GetComponent<Camera>();
        }

        void Update()
        {
            if( Input.mouseScrollDelta.y < 0 )
            {
                ZoomOut( 1.25f );
            }
            else if( Input.mouseScrollDelta.y > 0 )
            {
                ZoomIn( 1.25f );
            }

            if( this.isFollowing )
            {
                this.transform.position = this.followObject.position + OFFSET;
            }
        }
    }
}