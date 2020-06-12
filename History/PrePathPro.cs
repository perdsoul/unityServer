using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace Assets.Resources.Scripts.History
{
    class PrePathPro
    {
        public List<Vector3> ReducePath(List<Vector3> originPath)
        {
            List<Vector3> result = new List<Vector3>();
            int j = 0;
            result.Add(originPath[0]);
            for (int i=1;i<originPath.Count;i++)
            {
                if(result[i]!=result[j])
                {
                    result.Add(originPath[i]);
                    j = i;
                }
            }
            return result;
        }

        public float ContrastPath(List<Vector3> historyPath, List<Vector3>originPath)//对比两条路径相似度
        {
            float min = 500;
            List<Vector3> currentPath = new List<Vector3>();

            //对当前路径剪枝 
            if (originPath.Count>5&& historyPath.Count>5)
            {
                for(int i=5;i>1;i--)
                {
                    currentPath.Add(originPath[originPath.Count - i]);
                }
            }
            else if(historyPath.Count > 5)
            {
                currentPath = originPath;
            }
            else
            {
                return -1;
            }

            for(int i=0;i<historyPath.Count-currentPath.Count;i++)
            {
                float disSum = 0;
                for (int j=0;j< currentPath.Count;j++)
                {
                    Vector3 distance = currentPath[j] - historyPath[i + j];
                    disSum += distance.sqrMagnitude;
                }
                if(disSum<min)
                {
                    min = disSum;
                }
            }

            return min;
        }
    }
}
