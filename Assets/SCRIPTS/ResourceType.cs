
namespace TerraformingGame
{
    public enum ResourceType
    {
        Hydrogen,
        Nitrogen,
        Oxygen,
        CarbonDioxide,
        Water,
        Silicates,
        Iron
    }

    public static class ResourceData
    {
        public static string GetDisplayName( this ResourceType type )
        {
            switch( type )
            {
                case ResourceType.Hydrogen:
                    return "H2";
                case ResourceType.Nitrogen:
                    return "N2";
                case ResourceType.Oxygen:
                    return "O2";
                case ResourceType.CarbonDioxide:
                    return "CO2";
                case ResourceType.Water:
                    return "Water";
                case ResourceType.Silicates:
                    return "Silicates";
                case ResourceType.Iron:
                    return "Iron";
            }
            throw new System.Exception( "Type was not specified." );
        }

        // temperature in kelvins
        public static float GetMeltingPoint( this ResourceType type )
        {
            switch( type )
            {
                case ResourceType.Water:
                    return 273;
                case ResourceType.Silicates:
                    return 1373;
                case ResourceType.Iron:
                    return 1811;
            }
            throw new System.Exception( "Type was not specified." );
        }

        // temperature in kelvins
        public static float GetBoilingPoint( this ResourceType type )
        {
            switch( type )
            {
                case ResourceType.Water:
                    return 373;
            }
            throw new System.Exception( "Type was not specified." );
        }

        // density in kg per m^3
        public static float GetDensity( this ResourceType type )
        {
            switch( type )
            {
                case ResourceType.Water:
                    return 1000;
                case ResourceType.Silicates:
                    return 2650;
                case ResourceType.Iron:
                    return 7900;
            }
            throw new System.Exception( "Type was not specified." );
        }

        // specific heat in joules/kg*Kelvin
        public static float GetSpecificHeat( this ResourceType type )
        {
            switch( type )
            {
                // TODO - depends on temperature.
                case ResourceType.Water:
                    return 0.00418f;
                case ResourceType.Silicates:
                    return 0.0011f;
                case ResourceType.Iron:
                    return 0.00045f;
            }
            throw new System.Exception( "Type was not specified." );
        }
    }
}