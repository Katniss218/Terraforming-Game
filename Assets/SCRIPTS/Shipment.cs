﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraformingGame
{
    public class Shipment : MonoBehaviour
    {
        public Inventory inventory;

        public CelestialBody origin;
        public CelestialBody destination;


        private void OnArrival()
        {
            destination.inventory.Add( inventory );

            GameObject.Destroy( this.gameObject );
        }

        void Awake()
        {
            this.inventory = new Inventory();
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Vector3 dir = (destination.transform.position - this.transform.position).normalized;

            this.transform.position += dir * 0.015f * Time.deltaTime * Time.timeScale;

            if( Vector3.Distance( this.transform.position, destination.transform.position ) <= destination.GetRadius() )
            {
                OnArrival();
            }
        }
    }
}