using System;
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
            PlanetPanel.instance.planetRadius.text = "Radius: " + Math.Round( body.GetRadius() / Main.RADIUS_EARTH, 2 ).ToString() + " R⊕";
            PlanetPanel.instance.planetMass.text = "Mass: " + Math.Round(body.GetMass() / Main.MASS_EARTH, 2).ToString() + " M⊕";
            PlanetPanel.instance.Display( body.groundLayers[0] );
            PlanetPanel.instance.DisplayInventory();
            PlanetPanel.instance.DisplayTemperature();
            Main.cameraController.MakeFollow( body.transform );

            // Hook up the events to update UI when something about the selected object changes
            selectedBody.inventory.onInventoryChanged += PlanetPanel.instance.DisplayInventory;
            selectedBody.onTemperatureChanged += PlanetPanel.instance.DisplayTemperature;
        }

        public static void Deselect()
        {
            if( selectedBody == null )
            {
                return;
            }
            // Unhook the events
            selectedBody.inventory.onInventoryChanged -= PlanetPanel.instance.DisplayInventory;
            selectedBody.onTemperatureChanged -= PlanetPanel.instance.DisplayTemperature;

            selectedBody = null;
            PlanetPanel.instance.planetName.text = "";
            PlanetPanel.instance.planetRadius.text = "";
            PlanetPanel.instance.planetMass.text = "";
            PlanetPanel.instance.planetTemperature.text = "";
            PlanetPanel.instance.DisplayInventory();
            Main.cameraController.StopFollowing();
        }
    }
}