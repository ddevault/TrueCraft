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
            if (x + recipe.Pattern.GetLength(1) > craftingArea.Width || y + recipe.Pattern.GetLength(0) > craftingArea.Height)
                return false;
            for (int _x = 0; _x < recipe.Pattern.GetLength(1); _x++)
            {
                for (int _y = 0; _y < recipe.Pattern.GetLength(0); _y++)
                {
                    var supplied = craftingArea[(y + _y) * craftingArea.Width + (x + _x)];
                    var required = recipe.Pattern[_y, _x];
                    if (supplied.ID != required.ID || supplied.Count < required.Count ||
                        (recipe.SignificantMetadata && (required.Metadata != supplied.Metadata)))
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
                    if (TestRecipe(craftingArea, recipe, x, y))
                    {
                        // Check to make sure there aren't any sneaky unused items in the grid
                        for (int _x = 0; _x < x; x++)
                        {
                            for (int _y = 0; _y < y; _y++)
                            {
                                var supplied = craftingArea[(y + _y) * craftingArea.Width + (x + _x)];
                                if (!supplied.Empty)
                                    return false;
                            }
                        }
                        for (int _y = 0; _y < y; _y++)
                        {
                            for (int _x = 0; _x < x; x++)
                            {
                                var supplied = craftingArea[(y + _y) * craftingArea.Width + (x + _x)];
                                if (!supplied.Empty)
                                    return false;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }
    }
}