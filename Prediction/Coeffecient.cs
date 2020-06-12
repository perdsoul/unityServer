using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DbscanImplementation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resources.Scripts.DBscan;
using Resources.Scripts.History;
using UnityEngine;

namespace Resources.Scripts.Prediction
{
    public class Coeffecient
    {
        private PreGaze preGaze = new PreGaze();
        public double GetCoefficeent(Record[] cluster)
        {
            //string[] lines = System.IO.File.ReadAllLines(Application.streamingAssetsPath+"/testModel/testModel.vsg");
            string[] lines = System.IO.File.ReadAllLines(Application.streamingAssetsPath + "/" + Launcher.instance.GetSceneName + "/" + Launcher.instance.GetSceneName+".vsg");
            List<testModel> totalBoxList = new List<testModel>();
            foreach (var line in lines)
            {
                if (!line.StartsWith("#") && line!="")
                {
                    totalBoxList.Add(new testModel(line));
                }
            }

            //Debug.Log("整个场景中的空间块总数是"+totalBoxList.Count);
            List<testModel> predictBoxList = new List<testModel>();
            foreach (var box in cluster)
            {
                var boxInCluster = totalBoxList.Find(item => item.x == box.posX && item.y == box.posY && item.z == box.posZ);
                //Debug.Log(boxInCluster.x+"  "+boxInCluster.y+"  "+boxInCluster.z+"   "+boxInCluster.times);
                if (boxInCluster.times != 0)
                {
//                    if (!predictBoxList.Contains(boxInCluster))
//                    {
//                        predictBoxList.Add(boxInCluster);
//                    }
                    predictBoxList.Add(boxInCluster);
                }
            }

            Debug.Log("这个cluster空间块计次是"+cluster.Length);
            //Debug.Log("这个cluster有重复、去掉没有模型的空间块个数是"+predictBoxList.Count);

            string temp = System.IO.File.ReadAllText(Application.streamingAssetsPath+ "/" + Launcher.instance.GetSceneName+"/reuseInfo.json");
            
            Dictionary<string, string> reuseinfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(temp);
            
            List<String> modelList = new List<string>();
            foreach (var box in predictBoxList)
            {
                foreach (var model in box.models)
                {
                    if (reuseinfo.ContainsKey(model))
                    {
                        modelList.Add(reuseinfo[model]);
                    }
                    else
                    {
                        modelList.Add(model);
                    }
                }
            }
            //Debug.Log("这个cluster包括的模型总数是"+modelList.Count);
            //如果没有模型返回0。
            if (modelList.Count==0)
            {
                return 0;
            }
            
            int count = modelList.Count;
            double coeffecient = 0;
            string temp2 = System.IO.File.ReadAllText(Application.streamingAssetsPath+ "/" + Launcher.instance.GetSceneName+"/interestTable.json");
            var jo = JObject.Parse(temp2);

            foreach (var model in modelList)
            {
                var modelValue = jo[model];
                var volumn = (double)modelValue.SelectToken("volume");
                var reuseTimes = (int) modelValue.SelectToken("reuseTimes");
                var thisco = volumn * 0.00000001 + reuseTimes;
                coeffecient += thisco;
//                if (thisco>2)
//                {
//                    Debug.Log(model+"的兴趣度比较高："+thisco);
//                }
            }
            coeffecient = coeffecient / modelList.Count;
            return coeffecient;
        }
        
        public struct testModel
        {
            public int x;
            public int y;
            public int z;
            public int times;
            public string[] models;

            public testModel(string input)
            {
                string[] temp=Regex.Split(input,"\\s+",RegexOptions.IgnoreCase);
            
                x = int.Parse(temp[0]);
                y = int.Parse(temp[1]);
                z = int.Parse(temp[2]);
                times = int.Parse(temp[3]);
                models = new string[times];
                for (int i = 0; i < times; i++)
                {
                    models[i] = temp[i + 4];
                }
            }
        }
    }
}