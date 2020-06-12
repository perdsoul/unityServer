using System;
using System.Collections.Generic;
using System.Linq;
using DbscanImplementation;
using Resources.Scripts.History;
using UnityEngine;

namespace Resources.Scripts.DBscan
{
    /// <summary>
    /// DBSCAN algorithm class
    /// </summary>
    /// <typeparam name="T">Takes dataset item row (features, preferences, vector) type</typeparam>
    //public class DbscanAlgorithm<T> where T : DatasetItemBase
    //{

    //    private readonly Func<T, T, double> _metricFunc;

    //    /// <summary>
    //    /// Takes metric function to compute distances between dataset items T
    //    /// </summary>
    //    /// <param name="metricFunc"></param>
    //    public DbscanAlgorithm(Func<T, T, double> metricFunc)
    //    {
    //        _metricFunc = metricFunc;
    //    }

    //    /// <summary>
    //    /// Performs the DBSCAN clustering algorithm.
    //    /// </summary>
    //    /// <param name="allPoints">Dataset</param>
    //    /// <param name="epsilon">Desired region ball radius</param>
    //    /// <param name="minPts">Minimum number of points to be in a region</param>
    //    /// <param name="clusters">returns sets of clusters, renew the parameter</param>
    //    public void ComputeClusterDbscan(T[] allPoints, double epsilon, int minPts, out HashSet<T[]> clusters)
    //    {
    //        //Debug.Log(allPoints.Length);
    //        DbscanPoint<T>[] allPointsDbscan = allPoints.Select(x => new DbscanPoint<T>(x)).ToArray();
    //        int clusterId = 0;
    //        for (int i = 0; i < allPointsDbscan.Length; i++)
    //        {
    //            DbscanPoint<T> p = allPointsDbscan[i];
    //            if (p.IsVisited)
    //                continue;
    //            p.IsVisited = true;

    //            DbscanPoint<T>[] neighborPts = null;
    //            RegionQuery(allPointsDbscan, p.ClusterPoint, epsilon, out neighborPts);
    //            if (neighborPts.Length < minPts)
    //                p.ClusterId = (int)ClusterIds.Noise;
    //            else
    //            {
    //                clusterId++;
    //                ExpandCluster(allPointsDbscan, p, neighborPts, clusterId, epsilon, minPts);
    //            }
    //        }
    //        clusters = new HashSet<T[]>(
    //            allPointsDbscan
    //                .Where(x => x.ClusterId > 0)
    //                .GroupBy(x => x.ClusterId)
    //                .Select(x => x.Select(y => y.ClusterPoint).ToArray())
    //            );
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="allPoints">Dataset</param>
    //    /// <param name="point">point to be in a cluster</param>
    //    /// <param name="neighborPts">other points in same region with point parameter</param>
    //    /// <param name="clusterId">给定的clusterId</param>
    //    /// <param name="epsilon">区域球半径</param>
    //    /// <param name="minPts">一个区域点的最小数量</param>
    //    private void ExpandCluster(DbscanPoint<T>[] allPoints, DbscanPoint<T> point, DbscanPoint<T>[] neighborPts, int clusterId, double epsilon, int minPts)
    //    {
    //        point.ClusterId = clusterId;
    //        for (int i = 0; i < neighborPts.Length; i++)
    //        {
    //            DbscanPoint<T> pn = neighborPts[i];
    //            if (!pn.IsVisited)
    //            {
    //                pn.IsVisited = true;
    //                DbscanPoint<T>[] neighborPts2 = null;
    //                RegionQuery(allPoints, pn.ClusterPoint, epsilon, out neighborPts2);
    //                if (neighborPts2.Length >= minPts)
    //                {
    //                    neighborPts = neighborPts.Union(neighborPts2).ToArray();
    //                }
    //            }
    //            if (pn.ClusterId == (int)ClusterIds.Unclassified)
    //                pn.ClusterId = clusterId;
    //        }
    //    }

    //    /// <summary>
    //    /// Checks and searchs neighbor points for given point
    //    /// </summary>
    //    /// <param name="allPoints">Dataset</param>
    //    /// <param name="point">centered point to be searched neighbors</param>
    //    /// <param name="epsilon">radius of center point</param>
    //    /// <param name="neighborPts">result neighbors</param>
    //    private void RegionQuery(DbscanPoint<T>[] allPoints, T point, double epsilon, out DbscanPoint<T>[] neighborPts)
    //    {
    //        neighborPts = allPoints.Where(x => _metricFunc(point, x.ClusterPoint) <= epsilon).ToArray();
    //    }
    //}

    public class DbscanAlgorithm<T> where T : Record
    {
        /// <summary>
        /// Performs the DBSCAN clustering algorithm.
        /// </summary>
        /// <param name="allPoints">Dataset</param>
        /// <param name="epsilon">Desired region ball radius</param>
        /// <param name="minPts">Minimum number of points to be in a region</param>
        /// <param name="clusters">returns sets of clusters, renew the parameter</param>
        /// 
        private readonly Func<T, T, double> _metricFunc;

        /// <summary>
        /// Takes metric function to compute distances between dataset items T
        /// </summary>
        /// <param name="metricFunc"></param>
        public DbscanAlgorithm(Func<T, T, double> metricFunc)
        {
            _metricFunc = metricFunc;
        }

