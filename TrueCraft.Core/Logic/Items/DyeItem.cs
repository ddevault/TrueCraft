using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class DyeItem : ItemProvider
    {
        public enum DyeType
        {
            InkSac = 0,
            RoseRed = 1,
            CactusGreen = 2,
            CocoaBeans = 3,
            LapisLazuli = 4,
            PurpleDye = 5,
            CyanDye = 6,
            LightGrayDye = 7,
            GrayDye = 8,
            PinkDye = 9,
            LimeDye = 10,
            DandelionYellow = 11,
            LightBlueDye = 12,
            MagentaDye = 13,
            BoneMeal = 14
        }

        public static readonly short ItemID = 0x15F;

        public override short ID { get { return 0x15F; } }

        public override string DisplayName { get { return "Dye"; } }

        public class BoneMealRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,] { { new ItemStack(BoneItem.ItemID) } };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.BoneMeal); } }

            public bool SignificantMetadata { get { return false; } }
        }

        public class LightGrayDyeRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.InkSac),
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.BoneMeal),
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.BoneMeal),
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.LightGrayDye); } }

            public bool SignificantMetadata { get { return true; } }
        }

        public class GrayDyeRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.InkSac),
                            ItemStack.EmptyStack,
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.BoneMeal),
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.GrayDye); } }

            public bool SignificantMetadata { get { return true; } }
        }

        public class RoseRedRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(RoseBlock.BlockID),
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.RoseRed); } }

            public bool SignificantMetadata { get { return false; } }
        }

        public class OrangeDyeRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.DandelionYellow),
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.RoseRed),
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.RoseRed); } }

            public bool SignificantMetadata { get { return true; } }
        }

        public class DandelionYellowRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(FlowerBlock.BlockID)
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.DandelionYellow); } }

            public bool SignificantMetadata { get { return false; } }
        }

        public class LimeDyeRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.CactusGreen),
                            ItemStack.EmptyStack,
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.BoneMeal),
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.LimeDye); } }

            public bool SignificantMetadata { get { return true; } }
        }

        public class LightBlueDyeRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.LapisLazuli),
                            ItemStack.EmptyStack,
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.BoneMeal),
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.LightBlueDye); } }

            public bool SignificantMetadata { get { return true; } }
        }

        public class CyanDyeRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.LapisLazuli),
                            ItemStack.EmptyStack,
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.CactusGreen),
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.CyanDye); } }

            public bool SignificantMetadata { get { return true; } }
        }

        public class PurpleDyeRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.LapisLazuli),
                            ItemStack.EmptyStack,
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.RoseRed),
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.PurpleDye); } }

            public bool SignificantMetadata { get { return true; } }
        }

        public class MagentaDyeRecipe1 : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.PurpleDye),
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.PinkDye),
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.MagentaDye); } }

            public bool SignificantMetadata { get { return true; } }
        }

        public class MagentaDyeRecipe2 : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.LapisLazuli),
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.BoneMeal),
                            new ItemStack(DyeItem.ItemID, 2, DyeItem.DyeType.RoseRed),
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.MagentaDye); } }

            public bool SignificantMetadata { get { return true; } }
        }

        public class MagentaDyeRecipe3 : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.LapisLazuli),
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.PinkDye),
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.RoseRed),
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.MagentaDye); } }

            public bool SignificantMetadata { get { return true; } }
        }

        public class PinkDyeRecipe : ICraftingRecipe
        {
            public ItemStack[,] Pattern
            {
                get
                {
                    return new[,]
                    {
                        {
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.BoneMeal),
                            new ItemStack(DyeItem.ItemID, 1, DyeItem.DyeType.RoseRed),
                        }
                    };
                }
            }

            public ItemStack Output { get { return new ItemStack(ItemID, 1, (short)DyeItem.DyeType.PinkDye); } }

            public bool SignificantMetadata { get { return true; } }
        }
    }
}