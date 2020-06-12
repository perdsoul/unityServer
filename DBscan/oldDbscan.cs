using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DbscanImplementation;
using Resources.Scripts.DBscan;
//using Resources.Scripts.Prediction;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class Dbscan : MonoBehaviour
{
//    private HashSet<MyCustomDatasetItem[]> _clusters;
//    // Start is called before the first frame update
//    void Start()
//    {
//        Program dbscan = new Program();
//        _clusters= dbscan.Dbscan();
//        InvokeRepeating(nameof(PredictCluster),0,5);
//    }
//
//    private void PredictCluster()
//    {
//        Predict predict = new Predict();
//        Record[] predictedCluster = predict.GetPredictedCluster(transform, _clusters);
//        if (predictedCluster!=null)
//        {
//            Debug.Log("-------PREDICTED PLACE-------");
//            float x_sum=0, y_sum=0, z_sum=0;
//            foreach (var j in predictedCluster)
//            {
//                x_sum += j.X;
//                y_sum += j.Y;
//                z_sum += j.Z;
//            }
//            float x_ave = x_sum / predictedCluster.Length;
//            float y_ave = y_sum / predictedCluster.Length;
//            float z_ave = z_sum / predictedCluster.Length;
//            //把块变成坐标系
//            var exchangeAxis = new ExchangeAxis();
//            Vector3 pos = exchangeAxis.ModelIndex_to_unityPos(x_ave,y_ave,z_ave);
//            Debug.Log(pos.x+"   "+pos.y+"   "+pos.z);
//            Debug.Log("===============================");
//        }
//        else { Debug.Log("No suggestion");}
//    }
}
