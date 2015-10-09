using TrueCraft.API;
using TrueCraft.API.Windows;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Windows
{
    public class CraftingWindowArea : WindowArea
    {
        public static readonly int CraftingOutput = 0;
        public ICraftingRepository Repository { get; set; }
        public bool Process { get; set; }

        public CraftingWindowArea(ICraftingRepository repository, int startIndex, int width = 2, int height = 2)
            : base(startIndex, width * height + 1, width, height)
        {
            Repository = repository;
            WindowChange += HandleWindowChange;
        }

        private void HandleWindowChange(object sender, WindowChangeEventArgs e)
        {
            if (Repository == null)
                return;
            var current = Repository.GetRecipe(Bench);
            if (e.SlotIndex == CraftingOutput)
            {
                if (e.Value.Empty && current != null) // Item picked up
                {
                    RemoveItemFromOutput(current);
                    current = Repository.GetRecipe(Bench);
                }
            }
            if (current == null)
                Items[CraftingOutput] = ItemStack.EmptyStack;
            else
                Items[CraftingOutput] = current.Output;
        }

        private void RemoveItemFromOutput(ICraftingRecipe recipe)
        {
            // Locate area on crafting bench
            int x, y = 0;
            for (x = 0; x < Width; x++)
            {
                bool found = false;
                for (y = 0; y < Height; y++)
                {
                    if (Repository.TestRecipe(Bench, recipe, x, y))
                    {
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            // Remove items
            for (int _x = 0; _x < recipe.Pattern.GetLength(1); _x++)
            {
                for (int _y = 0; _y < recipe.Pattern.GetLength(0); _y++)
                {
                    var item = Items[(y + _y) * Width + (x + _x) + 1];
                    item.Count -= recipe.Pattern[_y, _x].Count;
                    Items[(y + _y) * Width + (x + _x) + 1] = item;
                }
            }
        }

        public WindowArea Bench
        {
            get
            {
                var result = new WindowArea(1, Width * Height, Width, Height);
                for (var i = 1; i < Items.Length; i++)
                    result.Items[i - 1] = Items[i];
                return result;
            }
        }
    }
}
