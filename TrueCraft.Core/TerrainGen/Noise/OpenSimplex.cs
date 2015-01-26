using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.Core.TerrainGen.Noise
{
    /*
     * OpenSimplex was originally created by Kurt Spencer.
     * https://gist.github.com/KdotJPG/b1270127455a94ac5d19
     * Ported to C# by Danel "creatorfromhell" Vidmar.
     */
    public class OpenSimplex : NoiseGen
    {
        private const double STRETCH_CONSTANT_2D = -0.211324865405187;
        private const double SQUISH_CONSTANT_2D = 0.366025403784439;
        private const double STRETCH_CONSTANT_3D = -1.0 / 6;
        private const double SQUISH_CONSTANT_3D = 1.0 / 3;
        private const double STRETCH_CONSTANT_4D = -0.138196601125011;
        private const double SQUISH_CONSTANT_4D = 0.309016994374947;
        private const double NORM_CONSTANT_2D = 47;
        private const double NORM_CONSTANT_3D = 103;
        private const double NORM_CONSTANT_4D = 30;
        private short[] Perm;
        private short[] PermGradIndex3D;

        public OpenSimplex() : this(new Random().Next())
        {
        }

        public OpenSimplex(short[] Perm)
        {
            this.Perm = Perm;
            PermGradIndex3D = new short[256];
            for (int I = 0; I < 256; I++)
            {
                PermGradIndex3D[I] = (short)((Perm[I] % (Gradients3D.Length / 3)) * 3);
            }
        }

        public OpenSimplex(long Seed)
        {
            Perm = new short[256];
            PermGradIndex3D = new short[256];
            short[] Source = new short[256];
            for (short I = 0; I < 256; I++)
                Source[I] = I;
            Seed = Seed * 6364136223846793005L + 1442695040888963407L;
            Seed = Seed * 6364136223846793005L + 1442695040888963407L;
            Seed = Seed * 6364136223846793005L + 1442695040888963407L;
            for (int I = 255; I >= 0; I--) {
                Seed = Seed * 6364136223846793005L + 1442695040888963407L;
                int R = (int)((Seed + 31) % (I + 1));
                if (R < 0)
                    R += (I + 1);
                Perm[I] = Source[R];
                PermGradIndex3D[I] = (short)((Perm[I] % (Gradients3D.Length / 3)) * 3);
                Source[R] = Source[I];
            }
        }

        public override double Value2D(double X, double Y)
        {
            //Place input coordinates onto grid.
            double StretchOffset = (X + Y) * STRETCH_CONSTANT_2D;
            double xs = X + StretchOffset;
            double ys = Y + StretchOffset;

            //Floor to get grid coordinates of rhombus (stretched square) super-cell origin.
            int xsb = Floor(xs);
            int ysb = Floor(ys);

            //Skew out to get actual coordinates of rhombus origin. We'll need these later.
            double SquishOffset = (xsb + ysb) * SQUISH_CONSTANT_2D;
            double xb = xsb + SquishOffset;
            double yb = ysb + SquishOffset;

            //Compute grid coordinates relative to rhombus origin.
            double xins = xs - xsb;
            double yins = ys - ysb;

            //Sum those together to get a value that determines which region we're in.
            double inSum = xins + yins;

            //Positions relative to origin point.
            double dx0 = X - xb;
            double dy0 = Y - yb;

            //We'll be defining these inside the next block and using them afterwards.
            double dx_ext, dy_ext;
            int xsv_ext, ysv_ext;
            double value = 0;

            //Contribution (1,0)
            double dx1 = dx0 - 1 - SQUISH_CONSTANT_2D;
            double dy1 = dy0 - 0 - SQUISH_CONSTANT_2D;
            double attn1 = 2 - dx1 * dx1 - dy1 * dy1;
            if (attn1 > 0)
            {
                attn1 *= attn1;
                value += attn1 * attn1 * Extrapolate2D(xsb + 1, ysb + 0, dx1, dy1);
            }

            //Contribution (0,1)
            double dx2 = dx0 - 0 - SQUISH_CONSTANT_2D;
            double dy2 = dy0 - 1 - SQUISH_CONSTANT_2D;
            double attn2 = 2 - dx2 * dx2 - dy2 * dy2;
            if (attn2 > 0)
            {
                attn2 *= attn2;
                value += attn2 * attn2 * Extrapolate2D(xsb + 0, ysb + 1, dx2, dy2);
            }
            if (inSum <= 1)
            {
                //We're inside the triangle (2-Simplex) at (0,0)
                double zins = 1 - inSum;
                if (zins > xins || zins > yins)
                {
                    //(0,0) is one of the closest two triangular vertices
                    if (xins > yins)
                    {
                        xsv_ext = xsb + 1;
                        ysv_ext = ysb - 1;
                        dx_ext = dx0 - 1;
                        dy_ext = dy0 + 1;
                    }
                    else
                    {
                        xsv_ext = xsb - 1;
                        ysv_ext = ysb + 1;
                        dx_ext = dx0 + 1;
                        dy_ext = dy0 - 1;
                    }
                }
                else
                {
                    //(1,0) and (0,1) are the closest two vertices.
                    xsv_ext = xsb + 1;
                    ysv_ext = ysb + 1;
                    dx_ext = dx0 - 1 - 2 * SQUISH_CONSTANT_2D;
                    dy_ext = dy0 - 1 - 2 * SQUISH_CONSTANT_2D;
                }
            }
            else
            {
                //We're inside the triangle (2-Simplex) at (1,1)
                double zins = 2 - inSum;
                if (zins < xins || zins < yins)
                {
                    //(0,0) is one of the closest two triangular vertices
                    if (xins > yins)
                    {
                        xsv_ext = xsb + 2;
                        ysv_ext = ysb + 0;
                        dx_ext = dx0 - 2 - 2 * SQUISH_CONSTANT_2D;
                        dy_ext = dy0 + 0 - 2 * SQUISH_CONSTANT_2D;
                    }
                    else
                    {
                        xsv_ext = xsb + 0;
                        ysv_ext = ysb + 2;
                        dx_ext = dx0 + 0 - 2 * SQUISH_CONSTANT_2D;
                        dy_ext = dy0 - 2 - 2 * SQUISH_CONSTANT_2D;
                    }
                }
                else
                {
                    //(1,0) and (0,1) are the closest two vertices.
                    dx_ext = dx0;
                    dy_ext = dy0;
                    xsv_ext = xsb;
                    ysv_ext = ysb;
                }
                xsb += 1;
                ysb += 1;
                dx0 = dx0 - 1 - 2 * SQUISH_CONSTANT_2D;
                dy0 = dy0 - 1 - 2 * SQUISH_CONSTANT_2D;
            }
            //Contribution (0,0) or (1,1)
            double attn0 = 2 - dx0 * dx0 - dy0 * dy0;
            if (attn0 > 0)
            {
                attn0 *= attn0;
                value += attn0 * attn0 * Extrapolate2D(xsb, ysb, dx0, dy0);
            }
            //Extra Vertex
            double attn_ext = 2 - dx_ext * dx_ext - dy_ext * dy_ext;
            if (attn_ext > 0)
            {
                attn_ext *= attn_ext;
                value += attn_ext * attn_ext * Extrapolate2D(xsv_ext, ysv_ext, dx_ext, dy_ext);
            }
            return value / NORM_CONSTANT_2D;
        }

        public override double Value3D(double X, double Y, double Z)
        {
            //Place input coordinates on simplectic honeycomb.
            double stretchOffset = (X + Y + Z) * STRETCH_CONSTANT_3D;
            double xs = X + stretchOffset;
            double ys = Y + stretchOffset;
            double zs = Z + stretchOffset;

            //Floor to get simplectic honeycomb coordinates of rhombohedron (stretched cube) super-cell origin.
            int xsb = Floor(xs);
            int ysb = Floor(ys);
            int zsb = Floor(zs);

            //Skew out to get actual coordinates of rhombohedron origin. We'll need these later.
            double squishOffset = (xsb + ysb + zsb) * SQUISH_CONSTANT_3D;
            double xb = xsb + squishOffset;
            double yb = ysb + squishOffset;
            double zb = zsb + squishOffset;

            //Compute simplectic honeycomb coordinates relative to rhombohedral origin.
            double xins = xs - xsb;
            double yins = ys - ysb;
            double zins = zs - zsb;

            //Sum those together to get a value that determines which region we're in.
            double inSum = xins + yins + zins;

            //Positions relative to origin point.
            double dx0 = X - xb;
            double dy0 = Y - yb;
            double dz0 = Z - zb;

            //We'll be defining these inside the next block and using them afterwards.
            double dx_ext0, dy_ext0, dz_ext0;
            double dx_ext1, dy_ext1, dz_ext1;
            int xsv_ext0, ysv_ext0, zsv_ext0;
            int xsv_ext1, ysv_ext1, zsv_ext1;
            double value = 0;
            if (inSum <= 1)
            {
                /*
                 * We're inside the tetrahedron (3-Simplex) at (0,0,0)
                 * Determine which two of (0,0,1), (0,1,0), (1,0,0) are closest.
                 */
                byte aPoint = 0x01;
                double aScore = xins;
                byte bPoint = 0x02;
                double bScore = yins;
                if (aScore >= bScore && zins > bScore)
                {
                    bScore = zins;
                    bPoint = 0x04;
                }
                else if (aScore < bScore && zins > aScore)
                {
                    aScore = zins;
                    aPoint = 0x04;
                }

                /*
                 * Now we determine the two lattice points not part of the tetrahedron that may contribute.
                 * This depends on the closest two tetrahedral vertices, including (0,0,0)
                 */
                double wins = 1 - inSum;
                if (wins > aScore || wins > bScore)
                {
                    //(0,0,0) is one of the closest two tetrahedral vertices.
                    byte c = (bScore > aScore ? bPoint : aPoint); //Our other closest vertex is the closest out of a and b.
                    if ((c & 0x01) == 0)
                    {
                        xsv_ext0 = xsb - 1;
                        xsv_ext1 = xsb;
                        dx_ext0 = dx0 + 1;
                        dx_ext1 = dx0;
                    }
                    else
                    {
                        xsv_ext0 = xsv_ext1 = xsb + 1;
                        dx_ext0 = dx_ext1 = dx0 - 1;
                    }

                    if ((c & 0x02) == 0)
                    {
                        ysv_ext0 = ysv_ext1 = ysb;
                        dy_ext0 = dy_ext1 = dy0;
                        if ((c & 0x01) == 0)
                        {
                            ysv_ext1 -= 1;
                            dy_ext1 += 1;
                        }
                        else
                        {
                            ysv_ext0 -= 1;
                            dy_ext0 += 1;
                        }
                    }
                    else
                    {
                        ysv_ext0 = ysv_ext1 = ysb + 1;
                        dy_ext0 = dy_ext1 = dy0 - 1;
                    }

                    if ((c & 0x04) == 0)
                    {
                        zsv_ext0 = zsb;
                        zsv_ext1 = zsb - 1;
                        dz_ext0 = dz0;
                        dz_ext1 = dz0 + 1;
                    }
                    else
                    {
                        zsv_ext0 = zsv_ext1 = zsb + 1;
                        dz_ext0 = dz_ext1 = dz0 - 1;
                    }
                }
                else
                {
                    //(0,0,0) is not one of the closest two tetrahedral vertices.
                    byte c = (byte)(aPoint | bPoint); //Our two extra vertices are determined by the closest two.
                    if ((c & 0x01) == 0)
                    {
                        xsv_ext0 = xsb;
                        xsv_ext1 = xsb - 1;
                        dx_ext0 = dx0 - 2 * SQUISH_CONSTANT_3D;
                        dx_ext1 = dx0 + 1 - SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        xsv_ext0 = xsv_ext1 = xsb + 1;
                        dx_ext0 = dx0 - 1 - 2 * SQUISH_CONSTANT_3D;
                        dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_3D;
                    }

                    if ((c & 0x02) == 0)
                    {
                        ysv_ext0 = ysb;
                        ysv_ext1 = ysb - 1;
                        dy_ext0 = dy0 - 2 * SQUISH_CONSTANT_3D;
                        dy_ext1 = dy0 + 1 - SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        ysv_ext0 = ysv_ext1 = ysb + 1;
                        dy_ext0 = dy0 - 1 - 2 * SQUISH_CONSTANT_3D;
                        dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_3D;
                    }

                    if ((c & 0x04) == 0)
                    {
                        zsv_ext0 = zsb;
                        zsv_ext1 = zsb - 1;
                        dz_ext0 = dz0 - 2 * SQUISH_CONSTANT_3D;
                        dz_ext1 = dz0 + 1 - SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        zsv_ext0 = zsv_ext1 = zsb + 1;
                        dz_ext0 = dz0 - 1 - 2 * SQUISH_CONSTANT_3D;
                        dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_3D;
                    }
                }

                //Contribution (0,0,0)
                double attn0 = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0;
                if (attn0 > 0)
                {
                    attn0 *= attn0;
                    value += attn0 * attn0 * Extrapolate3D(xsb + 0, ysb + 0, zsb + 0, dx0, dy0, dz0);
                }

                //Contribution (1,0,0)
                double dx1 = dx0 - 1 - SQUISH_CONSTANT_3D;
                double dy1 = dy0 - 0 - SQUISH_CONSTANT_3D;
                double dz1 = dz0 - 0 - SQUISH_CONSTANT_3D;
                double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1;
                if (attn1 > 0)
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate3D(xsb + 1, ysb + 0, zsb + 0, dx1, dy1, dz1);
                }

                //Contribution (0,1,0)
                double dx2 = dx0 - 0 - SQUISH_CONSTANT_3D;
                double dy2 = dy0 - 1 - SQUISH_CONSTANT_3D;
                double dz2 = dz1;
                double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2;
                if (attn2 > 0)
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate3D(xsb + 0, ysb + 1, zsb + 0, dx2, dy2, dz2);
                }

                //Contribution (0,0,1)
                double dx3 = dx2;
                double dy3 = dy1;
                double dz3 = dz0 - 1 - SQUISH_CONSTANT_3D;
                double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3;
                if (attn3 > 0)
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate3D(xsb + 0, ysb + 0, zsb + 1, dx3, dy3, dz3);
                }
            }
            else if (inSum >= 2)
            {
                /*
                 * We're inside the tetrahedron (3-Simplex) at (1,1,1)
                 * Determine which two tetrahedral vertices are the closest, out of (1,1,0), (1,0,1), (0,1,1) but not (1,1,1).
                 */
                byte aPoint = 0x06;
                double aScore = xins;
                byte bPoint = 0x05;
                double bScore = yins;
                if (aScore <= bScore && zins < bScore)
                {
                    bScore = zins;
                    bPoint = 0x03;
                }
                else if (aScore > bScore && zins < aScore)
                {
                    aScore = zins;
                    aPoint = 0x03;
                }

                /*
                 * Now we determine the two lattice points not part of the tetrahedron that may contribute.
                 * This depends on the closest two tetrahedral vertices, including (1,1,1)
                 */
                double wins = 3 - inSum;
                if (wins < aScore || wins < bScore)
                {
                    //(1,1,1) is one of the closest two tetrahedral vertices.
                    byte c = (bScore < aScore ? bPoint : aPoint); //Our other closest vertex is the closest out of a and b.
                    if ((c & 0x01) != 0)
                    {
                        xsv_ext0 = xsb + 2;
                        xsv_ext1 = xsb + 1;
                        dx_ext0 = dx0 - 2 - 3 * SQUISH_CONSTANT_3D;
                        dx_ext1 = dx0 - 1 - 3 * SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        xsv_ext0 = xsv_ext1 = xsb;
                        dx_ext0 = dx_ext1 = dx0 - 3 * SQUISH_CONSTANT_3D;
                    }

                    if ((c & 0x02) != 0)
                    {
                        ysv_ext0 = ysv_ext1 = ysb + 1;
                        dy_ext0 = dy_ext1 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D;
                        if ((c & 0x01) != 0)
                        {
                            ysv_ext1 += 1;
                            dy_ext1 -= 1;
                        }
                        else
                        {
                            ysv_ext0 += 1;
                            dy_ext0 -= 1;
                        }
                    }
                    else
                    {
                        ysv_ext0 = ysv_ext1 = ysb;
                        dy_ext0 = dy_ext1 = dy0 - 3 * SQUISH_CONSTANT_3D;
                    }

                    if ((c & 0x04) != 0)
                    {
                        zsv_ext0 = zsb + 1;
                        zsv_ext1 = zsb + 2;
                        dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_3D;
                        dz_ext1 = dz0 - 2 - 3 * SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        zsv_ext0 = zsv_ext1 = zsb;
                        dz_ext0 = dz_ext1 = dz0 - 3 * SQUISH_CONSTANT_3D;
                    }
                }
                else
                {
                    //(1,1,1) is not one of the closest two tetrahedral vertices.
                    byte c = (byte)(aPoint & bPoint); //Our two extra vertices are determined by the closest two.
                    if ((c & 0x01) != 0)
                    {
                        xsv_ext0 = xsb + 1;
                        xsv_ext1 = xsb + 2;
                        dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_3D;
                        dx_ext1 = dx0 - 2 - 2 * SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        xsv_ext0 = xsv_ext1 = xsb;
                        dx_ext0 = dx0 - SQUISH_CONSTANT_3D;
                        dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D;
                    }

                    if ((c & 0x02) != 0)
                    {
                        ysv_ext0 = ysb + 1;
                        ysv_ext1 = ysb + 2;
                        dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_3D;
                        dy_ext1 = dy0 - 2 - 2 * SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        ysv_ext0 = ysv_ext1 = ysb;
                        dy_ext0 = dy0 - SQUISH_CONSTANT_3D;
                        dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D;
                    }

                    if ((c & 0x04) != 0)
                    {
                        zsv_ext0 = zsb + 1;
                        zsv_ext1 = zsb + 2;
                        dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_3D;
                        dz_ext1 = dz0 - 2 - 2 * SQUISH_CONSTANT_3D;
                    }
                    else
                    {
                        zsv_ext0 = zsv_ext1 = zsb;
                        dz_ext0 = dz0 - SQUISH_CONSTANT_3D;
                        dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D;
                    }
                }

                //Contribution (1,1,0)
                double dx3 = dx0 - 1 - 2 * SQUISH_CONSTANT_3D;
                double dy3 = dy0 - 1 - 2 * SQUISH_CONSTANT_3D;
                double dz3 = dz0 - 0 - 2 * SQUISH_CONSTANT_3D;
                double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3;
                if (attn3 > 0)
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate3D(xsb + 1, ysb + 1, zsb + 0, dx3, dy3, dz3);
                }

                //Contribution (1,0,1)
                double dx2 = dx3;
                double dy2 = dy0 - 0 - 2 * SQUISH_CONSTANT_3D;
                double dz2 = dz0 - 1 - 2 * SQUISH_CONSTANT_3D;
                double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2;
                if (attn2 > 0)
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate3D(xsb + 1, ysb + 0, zsb + 1, dx2, dy2, dz2);
                }

                //Contribution (0,1,1)
                double dx1 = dx0 - 0 - 2 * SQUISH_CONSTANT_3D;
                double dy1 = dy3;
                double dz1 = dz2;
                double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1;
                if (attn1 > 0)
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate3D(xsb + 0, ysb + 1, zsb + 1, dx1, dy1, dz1);
                }

                //Contribution (1,1,1)
                dx0 = dx0 - 1 - 3 * SQUISH_CONSTANT_3D;
                dy0 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D;
                dz0 = dz0 - 1 - 3 * SQUISH_CONSTANT_3D;
                double attn0 = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0;
                if (attn0 > 0)
                {
                    attn0 *= attn0;
                    value += attn0 * attn0 * Extrapolate3D(xsb + 1, ysb + 1, zsb + 1, dx0, dy0, dz0);
                }
            }
            else
            {
                //We're inside the octahedron (Rectified 3-Simplex) in between.
                double aScore;
                byte aPoint;
                bool aIsFurtherSide;
                double bScore;
                byte bPoint;
                bool bIsFurtherSide;

                //Decide between point (0,0,1) and (1,1,0) as closest
                double p1 = xins + yins;
                if (p1 > 1)
                {
                    aScore = p1 - 1;
                    aPoint = 0x03;
                    aIsFurtherSide = true;
                }
                else
                {
                    aScore = 1 - p1;
                    aPoint = 0x04;
                    aIsFurtherSide = false;
                }

                //Decide between point (0,1,0) and (1,0,1) as closest
                double p2 = xins + zins;
                if (p2 > 1)
                {
                    bScore = p2 - 1;
                    bPoint = 0x05;
                    bIsFurtherSide = true;
                }
                else
                {
                    bScore = 1 - p2;
                    bPoint = 0x02;
                    bIsFurtherSide = false;
                }

                //The closest out of the two (1,0,0) and (0,1,1) will replace the furthest out of the two decided above, if closer.
                double p3 = yins + zins;
                if (p3 > 1)
                {
                    double score = p3 - 1;
                    if (aScore <= bScore && aScore < score)
                    {
                        aScore = score;
                        aPoint = 0x06;
                        aIsFurtherSide = true;
                    }
                    else if (aScore > bScore && bScore < score)
                    {
                        bScore = score;
                        bPoint = 0x06;
                        bIsFurtherSide = true;
                    }
                }
                else
                {
                    double score = 1 - p3;
                    if (aScore <= bScore && aScore < score)
                    {
                        aScore = score;
                        aPoint = 0x01;
                        aIsFurtherSide = false;
                    }
                    else if (aScore > bScore && bScore < score)
                    {
                        bScore = score;
                        bPoint = 0x01;
                        bIsFurtherSide = false;
                    }
                }

                //Where each of the two closest points are determines how the extra two vertices are calculated.
                if (aIsFurtherSide == bIsFurtherSide)
                {
                    if (aIsFurtherSide)
                    {
                        //Both closest points on (1,1,1) side

                        //One of the two extra points is (1,1,1)
                        dx_ext0 = dx0 - 1 - 3 * SQUISH_CONSTANT_3D;
                        dy_ext0 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D;
                        dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_3D;
                        xsv_ext0 = xsb + 1;
                        ysv_ext0 = ysb + 1;
                        zsv_ext0 = zsb + 1;

                        //Other extra point is based on the shared axis.
                        byte c = (byte)(aPoint & bPoint);
                        if ((c & 0x01) != 0)
                        {
                            dx_ext1 = dx0 - 2 - 2 * SQUISH_CONSTANT_3D;
                            dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D;
                            dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D;
                            xsv_ext1 = xsb + 2;
                            ysv_ext1 = ysb;
                            zsv_ext1 = zsb;
                        }
                        else if ((c & 0x02) != 0)
                        {
                            dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D;
                            dy_ext1 = dy0 - 2 - 2 * SQUISH_CONSTANT_3D;
                            dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D;
                            xsv_ext1 = xsb;
                            ysv_ext1 = ysb + 2;
                            zsv_ext1 = zsb;
                        }
                        else
                        {
                            dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D;
                            dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D;
                            dz_ext1 = dz0 - 2 - 2 * SQUISH_CONSTANT_3D;
                            xsv_ext1 = xsb;
                            ysv_ext1 = ysb;
                            zsv_ext1 = zsb + 2;
                        }
                    }
                    else
                    {
                        /*
                         * Both closest points on (0,0,0) side
                         * One of the two extra points is (0,0,0)
                         */
                        dx_ext0 = dx0;
                        dy_ext0 = dy0;
                        dz_ext0 = dz0;
                        xsv_ext0 = xsb;
                        ysv_ext0 = ysb;
                        zsv_ext0 = zsb;

                        //Other extra point is based on the omitted axis.
                        byte c = (byte)(aPoint | bPoint);
                        if ((c & 0x01) == 0)
                        {
                            dx_ext1 = dx0 + 1 - SQUISH_CONSTANT_3D;
                            dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_3D;
                            dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_3D;
                            xsv_ext1 = xsb - 1;
                            ysv_ext1 = ysb + 1;
                            zsv_ext1 = zsb + 1;
                        }
                        else if ((c & 0x02) == 0)
                        {
                            dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_3D;
                            dy_ext1 = dy0 + 1 - SQUISH_CONSTANT_3D;
                            dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_3D;
                            xsv_ext1 = xsb + 1;
                            ysv_ext1 = ysb - 1;
                            zsv_ext1 = zsb + 1;
                        }
                        else
                        {
                            dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_3D;
                            dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_3D;
                            dz_ext1 = dz0 + 1 - SQUISH_CONSTANT_3D;
                            xsv_ext1 = xsb + 1;
                            ysv_ext1 = ysb + 1;
                            zsv_ext1 = zsb - 1;
                        }
                    }
                }
                else
                {
                    //One point on (0,0,0) side, one point on (1,1,1) side
                    byte c1, c2;
                    if (aIsFurtherSide)
                    {
                        c1 = aPoint;
                        c2 = bPoint;
                    }
                    else
                    {
                        c1 = bPoint;
                        c2 = aPoint;
                    }

                    //One contribution is a permutation of (1,1,-1)
                    if ((c1 & 0x01) == 0)
                    {
                        dx_ext0 = dx0 + 1 - SQUISH_CONSTANT_3D;
                        dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_3D;
                        dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_3D;
                        xsv_ext0 = xsb - 1;
                        ysv_ext0 = ysb + 1;
                        zsv_ext0 = zsb + 1;
                    }
                    else if ((c1 & 0x02) == 0)
                    {
                        dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_3D;
                        dy_ext0 = dy0 + 1 - SQUISH_CONSTANT_3D;
                        dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_3D;
                        xsv_ext0 = xsb + 1;
                        ysv_ext0 = ysb - 1;
                        zsv_ext0 = zsb + 1;
                    }
                    else
                    {
                        dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_3D;
                        dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_3D;
                        dz_ext0 = dz0 + 1 - SQUISH_CONSTANT_3D;
                        xsv_ext0 = xsb + 1;
                        ysv_ext0 = ysb + 1;
                        zsv_ext0 = zsb - 1;
                    }

                    //One contribution is a permutation of (0,0,2)
                    dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D;
                    dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D;
                    dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D;
                    xsv_ext1 = xsb;
                    ysv_ext1 = ysb;
                    zsv_ext1 = zsb;
                    if ((c2 & 0x01) != 0)
                    {
                        dx_ext1 -= 2;
                        xsv_ext1 += 2;
                    }
                    else if ((c2 & 0x02) != 0)
                    {
                        dy_ext1 -= 2;
                        ysv_ext1 += 2;
                    }
                    else
                    {
                        dz_ext1 -= 2;
                        zsv_ext1 += 2;
                    }
                }

                //Contribution (1,0,0)
                double dx1 = dx0 - 1 - SQUISH_CONSTANT_3D;
                double dy1 = dy0 - 0 - SQUISH_CONSTANT_3D;
                double dz1 = dz0 - 0 - SQUISH_CONSTANT_3D;
                double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1;
                if (attn1 > 0)
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate3D(xsb + 1, ysb + 0, zsb + 0, dx1, dy1, dz1);
                }

                //Contribution (0,1,0)
                double dx2 = dx0 - 0 - SQUISH_CONSTANT_3D;
                double dy2 = dy0 - 1 - SQUISH_CONSTANT_3D;
                double dz2 = dz1;
                double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2;
                if (attn2 > 0)
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate3D(xsb + 0, ysb + 1, zsb + 0, dx2, dy2, dz2);
                }

                //Contribution (0,0,1)
                double dx3 = dx2;
                double dy3 = dy1;
                double dz3 = dz0 - 1 - SQUISH_CONSTANT_3D;
                double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3;
                if (attn3 > 0)
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate3D(xsb + 0, ysb + 0, zsb + 1, dx3, dy3, dz3);
                }

                //Contribution (1,1,0)
                double dx4 = dx0 - 1 - 2 * SQUISH_CONSTANT_3D;
                double dy4 = dy0 - 1 - 2 * SQUISH_CONSTANT_3D;
                double dz4 = dz0 - 0 - 2 * SQUISH_CONSTANT_3D;
                double attn4 = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4;
                if (attn4 > 0)
                {
                    attn4 *= attn4;
                    value += attn4 * attn4 * Extrapolate3D(xsb + 1, ysb + 1, zsb + 0, dx4, dy4, dz4);
                }

                //Contribution (1,0,1)
                double dx5 = dx4;
                double dy5 = dy0 - 0 - 2 * SQUISH_CONSTANT_3D;
                double dz5 = dz0 - 1 - 2 * SQUISH_CONSTANT_3D;
                double attn5 = 2 - dx5 * dx5 - dy5 * dy5 - dz5 * dz5;
                if (attn5 > 0)
                {
                    attn5 *= attn5;
                    value += attn5 * attn5 * Extrapolate3D(xsb + 1, ysb + 0, zsb + 1, dx5, dy5, dz5);
                }

                //Contribution (0,1,1)
                double dx6 = dx0 - 0 - 2 * SQUISH_CONSTANT_3D;
                double dy6 = dy4;
                double dz6 = dz5;
                double attn6 = 2 - dx6 * dx6 - dy6 * dy6 - dz6 * dz6;
                if (attn6 > 0)
                {
                    attn6 *= attn6;
                    value += attn6 * attn6 * Extrapolate3D(xsb + 0, ysb + 1, zsb + 1, dx6, dy6, dz6);
                }
            }

            //First extra vertex
            double attn_ext0 = 2 - dx_ext0 * dx_ext0 - dy_ext0 * dy_ext0 - dz_ext0 * dz_ext0;
            if (attn_ext0 > 0)
            {
                attn_ext0 *= attn_ext0;
                value += attn_ext0 * attn_ext0 * Extrapolate3D(xsv_ext0, ysv_ext0, zsv_ext0, dx_ext0, dy_ext0, dz_ext0);
            }

            //Second extra vertex
            double attn_ext1 = 2 - dx_ext1 * dx_ext1 - dy_ext1 * dy_ext1 - dz_ext1 * dz_ext1;
            if (attn_ext1 > 0)
            {
                attn_ext1 *= attn_ext1;
                value += attn_ext1 * attn_ext1 * Extrapolate3D(xsv_ext1, ysv_ext1, zsv_ext1, dx_ext1, dy_ext1, dz_ext1);
            }
            return value / NORM_CONSTANT_3D;
        }

        private double Extrapolate2D(int XS, int YS, double XD, double YD)
        {
            int Index = Perm[(Perm[XS & 0xFF] + YS) & 0xFF] & 0x0E;
            return Gradients2D[Index] * XD + Gradients2D[Index + 1] * YD;
        }
        private double Extrapolate3D(int XS, int YS, int ZS, double XD, double YD, double ZD)
        {
            int Index = PermGradIndex3D[(Perm[(Perm[XS & 0xFF] + YS) & 0xFF] + ZS) & 0xFF];
            return Gradients3D[Index] * XD + Gradients3D[Index + 1] * YD + Gradients3D[Index + 2] * ZD;
        } 

        private static short[] Gradients2D = new short[] {
            5, 2, 2, 5,
            -5, 2, -2, 5,
            5, -2, 2, -5,
            -5, -2, -2, -5,
        };

        private static short[] Gradients3D = new short[] {
            -11, 4, 4, -4, 11, 4, -4, 4, 11,
            11, 4, 4, 4, 11, 4, 4, 4, 11,
            -11, -4, 4, -4, -11, 4, -4, -4, 11,
            11, -4, 4, 4, -11, 4, 4, -4, 11,
            -11, 4, -4, -4, 11, -4, -4, 4, -11,
            11, 4, -4, 4, 11, -4, 4, 4, -11,
            -11, -4, -4, -4, -11, -4, -4, -4, -11,
            11, -4, -4, 4, -11, -4, 4, -4, -11,
       };
    }
}