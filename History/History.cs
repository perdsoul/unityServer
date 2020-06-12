﻿﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using Resources.Scripts.Prediction;
using Resources.Scripts.DBscan;


namespace Resources.Scripts.History
{
    public class History:MonoBehaviour
    {
        /*private string modelPath = null;
        //private string modelPath = "E:/CG project/server/Assets/StreamingAssets/testModel" + "/model.txt";
        private Dictionary<float, int> modelHistory = new Dictionary<float, int>();//储存历史出现的模型，计算关注度用*/

        //private string recordPath = null;
        private string recordPath = Application.streamingAssetsPath + "/testModel/" + "history.txt";
        //private string recordPath = "E:/CG project/server/Assets/StreamingAssets/testModel" + "/history.txt";
        private Dictionary<string, Path> pathHistory = new Dictionary<string, Path>();//连接与路径的映射表
        private Dictionary<string, List<BrowseRecord>> recordHistory = new Dictionary<string, List<BrowseRecord>>();//路径与记录的映射表

        private int pathNum;
        private bool hasChanged;
        private PreGaze preGaze = new PreGaze();


        //开启协程
        private void Start()
        {
            Debug.Log("start");
            pathNum = 0;
            //hasPredict = false;
            hasChanged = false;
            StartCoroutine(OutputRecord());
            StartCoroutine(Maintenance());
            //StartCoroutine(Predict());
        }
        IEnumerator OutputRecord()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (pathHistory.Count==0)
                {
                    continue;
                }
                foreach(Path path in pathHistory.Values)
                {
                    if(!path.isOvertime(path.cameraPos))
                    {
                        BrowseRecord tmpRecord =new BrowseRecord(
                            path.cameraPos, path.cameraRot);
                        recordHistory[path.name].Add(tmpRecord);
                        using (StreamWriter sw = new StreamWriter(recordPath, true, Encoding.Default))
                        {
                            //Debug.Log(path.name);
                            sw.WriteLine(tmpRecord.Output(path.name));
                            sw.Close();
                        }
                    }
                }
                
            }
        }

        //计算关注度用
        /*public void OutputModel(float _param)
        {
            if (!modelHistory.ContainsKey(_param))
            {
                modelHistory.Add(_param,1);
            }
            else
            {
                modelHistory[_param]++;
            }
            StreamWriter sw = new StreamWriter(modelPath, true, Encoding.Default);
            sw.WriteLine(_param.ToString());
            sw.Close();
        }
        public float FindModelNum(float _param)
        {
            if (!modelHistory.ContainsKey(_param))
            {
                return 0;
            }
            else
            {
                return modelHistory[_param];
            }
        }*/

        //模型历史管理
        /*private void ReadModel(string _path)
        {
            using (StreamReader sr = new StreamReader(_path))
            {
                string line;
                float _param;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "")
                    {
                        continue;
                    }
                    else
                    {
                        _param = float.Parse(line);
                    }
                    if (!modelHistory.ContainsKey(_param))
                    {
                        modelHistory.Add(_param, 1);
                    }
                    else
                    {
                        modelHistory[_param]++;
                    }
                }
            }
        }*/
        public void getPath(string _recordPath)
        {
            if ( recordPath == _recordPath)
            {
                return;
            }

            recordPath = _recordPath;

            ReadRecord(recordPath);
            
            //Debug.Log(recordPath);
        }
        //路径历史管理
        public void NewPath(string ctsMarker)
        {
            //Debug.Log("new path");
            Path tmpPath = new Path(pathNum);
            pathHistory.Add(ctsMarker, tmpPath);
            recordHistory.Add(tmpPath.name, new List<BrowseRecord>());
            pathNum++;
        }
        public void RemovePath(string ctsMarker)
        {
            pathHistory.Remove(ctsMarker);
        }
        public void UpdateCamera(string ctsMarker,ClientObjectAttribute clientObjectAttribute)
        {
            pathHistory[ctsMarker].UpdateCamera(clientObjectAttribute);
        }
        public void ReadRecord(string _path)
        {
            using (StreamReader sr = new StreamReader(_path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "")
                    {
                        continue;
                    }
                    string[] _record = line.Split('=');
                    if (!recordHistory.ContainsKey(_record[0]))
                    {
                        recordHistory.Add(_record[0],new List<BrowseRecord>());
                        pathNum++;
                    }
                    string[] liness = _record[1].Split('_');
                    string[] lines0 = liness[0].Split(',');
                    string[] lines1 = liness[1].Split(',');
                    Vector3 tmpPos = new Vector3(float.Parse(lines0[0]), float.Parse(lines0[1]), float.Parse(lines0[2]));
                    Vector3 tmpRot = new Vector3(float.Parse(lines1[0]), float.Parse(lines1[1]), float.Parse(lines1[2]));
                    DateTime tmpTime = Convert.ToDateTime(liness[2]);
                    recordHistory[_record[0]].Add(new BrowseRecord(tmpPos,tmpRot,tmpTime));
                }
            }
        }
        IEnumerator Maintenance()
        {
            while(true)
            {
                yield return new WaitForSeconds(1);
                if (pathHistory.Count == 0)
                {
                    continue;
                }
                foreach (Path path in pathHistory.Values)
                {
                    if(recordHistory[path.name].Count>500)
                    {
                        recordHistory[path.name].RemoveAt(0);
                        hasChanged = true;
                    }
                }
                if(isStop()&&hasChanged)//停止时更新内存内容
                {
                    using (StreamWriter sw = new StreamWriter(recordPath, false, Encoding.Default))
                    {
                        foreach(string pathName in recordHistory.Keys)
                        {
                            foreach(BrowseRecord browseRecord in recordHistory[pathName])
                            {
                                sw.WriteLine(browseRecord.Output(pathName));
                            }
                        }
                        sw.Close();
                    }
                    hasChanged = false;
                }
            }
        }

        //预测用
        public bool isStop()
        {
            if(pathHistory.Count==0)
            {
                return false;
            }
            foreach(Path path in pathHistory.Values)
            {
                if(!path.isStop())
                {
                    return false;
                }
            }
            return true;
        }

        /*IEnumerator Predict()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                if(!isStop())
                {
                    continue;
                }
                if(!hasPredict)
                {
                    //待修改
                    Record tmpDes = getPredict();
                    Debug.Log("Target: " + tmpDes.posX + " " + tmpDes.posY + " " + tmpDes.posZ + " " + tmpDes.rotX + " " + tmpDes.rotY + " " + tmpDes.rotZ);

                    Vector3 tmpPre = MoveForwardPredict(new Vector3(tmpDes.posX, tmpDes.posY, tmpDes.posZ));
                    Record prediction = new Record(tmpPre, new Vector3(tmpDes.rotX, tmpDes.rotY, tmpDes.rotZ));

                }
            }
        }*/
        /*public void Update()
        {
            
        }*/
        public List<BrowseRecord> getAllBrowseRecord()
        {
            List<BrowseRecord> result = new List<BrowseRecord>();
            foreach (List<BrowseRecord> browseRecords in recordHistory.Values)
            {
                foreach (BrowseRecord browseRecord in browseRecords)
                {
                    result.Add(browseRecord);
                }
            }
            return result;
        }
        public Record getPredict(string ctsMarker,ClientObjectAttribute cl)
        {
            Program dbscan = new Program();
            //Debug.Log("!!!!!!!!!!!!!Start Dbscan!!!!!!!!!!!!!");
            List<int> clusterIds;
            List<Cluster> clusterDes;
            HashSet<Record[]> _clusters = dbscan.Dbscan(out clusterIds,out clusterDes);
            Predict predict = new Predict();
            List<Record> gazePoints = preGaze.getGazePoints(getAllBrowseRecord());
            List<Record> predictedCluster = predict.GetPredictedCluster(
                new Vector3(cl.CameraPosX,cl.CameraPosY,cl.CameraPosZ),
                new Vector3(cl.CameraRotX,cl.CameraRotY,cl.CameraRotZ),
                _clusters, clusterDes
                );
            Record posRange = getRangeRecord(predictedCluster);
            Vector3 tmpPos = pathHistory[ctsMarker].cameraPos;
            Vector3 rot;
            float x_sum = 0, y_sum = 0, z_sum = 0, x_rot = 0, y_rot = 0, z_rot = 0;
            foreach (var j in predictedCluster)
            {
                x_sum += (float)j.posX;
                y_sum += (float)j.posY;
                z_sum += (float)j.posZ;
                x_rot += (float)j.rotX;
                y_rot += (float)j.rotY;
                z_rot += (float)j.rotZ;
            }
            float x_ave = x_sum / predictedCluster.Count;
            float y_ave = y_sum / predictedCluster.Count;
            float z_ave = z_sum / predictedCluster.Count;
            float x_averot = x_rot / predictedCluster.Count;
            float y_averot = y_rot / predictedCluster.Count;
            float z_averot = z_rot / predictedCluster.Count;

            if (tmpPos.x <= posRange.posX && tmpPos.y <= posRange.posY && tmpPos.z <= posRange.posZ
                && tmpPos.x >= posRange.rotX && tmpPos.y >= posRange.rotY && tmpPos.z >= posRange.rotZ)
            {
                rot = new Vector3(x_averot, y_averot, z_averot);
            }
            else
            {
                rot = new Vector3(x_ave, y_ave, z_ave);
                rot = rot - tmpPos;
            }
            //------
            /*if (predictedCluster != null)
            {
                //把块变成坐标系
                var exchangeAxis = new ExchangeAxis();
                Record pos = new Record(exchangeAxis.ModelIndex_to_unityPos(x_ave, y_ave, z_ave), new Vector3(x_averot, y_averot, z_averot));
                return pos;

            }
            else { Debug.Log("No suggestion"); }*/
            return new Record(MoveForwardPredict(pathHistory[ctsMarker],rot),rot);
        }
        public Record getRangeRecord(List<Record> records)
        {
            float maxX=-1000000;
            float maxY = -1000000;
            float maxZ = -1000000;
            float minX = 1000000;
            float minY = 1000000;
            float minZ = 1000000;
            foreach (Record record in records)
            {
                if(record.posX>maxX)
                {
                    maxX = (float)record.posX;
                }
                if (record.posY > maxY)
                {
                    maxY = (float)record.posY;
                }
                if (record.posZ > maxZ)
                {
                    maxZ = (float)record.posZ;
                }
                if (record.posX < minX)
                {
                    minX = (float)record.posX;
                }
                if (record.posX < minY)
                {
                    minY = (float)record.posX;
                }
                if (record.posX < minZ)
                {
                    minZ = (float)record.posX;
                }
            }
            return new Record(new Vector3(maxX, maxY, maxZ), new Vector3(minX,minY,minZ));
        }
        public Vector3 MoveForwardPredict(Path path, Vector3 rot)
        {
            float tmpSpeed = path.speed.sqrMagnitude;
            Vector3 tmpPre = path.cameraPos + rot.normalized * tmpSpeed * 5;
            return tmpPre;
        }

        //获取记录
        public List<Record> GetRecords()
        {
            List<Record> records = new List<Record>();
            foreach(List<BrowseRecord> browseRecords in recordHistory.Values)
            {
                foreach(BrowseRecord browseRecord in browseRecords)
                {
                    records.Add(new Record(browseRecord.pos,browseRecord.rot));
                }
            }
            return null;
        }
    }

   
}
