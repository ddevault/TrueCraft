using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API
{
    public enum TreeSpecies
    {
        Oak,
        Birch,
        Spruce
    }

    //The following enums are mainly for generation purposes only.
    public enum SpruceType
    {
        //TODO: Spruce types.
    }

    public enum OakType
    {
        Normal, //Uses layered circles for leaves
        BalloonBlocky, //Uses a "blocky" sphere for leaves
        Balloon, //Uses a sphere for leaves
        Branched //Uses multiple spheres for leaves and random extra logs acting as branches
    }
}