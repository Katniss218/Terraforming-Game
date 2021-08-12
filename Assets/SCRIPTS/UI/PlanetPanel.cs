using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerraformingGame.UI
{
    public class PlanetPanel : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI planetName;
        public TMPro.TextMeshProUGUI planetRadius;
        public TMPro.TextMeshProUGUI planetMass;
        public TMPro.TextMeshProUGUI planetTemperature;

        public Transform layerContent;

        public Transform inventoryContent;
        private List<InventoryEntry> layerEntries = new List<InventoryEntry>();
        private List<InventoryEntry> inventoryEntries = new List<InventoryEntry>();

        public GameObject inventoryEntryPrefab;

        private static PlanetPanel _instance = null;

        public static PlanetPanel instance
        {
            get
            {
                if( _instance == null )
                {
                    _instance = GameObject.FindObjectOfType<PlanetPanel>();
                }
                return _instance;
            }
        }

        public void Display( CelestialBodyLayer layer )
        {
            for( int i = 0; i < layerContent.childCount; i++ )
            {
                Destroy( layerContent.GetChild( i ).gameObject );
            }

            CelestialBodyResource[] resources = layer.GetResources();

            for( int i = 0; i < resources.Length; i++ )
            {
                GameObject go = Instantiate( inventoryEntryPrefab, layerContent );
                InventoryEntry entry = go.GetComponent<InventoryEntry>();
                layerEntries.Add( entry );

                entry.text.text = resources[i].amount.ToString();
            }
        }

        public void DisplayTemperature()
        {
            CelestialBody body = SelectionManager.GetSelectedBody();
            if( body != null )
            {
                instance.planetTemperature.text = "Temp: " + Math.Round( body.temperature, 1 ).ToString() + " K";
            }
        }

        public void DisplayInventory()
        {
            for( int i = 0; i < inventoryContent.childCount; i++ )
            {
                Destroy( inventoryContent.GetChild( i ).gameObject );
            }

            CelestialBody body = SelectionManager.GetSelectedBody();
            if( body != null )
            {
                for( int i = 0; i < body.inventory.resources.Count; i++ )
                {
                    GameObject go = Instantiate( inventoryEntryPrefab, inventoryContent );
                    InventoryEntry entry = go.GetComponent<InventoryEntry>();
                    inventoryEntries.Add( entry );

                    entry.text.text = body.inventory.resources[i].amount.ToString();
                }
            }
        }
    }
}