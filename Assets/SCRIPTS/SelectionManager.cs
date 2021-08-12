using System.Collections.Generic;
using TerraformingGame.UI;
using UnityEngine;

namespace TerraformingGame
{
    public class SelectionManager
    {
        private static CelestialBody selectedBody = null;

        public static CelestialBody GetSelectedBody()
        {
            return selectedBody;
        }

        public static void Select( CelestialBody body )
        {
            selectedBody = body;
            PlanetPanel.instance.planetName.text = body.gameObject.name;
            PlanetPanel.instance.planetMass.text = (body.GetMass() / Main.MASS_EARTH).ToString();
            PlanetPanel.instance.Display( body.groundLayers[0] );
            PlanetPanel.instance.DisplayInventory();

            // Hook up the events to update UI when something about the selected object changes
            selectedBody.inventory.onInventoryChanged += PlanetPanel.instance.DisplayInventory;
        }

        public static void Deselect()
        {
            if( selectedBody == null )
            {
                return;
            }
            // Unhook the events
            selectedBody.inventory.onInventoryChanged -= PlanetPanel.instance.DisplayInventory;

            selectedBody = null;
            PlanetPanel.instance.planetName.text = "";
            PlanetPanel.instance.planetMass.text = "";
            PlanetPanel.instance.DisplayInventory();
        }
    }
}