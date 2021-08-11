﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraformingGame
{
    public class SolarSystemPanelCameraController : MonoBehaviour
    {
        public CelestialBody body;

        public new Camera camera;

        void Awake()
        {
            this.camera = this.gameObject.AddComponent<Camera>();

        }

        // Start is called before the first frame update
        void Start()
        {
            this.camera.orthographic = true;
            this.camera.orthographicSize = body.GetRadius() * 1.35f + 10;
            this.camera.clearFlags = CameraClearFlags.SolidColor;
            this.camera.backgroundColor = Color.black;
        }

        // Update is called once per frame
        void Update()
        {
            this.transform.position = new Vector3( body.transform.position.x, body.transform.position.y, body.transform.position.z - body.GetRadius() * 2 );
        }
    }
}