using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraformingGame
{
    public class Inventory
    {
        // contains the resources that were mined but are still on a planet, awaiting shipment.

        public List<InventoryResource> resources;

        public Inventory()
        {
            this.resources = new List<InventoryResource>();
        }

        public void AddResource( ResourceType type, float amount )
        {
            // try to increase existing resources
            for( int i = 0; i < resources.Count; i++ )
            {
                if( resources[i].type != type )
                {
                    continue;
                }

                resources[i].amount += amount;
                return;
            }

            // if no existing resource found, add a new entry.
            resources.Add( new InventoryResource() { type = type, amount = amount } );
        }

        public void RemoveResource( ResourceType type, float amount )
        {
            for( int i = 0; i < resources.Count; i++ )
            {
                if( resources[i].type != type )
                {
                    continue;
                }

                resources[i].amount -= amount;

                if( resources[i].amount <= 0 )
                {
                    resources.RemoveAt( i );
                }
                return;
            }
        }
    }
}