using System;
using System.Collections.Generic;
using System.IO;
using DbscanImplementation;
using Resources.Scripts.History;
using Resources.Scripts.Prediction;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Resources.Scripts.DBscan
{
    class Program
    {
        public HashSet<Record[]> Dbscan(out List<int> clusterIds, out List<Cluster>clusterDes)
        {
            HashSet<Record[]> clusters;
        
            Record[] featureData = { };

           //List<Record> testPoints = Launcher.instance.history.GetRecords();
            List<Record> testPoints = new List<Record>();
            using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/" + Launcher.instance.GetSceneName + "/history.txt"))
           {
               string line;
               while ((line = sr.ReadLine()) != null)
               {
                   if (line == "")
                   {
                       continue;
                   }
                   string[] _record = line.Split('=');
                   string[] liness = _record[1].Split('_');
                   string[] lines0 = liness[0].Split(',');
                   string[] lines1 = liness[1].Split(',');
                   Vector3 tmpPos = new Vector3(float.Parse(lines0[0]), float.Parse(lines0[1]), float.Parse(lines0[2]));
                   Vector3 tmpRot = new Vector3(float.Parse(lines1[0]), float.Parse(lines1[1]), float.Parse(lines1[2]));
                   testPoints.Add(new Record(tmpPos,tmpRot));
               }
           }



            List<Record> testPointsIndex = new List<Record>();
            var exchangeAxis = new ExchangeAxis();
            foreach (var testPoint in testPoints)
            {
                //Vector3 iIdex =
                //    exchangeAxis.UnityPos_to_modelIndex(new Vector3((float)testPoint.posX, (float)testPoint.posY, (float)testPoint.posZ));
                //testPointsIndex.Add(new Record(iIdex, new Vector3((float)testPoint.rotX, (float)testPoint.rotY, (float)testPoint.rotZ)));
                testPointsIndex.Add(new Record(new Vector3((float)testPoint.posX, (float)testPoint.posY, (float)testPoint.posZ), 
                    new Vector3((float)testPoint.rotX, (float)testPoint.rotY, (float)testPoint.rotZ)));
            }

            //
            //            string path = Application.streamingAssetsPath + "/" + Launcher.instance.GetSceneName + "/history.txt";
            //            Debug.Log(path+"---------------------------");
            //            //读取数据点
            //            using (StreamReader sr = new StreamReader(path))
            //            {
            //                string line;
            //                while ((line = sr.ReadLine()) != null)
            //                {
            //                    if (line == "") { continue; }
            //                    line = line.Replace("(", "").Replace(")", "");
            //                    string[] lines = line.Split(',');
            //                    testPoints.Add(newRecord(float.Parse(lines[0]), float.Parse(lines[1]), float.Parse(lines[2])));
            //                }
            //            }
            /*
             //添加测试点
            for (int i = 0; i < 1000; i++)
            {
                //Test Points
                testPoints.Add(newRecord(1000,2,3));
                testPoints.Add(newRecord(1000,3,3));
                testPoints.Add(newRecord(1000,4,5));
                testPoints.Add(newRecord(1000,1,3));
                testPoints.Add(newRecord(1000,3,1));
                
                testPoints.Add(newRecord(2000,3,3));
                testPoints.Add(newRecord(2000,7,5));

                testPoints.Add(newRecord(6,2,3));
                testPoints.Add(newRecord(6,3,3));
                testPoints.Add(newRecord(6,4,5));

                testPoints.Add(newRecord(3000,1,3));
                testPoints.Add(newRecord(3000,3,1));
                testPoints.Add(newRecord(3000,7,5));
                testPoints.Add(newRecord(3000,3,3));
            }
            */
            Debug.Log("Total number of the counts: "+testPoints.Count);
            featureData = testPointsIndex.ToArray();
            var dbs = new DbscanAlgorithm<Record>((x, y) => Math.Sqrt(((x.posX - y.posX) * (x.posX - y.posX)) + ((x.posY - y.posY) * (x.posY - y.posY)) + ((x.posZ - y.posZ) * (x.posZ - y.posZ))));
            clusterIds = new List<int>();
            dbs.ComputeClusterDbscan(allPoints: featureData, epsilon: 3, minPts: 15, clusters: out clusters, ref clusterIds);
            clusterDes = dbs.GetClusterDes(clusters, clusterIds, Launcher.instance.history.getAllBrowseRecord());
            //dbs.ComputeClusterDbscan(allPoints: featureData, epsilon:10, minPts: 40, clusters: out clusters);
            Debug.Log("Below is the Result of the DBscan");
            //int count1 = 0;
            //foreach (Record[] i in clusters)
            //{
            //    Debug.Log("--------Cluster: "+(++count1)+"--------");
            //    Debug.Log(i[0].posX+"  "+i[0].posY+"  "+i[0].posZ);
            //}
            return clusters;
        }
    }
}
