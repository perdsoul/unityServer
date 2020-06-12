using System;
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
    public class PreGaze
    {
        RadianceCollector mRadianceCollector = null;

        public PreGaze()
        {
            mRadianceCollector = new VisibleRadianceCollector();
        }
        //判断路径最后是否有旋转过
        public bool isRotated(List<BrowseRecord> _record)
        {
            for(int i=_record.Count-1;i>0&&i> _record.Count - 6;i--)
            {
                if (_record[i].rot != _record[i - 1].rot)
                    return true;
            }
            return false;
        }

        //判断该路径中是否有注视行为
        public bool isValidGaze(List<BrowseRecord> _record,out HashSet<string>gezeComponents)
        {
            int mWebClientExchangeCode = 4000;
            int Param = 3840451;
            //List<BrowseRecord> tmpList = getLastPoints(_record);
            HashSet<string> result = new HashSet<string>();
            for (int i = 0; i< _record.Count; i++)
            {
                int height =Param % mWebClientExchangeCode;
                int width = Param / mWebClientExchangeCode;
                ClientObjectAttribute clientObject = new ClientObjectAttribute();
                clientObject.CameraPosX = _record[i].pos.x;
                clientObject.CameraPosY = _record[i].pos.y;
                clientObject.CameraPosZ = _record[i].pos.z;
                clientObject.CameraRotX = _record[i].rot.x;
                clientObject.CameraRotY = _record[i].rot.y;
                clientObject.CameraRotZ = _record[i].rot.z;
                mRadianceCollector.Setup();
                mRadianceCollector.Collect(width, height, clientObject);
                HashSet<string> components = mRadianceCollector.getCenterComponents(width, height, clientObject);
                if(result==new HashSet<string>())
                {
                    result = components;
                }
                else
                {
                    result.IntersectWith(components);
                }
                //Debug.Log(components);
            }
            if(result.Count==0)
            {
                gezeComponents = result;
                return true;
            }
            gezeComponents = null;
            return false;
        }

        public List<string> getGazeComponents(List<BrowseRecord> path)
        {
            List<string> gazeComponents=new List<string>();
            for(int i=0;i<path.Count-2;i++)
            {
                if(path[i]==path[i+1]&&path[i+1]==path[i+2])
                {
                    List<BrowseRecord>lastPoints= getLastPoints(path,i+2);
                    HashSet<string> gazePointCompo;
                    if(isRotated(lastPoints) && isValidGaze(lastPoints, out gazePointCompo))
                    {
                        foreach(string component in gazePointCompo)
                        {
                            gazeComponents.Add(component);
                        }
                    }
                }
            }
            return gazeComponents;
        }

        public List<Record> getGazePoints(List<BrowseRecord> path)
        {
            List<Record> gazePoints=new List<Record>();
            for (int i = 0; i < path.Count - 2; i++)
            {
                if (path[i] == path[i + 1] && path[i + 1] == path[i + 2])
                {
                    List<BrowseRecord> lastPoints = getLastPoints(path, i + 2);
                    HashSet<string> gazePointCompo;
                    if (isRotated(lastPoints) && isValidGaze(lastPoints, out gazePointCompo))
                    {
                        Record temp = new Record(lastPoints[lastPoints.Count - 1].pos,
                            lastPoints[lastPoints.Count - 1].rot);
                        gazePoints.Add(temp);
                    }
                }
            }
            return gazePoints;
        }

        public List<int> getGazePos(List<BrowseRecord> path)
        {
            List<int> Pos = new List<int>();
            for (int i = 0; i < path.Count - 2; i++)
            {
                if (path[i] == path[i + 1] && path[i + 1] == path[i + 2])
                {
                    List<BrowseRecord> lastPoints = getLastPoints(path, i + 2);
                    HashSet<string> gazePointCompo;
                    if (isRotated(lastPoints) &&isValidGaze(lastPoints, out gazePointCompo))
                    {
                        Pos.Add(i);
                    }
                }
            }
            return Pos;
        }

        public List<BrowseRecord> getLastPoints(List<BrowseRecord> _record,int count)
        {
            List<BrowseRecord> result = new List<BrowseRecord>();
            if(_record==null||count==0)
            {
                return null;
            }
            result.Add(_record[count - 1]);
            for (int i=count-1;i>0&&result.Count<3;i--)
            {
                if(_record[i].pos!= result[result.Count-1].pos||
                    _record[i].rot != result[result.Count - 1].rot)
                {
                    result.Add(_record[i]);
                }
            }
            return result;
        }
    }
}
