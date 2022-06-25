/******************************************************************************
 *
 * The MIT License (MIT)
 *
 * MIConvexHull, Copyright (c) 2015 David Sehnal, Matthew Campbell
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *  
 *****************************************************************************/

using System.Collections.Generic;
using System.Linq;

namespace NWH.DWP2.MiConvexHull
{
    /*
     * Code here handles triangulation related stuff.
     */

    /// <summary>
    ///     Class ConvexHullAlgorithm.
    /// </summary>
    internal partial class ConvexHullAlgorithm
    {
        /// <summary>
        ///     Computes the Delaunay triangulation.
        /// </summary>
        /// <typeparam name="TVertex">The type of the t vertex.</typeparam>
        /// <typeparam name="TCell">The type of the t cell.</typeparam>
        /// <param name="data">The data.</param>
        /// <returns>TCell[].</returns>
        internal static TCell[] GetDelaunayTriangulation<TVertex, TCell>(IList<TVertex> data)
            where TCell : TriangulationCell<TVertex, TCell>, new()
            where TVertex : IVertex
        {
            ConvexHullAlgorithm ch =
                new ConvexHullAlgorithm(data.Cast<IVertex>().ToArray(), true, Constants.DefaultPlaneDistanceTolerance);
            ch.GetConvexHull();
            ch.RemoveUpperFaces();
            return ch.GetConvexFaces<TVertex, TCell>();
        }


        /// <summary>
        ///     Removes up facing Tetrahedrons from the triangulation.
        /// </summary>
        private void RemoveUpperFaces()
        {
            IndexBuffer delaunayFaces = ConvexFaces;
            int         dimension     = NumOfDimensions - 1;

            // Remove the "upper" faces
            for (int i = delaunayFaces.Count - 1; i >= 0; i--)
            {
                int                candidateIndex = delaunayFaces[i];
                ConvexFaceInternal candidate      = FacePool[candidateIndex];
                if (candidate.Normal[dimension] >= 0.0)
                {
                    for (int fi = 0; fi < candidate.AdjacentFaces.Length; fi++)
                    {
                        int af = candidate.AdjacentFaces[fi];
                        if (af >= 0)
                        {
                            ConvexFaceInternal face = FacePool[af];
                            for (int j = 0; j < face.AdjacentFaces.Length; j++)
                            {
                                if (face.AdjacentFaces[j] == candidateIndex)
                                {
                                    face.AdjacentFaces[j] = -1;
                                }
                            }
                        }
                    }

                    delaunayFaces[i] = delaunayFaces[delaunayFaces.Count - 1];
                    delaunayFaces.Pop();
                }
            }
        }
    }
}