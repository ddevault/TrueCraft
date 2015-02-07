using System;
using System.Linq;
using System.Collections.Generic;
using TrueCraft.API;
using TrueCraft.API.Windows;
using TrueCraft.API.Logic;

namespace TrueCraft
{
    public class CraftingRepository : ICraftingRepository
    {
        private readonly List<ICraftingRecipe> Recipes = new List<ICraftingRecipe>();

        public void RegisterRecipe(ICraftingRecipe recipe)
        {
            Recipes.Add(recipe);
        }

        internal void DiscoverRecipes()
        {
            var recipeTypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes().Where(t =>
                    typeof(ICraftingRecipe).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    recipeTypes.Add(type);
                }
            }

            recipeTypes.ForEach(t =>
            {
                var instance = (ICraftingRecipe)Activator.CreateInstance(t);
                RegisterRecipe(instance);
            });
        }

        public ICraftingRecipe GetRecipe(IWindowArea craftingArea)
        {
            foreach (var r in Recipes)
            {
                if (MatchRecipe(r, craftingArea))
                    return r;
            }
            return null;
        }

        public bool TestRecipe(IWindowArea craftingArea, ICraftingRecipe recipe, int x, int y)
        {
            if (x + recipe.Pattern.GetLength(0) > craftingArea.Width || y + recipe.Pattern.GetLength(1) > craftingArea.Height)
                return false;
            for (int _x = 0; _x < recipe.Pattern.GetLength(0); _x++)
            {
                for (int _y = 0; _y < recipe.Pattern.GetLength(1); _y++)
                {
                    var supplied = craftingArea[(y + _y) * craftingArea.Width + (x + _x)];
                    var required = recipe.Pattern[_x, _y];
                    if (supplied.ID != required.ID || supplied.Count < required.Count)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool MatchRecipe(ICraftingRecipe recipe, IWindowArea craftingArea)
        {
            for (int x = 0; x < craftingArea.Width; x++)
            {
                for (int y = 0; y < craftingArea.Height; y++)
                {
                    var item = craftingArea[y * craftingArea.Width + x];
                    if (item.ID == recipe.Pattern[0, 0].ID &&
                        item.Count >= recipe.Pattern[0, 0].Count)
                    {
                        return TestRecipe(craftingArea, recipe, x, y);
                    }
                    if (!item.Empty)
                        return false;
                }
            }
            return false;
        }
    }
}