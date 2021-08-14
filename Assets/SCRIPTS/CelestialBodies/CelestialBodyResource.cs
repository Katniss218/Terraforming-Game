
namespace TerraformingGame
{
    public class CelestialBodyResource
    {
        // represents a resource in a layer (not in transport?)

        public ResourceType type;
        public double amount; // amount in cubic meters

        public float meltingPoint { get { return this.type.GetMeltingPoint(); } }
        public float boilingPoint { get { return this.type.GetBoilingPoint(); } }
        public float density { get { return this.type.GetDensity(); } }

        public StateOfMatter state;

        public override string ToString()
        {
            return this.type.GetDisplayName() + " - " + this.amount.ToString();
        }
    }
}