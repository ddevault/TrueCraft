/*
 * Copyright (c) 2011 Richard Klafter, Eric Swanson
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.TerrainGen.Noise
{
    public class CellNoise : NoiseGen
    {
        public int Seed { get; set; }
        public DistanceType DistanceFunction { get; set; }
        public CombinationFunctions CombinationFunction { get; set; }

        public CellNoise() : this(new Random().Next())
        {
        }

        public CellNoise(int seed)
        {
            Seed = seed;
            DistanceFunction = DistanceType.EUCLIDEAN3D;
            CombinationFunction = CombinationFunctions.D2MINUSD1;
        }

        public override double Value2D(double X, double Y)
        {
            double[] Distances = new double[3];
            //Declare some values for later use
            uint lastRandom, numberFeaturePoints;
            Vector3 randomDiff, featurePoint;
            int cubeX, cubeY;
            //Initialize values in distance array to large values
            for (int i = 0; i < Distances.Length; i++)
                Distances[i] = 6666;
            //1. Determine which cube the evaluation point is in
            int evalCubeX = Floor(X);
            int evalCubeY = Floor(Y);
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    cubeX = evalCubeX + i;
                    cubeY = evalCubeY + j;
                    //2. Generate a reproducible random number generator for the cube
                    lastRandom = lcgRandom(hash2d((uint)(cubeX + Seed), (uint)(cubeY)));
                    //3. Determine how many feature points are in the cube
                    numberFeaturePoints = probLookup(lastRandom);
                    //4. Randomly place the feature points in the cube
                    for (uint l = 0; l < numberFeaturePoints; ++l)
                    {
                        lastRandom = lcgRandom(lastRandom);
                        randomDiff.X = lastRandom / 0x100000000;
                        lastRandom = lcgRandom(lastRandom);
                        randomDiff.Y = lastRandom / 0x100000000;
                        lastRandom = lcgRandom(lastRandom);
                        randomDiff.Z = lastRandom / 0x100000000;
                        featurePoint = new Vector3(randomDiff.X + cubeX, randomDiff.Y + cubeY, 0);
                        //5. Find the feature point closest to the evaluation point.
                        //This is done by inserting the distances to the feature points into a sorted list
                        insert(Distances, Distance(new Vector3(X, Y, 0), featurePoint));
                    }
                }
            }
            return Combine(Distances);
        }

        /// <summary>
        /// Generates 3D Cell Noise
        /// </summary>
        /// <returns>The color worley noise returns at the input position</returns>
        public override double Value3D(double X, double Y, double Z)
        {
            double[] Distances = new double[3];
            //Declare some values for later use
            uint lastRandom, numberFeaturePoints;
            Vector3 randomDiff, featurePoint;
            int cubeX, cubeY, cubeZ;
            //Initialize values in distance array to large values
            for (int i = 0; i < Distances.Length; i++)
                Distances[i] = 6666;
            //1. Determine which cube the evaluation point is in
            int evalCubeX = Floor(X);
            int evalCubeY = Floor(Y);
            int evalCubeZ = Floor(Z);
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    for (int k = -1; k < 2; ++k)
                    {
                        cubeX = evalCubeX + i;
                        cubeY = evalCubeY + j;
                        cubeZ = evalCubeZ + k;
                        //2. Generate a reproducible random number generator for the cube
                        lastRandom = lcgRandom(hash((uint)(cubeX + Seed), (uint)(cubeY), (uint)(cubeZ)));
                        //3. Determine how many feature points are in the cube
                        numberFeaturePoints = probLookup(lastRandom);
                        //4. Randomly place the feature points in the cube
                        for (uint l = 0; l < numberFeaturePoints; ++l)
                        {
                            lastRandom = lcgRandom(lastRandom);
                            randomDiff.X = lastRandom / 0x100000000;
                            lastRandom = lcgRandom(lastRandom);
                            randomDiff.Y = lastRandom / 0x100000000;
                            lastRandom = lcgRandom(lastRandom);
                            randomDiff.Z = lastRandom / 0x100000000;
                            featurePoint = new Vector3(randomDiff.X + cubeX, randomDiff.Y + cubeY, randomDiff.Z + cubeZ);
                            //5. Find the feature point closest to the evaluation point.
                            //This is done by inserting the distances to the feature points into a sorted list
                            insert(Distances, Distance(new Vector3(X, Y, Z), featurePoint));
                        }
                        //6. Check the neighboring cubes to ensure their are no closer evaluation points.
                        // This is done by repeating steps 1 through 5 above for each neighboring cube
                    }
                }
            }
            return Combine(Distances);
        }

        private double Combine(double[] Array)
        {
            switch (CombinationFunction)
            {
                case CombinationFunctions.D1:
                    return Array[0];
                case CombinationFunctions.D2MINUSD1:
                    return Array[1] - Array[0];
                case CombinationFunctions.D3MINUSD1:
                    return Array[2] - Array[0];
                default:
                    return Array[0];
            }
        }

        private float Distance(Vector3 p1, Vector3 p2)
        {
            switch (DistanceFunction)
            {
                case DistanceType.EUCLIDEAN2D:
                    return (float)EuclidianDistance2D(p1, p2);
                case DistanceType.EUCLIDEAN3D:
                    return (float)EuclidianDistance3D(p1, p2);
                case DistanceType.CHEBYSHEV2D:
                    return (float)ChebyshevDistance2D(p1, p2);
                case DistanceType.CHEBYSHEV3D:
                    return (float)ChebyshevDistance3D(p1, p2);
                case DistanceType.MANHATTAN2D:
                    return (float)ManhattanDistance2D(p1, p2);
                case DistanceType.MANHATTAN3D:
                    return (float)ManhattanDistance3D(p1, p2);
                default:
                    return (float)EuclidianDistance3D(p1, p2);
            }
        }

        private double EuclidianDistance2D(Vector3 p1, Vector3 p2)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
        }

        private double EuclidianDistance3D(Vector3 p1, Vector3 p2)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y) + (p1.Z - p2.Z) * (p1.Z - p2.Z);
        }

        private double ManhattanDistance2D(Vector3 p1, Vector3 p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }

        private double ManhattanDistance3D(Vector3 p1, Vector3 p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) + Math.Abs(p1.Z - p2.Z);
        }

        private double ChebyshevDistance2D(Vector3 p1, Vector3 p2)
        {
            Vector3 diff = p1 - p2;
            return Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y));
        }

        private double ChebyshevDistance3D(Vector3 p1, Vector3 p2)
        {
            Vector3 diff = p1 - p2;
            return Math.Max(Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y)), Math.Abs(diff.Z));
        }
        /// <summary>
        /// Given a uniformly distributed random number this function returns the number of feature points in a given cube.
        /// </summary>
        /// <param name="value">a uniformly distributed random number</param>
        /// <returns>The number of feature points in a cube.</returns>
        // Generated using mathmatica with "AccountingForm[N[Table[CDF[PoissonDistribution[4], i], {i, 1, 9}], 20]*2^32]"
        private static uint probLookup(uint value)
        {
            if (value < 393325350)
                return 1;
            if (value < 1022645910)
                return 2;
            if (value < 1861739990)
                return 3;
            if (value < 2700834071)
                return 4;
            if (value < 3372109335)
                return 5;
            if (value < 3819626178)
                return 6;
            if (value < 4075350088)
                return 7;
            if (value < 4203212043)
                return 8;
            return 9;
        }
        /// <summary>
        /// Inserts value into array using insertion sort. If the value is greater than the largest value in the array
        /// it will not be added to the array.
        /// </summary>
        /// <param name="arr">The array to insert the value into.</param>
        /// <param name="value">The value to insert into the array.</param>
        private static void insert(double[] arr, double value)
        {
            double temp;
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                if (value > arr[i])
                    break;
                temp = arr[i];
                arr[i] = value;
                if (i + 1 < arr.Length)
                    arr[i + 1] = temp;
            }
        }
        /// <summary>
        /// LCG Random Number Generator.
        /// LCG: http://en.wikipedia.org/wiki/Linear_congruential_generator
        /// </summary>
        /// <param name="lastValue">The last value calculated by the lcg or a seed</param>
        /// <returns>A new random number</returns>
        private static uint lcgRandom(uint lastValue)
        {
            return (uint)((1103515245u * lastValue + 12345u) % 0x100000000u);
        }
        /// <summary>
        /// Constant used in FNV hash function.
        /// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
        /// </summary>
        private const uint OFFSET_BASIS = 2166136261;
        /// <summary>
        /// Constant used in FNV hash function
        /// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
        /// </summary>
        private const uint FNV_PRIME = 16777619;
        /// <summary>
        /// Hashes three integers into a single integer using FNV hash.
        /// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
        /// </summary>
        /// <returns>hash value</returns>
        private static uint hash(uint i, uint j, uint k)
        {
            return (uint)((((((OFFSET_BASIS ^ (uint)i) * FNV_PRIME) ^ (uint)j) * FNV_PRIME) ^ (uint)k) * FNV_PRIME);
        }
        /// <summary>
        /// Hashes three integers into a single integer using FNV hash.
        /// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
        /// </summary>
        /// <returns>hash value</returns>
        private static uint hash2d(uint i, uint j)
        {
            return (uint)((((OFFSET_BASIS ^ (uint)i) * FNV_PRIME) ^ (uint)j) * FNV_PRIME);
        }
    }

    public enum CombinationFunctions
    {
        D1,
        D2MINUSD1,
        D3MINUSD1
    }

    public enum DistanceType
    {
        EUCLIDEAN3D,
        EUCLIDEAN2D,
        MANHATTAN3D,
        MANHATTAN2D,
        CHEBYSHEV3D,
        CHEBYSHEV2D
    }
}