using TrueCraft.API;

namespace TrueCraft.API.Logic
{
    public interface ICraftingRecipe
    {
		ItemStack[,] Pattern { get; }
		ItemStack Output { get; }
    }
}