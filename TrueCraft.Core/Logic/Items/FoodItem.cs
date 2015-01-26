using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class FoodItem : ItemProvider
    {
        /// <summary>
        /// The amount of health this food restores.
        /// </summary>
        public abstract float Restores { get; }
    }
}