        public void ComputeClusterDbscan(T[] allPoints, double epsilon, int minPts, out HashSet<T[]> clusters, ref List<int> clusterIds)
        {

            DbscanPoint<T>[] allPointsDbscan = allPoints.Where(x => x.posX !=0|| x.posY != 0|| x.posZ != 0)
                .Select(x => new DbscanPoint<T>(x)).ToArray();
            var tree = new KDTree.KDTree<DbscanPoint<T>>(3);
            for (var i = 0; i < allPointsDbscan.Length; ++i)
            {
                Record temp = allPointsDbscan[i].ClusterPoint;
                tree.AddPoint(new double[] { temp.posX, temp.posY, temp.posZ }, allPointsDbscan[i]);
            }


            int clusterId = 0;
            for (int i = 0; i < allPointsDbscan.Length; i++)
            {
                DbscanPoint<T> p = allPointsDbscan[i];
                if (p.IsVisited)
                {
                    clusterIds.Add(p.ClusterId);
                    continue;
                }
                p.IsVisited = true;

                var neighborPts = RegionQuery(tree, p.ClusterPoint, epsilon);
                if (neighborPts.Length < minPts)
                    p.ClusterId = (int)ClusterIds.Noise;
                else
                {
                    clusterId++;
                    ExpandCluster(tree, p, neighborPts, clusterId, epsilon, minPts);
                }
                clusterIds.Add(p.ClusterId);
            }
            clusters = new HashSet<T[]>(
                allPointsDbscan
                    .Where(x => x.ClusterId > 0)
                    .GroupBy(x => x.ClusterId)
                    .Select(x => x.Select(y => y.ClusterPoint).ToArray())
                );
        }

        public List<Cluster>GetClusterDes(HashSet<Record[]>clusters, List<int> clusterIds,List<BrowseRecord>paths)
        {
            List<Cluster> clusterDes = new List<Cluster>();
            for (int i = 0; i < clusters.Count; i++)
            {
                clusterDes.Add(new Cluster(i + 1));
            }
            PreGaze preGaze = new PreGaze();
            List<int> gazePos = preGaze.getGazePos(paths);
            foreach(int pos in gazePos)
            {
                clusterDes[clusterIds[pos] - 1].gaze++;
            }
            for (int i = 1; i < clusterIds.Count; i++)
            {
                if (clusterIds[i - 1] != clusterIds[i] && clusterIds[i - 1] != -1 && clusterIds[i] != -1)
                {
                    clusterDes[clusterIds[i - 1] - 1].Add(clusterIds[i]);
                }
            }
            return clusterDes;
        }

        public bool CheckCenterEmpty(T[] clusterPoints, T centerPoint, double epsilon, int maxPts)
        {
            DbscanPoint<T>[] clusterPointsDbscan = clusterPoints.Select(x => new DbscanPoint<T>(x)).ToArray();
            var tree = new KDTree.KDTree<DbscanPoint<T>>(3);
            for (var i = 0; i < clusterPointsDbscan.Length; ++i)
            {
                Record temp = clusterPointsDbscan[i].ClusterPoint;
                tree.AddPoint(new double[] { temp.posX, temp.posY, temp.posZ }, clusterPointsDbscan[i]);
            }
            Record tempCenter = new DbscanPoint<T>(centerPoint).ClusterPoint;
            tree.AddPoint(new double[] { tempCenter.posX, tempCenter.posY, tempCenter.posZ }, new DbscanPoint<T>(centerPoint));
            var neighborPts = RegionQuery(tree, centerPoint, epsilon);
            if (neighborPts.Length > maxPts)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allPoints">Dataset</param>
        /// <param name="point">point to be in a cluster</param>
        /// <param name="neighborPts">other points in same region with point parameter</param>
        /// <param name="clusterId">given clusterId</param>
        /// <param name="epsilon">Desired region ball range</param>
        /// <param name="minPts">Minimum number of points to be in a region</param>
        private void ExpandCluster(KDTree.KDTree<DbscanPoint<T>> tree, DbscanPoint<T> p, DbscanPoint<T>[] neighborPts, int clusterId, double epsilon, int minPts)
        {
            p.ClusterId = clusterId;
            //for (int i = 0; i < neighborPts.Length; i++)
            //{
            //    DbscanPoint<T> pn = neighborPts[i];
            //    if (!pn.IsVisited)
            //    {
            //        pn.IsVisited = true;
            //        DbscanPoint<T>[] neighborPts2 = RegionQuery(tree, pn.ClusterPoint, epsilon); ;
            //        if (neighborPts2.Length >= minPts)
            //        {
            //            neighborPts = neighborPts.Union(neighborPts2).ToArray();
            //        }
            //    }
            //    if (pn.ClusterId == (int)ClusterIds.Unclassified)
            //        pn.ClusterId = clusterId;
            //}
            var queue = new Queue<DbscanPoint<T>>(neighborPts);
            while (queue.Count > 0)
            {
                var point = queue.Dequeue();
                if (point.ClusterId == (int)ClusterIds.Unclassified)
                {
                    point.ClusterId = clusterId;
                }

                if (point.IsVisited)
                {
                    continue;
                }

                point.IsVisited = true;
                var neighbors = RegionQuery(tree, point.ClusterPoint, epsilon);
                if (neighbors.Length >= minPts)
                {
                    foreach (var neighbor in neighbors.Where(neighbor => !neighbor.IsVisited))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        /// <summary>
        /// Checks and searchs neighbor points for given point
        /// </summary>
        /// <param name="allPoints">Dataset</param>
        /// <param name="point">centered point to be searched neighbors</param>
        /// <param name="epsilon">radius of center point</param>
        /// <param name="neighborPts">result neighbors</param>
        private DbscanPoint<T>[] RegionQuery(KDTree.KDTree<DbscanPoint<T>> tree, T point, double epsilon)
        {
            var neighbors = new List<DbscanPoint<T>>();
            var e = tree.NearestNeighbors(new[] { point.posX, point.posY, point.posZ }, 10, epsilon);
            while (e.MoveNext())
            {
                neighbors.Add(e.Current);
            }
            return neighbors.ToArray();
        }
    }
}