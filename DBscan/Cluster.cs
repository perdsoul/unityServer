using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resources.Scripts.DBscan
{
    public class Cluster
    {
        private int clusterId;
        private Dictionary<int, int> destPros;
        private int number;
        private int gazeNum;
        public int gaze
        {
            get
            {
                return gazeNum;
            }
            set
            {
                gazeNum=value;
            }
        }
        public Cluster(int id)
        {
            clusterId = id;
            number = 0;
            destPros = new Dictionary<int, int>();
            gazeNum++;
        }
        public int id
        {
            get
            {
                return clusterId;
            }
        }
        public void Add(int clusterId)
        {
            if(destPros.Keys.Contains(clusterId))
            {
                destPros[clusterId]++;
            }
            else
            {
                destPros.Add(clusterId, 1);
            }
            number++;
        }
        public double getPro(int clusterId)
        {
            if (destPros.Keys.Contains(clusterId))
            {
                return (double)destPros[clusterId]/number;
            }
            else
            {
                return 0;
            }
        }
        public int getMaxProId()
        {
            int maxPro = 0;int maxProId=0;
            foreach(int cluterId in destPros.Keys)
            {
                if(destPros[cluterId]>maxPro)
                {
                    maxPro = destPros[cluterId];
                    maxProId = cluterId;
                }
            }
            return maxProId;
        }
        public Dictionary<int,double> getAllPro()
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            foreach (KeyValuePair<int, int> destPro in destPros)
            {
                result.Add(destPro.Key, destPro.Value / number);
            }
            return result;
        }
    }
}
