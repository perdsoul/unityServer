using System;
using System.Collections.Generic;
using System.Linq;
using DbscanImplementation;
using Newtonsoft.Json;
using Resources.Scripts.DBscan;
using UnityEngine;

namespace Resources.Scripts.Prediction
{
    public class GetModels
    {
//        /**
//         * 根据传进来的坐标x，y，z求出这个坐标周围九个空间块包含的全部模型
//         */
//        public List<String> get(float x,float y,float z)
//        {
//            //获取所有的box对应的testModel类
//            string[] lines = System.IO.File.ReadAllLines(Application.streamingAssetsPath+"/testModel/testModel.vsg");
//            List<Coeffecient.testModel> totalBoxList = new List<Coeffecient.testModel>();
//            foreach (var line in lines)
//            {
//                if (!line.StartsWith("#") && line!="")
//                {
//                    totalBoxList.Add(new Coeffecient.testModel(line));
//                }
//            }
//            //根据坐标找出要检索的box们的序号
//            Record box_index = new Record(x,y,z,false);
//            List<Record> cluster = new List<Record>();
//            for (int i = 0; i < 3; i++)
//            {
//                for (int j = 0; j < 3; j++)
//                {
//                    for (int k= 0; k< 3; k++)
//                    {
//                        cluster.Add(new Record(new Vector3(box_index.posX + i, box_index.posY + j, box_index.posZ + k),new Vector3(box_index.rotX,box_index.rotY,box_index.rotZ)));
//                    }
//                }
//            }
//            //通过序号找出box对应的testModel类
//            List<Coeffecient.testModel> predictBoxList = new List<Coeffecient.testModel>();
//            foreach (var box in cluster)
//            {
//                var boxInCluster = totalBoxList.Find(item => item.x == box.posX && item.y == box.posY && item.z == box.posZ);
//                //Debug.Log(boxInCluster.x+"  "+boxInCluster.y+"  "+boxInCluster.z+"   "+boxInCluster.times);
//                if (boxInCluster.times != 0)
//                {
//                    predictBoxList.Add(boxInCluster);
//                }
//            }
//            //获取所有的模型
//            string temp = System.IO.File.ReadAllText(Application.streamingAssetsPath+"/testModel/reuseInfo.json");
//            Dictionary<string, string> reuseinfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(temp);
//            List<String> modelList = new List<string>();
//            foreach (var box in predictBoxList)
//            {
//                foreach (var model in box.models)
//                {
//                    if (reuseinfo.ContainsKey(model))
//                    {
//                        if (!modelList.Contains(reuseinfo[model]))
//                        {
//                            modelList.Add(reuseinfo[model]);
//                        }
//                    }
//                    else
//                    {
//                        if (!modelList.Contains(model))
//                        {
//                            modelList.Add(model);
//                        }
//                    }
//                }
//            }
//            Debug.Log("++++++这个cluster包括的模型总数是"+modelList.Count);
//            return modelList;
//        }
    }
}