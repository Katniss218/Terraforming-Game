
namespace TerraformingGame
{
    public class InventoryResource
    {
        public ResourceType type;
        public double amount;

        public override string ToString()
        {
            return this.type.GetDisplayName() + " - " + this.amount.ToString();
        }
    }
}