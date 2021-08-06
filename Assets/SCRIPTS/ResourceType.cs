
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
        // temperature in kelvins
        public static float GetMeltingPoint( this ResourceType type )
        {
            switch( type )
            {
                case ResourceType.Water:
                    return 273;
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
                case ResourceType.Iron:
                    return 7900;
            }
            throw new System.Exception( "Type was not specified." );
        }
    }
}