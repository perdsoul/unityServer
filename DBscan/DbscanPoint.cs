
using Resources.Scripts.DBscan;

namespace DbscanImplementation
{
    public class DbscanPoint<T>
    {
        public bool IsVisited;
        public T ClusterPoint;
        public int ClusterId;

        public DbscanPoint(T x)
        {
            ClusterPoint = x;
            IsVisited = false;
            ClusterId = (int)ClusterIds.Unclassified;
        }

    }
}